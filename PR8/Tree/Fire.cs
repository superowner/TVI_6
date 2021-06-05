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
    class Fire
    {
        struct VertexData
        {
            public Vector3 position;
            public Vector4 color;
            public float size;
            public float alpha;
        }

        int vao;
        int vbo;
        int shader;

        int total_particles = 12000;

        int min_livetime = 60;
        int max_livetime = 180;

        float min_temperature = 800;
        float max_temperature = 2100;

        string vertex_shader_src = @"
#version 450 core
in vec3 vertex;
in vec4 color;
in float size;
in float alpha;

uniform mat4 modelview;
uniform mat4 projection;

out vec4 gcoord;
out vec4 clr;
out float size_factor;
out float alphak;

void main() {
  gl_Position = vec4(vertex, 1.0);
  clr = color;
  size_factor = size;
  alphak = alpha;
}
";

        string geometry_shader_src = @"
#version 450 core
layout (points) in;
layout (triangle_strip, max_vertices=4) out;

in vec4 gcoord[];
in vec4 clr[];
in float size_factor[];
in float alphak[];

out vec2 tex_coord;
out vec4 color;
out float alpha;

uniform mat4 modelview;
uniform mat4 projection;

void main()
{
  color = clr[0];

  vec3 pos = gl_in[0].gl_Position.xyz;

  float side = 0.1*size_factor[0];
  vec3 dx = vec3(modelview[0][0], modelview[1][0], modelview[2][0]);
  vec3 dy = vec3(modelview[0][1], modelview[1][1], modelview[2][1]);

  alpha = alphak[0];
  tex_coord = vec2(0, 0);
  gl_Position = projection*modelview*vec4(pos-side*dx-side*dy, 1.0);
  EmitVertex();

  tex_coord = vec2(1, 0);
  gl_Position = projection*modelview*vec4(pos+side*dx-side*dy, 1.0);
  EmitVertex();

  tex_coord = vec2(0, 1);
  gl_Position = projection*modelview*vec4(pos-side*dx+side*dy, 1.0);
  EmitVertex();

  tex_coord = vec2(1, 1);
  gl_Position = projection*modelview*vec4(pos+side*dx+side*dy, 1.0);
  EmitVertex();

  EndPrimitive();
}
";

        string fragment_shader_src = @"
#version 450 core
in vec2 tex_coord;
in vec4 color;
in float alpha;

uniform sampler2D tex;

out vec4 FragColor;

void main() {
  vec4 c = texture2D(tex, tex_coord)*color;
  FragColor = vec4(c.rgb, c.a*alpha);
}
";

        internal class Particle
        {
            public Vector3 position;
            public Vector3 motion;
            public float temperature;
            public int livetime;
            public float size;
        }

        Particle[] particles;
        VertexData[] vertices;

        public int ParticleTexture;

        public float Temperature;
        public float Transparency;

        private Random rnd = new Random();

        private Vector4 TempToColor(float temperature)
        {
            Vector4 result;

            if (temperature < 6600)
            {
                result.X = 1.0f;
                result.Y = 99.4708025861f * (int)Math.Log(temperature / 100) - 161.1195681661f;
                if (temperature < 1900)
                    result.Z = 0;
                else
                    result.Z = 138.5177312231f * (float)Math.Log(temperature / 100 - 10) - 305.0447927307f;
            }
            else
            {
                result.X = 329.698727446f * (float)Math.Pow(temperature / 100 - 60.0, -0.1332047592);
                result.Y = 288.1221695283f * (float)Math.Pow(temperature / 100 - 60.0, -0.0755148492);
                result.Z = 1.0f;
            }

            if (result.X < 0) result.X = 0;
            if (result.X > 1.0f) result.X = 1.0f;
            if (result.Y < 0) result.Y = 0;
            if (result.Y > 1.0f) result.Y = 1.0f;
            if (result.Z < 0) result.Z = 0;
            if (result.Z > 1.0f) result.Z = 1.0f;

            result.W = 1.0f;
            return result;
        }

        private Particle GenParticle()
        {
            Particle result = new Particle();
            result.livetime = rnd.Next(max_livetime - min_livetime) + min_livetime;

            double angle = rnd.NextDouble() * 360.0;
            double distance = rnd.NextDouble();
            //distance = distance * distance;

            result.temperature = (float)(rnd.NextDouble() * (max_temperature - min_temperature)*(1-distance) + min_temperature);
//            result.temperature = (float)(Math.Pow(rnd.NextDouble() * (max_temperature - min_temperature), (2-distance)) + min_temperature);

            result.size = (float)(rnd.NextDouble()*2.5+0.5);
            result.position = new Vector3((float)(Math.Cos(angle)*distance), 0.0f, (float)(Math.Sin(angle)*distance));
            result.motion = new Vector3(0, (float)(rnd.NextDouble()*0.1), 0);
            return result;
        }

        private void MakeVertices()
        {
            for (int i = 0; i < particles.Length && i < vertices.Length; i++)
            {
                vertices[i].position = particles[i].position;

                if (particles[i].temperature > 400)
                {
                    vertices[i].size = particles[i].size * 0.25f;
                    vertices[i].color = TempToColor(particles[i].temperature);
                }
                else if (particles[i].temperature > 180)
                {
                    vertices[i].size = particles[i].size * 0.15f;
                    vertices[i].color = new Vector4(0.5f, 0.5f, 0.5f, 0.0f);
                }
                else
                {
                    vertices[i].color = new Vector4(0.5f, 0.5f, 0.5f, 0.8f);
                    vertices[i].size = (particles[i].temperature - 175) / 15.0f+1.0f;
                }

                if (particles[i].livetime < 10 && particles[i].temperature < 120)
                {
                    vertices[i].size *= (20 - particles[i].livetime) / 2;
                    vertices[i].alpha = particles[i].livetime * 0.025f;
                }
                else
                    vertices[i].alpha = 0.4f;

            }
        }

        public void NextFrame()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Particle p = particles[i];
                p.livetime--;
                if (p.livetime < 0)
                    particles[i] = GenParticle();
                else
                {
                    p.position = p.position + p.motion;
                    int c = 0;
                    if (p.temperature > 400)
                        p.temperature -= 25;
                    else if (p.temperature > 200)
                        p.temperature -= 5;
                    else
                        p.temperature -= 1;
                }
            }
        }

        public void Init()
        {
            particles = new Particle[total_particles];
            vertices = new VertexData[total_particles];
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = GenParticle();
            }
        }

        public void Buffer()
        {
/*            particles = new Particle[total_particles];
            for (int i = 0; i < total_particles; i++)
                particles[i] = new Particle();

            particles[0].position = new Vector3(0.0f, 1.5f, 0.0f);
            particles[0].motion = Vector3.Zero;*/


            MakeVertices();


            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);

//            GL.BufferStorage(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * total_particles, IntPtr.Zero, BufferStorageFlags.None);

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * total_particles, vertices, BufferUsageHint.StreamDraw);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "position"));
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "color"));
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "size"));
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(VertexData)), (int)Marshal.OffsetOf(typeof(VertexData), "alpha"));

            GL.BindVertexArray(0);

            shader = GL.CreateProgram();
            int vertex_shader = GL.CreateShader(ShaderType.VertexShader);
            int fragment_shader = GL.CreateShader(ShaderType.FragmentShader);
            int geometry_shader = GL.CreateShader(ShaderType.GeometryShader);
            string result;

            GL.ShaderSource(vertex_shader, vertex_shader_src);
            GL.CompileShader(vertex_shader);
            result = GL.GetShaderInfoLog(vertex_shader);

            GL.ShaderSource(geometry_shader, geometry_shader_src);
            GL.CompileShader(geometry_shader);
            result = GL.GetShaderInfoLog(geometry_shader);

            GL.ShaderSource(fragment_shader, fragment_shader_src);
            GL.CompileShader(fragment_shader);
            result = GL.GetShaderInfoLog(fragment_shader);

            GL.AttachShader(shader, vertex_shader);
            GL.AttachShader(shader, geometry_shader);
            GL.AttachShader(shader, fragment_shader);

            GL.BindAttribLocation(shader, 0, "vertex");
            GL.BindAttribLocation(shader, 1, "color");
            GL.BindAttribLocation(shader, 2, "size");
            GL.BindAttribLocation(shader, 3, "alpha");

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
            GL.Uniform1(GL.GetUniformLocation(shader, "tex"), 0);

            MakeVertices();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
//            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(VertexData)) * total_particles, vertices, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, Marshal.SizeOf(typeof(VertexData)) * vertices.Length, vertices);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ParticleTexture);

            GL.DrawArrays(PrimitiveType.Points, 0, vertices.Length);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
