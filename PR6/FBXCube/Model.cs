using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using FbxSharp;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FBXCube
{
    struct ModelVertexData
    {
        public OpenTK.Vector3 position;
        public OpenTK.Vector2 tex_coord;
        public OpenTK.Vector4 bone_indices;
/*        public int bone0;
        public int bone1;
        public int bone2;
        public int bone3;*/
        public OpenTK.Vector4 bone_weights;
    }

    static class Textures
    {
        private static int LoadTexture(string filename, out bool alpha)
        {
            int res = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, res);
            Bitmap teximage = Image.FromFile(filename) as Bitmap;

            alpha = (teximage.PixelFormat == System.Drawing.Imaging.PixelFormat.Alpha) ||
                    (teximage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppArgb1555) ||
                    (teximage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb) ||
                    (teximage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            int tex_width = teximage.Width;
            int tex_height = teximage.Height;
            int level = 0;
            do
            {
                Bitmap texture_layer = new Bitmap(tex_width, tex_height);
                using (Graphics texture = Graphics.FromImage(texture_layer))
                {
                    texture.CompositingMode = CompositingMode.SourceCopy;
                    texture.CompositingQuality = CompositingQuality.HighQuality;
                    texture.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    texture.SmoothingMode = SmoothingMode.HighQuality;
                    texture.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    texture.DrawImage(teximage, 0, 0, tex_width, tex_height);
                    BitmapData texdata = texture_layer.LockBits(new Rectangle(0, 0, tex_width, tex_height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, level, PixelInternalFormat.Rgba, tex_width, tex_height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, texdata.Scan0);
                    tex_width /= 2;
                    tex_height /= 2;
                    ++level;
                    texture_layer.UnlockBits(texdata);
                }
            } while (tex_width > 0 && tex_height > 0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Repeat);

            return res;
        }

        public class Texture
        {
            public int id;
            public string filename;
            public int gl_id;
            public bool has_alpha;
        }

        private static List<Texture> textures = new List<Texture>();

        public static void Load()
        {
            foreach (Texture tex in textures)
                if (tex.gl_id < 0)
                    tex.gl_id = LoadTexture(tex.filename, out tex.has_alpha);
        }

        public static int AddTexture(string file)
        {
            int index = textures.FindIndex(texture => texture.filename == file);
            if (index < 0)
            {
                textures.Add(new Texture { id = -1, filename = file, gl_id = -1 });
                return textures.Count - 1;
            }
            else
                return index;
        }
        public static Texture GetTexture(int index) {
            return textures[index];
        }
    }

    class Model
    {
        private class ModelMesh
        {
            public int texture;
            public Bone parentBone;
            public string name;

            public ModelVertexData[] vertexes;
            public int[] indices;

            private int vao;
            private int vbo;
            private int vibo;

            public void Buffer()
            {
                vao = GL.GenVertexArray();
                vbo = GL.GenBuffer();
                vibo = GL.GenBuffer();
                GL.BindVertexArray(vao);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.IndexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, vibo);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.IndexArray);

                GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(ModelVertexData)) * vertexes.Length, vertexes, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticDraw);

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                GL.EnableVertexAttribArray(3);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "position"));
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "tex_coord"));
                GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "bone_indices"));
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "bone_weights"));

                GL.BindVertexArray(0);

            }

            public void Render()
            {
                GL.BindVertexArray(vao);

                GL.BindTexture(TextureTarget.Texture2D, Textures.GetTexture(texture).gl_id);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, vibo);

                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }
        }

        private class ModelVertex
        {
            public OpenTK.Vector3 position;
            public OpenTK.Vector2 tex_coord;
            public bool tex_ccord_assigned;
            public List<Tuple<int, float>> bones = new List<Tuple<int, float>>();
            public ModelVertex Duplicate()
            {
                return new ModelVertex { position = this.position, tex_coord = this.tex_coord, tex_ccord_assigned = this.tex_ccord_assigned, bones = new List<Tuple<int, float>>(this.bones) };
            }
        }

        public class Bone
        {
            public int index;
            public string name;
            public OpenTK.Matrix4 transform { get; private set; }
            OpenTK.Matrix4 cur_transform;
            public float Length { get; private set; }
            public Bone parent;
            public List<Bone> children = new List<Bone>();

            public Matrix4 gt;
            public Matrix4 lt;
            public Matrix4 fix;

            public Matrix4 raw_translation { get; private set; }
            public Matrix4 raw_rotation { get; private set; }
            public Matrix4 raw_scale { get; private set; }

            public OpenTK.Vector3 vec_translation;
            public OpenTK.Vector3 vec_rotation;
            public OpenTK.Vector3 vec_scale;

//            public OpenTK.Vector3 rotation;
//            public OpenTK.Vector3 full_rotation;


            public void TransposeTransform()
            {
                transform.Transpose();
            }

            public void ResetTransform()
            {
                cur_transform = transform;
            }

 
            private Bone()
            {
                transform = Matrix4.Identity;
            }

            public static Bone FromNode(Node node)
            {
                Bone bone = new Bone();
                bone.name = node.Name;

                FbxSharp.Vector3 trans = node.LclTranslation.Get();
                OpenTK.Vector3 translation = new OpenTK.Vector3((float)trans.X, (float)trans.Y, (float)trans.Z);

                FbxSharp.Vector3 scale = node.LclScaling.Get();
                OpenTK.Vector3 scaling = new OpenTK.Vector3((float)scale.X, (float)scale.Y, (float)scale.Z);

                FbxSharp.Vector3 rot = node.LclRotation.Get();
                OpenTK.Vector3 rotation = new OpenTK.Vector3((float)rot.X, (float)rot.Y, (float)rot.Z);

                var order = node.RotationOrder.Get();
                Matrix4 mt = Matrix4.CreateTranslation(translation);
                Matrix4 mrx = Matrix4.CreateRotationX((float)(rotation.X * 180.0 / Math.PI));
                Matrix4 mry = Matrix4.CreateRotationY((float)(rotation.Y * 180.0 / Math.PI));
                Matrix4 mrz = Matrix4.CreateRotationZ((float)(rotation.Z * 180.0 / Math.PI));
                Matrix4 ms = Matrix4.CreateScale(scaling.X, scaling.Y, scaling.Z);
                Matrix4 mr;
                switch (order)
                {
                    case Node.ERotationOrder.OrderXYZ:
                        mr = mrx * mry * mrz;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                Matrix4 xx = ms * mr * mt;
                //                bone.fix = ms * mr * mt;

                bone.raw_rotation = mr;
                bone.raw_translation = mt;
                bone.raw_scale = ms;
                bone.vec_translation = translation;
                bone.vec_rotation = rotation;
                bone.vec_scale = scaling;


                bone.transform = (node.ScalingActive.Get()  ? ms : Matrix4.Identity) * 
                                 (node.RotationActive.Get() ? mr : Matrix4.Identity) *
                                 (node.TranslationActive.Get() ? mt : Matrix4.Identity);

                bone.cur_transform = bone.transform;

                return bone;
            }

            public OpenTK.Matrix4 Transform { get { return this.cur_transform; } }


            public void ApplyTransform(OpenTK.Matrix4 new_transform)
            {
                cur_transform = cur_transform * new_transform;
            }

        }

        List<ModelMesh> meshes = new List<ModelMesh>();
        List<Bone> bones = new List<Bone>();


        private Matrix4 GetBoneTransform(Bone bone, int MaxDepth)
        {
            int depth = 0;
            Bone tmp;
            for (tmp = bone; tmp.parent != null; tmp = tmp.parent) depth++;
            Matrix4 result = Matrix4.Identity;

            tmp = bone;
            depth--;
            while (tmp.parent != null)
            {
                if (depth <= MaxDepth)
                    result = (tmp.fix.Inverted() * result * tmp.Transform * tmp.fix);
                tmp = tmp.parent;
                depth--;
            }
            return result;
        }

        public Matrix4[] GetBoneTransforms()
        {
            Matrix4[] result = new Matrix4[bones.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetBoneTransform(bones[i], 99);

            return result;
        }

        private Model()
        {
        }

        public static Model FromFile(string FileName)
        {
            Model result = new Model();
            Importer importer = new Importer();

            Scene sc = importer.Import(FileName);

            Dictionary<Bone, Node> NodeByBone = new Dictionary<Bone, Node>();
            Dictionary<Node, Bone> BoneByNode = new Dictionary<Node, Bone>();

            Stack<Tuple<Bone, Node>> stack = new Stack<Tuple<Bone, Node>>();

            Bone root = Bone.FromNode(sc.GetRootNode());

            //            root.TransposeTransform();
            stack.Push(new Tuple<Bone, Node>(root, sc.GetRootNode()));
            BoneByNode.Add(sc.GetRootNode(), root);
            NodeByBone.Add(root, sc.GetRootNode());

            while (stack.Count > 0)
            {
                Tuple<Bone, Node> cur = stack.Pop();
                foreach (Node child in cur.Item2.ChildNodes)
                {
                    Bone newBone = Bone.FromNode(child);
                    newBone.parent = cur.Item1;
                    cur.Item1.children.Add(newBone);
                    stack.Push(new Tuple<Bone, Node>(newBone, child));
                    BoneByNode.Add(child, newBone);
                    NodeByBone.Add(newBone, child);
                }
            }

            Queue<Bone> boneTree = new Queue<Bone>();
            boneTree.Enqueue(root);

            while (boneTree.Count > 0)
            {
                Bone cur = boneTree.Dequeue();
                result.bones.Add(cur);
                foreach (Bone child in cur.children)
                    boneTree.Enqueue(child);
            }
            for (int i = 0; i < result.bones.Count; i++)
                result.bones[i].index = i;

            foreach (Node node in sc.Nodes)
                if (node.GetNodeAttributeCount() > 0 && node.GetNodeAttributeByIndex(0) is Mesh)
                {
                    Mesh mesh = node.GetNodeAttributeByIndex(0) as Mesh;
                    ModelMesh mod_mesh = new ModelMesh();
                    mod_mesh.name = mesh.Name;
                    mod_mesh.parentBone = BoneByNode[node];
                    result.meshes.Add(mod_mesh);

                    FbxSharp.Matrix transform = node.EvaluateGlobalTransform();
                    List<ModelVertex> mesh_vertexes = new List<ModelVertex>(mesh.GetControlPointsCount());

                    for (int i = 0; i < mesh.GetControlPointsCount(); i++)
                    {
                        FbxSharp.Vector3 pos = transform.MultNormalize(mesh.GetControlPointAt(i)).ToVector3();
                        mesh_vertexes.Add(new ModelVertex { position = new OpenTK.Vector3((float)pos.X, (float)pos.Y, (float)pos.Z) });
                    }
                    if (mesh.GetDeformerCount() == 1)
                    {
                        if (!(mesh.GetDeformer(0) is Skin))
                            throw new NotImplementedException("Only Skin deformers are implemented");
                        Skin skin = mesh.GetDeformer(0) as Skin;

                        foreach (Cluster cluster in skin.Clusters)
                        {
                            var cnode = cluster.GetLink();
                            var cbone = BoneByNode[cnode];
                            var boneIndex = cbone.index;

                            for (int i = 0; i < cluster.ControlPointIndices.Count; i++)
                            {
                                int index = cluster.ControlPointIndices[i];
                                float weight = (float)cluster.ControlPointWeights[i];

                                mesh_vertexes[index].bones.Add(new Tuple<int, float>(boneIndex, weight));
                            }
                        }
                    }

                    Layer layer = mesh.GetLayer(0);
                    LayerElementMaterial matelem = layer.GetMaterials();
                    List<int> matindexes = matelem.MaterialIndexes.List;// GetIndexArray().List;
                    if (matelem.ReferenceMode != LayerElement.EReferenceMode.IndexToDirect)
                        throw new NotImplementedException("A materials must have a reference mode of IndexToDirect");
                    if (matelem.MappingMode == LayerElement.EMappingMode.AllSame)
                    {
                        // only one material
                        SurfaceMaterial material = node.GetMaterial(matindexes[0]);
                        Property tex_prop = material.FindProperty(prop => prop.Name.ToLower().Contains("diffuse"));
                        if (tex_prop != null)
                        {
                            foreach (FbxObject p in tex_prop.SrcObjects)
                                if (p is Texture)
                                {
                                    Texture texture = p as Texture;
                                    mod_mesh.texture = Textures.AddTexture(texture.RelativeFilename);
                                }
                        }

                        //                        polygonsByMaterial[material] = new List<PolygonBuilder>(polygons);
                    }
                    else if (matelem.MappingMode == LayerElement.EMappingMode.ByPolygon)
                    {
                        /*                      // multiple materials
                                              foreach (var mat in node.Materials)
                                              {
                                                  polygonsByMaterial[mat] = new List<PolygonBuilder>();
                                              }
                                              int i;
                                              for (i = 0; i < matindexes.Count; i++)
                                              {
                                                  var mat = node.Materials[matindexes[i]];
                                                  polygonsByMaterial[mat].Add(polygons[i]);
                                              }*/
                        throw new NotImplementedException("Materials must have mapping modes of AllSame or ByPolygon");
                    }
                    else
                    {
                        throw new NotImplementedException("Materials must have mapping modes of AllSame or ByPolygon");
                    }

                    int index_count = 0;
                    foreach (List<long> polygon in mesh.PolygonIndexes)
                    {
                        index_count += polygon.Count;
                        if (polygon.Count != 3)
                            throw new NotImplementedException("Model must be trianglized");
                    }

                    int[] mesh_indices = new int[index_count];
                    int cur_index = 0;
                    foreach (List<long> polygon in mesh.PolygonIndexes)
                        foreach (long index in polygon)
                            mesh_indices[cur_index++] = (int)index;

                    if (layer.GetUVs() != null)
                    {
                        var uvElement = layer.GetUVs();
                        if (uvElement.MappingMode != LayerElement.EMappingMode.ByPolygonVertex)
                        {
                            throw new NotImplementedException("UV layer elements must have a mapping mode of ByPolygonVertex");
                        }
                        if (uvElement.ReferenceMode != LayerElement.EReferenceMode.Direct &&
                            uvElement.ReferenceMode != LayerElement.EReferenceMode.IndexToDirect)
                        {
                            throw new NotImplementedException("UV layer elements must have a reference mode of Direct or IndexToDirect");
                        }
                        int k = 0;
                        for (int i = 0; i < mesh_indices.Length; i++)
                        {
                            int nindex;
                            if (uvElement.ReferenceMode == LayerElement.EReferenceMode.Direct)
                            {
                                nindex = k;
                            }
                            else
                            {
                                nindex = uvElement.GetIndexArray().GetAt(k);
                            }

                            var v = uvElement.GetDirectArray().GetAt(nindex);

                            if (mesh_vertexes[mesh_indices[i]].tex_ccord_assigned)
                            {
                                ModelVertex newVertex = mesh_vertexes[mesh_indices[i]].Duplicate();
                                newVertex.tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                mesh_indices[i] = mesh_vertexes.Count;
                                mesh_vertexes.Add(newVertex);
                            }
                            else
                            {
                                mesh_vertexes[mesh_indices[i]].tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                mesh_vertexes[mesh_indices[i]].tex_ccord_assigned = true;
                            }
                            k++;
                        }

                    }
                    mod_mesh.vertexes = mesh_vertexes.Select(vertex => new ModelVertexData
                    {
                        position = vertex.position,
                        tex_coord = vertex.tex_coord,
                        bone_indices = new OpenTK.Vector4(vertex.bones.Count > 0 ? vertex.bones[0].Item1 : -1.0f,
                                                          vertex.bones.Count > 1 ? vertex.bones[1].Item1 : -1.0f,
                                                          vertex.bones.Count > 2 ? vertex.bones[2].Item1 : -1.0f,
                                                          vertex.bones.Count > 3 ? vertex.bones[3].Item1 : -1.0f),

                        bone_weights = new OpenTK.Vector4(vertex.bones.Count > 0 ? vertex.bones[0].Item2 : 0.0f,
                                                          vertex.bones.Count > 1 ? vertex.bones[1].Item2 : 0.0f,
                                                          vertex.bones.Count > 2 ? vertex.bones[2].Item2 : 0.0f,
                                                          vertex.bones.Count > 3 ? vertex.bones[3].Item2 : 0.0f)
                    }).ToArray();
                    mod_mesh.indices = mesh_indices;
                }

            foreach (Bone bone in result.bones)
            {
                FbxSharp.Matrix m = NodeByBone[bone].EvaluateGlobalTransform();
                Matrix4 mm = new Matrix4((float)m.M00, (float)m.M10, (float)m.M20, (float)m.M30,
                                         (float)m.M01, (float)m.M11, (float)m.M21, (float)m.M31,
                                         (float)m.M02, (float)m.M12, (float)m.M22, (float)m.M32,
                                         (float)m.M03, (float)m.M13, (float)m.M23, (float)m.M33);
                bone.gt = mm;
                m = NodeByBone[bone].EvaluateLocalTransform();
                mm = new Matrix4((float)m.M00, (float)m.M10, (float)m.M20, (float)m.M30,
                                         (float)m.M01, (float)m.M11, (float)m.M21, (float)m.M31,
                                         (float)m.M02, (float)m.M12, (float)m.M22, (float)m.M32,
                                         (float)m.M03, (float)m.M13, (float)m.M23, (float)m.M33);
                bone.lt = mm;

            }

            return result;
        }

        public void Buffer()
        {
            foreach (ModelMesh mesh in meshes)
                mesh.Buffer();
        }

        public void Render()
        {
            foreach (ModelMesh mesh in meshes)
                mesh.Render();
        }

        public Bone GetBone(int index)
        {
            return bones[index];
        }

        public Bone GetBoneByName(string name)
        {
            return bones.Find(bone => bone.name == name);
        }

        public int GetBonesCount()
        {
            return bones.Count;
        }
    }
}
