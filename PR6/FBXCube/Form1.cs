using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FbxSharp;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace FBXCube
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            orderComboBox.SelectedIndex = 0;
        }

        Matrix4 cam;

        Background back;

        int shader;
        int vertex_shader;
        int fragment_shader;

        float distance = 5.0f;

        float rotx, roty;
        float camy = 1.0f;

        int bgtex;

        Point m;
        string vertex_shader_src = @"
attribute vec3 vertex;
attribute vec2 tex_coord;
attribute vec4 bone_indices;
attribute vec4 bone_weights;
//attribute vec3 normal;

uniform mat4 modelview;
uniform mat4 projection;

uniform mat4 bones[100];

varying vec2 tc;

void main() {
  vec4 vert;
//  vec4 vert = vec4(vertex, 1.0);

  int index = int(bone_indices.x);
  vert = bone_weights.x*(bones[index]*vec4(vertex, 1.0));
  index = int(bone_indices.y);
  if (index >= 0)
    vert += bone_weights.y*(bones[index]*vec4(vertex, 1.0));
  index = int(bone_indices.z);
  if (index >= 0)
    vert += bone_weights.z*(bones[index]*vec4(vertex, 1.0));
  index = int(bone_indices.w);
  if (index >= 0)
    vert += bone_weights.w*(bones[index]*vec4(vertex, 1.0));

  gl_Position = projection*modelview*vert;
  tc = tex_coord;
}
";

        string fragment_shader_src = @"
varying vec2 tc;
uniform sampler2D tex;

void main() {
  vec4 c = texture2D(tex, tc);
  if (c.a < 0.95)
    discard;
  else
    gl_FragColor = c;
}
";

        Model model;
        Bvh skeleton;
        GFXSkeleton gfxSkel;

        Dictionary<string, string> bone_dict = new Dictionary<string, string>();
        Matrix4 motion_trans = Matrix4.CreateRotationX(-(float)(Math.PI / 2));


        private void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);
            float aspect = (float)glControl.Width / (float)glControl.Height;
            cam = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 1.0f, 1000.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            back = new Background();
            int bgtexindex = Textures.AddTexture("textures\\grass.png");
            model = Model.FromFile("my_human\\human_copy2.fbx");
            Textures.Load();
            model.Buffer();
            back.Buffer();
            GFXBone.Buffer();

            bgtex = Textures.GetTexture(bgtexindex).gl_id;
            skeleton = new Bvh("jumpy.bvh");

            gfxSkel = GFXSkeleton.FromBVH(skeleton);

            animationTrackBar.Maximum = skeleton.framesNum - 1;

            shader = GL.CreateProgram();
            vertex_shader = GL.CreateShader(ShaderType.VertexShader);
            fragment_shader = GL.CreateShader(ShaderType.FragmentShader);

            string result;
            logTextBox.AppendText(GL.GetString(StringName.ShadingLanguageVersion) + "\r\n");

            GL.ShaderSource(vertex_shader, vertex_shader_src);
            GL.CompileShader(vertex_shader);
            result = GL.GetShaderInfoLog(vertex_shader);
            logTextBox.AppendText(result);


            GL.ShaderSource(fragment_shader, fragment_shader_src);
            GL.CompileShader(fragment_shader);
            result = GL.GetShaderInfoLog(fragment_shader);
            logTextBox.AppendText(result);

            GL.AttachShader(shader, vertex_shader);
            GL.AttachShader(shader, fragment_shader);

            GL.BindAttribLocation(shader, 0, "vertex");
            GL.BindAttribLocation(shader, 1, "tex_coord");
            GL.BindAttribLocation(shader, 2, "bone_indices");
            GL.BindAttribLocation(shader, 3, "bone_weights");

            GL.LinkProgram(shader);
            result = GL.GetProgramInfoLog(shader);
            logTextBox.AppendText(result);

            int LinkRes = 0;
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out LinkRes);
            if (LinkRes != 1)
                return;
           
            bone_dict.Add("Model::Hips", "Hips");
            bone_dict.Add("Model::LHipJoint", "LHipJoint");
            bone_dict.Add("Model::RHipJoint", "RHipJoint");
            bone_dict.Add("Model::LeftUpLeg", "LeftUpLeg");
            bone_dict.Add("Model::RightUpLeg", "RightUpLeg");

            bone_dict.Add("Model::LeftLeg", "LeftLeg");
            bone_dict.Add("Model::RightLeg", "RightLeg");
            bone_dict.Add("Model::LeftFoot", "LeftFoot");
            bone_dict.Add("Model::RightFoot", "RightFoot");
            bone_dict.Add("Model::LeftToeBase", "LeftToeBase");
            bone_dict.Add("Model::RightToeBase", "RightToeBase");

            bone_dict.Add("Model::LowerBack", "LowerBack");
            bone_dict.Add("Model::Spine", "Spine");
            bone_dict.Add("Model::Spine1", "Spine1");

            bone_dict.Add("Model::LeftShoulder", "LeftShoulder");
            bone_dict.Add("Model::RightShoulder", "RightShoulder");

            bone_dict.Add("Model::LeftArm", "LeftArm");
            bone_dict.Add("Model::RightArm", "RightArm");

            bone_dict.Add("Model::LeftForeArm", "LeftForeArm");
            bone_dict.Add("Model::RightForeArm", "RightForeArm");

            bone_dict.Add("Model::LeftHand", "LeftHand");
            bone_dict.Add("Model::RightHand", "RightHand");

            bone_dict.Add("Model::LeftFingerBase", "LeftFingerBase");
            bone_dict.Add("Model::RightFingerBase", "RightFingerBase");
            bone_dict.Add("Model::LeftHandFinger1", "LeftHandFinger1");
            bone_dict.Add("Model::RightHandFinger1", "RightHandFinger1");
            bone_dict.Add("Model::LThumb", "LThumb");
            bone_dict.Add("Model::RThumb", "RThumb");


            ProcessBones(model.GetBone(0), treeView1.Nodes);

            GL.UseProgram(shader);
            makeSkeletonTransforms(model, skeleton);

            Model.Bone b = model.GetBoneByName("Model::Hips");
            Matrix4 gt = Matrix4.Identity;
            Matrix4 fx = b.fix;
            b.fix = Matrix4.Identity;
            b = b.parent;
            gt = b.gt;

            gfxSkel.gt = gt;

            b.fix = fx*b.lt;

            ApplyAnimation(0);
            treeView1.ExpandAll();
            timer1.Enabled = true;
        }

        private void ProcessBones(Model.Bone bone, TreeNodeCollection parent)
        {
            TreeNode boneNode = new TreeNode(bone.name);
            boneNode.Tag = bone;
            parent.Add(boneNode);
            foreach (Model.Bone b in bone.children)
                ProcessBones(b, boneNode.Nodes);
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

        Matrix4 motion_fix = new Matrix4(1.0f,  0.0f, 0.0f, 0.0f,
                                         0.0f,  0.0f, 1.0f, 0.0f,
                                         0.0f, -1.0f, 0.0f, 0.0f,
                                         0.0f,  0.0f, 0.0f, 1.0f);

        private Matrix4 getMotionTransform(Motion motion)
        {
            Matrix4 transform = Matrix4.Identity;

            Matrix4 mrx = Matrix4.CreateRotationX((float)((negRXcheckBox.Checked ? -1.0 : 1.0) * motion.rotation.X * Math.PI / 180.0));
            Matrix4 mry = Matrix4.CreateRotationY((float)((negRYcheckBox.Checked ? -1.0 : 1.0) * motion.rotation.Y * Math.PI / 180.0));
            Matrix4 mrz = Matrix4.CreateRotationZ((float)((negRZcheckBox.Checked ? -1.0 : 1.0) * motion.rotation.Z * Math.PI / 180.0));

            switch (orderComboBox.SelectedIndex)
            {
                case 0: transform = mrx * mry * mrz; break;     // XYZ
                case 1: transform = mrx * mrz * mry; break;     // XZY
                case 2: transform = mry * mrx * mrz; break;     // YXZ
                case 3: transform = mry * mrz * mrx; break;     // YZX
                case 4: transform = mrz * mrx * mry; break;     // ZXY
                case 5: transform = mrz * mry * mrx; break;     // ZYX
                default: transform = Matrix4.Identity; break;
            }
            transform = transform * Matrix4.CreateTranslation(motion.translation);
            return transform;
        }

        private void ApplyAnimation(int frame)
        {
            Model.Bone root = model.GetBoneByName("Model::Hips").parent;
            for (int i = 0; i < model.GetBonesCount(); i++)
            {
                Model.Bone bone = model.GetBone(i);
                string anim_bone_name;
                if (bone.name != "")
                    if (bone_dict.TryGetValue(bone.name, out anim_bone_name))
                    {
                        BvhPart animation_bone = skeleton.BvhPartsLinear.Find(abone => abone.name == anim_bone_name);
                        if (animation_bone != null)
                        {
                            if (animation_bone.parent == null && bone.parent != null)
                                bone = bone.parent;

                            bone.ResetTransform();
                            Matrix4 motion = getMotionTransform(animation_bone.motion[frame]);

                            bone.ApplyTransform(motion);
                            gfxSkel.SetTransform(animation_bone.name, motion);
                        }
                    }
            }
        }

        private void makeSkeletonTransforms(Model model, Bvh skeleton)
        {
            for (int i = 0; i < model.GetBonesCount(); i++)
            {
                Model.Bone bone = model.GetBone(i);
                bone.fix = Matrix4.Identity;
                string anim_bone_name;
                if (bone_dict.TryGetValue(bone.name, out anim_bone_name))
                {
                    BvhPart animation_bone = skeleton.BvhPartsLinear.Find(abone => abone.name == anim_bone_name);
                    if (animation_bone != null)
                        bone.fix = Matrix4.CreateTranslation(animation_bone.offset);
                }
            }
        }

        private void animationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            frameLabel.Text = String.Format("frame:\n{0}/{1}", animationTrackBar.Value, animationTrackBar.Maximum);

            ApplyAnimation(animationTrackBar.Value);
        }

        private void RenderFrame(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
//            i++;

            Matrix4 projection = cam;
            Matrix4 real_cam = Matrix4.Identity;
            real_cam = Matrix4.CreateTranslation(0.0f, 0.0f, -distance) * real_cam;
            real_cam = Matrix4.CreateRotationX((float)(roty / 180 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateRotationY((float)(rotx / 180 * Math.PI)) * real_cam;
            real_cam = Matrix4.CreateTranslation(0.0f, -camy, 0.0f) * real_cam;

            real_cam = Matrix4.CreateRotationX((float)(-Math.PI / 2.0))*real_cam;
            real_cam = Matrix4.CreateRotationZ((float)(Math.PI)) * real_cam;

            Matrix4 light_mat = real_cam;

            Matrix3 normalMatrix = new Matrix3(real_cam);
            normalMatrix.Invert();
            normalMatrix.Transpose();

            GL.BindTexture(TextureTarget.Texture2D, bgtex);
            back.Render(projection, real_cam);

            GL.UseProgram(shader);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelview"), false, ref real_cam);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(GL.GetUniformLocation(shader, "tex"), 0);

            int bones_uniform = GL.GetUniformLocation(shader, "bones");
            if (bones_uniform >= 0)
            {
                Matrix4[] bones = model.GetBoneTransforms();
                float[] bones_data = new float[bones.Length * 16];
                for (int i = 0; i < bones.Length; i++)
                {
                    bones_data[i * 16 + 0] = bones[i].M11;
                    bones_data[i * 16 + 1] = bones[i].M12;
                    bones_data[i * 16 + 2] = bones[i].M13;
                    bones_data[i * 16 + 3] = bones[i].M14;

                    bones_data[i * 16 + 4] = bones[i].M21;
                    bones_data[i * 16 + 5] = bones[i].M22;
                    bones_data[i * 16 + 6] = bones[i].M23;
                    bones_data[i * 16 + 7] = bones[i].M24;

                    bones_data[i * 16 + 8] = bones[i].M31;
                    bones_data[i * 16 + 9] = bones[i].M32;
                    bones_data[i * 16 + 10] = bones[i].M33;
                    bones_data[i * 16 + 11] = bones[i].M34;

                    bones_data[i * 16 + 12] = bones[i].M41;
                    bones_data[i * 16 + 13] = bones[i].M42;
                    bones_data[i * 16 + 14] = bones[i].M43;
                    bones_data[i * 16 + 15] = bones[i].M44;
                }
                GL.UniformMatrix4(bones_uniform, bones.Length, false, bones_data);
            }

            model.Render();

            if (drawBonesCheckBox.Checked)
              gfxSkel.Render(projection, real_cam);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.UseProgram(0);

            glControl.SwapBuffers();
        }

        private void boneTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            Matrix4 trans = Matrix4.CreateRotationZ((float)(boneTrackBarRZ.Value * Math.PI / 180.0)) *
                            Matrix4.CreateRotationY((float)(boneTrackBarRY.Value * Math.PI / 180.0)) *
                            Matrix4.CreateRotationX((float)(boneTrackBarRX.Value * Math.PI / 180.0));
            if (treeView1.SelectedNode != null)
            {
                (treeView1.SelectedNode.Tag as Model.Bone).ResetTransform();
                (treeView1.SelectedNode.Tag as Model.Bone).ApplyTransform(trans);
            }
        }

        private void boneTrackBar2_ValueChanged(object sender, EventArgs e)
        {
            Matrix4 trans = Matrix4.CreateRotationZ((float)(boneTrackBarRZ.Value * Math.PI / 180.0)) *
                            Matrix4.CreateRotationY((float)(boneTrackBarRY.Value * Math.PI / 180.0)) *
                            Matrix4.CreateRotationX((float)(boneTrackBarRX.Value * Math.PI / 180.0));
            if (treeView1.SelectedNode != null)
            {
                (treeView1.SelectedNode.Tag as Model.Bone).ResetTransform();
                (treeView1.SelectedNode.Tag as Model.Bone).ApplyTransform(trans);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                Model.Bone b = treeView1.SelectedNode.Tag as Model.Bone;
                logTextBox.Clear();
                logTextBox.AppendText(String.Format("{0,5} {1,5} {2,5} {3,5}\r\n", b.lt.Row0.X, b.lt.Row0.Y, b.lt.Row0.Z, b.lt.Row0.W));
                logTextBox.AppendText(String.Format("{0,5} {1,5} {2,5} {3,5}\r\n", b.lt.Row1.X, b.lt.Row1.Y, b.lt.Row1.Z, b.lt.Row1.W));
                logTextBox.AppendText(String.Format("{0,5} {1,5} {2,5} {3,5}\r\n", b.lt.Row2.X, b.lt.Row2.Y, b.lt.Row2.Z, b.lt.Row2.W));
                logTextBox.AppendText(String.Format("{0,5} {1,5} {2,5} {3,5}\r\n", b.lt.Row3.X, b.lt.Row3.Y, b.lt.Row3.Z, b.lt.Row3.W));
                string anim_bone_name;
                if (b.name != "")
                    if (bone_dict.TryGetValue(b.name, out anim_bone_name))
                    {
                        BvhPart animation_bone = skeleton.BvhPartsLinear.Find(abone => abone.name == anim_bone_name);
                        if (animation_bone != null)
                        {
                            Matrix4 m = getMotionTransform(animation_bone.motion[animationTrackBar.Value]);
                            label2.Text = String.Format("{0,6:##0.00}  {1,6:##0.00}  {2,6:##0.00}  {3,6:##0.00}\r\n"+
                                                        "{4,6:##0.00}  {5,6:##0.00}  {6,6:##0.00}  {7,6:##0.00}\r\n"+
                                                        "{8,6:##0.00}  {9,6:##0.00}  {10,6:##0.00}  {11,6:##0.00}\r\n"+
                                                        "{12,6:##0.00}  {13,6:##0.00}  {14,6:##0.00}  {15,6:##0.00}",
                                                        m.M11, m.M12, m.M13, m.M14,
                                                        m.M21, m.M22, m.M23, m.M24,
                                                        m.M31, m.M32, m.M33, m.M34,
                                                        m.M41, m.M42, m.M43, m.M44);
                        }
                    }
            }
        }

        private void boneTransformChanged(object sender, EventArgs e)
        {
            Matrix4 mrx = Matrix4.CreateRotationX((float)(boneTrackBarRX.Value * Math.PI / 180.0));
            Matrix4 mry = Matrix4.CreateRotationY((float)(boneTrackBarRY.Value * Math.PI / 180.0));
            Matrix4 mrz = Matrix4.CreateRotationZ((float)(boneTrackBarRZ.Value * Math.PI / 180.0));

            Matrix4 trans;
            switch (orderComboBox.SelectedIndex)
            {
                case 0: trans = mrx * mry * mrz; break;     // XYZ
                case 1: trans = mrx * mrz * mry; break;     // XZY
                case 2: trans = mry * mrx * mrz; break;     // YXZ
                case 3: trans = mry * mrz * mrx; break;     // YZX
                case 4: trans = mrz * mrx * mry; break;     // ZXY
                case 5: trans = mrz * mry * mrx; break;     // ZYX
                default: trans = Matrix4.Identity; break;
            }
            trans = trans * Matrix4.CreateTranslation(boneTrackBarTX.Value*0.01f, boneTrackBarTY.Value * 0.01f, boneTrackBarTZ.Value * 0.01f);

            rotXlabel.Text = String.Format("RotX: {0,4:#000}°", boneTrackBarRX.Value);
            rotYlabel.Text = String.Format("RotY: {0,4:#000}°", boneTrackBarRY.Value);
            rotZlabel.Text = String.Format("RotZ: {0,4:#000}°", boneTrackBarRZ.Value);

            transXlabel.Text = String.Format("TransX: {0:#0.00}", boneTrackBarTX.Value * 0.01f);
            transYlabel.Text = String.Format("TransY: {0:#0.00}", boneTrackBarTY.Value * 0.01f);
            transZlabel.Text = String.Format("TransZ: {0:#0.00}", boneTrackBarTZ.Value * 0.01f);

            if (treeView1.SelectedNode != null)
            {
                (treeView1.SelectedNode.Tag as Model.Bone).ResetTransform();
                (treeView1.SelectedNode.Tag as Model.Bone).ApplyTransform(trans);

                Matrix4 m = trans;

                label2.Text = String.Format("{0,6:##0.00}  {1,6:##0.00}  {2,6:##0.00}  {3,6:##0.00}\r\n" +
                                            "{4,6:##0.00}  {5,6:##0.00}  {6,6:##0.00}  {7,6:##0.00}\r\n" +
                                            "{8,6:##0.00}  {9,6:##0.00}  {10,6:##0.00}  {11,6:##0.00}\r\n" +
                                            "{12,6:##0.00}  {13,6:##0.00}  {14,6:##0.00}  {15,6:##0.00}",
                                            m.M11, m.M12, m.M13, m.M14,
                                            m.M21, m.M22, m.M23, m.M24,
                                            m.M31, m.M32, m.M33, m.M34,
                                            m.M41, m.M42, m.M43, m.M44);
            }
            else
                if (animationTrackBar.Value > 0)
                ApplyAnimation(animationTrackBar.Value);
        }

        private void resetBoneTransformbutton_Click(object sender, EventArgs e)
        {
            boneTrackBarRX.Value = 0;
            boneTrackBarRY.Value = 0;
            boneTrackBarRZ.Value = 0;
            boneTrackBarTX.Value = 0;
            boneTrackBarTY.Value = 0;
            boneTrackBarTZ.Value = 0;
            boneTransformChanged(sender, e);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                model = Model.FromFile(openFileDialog1.FileName);
                Textures.Load();
                model.Buffer();

                treeView1.Nodes.Clear();

                ProcessBones(model.GetBone(0), treeView1.Nodes);

                GL.UseProgram(shader);

                makeSkeletonTransforms(model, skeleton);

                Model.Bone b = model.GetBoneByName("Model::Hips");
                Matrix4 gt = Matrix4.Identity;
                Matrix4 fx = b.fix;
                b.fix = Matrix4.Identity;
                b = b.parent;
                gt = b.gt;

                gfxSkel.gt = gt;

                b.fix = fx * b.lt;

                ApplyAnimation(0);
                treeView1.ExpandAll();
            }
        }

        private void resetAllBoneTransformButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < model.GetBonesCount(); i++)
            {
                var bone = model.GetBone(i);
                bone.ResetTransform();
            }
            boneTrackBarRX.Value = 0;
            boneTrackBarRY.Value = 0;
            boneTrackBarRZ.Value = 0;
            boneTrackBarTX.Value = 0;
            boneTrackBarTY.Value = 0;
            boneTrackBarTZ.Value = 0;
            boneTransformChanged(sender, e);
        }

        private void boneTrackBar3_ValueChanged(object sender, EventArgs e)
        {
        }

    }
}
