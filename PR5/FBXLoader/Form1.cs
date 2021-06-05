using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FBXLoader
{
    public partial class Form1 : Form
    {
        
        Matrix4 cam;
        int shader;
        int vertex_shader;
        int fragment_shader;
        float distance = 5.0f;
        float rotx;
        float roty;
        float camx;
        float camy;
        float camz;
        Point m;

        bool m_fwd, m_back, m_left, m_right;
        bool m_fast;

        Model house;
        Background back;

        string vertex_shader_src = @"
attribute vec3 vertex;
attribute vec2 tex_coord;
//attribute vec3 normal;

uniform mat4 modelview;
uniform mat4 projection;

varying vec2 tc;

void main() {
  vec4 vert = vec4(vertex, 1.0);
  gl_Position = projection*modelview*vert;
  tc = tex_coord;
}
";

        string fragment_shader_src = @"
varying vec2 tc;
uniform sampler2D tex;

void main() {
  vec4 c = texture2D(tex, tc);
  gl_FragColor = c;
}
";


        public Form1()
        {
            InitializeComponent();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            float aspect = (float)glControl.Width / (float)glControl.Height;
            cam = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 0.1f, 1000.0f);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        private void glControl_MouseWheel(object Sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                distance *= 1.2f;
            else if (e.Delta > 0)
                distance /= 1.2f;
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            m = new Point(e.X, e.Y);
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                rotx += (e.X - m.X) / 2.0f;
                roty += (e.Y - m.Y) / 2.0f;
            }
            if (e.Button == MouseButtons.Left)
            {
                camy += (e.Y - m.Y) / 100.0f;
                if (camy < 0.0f) camy = 0.0f;
                if (camy > 2.0f) camy = 2.0f;
            }
            m.X = e.X;
            m.Y = e.Y;
        }

        private void RenderFrame(object sender, EventArgs e)
        {
            float speed = 0.1f;
            if (m_fast)
                speed = 1.0f;

            if (m_fwd)
            {
                camx -= (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camz += (float)(speed * Math.Cos(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camy += (float)(speed * Math.Sin(roty / 180.0 * Math.PI));
            }
            else if (m_back)
            {
                camx += (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camz -= (float)(speed * Math.Cos(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camy -= (float)(speed * Math.Sin(roty / 180.0 * Math.PI));
            }
            else if(m_left)
            {
                camx += (float)(speed * Math.Cos(rotx / 180.0 * Math.PI));
                camz -= (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camy -= (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));

            }
            else if (m_right)
            {
                camx -= (float)(speed * Math.Cos(rotx / 180.0 * Math.PI));
                camz -= (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
                camy += (float)(speed * Math.Sin(rotx / 180.0 * Math.PI) * Math.Cos((roty / 180.0 * Math.PI)));
            }


            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            //            i++;

            Matrix4 projection = cam;
            Matrix4 real_cam = Matrix4.Identity;

            real_cam = Matrix4.CreateRotationX((float)(roty / 180.0 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateRotationY((float)(rotx / 180.0 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateTranslation(camx, camy, camz)*real_cam;
            real_cam = Matrix4.CreateTranslation(0.0f, 0.0f, -distance) * real_cam;
            real_cam = Matrix4.CreateRotationX((float)(roty / 180 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateRotationY((float)(rotx / 180 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateTranslation(0.0f, -camy, 0.0f) * real_cam;
            Matrix4 light_mat = real_cam;
            //  real_cam = Matrix4.CreateRotationY((float)((double)i / 180.0 * Math.PI)) * real_cam;

            Matrix3 normalMatrix = new Matrix3(real_cam);
            normalMatrix.Invert();
            normalMatrix.Transpose();

            back.Render(projection, real_cam);

            GL.UseProgram(shader);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelview"), false, ref real_cam);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);
                        GL.UniformMatrix3(GL.GetUniformLocation(shader, "normal_matrix"), false, ref normalMatrix);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex"), 0);

                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            house.Render();

                        GL.Disable(EnableCap.Blend);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);

            glControl.SwapBuffers();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            float aspect = (float)glControl.Width / (float)glControl.Height;
            cam = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 1f, 1000.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            house = Model.FromFile("new_house.fbx");
            back = new Background("heightmap.png", "texturemap3.png", -15.2f);
            int bgtex1 = Textures.AddTexture("pebbles.jpg");
            int bgtex2 = Textures.AddTexture("grass.jpg");
            int bgtex3 = Textures.AddTexture("rock.jpg");
            int bgtex4 = Textures.AddTexture("sand.jpg");
            Textures.Load();
            back.texture1 = Textures.GetTexture(bgtex1).gl_id;
            back.texture2 = Textures.GetTexture(bgtex2).gl_id;
            back.texture3 = Textures.GetTexture(bgtex3).gl_id;
            back.texture4 = Textures.GetTexture(bgtex4).gl_id;
            back.Buffer();
            house.Buffer();
            

            //Skeleton skel = Skeleton.FromFile("05.asf");

            shader = GL.CreateProgram();
            vertex_shader = GL.CreateShader(ShaderType.VertexShader);
            fragment_shader = GL.CreateShader(ShaderType.FragmentShader);

            string result;
//            logTextBox.AppendText(GL.GetString(StringName.ShadingLanguageVersion) + "\r\n");

            GL.ShaderSource(vertex_shader, vertex_shader_src);
            GL.CompileShader(vertex_shader);
            result = GL.GetShaderInfoLog(vertex_shader);
//            logTextBox.AppendText(result);


            GL.ShaderSource(fragment_shader, fragment_shader_src);
            GL.CompileShader(fragment_shader);
            result = GL.GetShaderInfoLog(fragment_shader);
//            logTextBox.AppendText(result);

            GL.AttachShader(shader, vertex_shader);
            GL.AttachShader(shader, fragment_shader);

            GL.BindAttribLocation(shader, 0, "vertex");
            GL.BindAttribLocation(shader, 1, "tex_coord");

            GL.LinkProgram(shader);
            result = GL.GetProgramInfoLog(shader);
//            logTextBox.AppendText(result);

            int LinkRes = 0;
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out LinkRes);
            if (LinkRes != 1)
                return;
            timer1.Enabled = true;
        }

        private void glControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                m_fwd = true;
            if (e.KeyCode == Keys.S)
                m_back = true;
            if (e.KeyCode == Keys.ShiftKey)
                m_fast = true;
            if (e.KeyCode == Keys.A)
                m_left = true;
            if (e.KeyCode == Keys.D)
                m_right = true;
        }

        private void glControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                m_fwd = false;
            if (e.KeyCode == Keys.S)
                m_back = false;
            if (e.KeyCode == Keys.ShiftKey)
                m_fast = false;
            if (e.KeyCode == Keys.D)
                m_right = false;
            if (e.KeyCode == Keys.A)
                m_left = false;

        }
    }
}

