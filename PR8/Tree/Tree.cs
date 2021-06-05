using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace Tree
{
    internal class Tree
    {
        struct VertexData
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 tex_coord;
        }

        public class Branch
        {
            public Branch parent;
            public float Length;
            public float Angle1;
            public float Angle2;
            public float Thickness;


            public Matrix4 GetBranchTransform()
            {
//                Matrix4 transform = Matrix4.Identity;
                Matrix4 transform = Matrix4.CreateRotationX((float)(Angle1 * Math.PI / 180.0)) * Matrix4.CreateRotationZ((float)(Angle2 * Math.PI / 180.0));
                Branch p = parent;
                while (p != null)
                {
                    transform = transform * Matrix4.CreateTranslation(0.0f, p.Length, 0.0f) * Matrix4.CreateRotationX((float)(p.Angle1 * Math.PI / 180.0)) * Matrix4.CreateRotationZ((float)(p.Angle2 * Math.PI / 180.0));
                    p = p.parent;
                }
                return transform;
            }
        }


        string vertex_shader_src = @"
#version 450 core
in vec3 vertex;
in vec2 tex_coord;
in vec3 normal;

uniform mat4 modelview;
uniform mat4 projection;
uniform mat4 shadowMat;

out vec2 tc;
out vec4 shadow_coord;
out vec3 norm;
out vec3 pos;

void main() {
  gl_Position = projection*modelview*vec4(vertex, 1.0);
  tc = tex_coord;
  shadow_coord = shadowMat * vec4(vertex, 1.0);
  pos = vertex.xyz;
  norm = normal;
}
";

        string fragment_shader_src = @"
#version 450 core
in vec2 tc;
in vec4 shadow_coord;
in vec3 norm;


uniform sampler2D tex;
uniform sampler2D shadow;
uniform vec3 light;

out vec4 FragColor;
void main() {
  float shadow_shade = 1.0;
  if (texture2D(shadow, (shadow_coord.xy*0.5)/shadow_coord.w+vec2(0.5)).x < shadow_coord.z*0.5/shadow_coord.w+0.49995){
    shadow_shade = 0.4;
  }
  float light_shade = 0.5+max(0.0, dot(norm, light));

  FragColor = texture2D(tex, tc)*min(light_shade, shadow_shade);
}
";

        string shadow_shader_src = @"
attribute vec3 vertex;

uniform mat4 modelview;
uniform mat4 projection;
void main() {
  gl_Position = projection*modelview*vec4(vertex, 1.0);
}
";

        public Branch Root;

        List<Branch> allBranches = new List<Branch>();

        public void AddBranch(Branch branch)
        {
            allBranches.Add(branch);
        }

        int shader;

        int shadow_shader;

        int vao;
        int vbo;

        public int Texture;

        public int ShadowTexture;
        public Matrix4 ShadowMatrix;

        int polyEdges = 12;

//        float thickK = 0.8f;

        public Vector3 Light;

        int totalVertices;

        VertexData[] GenVertices()
        {
            VertexData[] vertices = new VertexData[polyEdges*6*allBranches.Count];

            int vindex = 0;
            foreach (var branch in allBranches)
            {
                Matrix4 t = branch.GetBranchTransform();
                Matrix4 p = Matrix4.Identity;

                var parentThickness = branch.Thickness;
                var parentLength = 0.0f;

                if (branch.parent != null)
                {
                    p = branch.parent.GetBranchTransform();
                    parentThickness = branch.parent.Thickness;
                    parentLength = branch.parent.Length;
                }
                else
                    parentThickness = branch.Thickness * 1.2f;


                for (int i = 0; i < polyEdges; i++)
                {
                    float angle = (float)(Math.PI*2*i/polyEdges);
                    float angle2 = (float)(Math.PI*2*(i+1)/polyEdges);
                    Vector4 coord;

                    Vector3 normvec = (new Vector4(Vector3.Cross(Vector3.UnitY, new Vector3((float)(Math.Sin((angle+angle2)/2.0)), 0.0f, (float)(-Math.Cos((angle+angle2)/2.0)))), 0.0f)*t).Xyz.Normalized();

                    coord = new Vector4((float)(parentThickness * Math.Cos(angle)), parentLength, (float)(parentThickness * Math.Sin(angle)), 1.0f)*p;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)i/(float)(polyEdges-1), 0.0f) };

                    coord = new Vector4((float)(branch.Thickness * Math.Cos(angle)), branch.Length, (float)(branch.Thickness * Math.Sin(angle)), 1.0f)*t;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)i/(float)(polyEdges-1), 1.0f) };

                    coord = new Vector4((float)(parentThickness * Math.Cos(angle2)), parentLength, (float)(parentThickness * Math.Sin(angle2)), 1.0f)*p;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)(i+1) / (float)(polyEdges - 1), 0.0f) };


                    coord = new Vector4((float)(parentThickness * Math.Cos(angle2)), parentLength, (float)(parentThickness * Math.Sin(angle2)), 1.0f)*p;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)(i + 1) / (float)(polyEdges - 1), 0.0f) };

                    coord = new Vector4((float)(branch.Thickness * Math.Cos(angle)), branch.Length, (float)(branch.Thickness * Math.Sin(angle)), 1.0f)*t;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)i / (float)(polyEdges - 1), 1.0f) };

                    coord = new Vector4((float)(branch.Thickness * Math.Cos(angle2)), branch.Length, (float)(branch.Thickness * Math.Sin(angle2)), 1.0f)*t;
                    vertices[vindex++] = new VertexData { position = coord.Xyz, normal = normvec, tex_coord = new Vector2((float)(i + 1) / (float)(polyEdges - 1), 1.0f) };

                }
            }

            return vertices;
        }

        public void Buffer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            var vertices = GenVertices();

            totalVertices = vertices.Length;

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "position"));
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "tex_coord"));
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "normal"));

            GL.BindVertexArray(0);

            shader = GL.CreateProgram();
            int vertex_shader = GL.CreateShader(ShaderType.VertexShader);
            int fragment_shader = GL.CreateShader(ShaderType.FragmentShader);

            string result;

            GL.ShaderSource(vertex_shader, vertex_shader_src);
            GL.CompileShader(vertex_shader);
            result = GL.GetShaderInfoLog(vertex_shader);


            GL.ShaderSource(fragment_shader, fragment_shader_src);
            GL.CompileShader(fragment_shader);
            result = GL.GetShaderInfoLog(fragment_shader);

            GL.AttachShader(shader, vertex_shader);
            GL.AttachShader(shader, fragment_shader);

            GL.BindAttribLocation(shader, 0, "vertex");
            GL.BindAttribLocation(shader, 1, "tex_coord");
            GL.BindAttribLocation(shader, 2, "normal");

            GL.LinkProgram(shader);
            result = GL.GetProgramInfoLog(shader);

            int LinkRes = 0;
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out LinkRes);
            if (LinkRes != 1)
                return;

            shadow_shader = GL.CreateProgram();
            int shadow_vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shadow_vertex, shadow_shader_src);
            GL.CompileShader(shadow_vertex);
            GL.AttachShader(shadow_shader, shadow_vertex);
            GL.BindAttribLocation(shadow_shader, 0, "vertex");
            GL.LinkProgram(shadow_shader);
        }

        public void Render(Matrix4 projection, Matrix4 modelView)
        {
            GL.BindVertexArray(vao);
            GL.UseProgram(shader);

            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelview"), false, ref modelView);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "shadowMat"), false, ref ShadowMatrix);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex"), 0);
            GL.Uniform1(GL.GetUniformLocation(shader, "shadow"), 1);
            GL.Uniform3(GL.GetUniformLocation(shader, "light"), ref Light);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, ShadowTexture);

            GL.DrawArrays(PrimitiveType.Triangles, 0, totalVertices);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindVertexArray(0);
        }

        public void RenderShadow(Matrix4 lightProjection, Matrix4 lightModelView, int shadowTexture, int shadowSize)
        {
            GL.ColorMask(false, false, false, false);
            GL.Viewport(0, 0, shadowSize, shadowSize);
            GL.Clear(ClearBufferMask.DepthBufferBit);
//            GL.Disable(EnableCap.Texture2D);

            int local_shader = shader;

            GL.BindVertexArray(vao);
            GL.UseProgram(local_shader);
            GL.UniformMatrix4(GL.GetUniformLocation(local_shader, "modelview"), false, ref lightModelView);
            GL.UniformMatrix4(GL.GetUniformLocation(local_shader, "projection"), false, ref lightProjection);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, totalVertices);
            GL.BindVertexArray(0);

            GL.UseProgram(0);
            GL.ColorMask(true, true, true, true);

        }
    }
}
