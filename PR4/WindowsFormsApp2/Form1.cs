using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace WindowsFormsApp2
{
    struct Vertex
    {
        public Vector4 position;
        public Vector4 color;
        public Vector2 tex_coord;
        public Vector3 normal;
    }
    public partial class Form1 : Form
    {
        int LoadTexture(string filename)
        {
            int res = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, res);
            Bitmap teximage = Image.FromFile(filename) as Bitmap;
            BitmapData texdata = teximage.LockBits(new Rectangle(0, 0, teximage.Width, teximage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, teximage.Width, teximage.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, texdata.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            teximage.UnlockBits(texdata);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return res;

        }

        Vertex[,] cube = new Vertex[,] {

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4( 1,-1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color2Vector(Color.Orange), normal = new Vector3(0,0,-1), tex_coord = new Vector2(0, 1)} }, //1

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4( 1,-1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4( 1,-1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1, 1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color2Vector(Color.Violet), normal = new Vector3(0,0,1), tex_coord = new Vector2(0, 1)} }, //2

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4(-1,-1, 1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color2Vector(Color.Blue), normal = new Vector3(-1,0,0), tex_coord = new Vector2(0, 1)} }, //3

                                             { new Vertex {position = new Vector4(1,-1,-1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4(1,-1, 1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(1, 1,-1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4(1,-1, 1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4(1, 1, 1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4(1, 1,-1,1), color = Color2Vector(Color.Pink), normal = new Vector3(1,0,0), tex_coord = new Vector2(0, 1)} }, //4

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4(-1,-1, 1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1,-1, 1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color2Vector(Color.Magenta), normal = new Vector3(0,-1,0), tex_coord = new Vector2(0, 1)} }, //5

                                             { new Vertex {position = new Vector4(-1, 1,-1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(0, 0)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(0, 1)} },

                                             { new Vertex {position = new Vector4(-1, 1, 1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(1, 0)},
                                               new Vertex {position = new Vector4( 1, 1, 1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(1, 1)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color2Vector(Color.Red), normal = new Vector3(0,1,0), tex_coord = new Vector2(0, 1)} }, //6
                                          };

        float rotx = 0.0f;
        float roty = 0.0f;
        float rotz = 0.0f;
        int cube_texture;
        int cube_buffer;
        int vertex_pgm, fragment_pgm;
        int shader;
        int position_attrib, texcoord_attrib, color_attrib;
        string VertexProgram = @"#version 330 core
                                    in vec4 position;
                                    in vec2 texcoord;
                                    in vec4 color;
                                    uniform mat4 modelView;
                                    uniform mat4 projection;
                                    out vec2 tc;
                                    out vec4 col;
                                    void main() {
                                        tc = texcoord;
                                        col = color;
                                        gl_Position = projection*modelView*position, 1.0;
                                    }";
        string FragmentProgram = @"#version 330 core
                                    in vec2 tc;
                                    in vec4 col;
                                    uniform sampler2D txtr;
                                    out vec4 color;
                                    float radius = 0.25;
                                    void main() {
                                        if( col.r >= 0.95 && col.g >= 0.6469 && col.g <= 0.6471 && col.b == 0.0)
                                            if ((tc.x-0.5)*(tc.x-0.5)+(tc.y-0.5)*(tc.y-0.5) < radius*radius)
                                                discard;
                                        if(col.r >= 0.92 && col.r <= 0.94 && col.g >= 0.5 && col.g <= 0.52 && col.b >= 0.92 && col.b <= 0.94){
                                            if ((tc.x-0.25)*(tc.x-0.25)+(tc.y-0.25)*(tc.y-0.25) < radius*radius / 2)
                                                discard;
                                            if ((tc.x-0.75)*(tc.x-0.75)+(tc.y-0.25)*(tc.y-0.25) < radius*radius / 2)
                                                discard;
                                            if ((tc.x-0.25)*(tc.x-0.25)+(tc.y-0.75)*(tc.y-0.75) < radius*radius / 2)
                                                discard;
                                            if ((tc.x-0.75)*(tc.x-0.75)+(tc.y-0.75)*(tc.y-0.75) < radius*radius / 2)
                                                discard;
                                        }
                                        if(col.r <= 0.1 && col.g <= 0.1 && col.b >= 0.95){
                                            if((tc.y >= 0.26 && tc.y <= 0.36) || (tc.y >= 0.66 && tc.y <= 0.76))
                                                discard;
                                        }
                                        if(col.r >= 0.95 && col.g >= 0.75 && col.g <= 0.76 && col.b >= 0.79 && col.b <= 0.80){
                                            if( (tc.y >= 0.4 && tc.y <= 0.6) || (tc.x >= 0.4 && tc.x <= 0.6))
                                                discard;
                                        }
                                        if(col.r >= 0.95 && col.g == 0.0 && col.b >= 0.95){
                                            if( (abs(tc.x - tc.y)) <= 0.1 || (abs(tc.x + tc.y - 1)) <= 0.1)
                                                discard;
                                        }
                                        if(col.r >= 0.95 && col.g == 0.0 && col.b == 0.0){
                                            if( (abs(tc.x - tc.y) <= 0.1 || abs(tc.x + tc.y - 1) <= 0.1) 
                                                && !((tc.x >= 0.35) && (tc.x <= 0.65) && tc.y >= 0.35 && tc.y <= 0.65 && (abs(tc.x - tc.y) <= 0.15) && (abs(tc.x + tc.y - 1) <= 0.15)))
                                                discard;
                                        }
                                        vec4 texel = texture2D(txtr, tc);
                                        color = texel*col;
                                    }";
        Matrix4 projection;
        Matrix4 modelView; 

        public Form1()
        {
            InitializeComponent();
        }

        public static Vector4 Color2Vector(Color color)
        {
            return new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, 1.0f);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            float aspect = (float)glControl1.Width / glControl1.Height;
            projection = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 1.0f, 100.0f);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            cube_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, cube_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(Vertex)) * cube.Length, cube, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            cube_texture = LoadTexture("texture.jpg");
            vertex_pgm = GL.CreateShader(ShaderType.VertexShader);
            fragment_pgm = GL.CreateShader(ShaderType.FragmentShader);
            shader = GL.CreateProgram();

            int success;
            GL.ShaderSource(vertex_pgm, VertexProgram);
            GL.CompileShader(vertex_pgm);
            GL.GetShader(vertex_pgm, ShaderParameter.CompileStatus, out success);
            if (success == 0)
                MessageBox.Show(GL.GetShaderInfoLog(vertex_pgm));

            GL.ShaderSource(fragment_pgm, FragmentProgram);
            GL.CompileShader(fragment_pgm);
            GL.GetShader(fragment_pgm, ShaderParameter.CompileStatus, out success);
            if (success == 0)
                MessageBox.Show(GL.GetShaderInfoLog(fragment_pgm));

            GL.AttachShader(shader, vertex_pgm);
            GL.AttachShader(shader, fragment_pgm);
            GL.LinkProgram(shader);
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out success);
            if (success != 1)
                MessageBox.Show(GL.GetProgramInfoLog(shader));

            position_attrib = GL.GetAttribLocation(shader, "position");
            color_attrib = GL.GetAttribLocation(shader, "color");
            texcoord_attrib = GL.GetAttribLocation(shader, "texcoord");

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            rotx += 3.5f;
            roty += 0.5f;
            rotz += 0.1f;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            modelView = Matrix4.CreateRotationX((float)(rotx * Math.PI / 180.0f)) *
                Matrix4.CreateRotationY((float)(roty * Math.PI / 180.0f)) *
                Matrix4.CreateRotationZ((float)(rotz * Math.PI / 180.0f)) *
                Matrix4.CreateTranslation(0.0f, 0.0f, -5.0f);

            GL.BindBuffer(BufferTarget.ArrayBuffer, cube_buffer);
            GL.UseProgram(shader);

            GL.EnableVertexAttribArray(position_attrib);
            GL.EnableVertexAttribArray(color_attrib);
            GL.EnableVertexAttribArray(texcoord_attrib);

            GL.VertexAttribPointer(position_attrib, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(Vertex)), (int)Marshal.OffsetOf(typeof(Vertex), "position"));
            GL.VertexAttribPointer(color_attrib, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(Vertex)), (int)Marshal.OffsetOf(typeof(Vertex), "color"));
            GL.VertexAttribPointer(texcoord_attrib, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(Vertex)), (int)Marshal.OffsetOf(typeof(Vertex), "tex_coord"));

            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelView"), false, ref modelView);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);
            GL.Uniform1(GL.GetUniformLocation(shader, "txtr"), 0);
            GL.BindTexture(TextureTarget.Texture2D, cube_texture);
            GL.DrawArrays(PrimitiveType.Triangles, 0, cube.Length);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DisableVertexAttribArray(position_attrib);
            GL.DisableVertexAttribArray(color_attrib);
            GL.DisableVertexAttribArray(texcoord_attrib);

            GL.UseProgram(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, cube_buffer);
            glControl1.SwapBuffers();

        }
    }
}
