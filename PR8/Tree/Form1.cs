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
using System.Runtime.InteropServices;

namespace Tree
{
    public partial class Form1 : Form
    {
        Background bg;

        int bg_texindex;
        int bark_texindex;
        int fireflare_texindex;

        Matrix4 projection, modelView;

        float FieldOfView = 90.0f;

        float distance = 5.0f;
        float rotx;
        float roty;
        float camy = 2.0f;
        Point m;

        int shadow_size = 2048;

        int shadow_buffer;

        Tree tree = new Tree();

        Fire fire = new Fire();

        int shadow_texture;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bg = new Background();
            bg.Buffer();
            bg_texindex = Textures.AddTexture("Grass15.jpg");
            bark_texindex = Textures.AddTexture("bark_c.png");
            fireflare_texindex = Textures.AddTexture("flare.png");

            Textures.Load();
            bg.texture = Textures.GetTexture(bg_texindex).gl_id;

            CreateProjection();
            modelView = Matrix4.CreateTranslation(0, 0, -5.0f);
            GenerateTree();
            tree.Texture = Textures.GetTexture(bark_texindex).gl_id;
            tree.Buffer();

            fire.Init();
            fire.ParticleTexture = Textures.GetTexture(fireflare_texindex).gl_id;
            fire.Buffer();

            shadow_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, shadow_texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadow_size, shadow_size, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            tree.ShadowTexture = shadow_texture;
            bg.ShadowTexture = shadow_texture;

            shadow_buffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadow_buffer);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, shadow_texture, 0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                MessageBox.Show(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString());

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.DepthTest);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            CreateProjection();
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
                if (camy > 10.0f) camy = 10.0f;
            }
            m.X = e.X;
            m.Y = e.Y;
        }

        Random rnd = new Random();

        float[] finish_probability = { 0.0f, 0.05f, 0.01f, 0.25f, 0.40f, 0.85f, 0.95f, 1.0f };

        void CreateProjection()
        {
            float aspect = (float)glControl1.Width/(float)glControl1.Height;
            projection = Matrix4.CreatePerspectiveFieldOfView((float)(FieldOfView*Math.PI/180.0) / aspect, aspect, 1.0f, 1000.0f);
        }

        void GenTreeStep(int StepIndex, Tree.Branch parent)
        {
            Tree.Branch b = new Tree.Branch();
            b.parent = parent;
            if (parent != null)
                b.Thickness = parent.Thickness * 0.8f;
            else
                b.Thickness = (float)(rnd.NextDouble() * 0.5 + 0.25);
            b.Angle1 = rnd.Next(-30, 30);
            b.Angle2 = rnd.Next(-30, 30);
            b.Length = (float)(rnd.NextDouble()*2.0+1.0);
            tree.AddBranch(b);
            if (rnd.NextDouble() >= finish_probability[StepIndex])
            {
                GenTreeStep(StepIndex + 1, b);
                GenTreeStep(StepIndex + 1, b);
            }
        }

        void GenerateTree()
        {
            /*   Tree.Branch b = new Tree.Branch();
            b.Length = 2.0f;
            b.Tickness = 0.1f;
            b.parent = null;
            tree.AddBranch(b);*/
            GenTreeStep(0, null);
        }

        void Render()
        {
            Matrix4 lightProjection = Matrix4.CreateOrthographic(40, 40, -100.0f, 100.0f);
            Matrix4 lightPos = Matrix4.CreateTranslation(4.0f, 0.0f, 0.0f) * Matrix4.CreateRotationY((float)(azimuthTrack.Value * Math.PI / 180.0)) * Matrix4.CreateRotationX((float)(elevationTrack.Value * Math.PI / 180.0)) * Matrix4.CreateTranslation(0.0f, 0.0f, -25.0f);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadow_buffer);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            tree.RenderShadow(lightProjection, Matrix4.CreateTranslation(4.0f, 0.0f, 0.0f) * lightPos, shadow_texture, shadow_size);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            bg.ShadowMatrix = lightPos * lightProjection;
            tree.ShadowMatrix = lightPos * lightProjection;
            tree.Light = (Matrix4.CreateRotationY((float)(azimuthTrack.Value * Math.PI / 180.0)) * Matrix4.CreateRotationX((float)(elevationTrack.Value * Math.PI / 180.0)) * (-Vector4.UnitZ)).Xyz;

            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);


            Matrix4 modelView = Matrix4.CreateTranslation(0.0f, -camy, 0.0f) * Matrix4.CreateRotationY((float)(rotx / 180 * Math.PI)) * Matrix4.CreateRotationX((float)(roty / 180 * Math.PI)) * Matrix4.CreateTranslation(0.0f, 0.0f, -distance);

            fire.NextFrame();

            bg.Render(projection, modelView);
            tree.Render(projection, Matrix4.CreateTranslation(4.0f, 0.0f, 0.0f) * modelView);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.DepthMask(false);

            fire.Render(projection, modelView);

            GL.DepthMask(true);

            GL.Disable(EnableCap.Blend);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Render();

            glControl1.SwapBuffers();
        }
    }
}
