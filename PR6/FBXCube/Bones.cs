using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace FBXCube
{
    public class GFXBone
    {
        static Vector3[] vertexes = {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),

            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f)
        };

        static int[] indices = {0, 2, 1, 3, 5, 7, 4, 6, 6, 5, 5, 1, 4, 0, 6, 2, 7, 3};

        public GFXBone(string name, GFXSkeleton skeleton)
        {
            this.name = name;
            this.skel = skeleton;
            transform = Matrix4.Identity;
        }

        public string name { get; private set; }

        public GFXBone parent;
        private GFXSkeleton skel;
        public Vector3 offset;
        public float length;

        public Matrix4 transform;
        public Matrix4 local;

        static int vbo;
        static int ibo;
        static int vao;
        static int shader;

        static string vertex_shader_src = @"
attribute vec3 vertex;

uniform mat4 modelview;
uniform mat4 projection;
uniform mat4 transform;

varying vec3 tc;

void main() {
  gl_Position = projection*modelview*transform*vec4(vertex, 1.0);
  tc = vertex;
}
";

        static string fragment_shader_src = @"
varying vec3 tc;
uniform vec3 ncolor;

void main() {
  if (tc.x + tc.y + tc.z < 0.333)
    gl_FragColor = vec4(ncolor, 1.0);
  else if (tc.x < 0.05)
    if (tc.y < 0.05)
      if (tc.z < 0.05)
        gl_FragColor = vec4(ncolor, 1.0);
      else
        gl_FragColor = vec4(0.0, 0.0, 1.0, 1.0);
    else
      if (tc.z < 0.05)
        gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);
      else
        discard;
  else
    if (tc.y < 0.05)
      if (tc.z < 0.05)
        gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
      else
        discard;
    else
      discard;
}
";

        public static void Buffer()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.IndexArray);

            GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(Vector3)) * vertexes.Length, vertexes, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Vector3)), 0);

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

            GL.LinkProgram(shader);
            result = GL.GetProgramInfoLog(shader);

            int LinkRes = 0;
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out LinkRes);
        }

        public void Render(Matrix4 projection, Matrix4 modelview)
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.UseProgram(shader);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "modelview"), false, ref modelview);
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), false, ref projection);
            Matrix4 t = transform;
            GFXBone bone = this.parent;
            while (bone != null)
            {
                t = t*bone.transform*Matrix4.CreateTranslation(bone.offset);
                bone = bone.parent;
            }
            t = t * skel.gt;
            t = Matrix4.CreateScale(0.1f) * t;
            GL.UniformMatrix4(GL.GetUniformLocation(shader, "transform"), false, ref t);
            Vector3 clr;
            if (name == "Hips")
                clr = new Vector3(1.0f, 0.0f, 1.0f);
            else
                clr = new Vector3(1.0f, 1.0f, 1.0f);
            GL.Uniform3(GL.GetUniformLocation(shader, "ncolor"), ref clr);
            GL.DrawElements(PrimitiveType.TriangleStrip, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
        }
    }

    public class GFXSkeleton
    {
        private GFXSkeleton() { }

        private Dictionary<string, GFXBone> bones = new Dictionary<string, GFXBone>();

        public GFXBone root { get; private set; }

        public Matrix4 gt;

        public static GFXSkeleton FromBVH(Bvh bvh)
        {
            GFXSkeleton result = new GFXSkeleton();

            result.root = new GFXBone(bvh.root.name, result);

            Queue<Tuple<BvhPart, GFXBone>> q = new Queue<Tuple<BvhPart, GFXBone>>();

            q.Enqueue(new Tuple<BvhPart, GFXBone>(bvh.root, result.root));
            while (q.Count > 0)
            {
                var qi = q.Dequeue();
                foreach (var bn in qi.Item1.child)
                    if (bn.name != "" && bn.name != null)
                    {
                        GFXBone bone = new GFXBone(bn.name, result);
                        bone.parent = qi.Item2;
                        q.Enqueue(new Tuple<BvhPart, GFXBone>(bn, bone));
                    }

                qi.Item2.offset = qi.Item1.offset;
                qi.Item2.length = qi.Item2.offset.Length;

                result.bones.Add(qi.Item2.name, qi.Item2);
            }
            return result;
        }

        public void SetTransform(string name, Matrix4 transform)
        {
            GFXBone bone;
            if (bones.TryGetValue(name, out bone))
                bone.transform = transform;
        }

        public void Render(Matrix4 projection, Matrix4 modelview)
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            foreach (var bone in bones.Values)
                bone.Render(projection, modelview);
        }
    }
}