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
    class Background
    {



        struct VertexData
        {
            public Vector3 position;
            public Vector2 tex_coord;
        }

        VertexData[] vertexes = { new VertexData { position = new Vector3(20.0f, 0.0f, 20.0f), tex_coord = new Vector2(10.0f, 10.0f) },
                                  new VertexData { position = new Vector3(-20.0f, 0.0f, 20.0f), tex_coord = new Vector2(0.0f, 10.0f) },
                                  new VertexData { position = new Vector3(20.0f, 0.0f, -20.0f), tex_coord = new Vector2(10.0f, 0.0f) },
                                  new VertexData { position = new Vector3(-20.0f, 0.0f, -20.0f), tex_coord = new Vector2(0.0f, 0.0f) } };

        int vao;
        int vbo;
        int shader;
        public int texture;
        public int ShadowTexture;

        public Matrix4 ShadowMatrix = Matrix4.Identity;

        string vertex_shader_src = @"
attribute vec3 vertex;
attribute vec2 tex_coord;

uniform mat4 modelview;
uniform mat4 projection;
uniform mat4 shadowMat;

varying vec2 tc;
varying vec4 shadow_coord;

void main() {
  gl_Position = projection*modelview*vec4(vertex, 1.0);
  tc = tex_coord;
  shadow_coord = shadowMat * vec4(vertex, 1.0);
}
";

        string fragment_shader_src = @"
varying vec2 tc;
varying vec4 shadow_coord;
uniform sampler2D tex;
uniform sampler2D shadow;
void main() {
  float visibility = 1.0;
  if (texture2D(shadow, (shadow_coord.xy*0.5)/shadow_coord.w+vec2(0.5)).x < shadow_coord.z*0.5/shadow_coord.w+0.5){
    visibility = 0.5;
  }
  gl_FragColor = texture2D(tex, tc)*visibility;
}
";


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

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * vertexes.Length, vertexes, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "position"));
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "tex_coord"));

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

            GL.LinkProgram(shader);
            result = GL.GetProgramInfoLog(shader);

            int LinkRes = 0;
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out LinkRes);
            if (LinkRes != 1)
                return;
        }

        public void Render(Matrix4 projection, Matrix4 modelview)
        {
            GL.BindVertexArray(vao);
            GL.UseProgram(shader);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelview"), false, ref modelview);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "shadowMat"), false, ref ShadowMatrix);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex"), 0);
            GL.Uniform1(GL.GetUniformLocation(shader, "shadow"), 1);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, ShadowTexture);

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindVertexArray(0);
        }
    }
}
