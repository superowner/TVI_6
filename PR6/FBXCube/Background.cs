using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace FBXCube
{
    class Background
    {



        struct VertexData
        {
            public Vector3 position;
            public Vector2 tex_coord;
        }

        VertexData[] vertexes = { new VertexData { position = new Vector3( 2.0f,  2.0f, 0.0f), tex_coord = new Vector2(1.0f, 1.0f) },
                                  new VertexData { position = new Vector3(-2.0f,  2.0f, 0.0f), tex_coord = new Vector2(0.0f, 1.0f) },
                                  new VertexData { position = new Vector3( 2.0f, -2.0f, 0.0f), tex_coord = new Vector2(1.0f, 0.0f) },
                                  new VertexData { position = new Vector3(-2.0f, -2.0f, 0.0f), tex_coord = new Vector2(0.0f, 0.0f) } };

        int vao;
        int vbo;
        int shader;

        string vertex_shader_src = @"
attribute vec3 vertex;
attribute vec2 tex_coord;

uniform mat4 modelview;
uniform mat4 projection;

varying vec2 tc;

void main() {
  gl_Position = projection*modelview*vec4(vertex, 1.0);
  tc = tex_coord;
}
";

        string fragment_shader_src = @"
varying vec2 tc;
uniform sampler2D tex;

void main() {
  gl_FragColor = texture2D(tex, tc);
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

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * vertexes.Length, vertexes, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
//            GL.EnableVertexAttribArray(2);
//            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "position"));
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "tex_coord"));

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
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }
    }
}
