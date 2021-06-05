using System;

using System.Collections.Generic;

using System.ComponentModel;

using System.Data;

using System.Drawing;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;

using OpenTK;

using System.Drawing.Imaging;

using System.Runtime.InteropServices;

namespace OGLTest

{

    public partial class Form1 : Form

    {

        public Form1()

        {

            InitializeComponent();

        }

        struct VertexData

        {

            public Vector3 vertex;

            public Vector2 tex_coord;

            public Vector3 normal;

            public Vector3 tangent;

        }

        Matrix4 cam;

        int tex;

        int cube_diff, cube_bump, cube_pom;

        float distance = 5.0f;

        float rotx, roty;

        Vector3 light = new Vector3(15.0f, 15.0f, 15.0f);

        Point m;

        string shadow_shader_src = @"

#version 120

attribute vec3 vertex;

uniform mat4 modelview;

uniform mat4 projection;

void main() {

gl_Position = projection*modelview*vec4(vertex, 1.0);

}

";

        string grass_vertex_src = @"

attribute vec3 vertex;

attribute vec2 texcoord;

varying vec2 tc;

varying vec4 shadow_coord;

uniform mat4 modelview;

uniform mat4 projection;

uniform mat4 shadowMat;

void main() {

gl_Position = projection*modelview*vec4(vertex, 1.0);

tc = texcoord;

shadow_coord = shadowMat * vec4(vertex, 1.0);

}

";

        string grass_fragment_src = @"

varying vec2 tc;

varying vec4 shadow_coord;

uniform sampler2D texture;

uniform sampler2D shadow;

void main() {

float visibility = 1.0;

if (texture2D(shadow, (shadow_coord.xy*0.5)/shadow_coord.w+vec2(0.5)).x < shadow_coord.z*0.5/shadow_coord.w+0.5){

visibility = 0.5;

}

gl_FragColor = texture2D(texture, tc)*visibility;

}

";

        string vertex_shader_src = @"

#version 120

attribute vec3 vertex;

attribute vec2 tex_coord;

attribute vec3 normal;

attribute vec3 tangent;

uniform mat4 modelview;

uniform mat4 projection;

uniform mat3 normal_matrix;

uniform vec3 light;

uniform vec3 camera;

varying vec2 tc;

varying vec3 light_dir;

varying vec3 camera_dir;

varying vec3 nnn;

varying vec3 halfVec;

void main() {

gl_Position = projection*modelview*vec4(vertex, 1.0);

tc = tex_coord;

vec3 n = normalize (normal_matrix * normal);

vec3 t = normalize (normal_matrix * tangent);

vec3 b = cross (n, t);

vec3 vertexPosition = vec3(modelview * vec4(vertex, 1.0));

vec3 lightDir = normalize(light - vertexPosition);

vec3 cameraDir = normalize(camera - vertexPosition);

vec3 v;

v.x = dot (lightDir, t);

v.y = dot (lightDir, b);

v.z = dot (lightDir, n);

light_dir = normalize (v);

v.x = dot (cameraDir, t);

v.y = dot (cameraDir, b);

v.z = dot (cameraDir, n);

camera_dir = normalize (v);

// vertexPosition = normalize(vertexPosition);

vec3 halfVector = normalize(camera_dir + lightDir);

v.x = dot (halfVector, t);

v.y = dot (halfVector, b);

v.z = dot (halfVector, n);

halfVec = v ;

}

";

        string fragment_shader_src = @"

#version 120

varying vec2 tc;

varying vec3 light_dir;

varying vec3 camera_dir;

varying vec3 nnn;

varying vec3 halfVec;

uniform sampler2D tex_diff;

uniform sampler2D tex_bump;

uniform sampler2D tex_height;

uniform float scale;

void main() {

const float numSteps = 25.0;

const float specExp = 20.0;

// const float eps = 0.05;

float step = 1.0 / numSteps;

vec2 dtex = step * scale * vec2(-camera_dir.x, camera_dir.y) / camera_dir.z; // adjustment for one layer

float height = 1.0; // height of the layer

vec2 tex = tc; // our initial guess

float h = texture2D ( tex_height, tex ).r;

while ( h < height )

{

height -= step;

tex += dtex;

h = texture2D ( tex_height, tex ).r;

}

// now find point via linear interpolation

vec2 prev = tex - dtex; // previous point

float hPrev = texture2D ( tex_height, prev ).r - (height + step); // < 0

float hCur = h - height; // > 0

float weight = hCur / (hCur - hPrev );

tex = weight * prev + (1.0 - weight) * tex;

vec4 color = texture2D ( tex_diff, tex );

vec3 n = normalize ( texture2D ( tex_bump, tex ).rgb * 2.0 - vec3 ( 1.0 ) );

float diff = max ( dot ( n, normalize ( light_dir ) ), 0.15 );

float spec = pow ( max ( dot ( n, normalize ( halfVec ) ), 0.0 ), specExp );

// now compute shadow

float vis = 1.0; // light visibility

float sampleWeight = 9.0 / numSteps; // weight of one sample

vec2 ltex;

if ( diff > 0.05 ) // no sense checking other case

{

height = texture2D ( tex_height, tex ).r;

step = (1.0 - height) / numSteps;

ltex = scale * step * vec2(light_dir.x, -light_dir.y) / light_dir.z;

height += step; // move one step off the surface

tex += ltex;

sampleWeight *= pow ( 1.0 - light_dir.z, 3.0 ); // to fight nearly perpendicular lighting

while ( 1.0-height > 0.05 )

{

if ( texture2D ( tex_height, tex ).r > height )

vis -= sampleWeight;

height += step;

tex += ltex;

}

}

else // we're back-facing

diff = 0.15;

gl_FragColor = vec4 ( color.rgb * min(diff, max(vis, 0.15)), 1.0 );

/* vec3 bump = normalize(2.0 * texture2D (tex_bump, tc).rgb - 1.0);

float lamberFactor = max (dot (light_dir, bump), 0.15) ;

vec4 diffuseMaterial = texture2D(tex_diff, tc);

vec4 diffuseLight = vec4(1.0);

vec4 specularMaterial ;

vec4 specularLight ;

float shininess ;

if (lamberFactor > 0.0)

{

diffuseLight = vec4(1.0);

specularMaterial = vec4(1.0);

specularLight = vec4(1.0);

shininess = pow (max (dot (halfVec, bump), 0.0), 8.0);

gl_FragColor = vec4((diffuseMaterial * diffuseLight * lamberFactor+specularMaterial * specularLight * shininess).xyz, 1.0);

}*/

}

";

        VertexData[] vertexes = {

new VertexData { vertex = new Vector3(-1.0f, -1.0f, 1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, 1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, 1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, 1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(0, 0, 1), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, -1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, -1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(0, 0, -1), tangent = new Vector3(-1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, -1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, 1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, 1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, 1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(1, 0, 0), tangent = new Vector3(0, 0, -1) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, -1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, -1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, 1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(-1, 0, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, -1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, 1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, -1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, 1.0f, 1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3( 1.0f, 1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(0, 1, 0), tangent = new Vector3(1, 0, 0) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 0), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, -1.0f), tex_coord = new Vector2(0, 1), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3( 1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 1), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

new VertexData { vertex = new Vector3(-1.0f, -1.0f, 1.0f), tex_coord = new Vector2(1, 0), normal = new Vector3(0, -1, 0), tangent = new Vector3(0, 0, 1) },

};

        Shader cubeShader;

        Shader grassShader;

        Shader shadowShader;

        Mesh<VertexData> cube;

        MeshRenderer cubeVisual;

        MeshRenderer cubeShadow;

        private int LoadTexture(string filename)

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

            return res;

        }

        private void Form1_Load(object sender, EventArgs e)

        {

            GL.ClearColor(0.0f, 0.5f, 1.0f, 1.0f);

            float aspect = (float)glControl1.Width / (float)glControl1.Height;

            cam = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 1.0f, 1000.0f);

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Texture2D);

            tex = LoadTexture("grass.jpg");

            cube_diff = LoadTexture("relief_diff.jpg");

            cube_bump = LoadTexture("relief_bump.png");

            cube_pom = LoadTexture("relief_height2.png");

            shadowTex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, shadowTex);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, shadowSize, shadowSize, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            cubeShader = new Shader();

            cubeShader.AttachShader(ShaderType.VertexShader, vertex_shader_src);

            cubeShader.AttachShader(ShaderType.FragmentShader, fragment_shader_src);

            cubeShader.CompileAndLink();

            string result;

            textBox1.AppendText(("OpenGL: " + GL.GetString(StringName.Version)) + "\r\n");

            textBox1.AppendText("GLSL: " + GL.GetString(StringName.ShadingLanguageVersion) + "\r\n");

            if (!cubeShader.Ready)

            {

                textBox1.AppendText("cube vertex: " + cubeShader.CompilationVertexLog + "\r\n");

                textBox1.AppendText("cube fragment: " + cubeShader.CompilationFragmentLog + "\r\n");

                textBox1.AppendText("cube link: " + cubeShader.LinkingLog + "\r\n");

                return;

            }

            cube = new Mesh<VertexData>(vertexes);

            cube.Buffer();

            cubeVisual = new MeshRenderer(cube);

            cubeVisual.Shader = cubeShader;

            cubeVisual.Prepare();

            grassShader = new Shader();

            grassShader.AttachShader(ShaderType.VertexShader, grass_vertex_src);

            grassShader.AttachShader(ShaderType.FragmentShader, grass_fragment_src);

            grassShader.CompileAndLink();

            if (!grassShader.Ready)

            {

                textBox1.AppendText("grass vertex: " + grassShader.CompilationVertexLog + "\r\n");

                textBox1.AppendText("grass fragment: " + grassShader.CompilationFragmentLog + "\r\n");

                textBox1.AppendText("grass link: " + grassShader.LinkingLog + "\r\n");

                return;

            }

            shadowShader = new Shader();

            shadowShader.AttachShader(ShaderType.VertexShader, shadow_shader_src);

            shadowShader.CompileAndLink();

            if (!shadowShader.Ready)

            {

                textBox1.AppendText("shadow vertex: " + shadowShader.CompilationVertexLog + "\r\n");

                textBox1.AppendText("shadow link: " + shadowShader.LinkingLog + "\r\n");

                return;

            }

            cubeShadow = new MeshRenderer(cube);

            cubeShadow.Shader = shadowShader;

            cubeShadow.Prepare();

            /* shadow_vao = GL.GenVertexArray();

            GL.BindVertexArray(shadow_vao);

            GL.EnableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.UseProgram(shadow_shader);

            vertex_attrib = GL.GetAttribLocation(shadow_shader, "vertex");

            GL.EnableVertexAttribArray(vertex_attrib);

            GL.VertexAttribPointer(vertex_attrib, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "vertex"));

            GL.BindVertexArray(0);*/

            // GL.Enable(EnableCap.CullFace);

            timer1.Enabled = true;

            label1.Text = GL.GetError().ToString();

        }

        int i = 0;

        private void timer1_Tick(object sender, EventArgs e)

        {

            makeLight();

            RenderShadow();

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderGrass();

            RenderCube();

            if (checkBox1.Checked)

            {

                GL.MatrixMode(MatrixMode.Projection);

                GL.LoadIdentity();

                GL.MatrixMode(MatrixMode.Modelview);

                GL.LoadIdentity();

                GL.Enable(EnableCap.Texture2D);

                GL.BindTexture(TextureTarget.Texture2D, shadowTex);

                GL.Begin(PrimitiveType.TriangleStrip);

                GL.TexCoord2(1.0f, 0.0f);

                GL.Vertex3(1.0f, -1.0f, 0.0f);

                GL.TexCoord2(1.0f, 1.0f);

                GL.Vertex3(1.0f, 1.0f, 0.0f);

                GL.TexCoord2(0.0f, 0.0f);

                GL.Vertex3(-1.0f, -1.0f, 0.0f);

                GL.TexCoord2(0.0f, 1.0f);

                GL.Vertex3(-1.0f, 1.0f, 0.0f);

                GL.End();

            }

            glControl1.SwapBuffers();

            i++;

        }

        int shadowTex;

        int shadowSize = 512;

        Matrix4 light_projection = Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI / 2), 1.0f, 1.0f, 1000.0f);

        // Matrix4 light_projection = Matrix4.CreateOrthographic(50.0f, 50.0f, 1.0f, 1000.0f);

        Matrix4 light_cam;

        private void makeLight()

        {

            light = new Vector3(lightDistance * (float)(Math.Sin(lightHangle * Math.PI / 180.0) * Math.Sin(lightVangle * Math.PI / 180.0)),

            lightDistance * (float)(Math.Cos(lightVangle * Math.PI / 180.0)),

            lightDistance * (float)(Math.Cos(lightHangle * Math.PI / 180.0) * Math.Sin(lightVangle * Math.PI / 180.0)));

            float s = lightVangle < 0.0f ? -1.0f : 1.0f;

            Vector3 light_up = new Vector3(-s * (float)(Math.Sin(lightHangle * Math.PI / 180.0) * Math.Cos(lightVangle * Math.PI / 180.0)),

            (float)(Math.Sin(Math.Abs(lightVangle) * Math.PI / 180.0)),

            -s * (float)(Math.Cos(lightHangle * Math.PI / 180.0) * Math.Cos(lightVangle * Math.PI / 180.0)));

            light_cam = Matrix4.LookAt(light, new Vector3(0.0f, 0.0f, 0.0f), light_up);

        }

        private void RenderShadow()

        {

            GL.ColorMask(false, false, false, false);

            GL.Viewport(0, 0, shadowSize, shadowSize);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);

            Matrix4 light_matrix = Matrix4.CreateRotationY((float)((double)i / 180.0 * Math.PI)) * light_cam;

            shadowShader.Uniforms["projection"] = light_projection;

            shadowShader.Uniforms["modelview"] = light_matrix;

            cubeShadow.Render();

            GL.ColorMask(true, true, true, true);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, shadowTex);

            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, 0, 0, shadowSize, shadowSize, 0);

        }

        private void RenderCube()

        {

            Matrix4 projection = cam;

            Matrix4 real_cam = Matrix4.Identity;

            real_cam = Matrix4.CreateTranslation(0.0f, 0.0f, -distance) * real_cam;

            real_cam = Matrix4.CreateRotationX((float)(roty / 180 * Math.PI)) * real_cam;

            real_cam = Matrix4.CreateRotationY((float)(rotx / 180 * Math.PI)) * real_cam;

            Matrix4 light_mat = real_cam;

            real_cam = Matrix4.CreateRotationY((float)((double)i / 180.0 * Math.PI)) * real_cam;

            Matrix3 normalMatrix = new Matrix3(real_cam);

            cubeShader.Activate();

            cubeShader.Uniforms["modelview"] = real_cam;

            cubeShader.Uniforms["projection"] = projection;

            cubeShader.Uniforms["normal_matrix"] = normalMatrix;

            cubeShader.Uniforms["tex_diff"] = 0;

            cubeShader.Uniforms["tex_bump"] = 1;

            cubeShader.Uniforms["tex_height"] = 2;

            cubeShader.Uniforms["scale"] = (float)trackBar1.Value / 1000.0f;

            Vector4 light_vec = new Vector4(this.light, 1.0f);

            light_vec = light_vec * light_mat;

            Vector3 light = new Vector3(light_vec);

            Vector3 camera_pos = new Vector3(-real_cam.Row3);

            cubeShader.Uniforms["light"] = light_vec.Xyz;

            cubeShader.Uniforms["camera"] = camera_pos;

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, cube_diff);

            GL.ActiveTexture(TextureUnit.Texture1);

            GL.BindTexture(TextureTarget.Texture2D, cube_bump);

            GL.ActiveTexture(TextureUnit.Texture2);

            GL.BindTexture(TextureTarget.Texture2D, cube_pom);

            cubeVisual.Render();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, 0);

        }

        private void RenderGrass()

        {

            GL.Enable(EnableCap.Texture2D);

            grassShader.Activate();

            // GL.UseProgram(grass_shader);

            Matrix4 model = Matrix4.CreateTranslation(0.0f, 0.0f, -distance);

            model = Matrix4.CreateRotationX((float)(roty / 180 * Math.PI)) * model;

            model = Matrix4.CreateRotationY((float)(rotx / 180 * Math.PI)) * model;

            grassShader.Uniforms["projection"] = cam;

            grassShader.Uniforms["modelview"] = model;

            // GL.UniformMatrix4(GL.GetUniformLocation(grass_shader, "projection"), false, ref cam);

            // GL.UniformMatrix4(GL.GetUniformLocation(grass_shader, "modelview"), false, ref model);

            Matrix4 shadow_proj = Matrix4.Identity;

            shadow_proj = light_cam * light_projection;

            grassShader.Uniforms["shadowMat"] = shadow_proj;

            // GL.UniformMatrix4(GL.GetUniformLocation(grass_shader, "shadowMat"), false, ref shadow_proj);

            grassShader.Uniforms["texture"] = 0;

            grassShader.Uniforms["shadow"] = 1;

            // GL.Uniform1(GL.GetUniformLocation(grass_shader, "texture"), 0);

            // GL.Uniform1(GL.GetUniformLocation(grass_shader, "shadow"), 1);

            int vertex_attr = GL.GetAttribLocation(grassShader.ProgramID, "vertex");

            int texcrd_attr = GL.GetAttribLocation(grassShader.ProgramID, "texcoord");

            // int vertex_attr = grassShader.AttributeLocations["vertex"];

            // int texcrd_attr = grassShader.AttributeLocations["texcoord"];

            GL.Enable(EnableCap.Texture2D);

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.ActiveTexture(TextureUnit.Texture1);

            GL.BindTexture(TextureTarget.Texture2D, shadowTex);

            GL.Begin(PrimitiveType.Triangles);

            GL.VertexAttrib2(texcrd_attr, new Vector2(0.0f, 0.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(-5.0f, -1.2f, -5.0f));

            // GL.TexCoord2(0.0f, 0.0f);

            // GL.Vertex3(-5.0f, -1.2f, -5.0f);

            GL.VertexAttrib2(texcrd_attr, new Vector2(1.0f, 1.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(5.0f, -1.2f, 5.0f));

            // GL.TexCoord2(1.0f, 1.0f);

            // GL.Vertex3(5.0f, -1.2f, 5.0f);

            GL.VertexAttrib2(texcrd_attr, new Vector2(1.0f, 0.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(5.0f, -1.2f, -5.0f));

            // GL.TexCoord2(1.0f, 0.0f);

            // GL.Vertex3(5.0f, -1.2f, -5.0f);

            GL.VertexAttrib2(texcrd_attr, new Vector2(0.0f, 1.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(-5.0f, -1.2f, 5.0f));

            // GL.TexCoord2(0.0f, 1.0f);

            // GL.Vertex3(-5.0f, -1.2f, 5.0f);

            GL.VertexAttrib2(texcrd_attr, new Vector2(1.0f, 1.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(5.0f, -1.2f, 5.0f));

            // GL.TexCoord2(1.0f, 1.0f);

            // GL.Vertex3(5.0f, -1.2f, 5.0f);

            GL.VertexAttrib2(texcrd_attr, new Vector2(0.0f, 0.0f));

            GL.VertexAttrib3(vertex_attr, new Vector3(-5.0f, -1.2f, -5.0f));

            // GL.TexCoord2(0.0f, 0.0f);

            // GL.Vertex3(-5.0f, -1.2f, -5.0f);

            GL.End();

            GL.Rotate((float)(i), 0.0f, 1.0f, 0.0f);

            GL.Disable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.UseProgram(0);

            GL.ActiveTexture(TextureUnit.Texture0);

        }

        private void View_MouseWheel(object sender, MouseEventArgs e)

        {

            if (e.Delta > 0)

                distance /= 1.1f;

            if (e.Delta < 0)

                distance *= 1.1f;

            if (distance < 3.0f)

                distance = 3.0f;

            if (distance > 25.0f)

                distance = 25.0f;

        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)

        {

            m = new Point(e.X, e.Y);

        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)

        {

            if (e.Button == System.Windows.Forms.MouseButtons.Right)

            {

                rotx = rotx + (e.X - m.X) / 2;

                roty = roty + (e.Y - m.Y) / 2;

                if (roty > 75.0f)

                    roty = 75.0f;

                if (roty < -75.0f)

                    roty = -75.0f;

                m = new Point(e.X, e.Y);

            }

        }

        private void glControl1_Resize(object sender, EventArgs e)

        {

            float aspect = (float)glControl1.Width / (float)glControl1.Height;

            cam = Matrix4.CreatePerspectiveFieldOfView((float)(90 / aspect / 180 * Math.PI), aspect, 1.0f, 1000.0f);

        }

        float lightVangle = 0.0f;

        float lightHangle = 0.0f;

        float lightDistance = 15.0f;

        private void trackBar2_ValueChanged(object sender, EventArgs e)

        {

            label2.Text = trackBar2.Value.ToString();

            lightVangle = trackBar2.Value;

        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)

        {

            label3.Text = trackBar3.Value.ToString();

            lightHangle = trackBar3.Value;

        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)

        {

            lightDistance = trackBar4.Value;

        }

    }

    [Serializable]

    public class ShaderException : Exception

    {

        public ShaderException() { }

        public ShaderException(string message) : base(message) { }

        public ShaderException(string message, Exception inner) : base(message, inner) { }

    }

    internal class Shader : IDisposable

    {

        private string VertexSource;

        private string GeometrySource;

        private string FragmentSource;

        public bool Ready { get; private set; }

        private int VertexShaderID;

        private int GeometryShaderID;

        private int FragmentShaderID;

        public int ProgramID { get; private set; }

        private Dictionary<String, int> attrLocations = new Dictionary<string, int>();

        private Dictionary<String, object> uniforms = new Dictionary<string, object>();

        private string cVertexLog;

        private string cGeometryLog;

        private string cFragmentLog;

        private string cLinkingLog;

        public string CompilationVertexLog { get { return cVertexLog; } }

        public string CompilationGeometryLog { get { return cGeometryLog; } }

        public string CompilationFragmentLog { get { return cFragmentLog; } }

        public string LinkingLog { get { return cLinkingLog; } }

        public static bool DefaultStrictCompilation;

        public bool StrictCompilation { get; set; }

        public class Attributes

        {

            private Shader parent;

            internal Attributes(Shader parent)

            {

                this.parent = parent;

            }

            public bool Has(string name)

            {

                if (parent.Ready)

                    return GL.GetAttribLocation(parent.ProgramID, name) > -1;

                else

                    return parent.attrLocations.ContainsKey(name);

            }

            public int this[string name]
            {

                get

                {

                    if (parent.Ready)

                        return GL.GetAttribLocation(parent.ProgramID, name);

                    int value;

                    if (parent.attrLocations.TryGetValue(name, out value))

                        return value;

                    return -1;

                }

                set { parent.attrLocations[name] = value; }

            }

        }

        public Attributes AttributeLocations;

        public class UniformStorage

        {

            private Shader parent;

            public UniformStorage(Shader parent)

            {

                this.parent = parent;

            }

            public object this[string name]

            {

                set

                {

                    parent.uniforms[name] = value;

                    parent.SetUniform(name, value);

                }

            }

        }

        public UniformStorage Uniforms;

        public void AttachShader(ShaderType type, string ShaderSource)

        {

            switch (type)

            {

                case ShaderType.VertexShader:

                    if (Ready)

                        throw new ShaderException("Changing shader contents is not allowed after linking complete");

                    if (!String.IsNullOrEmpty(VertexSource))

                        throw new ShaderException("This shader program already has Vertex step");

                    VertexSource = ShaderSource;

                    return;

                case ShaderType.GeometryShader:

                    if (Ready)

                        throw new ShaderException("Changing shader contents is not allowed after linking complete");

                    if (!String.IsNullOrEmpty(GeometrySource))

                        throw new ShaderException("This shader program already has Geometry step");

                    GeometrySource = ShaderSource;

                    return;

                case ShaderType.FragmentShader:

                    if (Ready)

                        throw new ShaderException("Changing shader contents is not allowed after linking complete");

                    if (!String.IsNullOrEmpty(FragmentSource))

                        throw new ShaderException("This shader program already has Fragment step");

                    FragmentSource = ShaderSource;

                    return;

                default:

                    throw new NotImplementedException(String.Format("Shader type {0} is not implemented yet.", type));

            }

        }

        private int CompileShader(int ShaderID, string ShaderSource, out string ShaderLog)

        {

            GL.ShaderSource(ShaderID, ShaderSource);

            GL.CompileShader(ShaderID);

            ShaderLog = GL.GetShaderInfoLog(ShaderID);

            int result;

            GL.GetShader(ShaderID, ShaderParameter.CompileStatus, out result);

            return result;

        }

        public void CompileAndLink()

        {

            ProgramID = GL.CreateProgram();

            if (!String.IsNullOrEmpty(VertexSource))

                VertexShaderID = GL.CreateShader(ShaderType.VertexShader);

            if (!String.IsNullOrEmpty(GeometrySource))

                GeometryShaderID = GL.CreateShader(ShaderType.GeometryShader);

            if (!String.IsNullOrEmpty(FragmentSource))

                FragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);

            int vresult, gresult, fresult, lresult;

            if (VertexShaderID != 0)

                vresult = CompileShader(VertexShaderID, VertexSource, out cVertexLog);

            else

                vresult = 1;

            if (GeometryShaderID != 0)

                gresult = CompileShader(GeometryShaderID, GeometrySource, out cGeometryLog);

            else

                gresult = 1;

            if (FragmentShaderID != 0)

                fresult = CompileShader(FragmentShaderID, FragmentSource, out cFragmentLog);

            else

                fresult = 1;

            foreach (var loc in attrLocations)

                GL.BindAttribLocation(ProgramID, loc.Value, loc.Key);

            if (vresult == 1 && gresult == 1 && fresult == 1)

            {

                if (VertexShaderID != 0)

                    GL.AttachShader(ProgramID, VertexShaderID);

                if (GeometryShaderID != 0)

                    GL.AttachShader(ProgramID, GeometryShaderID);

                if (FragmentShaderID != 0)

                    GL.AttachShader(ProgramID, FragmentShaderID);

                GL.LinkProgram(ProgramID);

                cLinkingLog = GL.GetProgramInfoLog(ProgramID);

                GL.GetProgram(ProgramID, GetProgramParameterName.LinkStatus, out lresult);

            }

            else

                lresult = 0;

            if (StrictCompilation && (vresult == 0 || gresult == 0 || fresult == 0 || lresult == 0))

            {

                string message = "Shader compilation and linking failed due to errors:";

                if (vresult == 0)

                    message += " vertex: " + cVertexLog;

                if (gresult == 0)

                    message += " geometry: " + cGeometryLog;

                if (fresult == 0)

                    message += " fragment: " + cFragmentLog;

                if (lresult == 0)

                    message += " linking: " + cLinkingLog;

                throw new ShaderException(message);

            }

            if (vresult == 1 && gresult == 1 && fresult == 1 && lresult == 1)

            {

                Ready = true;

                FragmentSource = null;

                GeometrySource = null;

                VertexSource = null;

            }

        }

        private void SetUniform(string name, object value)

        {

            if (!Ready) return;

            int location = GL.GetUniformLocation(ProgramID, name);

            if (location < 0) return;

            int curProgram;

            GL.GetInteger(GetPName.CurrentProgram, out curProgram);

            if (curProgram != ProgramID)

                GL.UseProgram(ProgramID);

            if (value.GetType() == typeof(int))

                GL.Uniform1(location, (int)value);

            if (value.GetType() == typeof(uint))

                GL.Uniform1(location, (uint)value);

            if (value.GetType() == typeof(float))

                GL.Uniform1(location, (float)value);

            if (value.GetType() == typeof(Vector2))

                GL.Uniform2(location, (Vector2)value);

            if (value.GetType() == typeof(Vector3))

                GL.Uniform3(location, (Vector3)value);

            if (value.GetType() == typeof(Vector4))

                GL.Uniform4(location, (Vector4)value);

            if (value.GetType() == typeof(Matrix2))

            { Matrix2 v = (Matrix2)value; GL.UniformMatrix2(location, false, ref v); }

            if (value.GetType() == typeof(Matrix3))

            { Matrix3 v = (Matrix3)value; GL.UniformMatrix3(location, false, ref v); }

            if (value.GetType() == typeof(Matrix4))

            { Matrix4 v = (Matrix4)value; GL.UniformMatrix4(location, false, ref v); }

            if (curProgram != ProgramID)

                GL.UseProgram(curProgram);

        }

        public void Activate()

        {

            if (!Ready)

                CompileAndLink();

            GL.UseProgram(ProgramID);

            foreach (var uni in uniforms)

                SetUniform(uni.Key, uni.Value);

        }

        public static void Deactivate()

        {

            GL.UseProgram(0);

        }

        public Shader()

        {

            AttributeLocations = new Attributes(this);

            Uniforms = new UniformStorage(this);

            StrictCompilation = DefaultStrictCompilation;

        }

        void IDisposable.Dispose()

        {

            if (VertexShaderID != 0)

                GL.DeleteShader(VertexShaderID);

            if (GeometryShaderID != 0)

                GL.DeleteShader(GeometryShaderID);

            if (FragmentShaderID != 0)

                GL.DeleteShader(FragmentShaderID);

            if (ProgramID != 0)

                GL.DeleteProgram(ProgramID);

            VertexShaderID = 0;

            GeometryShaderID = 0;

            FragmentShaderID = 0;

            ProgramID = 0;

        }

        ~Shader()

        {

        }

    }

    public interface IRenderableMesh

    {

        void Render();

        void Buffer();

        void Unbuffer();

        void Activate();

        void Deactivate();

        bool Buffered { get; }

        Type GetVertexType();

    }

    internal class Mesh<Vertex> : IDisposable, IRenderableMesh where Vertex : struct

    {

        private Vertex[] intData;

        public Vertex[] data { get { return intData; } set { intData = value; needBuffering = true; } }

        private bool needBuffering;

        public int VBO { get; private set; }

        public Mesh()

        { }

        public Mesh(Vertex[] data)

        {

            intData = data;

            needBuffering = true;

        }

        public void Buffer()

        {

            if (VBO == 0)

                VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(Vertex)) * intData.Length, intData, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        public void Unbuffer()

        {

            GL.DeleteBuffer(VBO);

            VBO = 0;

        }

        public bool Buffered { get { return VBO != 0 && !needBuffering; } }

        public void Activate()

        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

        }

        void IRenderableMesh.Deactivate()

        {

            Deactivate();

        }

        public static void Deactivate()

        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        public void Render()

        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            GL.DrawArrays(PrimitiveType.Triangles, 0, intData.Length);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }

        public Type GetVertexType()

        { return typeof(Vertex); }

        void IDisposable.Dispose()

        {

            Unbuffer();

        }

    }

    internal class MeshRenderer : IDisposable

    {

        private IRenderableMesh mesh;

        private Shader shader;

        private bool shaderPrepared;

        public int VAO { get; private set; }

        public Shader Shader { get { return shader; } set { shader = value; shaderPrepared = false; } }

        public Matrix4 ModelView { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Dictionary<string, string> VertexAttributeBindings = new Dictionary<string, string>();

        private void PrepareShader()

        {

            if (shaderPrepared) return;

            GL.BindVertexArray(VAO);

            GL.EnableClientState(ArrayCap.VertexArray);

            GL.EnableClientState(ArrayCap.IndexArray);

            var VertexType = mesh.GetVertexType();

            foreach (var field in VertexType.GetFields())

            {

                string attrname;

                if (VertexAttributeBindings.ContainsKey(field.Name))

                    attrname = VertexAttributeBindings[field.Name];

                else

                    attrname = field.Name;

                int location = Shader.AttributeLocations[attrname];

                if (location > -1)

                {

                    if (field.FieldType == typeof(float))

                    {

                        GL.EnableVertexAttribArray(location);

                        GL.VertexAttribPointer(location, 1, VertexAttribPointerType.Float, false, Marshal.SizeOf(VertexType), Marshal.OffsetOf(VertexType, field.Name));

                    }

                    else if (field.FieldType == typeof(int))

                    {

                        GL.EnableVertexAttribArray(location);

                        GL.VertexAttribPointer(location, 1, VertexAttribPointerType.Int, false, Marshal.SizeOf(VertexType), Marshal.OffsetOf(VertexType, field.Name));

                    }

                    else if (field.FieldType == typeof(Vector2))

                    {

                        GL.EnableVertexAttribArray(location);

                        GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf(VertexType), Marshal.OffsetOf(VertexType, field.Name));

                    }

                    else if (field.FieldType == typeof(Vector3))

                    {

                        GL.EnableVertexAttribArray(location);

                        GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(VertexType), Marshal.OffsetOf(VertexType, field.Name));

                    }

                    else if (field.FieldType == typeof(Vector4))

                    {

                        GL.EnableVertexAttribArray(location);

                        GL.VertexAttribPointer(location, 4, VertexAttribPointerType.Float, false, Marshal.SizeOf(VertexType), Marshal.OffsetOf(VertexType, field.Name));

                    }

                    else

                        throw new NotImplementedException(String.Format("Passing \"{0}\" as vertex attribute is not implemented yet.", field.FieldType.Name));

                }

            }

            GL.BindVertexArray(0);

            shaderPrepared = true;

        }

        public MeshRenderer(IRenderableMesh Mesh)

        {

            mesh = Mesh;

        }

        public void Prepare()

        {

            if (VAO != 0)

                Dispose();

            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);

            mesh.Activate();

            GL.EnableClientState(ArrayCap.VertexArray);

            GL.EnableClientState(ArrayCap.IndexArray);

            if (shader != null && shader.Ready)

                PrepareShader();

            mesh.Deactivate();

            GL.BindVertexArray(0);

        }

        public void Dispose()

        {

        }

        public void Render()

        {

            GL.BindVertexArray(VAO);

            shader.Activate();

            mesh.Render();

            Shader.Deactivate();

            GL.BindVertexArray(0);

        }

    }

}