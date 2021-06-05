using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FBXLoader
{
    class Background
    {



        struct VertexData
        {
            public Vector3 position;
            public Vector2 tex_coord;
            public Vector4 tex_weights;
        }

/*        VertexData[] vertexes = { new VertexData { position = new Vector3(20.0f, -1.0f, 20.0f), tex_coord = new Vector2(10.0f, 10.0f) },
                                  new VertexData { position = new Vector3(-20.0f, -1.0f, 20.0f), tex_coord = new Vector2(0.0f, 10.0f) },
                                  new VertexData { position = new Vector3(20.0f, -1.0f, -20.0f), tex_coord = new Vector2(10.0f, 0.0f) },
                                  new VertexData { position = new Vector3(-20.0f, -1.0f, -20.0f), tex_coord = new Vector2(0.0f, 0.0f) } };*/
        VertexData[] vertices;
        int[] indices;


        int vao;
        int vbo;
        int vio;
        int shader;
        public int texture1, texture2, texture3, texture4;

        string vertex_shader_src = @"
attribute vec3 vertex;
attribute vec2 tex_coord;
attribute vec4 tex_weights;

uniform mat4 modelview;
uniform mat4 projection;

varying vec2 tc;
varying vec4 texweights;

void main() {
  gl_Position = projection*modelview*vec4(vertex, 1.0);
  tc = tex_coord;
  texweights = tex_weights;
}
";

        string fragment_shader_src = @"
varying vec2 tc;
uniform sampler2D tex1;
uniform sampler2D tex2;
uniform sampler2D tex3;
uniform sampler2D tex4;
varying vec4 texweights;

void main() {
  //gl_FragColor = vec4(0.0, 0.0, 0.0, 1.0);
  gl_FragColor = texture2D(tex1, tc)*texweights.r+
                 texture2D(tex2, tc)*texweights.g+
                 texture2D(tex3, tc)*texweights.b+
                 texture2D(tex4, tc)*texweights.a;
//  gl_FragColor = texweights;
}
";

        int mapSize = 1024;
        float mapScale = 0.25f;

        void GenerateHeightMap(BitmapData data, BitmapData texture)
        {
            vertices = new VertexData[mapSize * mapSize];
            for (int y = 0; y < mapSize; y++)
              for (int x = 0; x < mapSize; x++)
              {
                  vertices[y * mapSize + x] = new VertexData { position = new Vector3((x-mapSize/2)*mapScale, ((UInt16)Marshal.ReadByte(data.Scan0, data.Stride*y+4*x))*hscale+hoffset, (y-mapSize/2)*mapScale), tex_coord = new Vector2(x, y),
                                                               tex_weights = new Vector4(Marshal.ReadByte(texture.Scan0 + data.Stride * y + x * 4 + 0) / 255.0f, Marshal.ReadByte(texture.Scan0 + data.Stride * y + x * 4 + 1) / 255.0f,
                                                               Marshal.ReadByte(texture.Scan0 + data.Stride * y + x * 4 + 2) / 255.0f, Marshal.ReadByte(texture.Scan0 + data.Stride * y + x * 4 + 3) / 255.0f)
                  };
              }
            indices = new int[(2 * mapSize + 4) * (mapSize - 2) + 4];
            int curIndex = 0;
            for (int stripe = 0; stripe < mapSize - 1; stripe++)
            {
                indices[curIndex++] = stripe * mapSize;
                indices[curIndex++] = (stripe + 1) * mapSize;
                for (int cell = 0; cell < mapSize - 1; cell++)
                {
                    indices[curIndex++] = stripe * mapSize + cell + 1;
                    indices[curIndex++] = (stripe+1) * mapSize + cell + 1;
                }
                if (stripe < mapSize - 2)
                {
                    indices[curIndex++] = (stripe + 1) * mapSize + mapSize - 1;
                    indices[curIndex++] = (stripe + 1) * mapSize;
                }
            }
        }

        string heightmap;
        string texmap;

        float hoffset = 0.0f, hscale = 0.1f;

        public Background(string HeightMapFile, string TextureMapFile, float Offset = 0.0f, float Scale = 0.1f)
        {
            heightmap = HeightMapFile;
            texmap = TextureMapFile;
            hoffset = Offset;
            hscale = Scale;
        }

        public void Buffer()
        {
            Bitmap bmp = Image.FromFile(heightmap) as Bitmap;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, mapSize, mapSize), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Bitmap tex = Image.FromFile(texmap) as Bitmap;
            BitmapData texdata = tex.LockBits(new Rectangle(0, 0, mapSize, mapSize), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GenerateHeightMap(data, texdata);
            bmp.UnlockBits(data);
            tex.UnlockBits(texdata);

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            vio = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vio);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "position"));
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "tex_coord"));
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "tex_weights"));

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
            GL.Uniform1(GL.GetUniformLocation(shader, "tex1"), (int)0);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex2"), (int)1);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex3"), (int)2);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex4"), (int)3);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, texture3);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, texture4);

            GL.DrawElements(PrimitiveType.TriangleStrip, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindVertexArray(0);
        }
    }
}
