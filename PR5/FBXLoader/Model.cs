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

namespace FBXLoader
{
    struct ModelVertexData
    {
        public OpenTK.Vector3 position;
        public OpenTK.Vector2 tex_coord;
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

                    if (tex_width == 0 && tex_height > 0)
                        tex_width = 1;
                    if (tex_height == 0 && tex_width > 0)
                        tex_height = 1;

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
            public Bone parentBone;
            public string name;

            public ModelVertexData[] vertices;
            public Dictionary<Textures.Texture, int[]> indicesByMaterial = new Dictionary<Textures.Texture, int[]>();

            private int vbo;
            private int vao;
            public Dictionary<Textures.Texture, int> indexBuffers = new Dictionary<Textures.Texture, int>();

            public void Buffer()
            {
                vao = GL.GenVertexArray();
                vbo = GL.GenBuffer();
                foreach (var texture in indicesByMaterial.Keys)
                    indexBuffers.Add(texture, GL.GenBuffer());
                GL.BindVertexArray(vao);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.IndexArray);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.EnableClientState(ArrayCap.IndexArray);

                GL.BufferData(BufferTarget.ArrayBuffer, Marshal.SizeOf(typeof(ModelVertexData)) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

                foreach (var matBuffer in indexBuffers)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, matBuffer.Value);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indicesByMaterial[matBuffer.Key].Length, indicesByMaterial[matBuffer.Key], BufferUsageHint.StaticDraw);
                }

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                GL.EnableVertexAttribArray(3);

                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "position"));
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "tex_coord"));
                //GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "bone_indices"));
                //GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, (int)Marshal.SizeOf(typeof(ModelVertexData)), (int)Marshal.OffsetOf(typeof(ModelVertexData), "bone_weights"));

                GL.BindVertexArray(0);

            }

            public void Render()
            {
                GL.BindVertexArray(vao);

                foreach (var matBuffer in indexBuffers)
                {
                    GL.BindTexture(TextureTarget.Texture2D, matBuffer.Key.gl_id);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, matBuffer.Value);
                    GL.DrawElements(PrimitiveType.Triangles, indicesByMaterial[matBuffer.Key].Length, DrawElementsType.UnsignedInt, 0);
                }

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
            OpenTK.Matrix4 transform;
            OpenTK.Matrix4 cur_transform;
            public float Length { get; private set; }
            public Bone parent;
            public List<Bone> children = new List<Bone>();

            public Matrix4 gt;

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

            public void ApplyTransform(OpenTK.Matrix4 new_transform)
            {
                cur_transform = gt.Inverted() * new_transform * gt * cur_transform;
            }

            private Bone()
            {
                transform = Matrix4.Identity;
            }

            public static Bone FromNode(Node node)
            {
                Bone bone = new Bone();
                bone.name = node.Name;

                OpenTK.Vector3 translation = OpenTK.Vector3.Zero;
                if (node.TranslationActive.Get())
                {
                    FbxSharp.Vector3 trans = node.LclTranslation.Get();
                    translation = new OpenTK.Vector3((float)trans.X, (float)trans.Y, (float)trans.Z);
                }

                OpenTK.Vector3 scaling = OpenTK.Vector3.One;
                if (node.ScalingActive.Get())
                {
                    FbxSharp.Vector3 scale = node.LclScaling.Get();
                    scaling = new OpenTK.Vector3((float)scale.X, (float)scale.Y, (float)scale.Z);
                }

                OpenTK.Vector3 rotation = OpenTK.Vector3.Zero;
                if (node.RotationActive.Get())
                {
                    FbxSharp.Vector3 rot = node.LclRotation.Get();
                    rotation = new OpenTK.Vector3((float)rot.X, (float)rot.Y, (float)rot.Z);
                }

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

                bone.transform = ms * mr * mt;

                bone.cur_transform = bone.transform;

                return bone;
            }

            public OpenTK.Matrix4 Transform { get { return this.cur_transform; } }

        }

        List<ModelMesh> meshes = new List<ModelMesh>();
        List<Bone> bones = new List<Bone>();

        public Matrix4[] GetBoneTransforms()
        {
            Matrix4[] result = new Matrix4[bones.Count];
            for (int i = 0; i < bones.Count; i++)
            {
                result[i] = Matrix4.Identity;
                Bone bone = bones[i];
                while (bone != null)
                {
                    result[i] = result[i]*bone.Transform;
                    bone = bone.parent;
                }
            }
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
            root.TransposeTransform();
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
                    List<ModelVertex> mesh_vertices = new List<ModelVertex>(mesh.GetControlPointsCount());

                    for (int i = 0; i < mesh.GetControlPointsCount(); i++)
                    {
                        FbxSharp.Vector3 pos = transform.MultNormalize(new FbxSharp.Vector4(mesh.GetControlPointAt(i).ToVector3())).ToVector3();
//                        FbxSharp.Vector3 pos = mesh.GetControlPointAt(i).ToVector3();
                        mesh_vertices.Add(new ModelVertex { position = new OpenTK.Vector3((float)pos.X, (float)pos.Y, (float)pos.Z) });
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

                                mesh_vertices[index].bones.Add(new Tuple<int, float>(boneIndex, weight));
                            }
                        }
                    }

/*                    var polygons = mesh.PolygonIndexes.Select(p =>
                                                              new PolygonBuilder
                                                              {
                                                                PolygonVertexIndexes = p,
                                                                Vertexes = p.Select(ix => mesh_vertices[(int)ix]).ToList(),
                                                              }).ToList();*/

                    var polygons = mesh.PolygonIndexes.Select(poly => new List<int>(poly.Select(index => (int)(index)))).ToList();
                    var polygonsByMaterial = new Dictionary<SurfaceMaterial, List<List<int>>>();
                    Dictionary<SurfaceMaterial, Textures.Texture> materialTextures = new Dictionary<SurfaceMaterial, Textures.Texture>(); 

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
                        int texIndex = -1;
                        if (tex_prop != null)
                        {
                            foreach (FbxObject p in tex_prop.SrcObjects)
                                if (p is Texture)
                                {
                                    Texture texture = p as Texture;
                                    texIndex = Textures.AddTexture(texture.RelativeFilename);
                                }
                        }
                        materialTextures.Add(material, Textures.GetTexture(texIndex));
                        polygonsByMaterial[material] = new List<List<int>>(polygons);
                    }
                    else if (matelem.MappingMode == LayerElement.EMappingMode.ByPolygon)
                    {
                        // multiple materials
                        foreach (var material in node.Materials)
                        {
                            polygonsByMaterial[material] = new List<List<int>>();

                            Property tex_prop = material.FindProperty(prop => prop.Name.ToLower().Contains("diffuse"));
                            int texIndex = -1;
                            if (tex_prop != null)
                            {
                                foreach (FbxObject p in tex_prop.SrcObjects)
                                    if (p is Texture)
                                    {
                                        Texture texture = p as Texture;
                                        texIndex = Textures.AddTexture(texture.RelativeFilename);
                                    }
                            }
                            materialTextures.Add(material, Textures.GetTexture(texIndex));
                        }
                        int i;
                        for (i = 0; i < matindexes.Count; i++)
                        {
                            var mat = node.Materials[matindexes[i]];
                            polygonsByMaterial[mat].Add(polygons[i]);
                        }
                        //throw new NotImplementedException("Materials must have mapping modes of AllSame or ByPolygon");
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

                        foreach (var polygon in polygons)
                            for (int i = 0; i < polygon.Count; i++)
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
                                if (mesh_vertices[polygon[i]].tex_ccord_assigned)
                                {
                                    ModelVertex newVertex = mesh_vertices[polygon[i]].Duplicate();
                                    newVertex.tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                    polygon[i] = mesh_vertices.Count;
                                    mesh_vertices.Add(newVertex);
                                }
                                else
                                {
                                    mesh_vertices[polygon[i]].tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                    mesh_vertices[polygon[i]].tex_ccord_assigned = true;
                                }
                                k++;
                            }
                    }

                    /*   int[] mesh_indices = new int[index_count];
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

                               if (mesh_vertices[mesh_indices[i]].tex_ccord_assigned)
                               {
                                   ModelVertex newVertex = mesh_vertices[mesh_indices[i]].Duplicate();
                                   newVertex.tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                   mesh_indices[i] = mesh_vertices.Count;
                                   mesh_vertices.Add(newVertex);
                               }
                               else
                               {
                                   mesh_vertices[mesh_indices[i]].tex_coord = new OpenTK.Vector2((float)v.X, 1.0f - (float)v.Y);
                                   mesh_vertices[mesh_indices[i]].tex_ccord_assigned = true;
                               }
                               k++;
                           }

                       }*/
                       mod_mesh.vertices = mesh_vertices.Select(vertex => new ModelVertexData
                       {
                           position = vertex.position,
                           tex_coord = vertex.tex_coord,
                       }).ToArray();
                    foreach (var material in polygonsByMaterial.Keys)
                    {
                        mod_mesh.indicesByMaterial.Add(materialTextures[material], polygonsByMaterial[material].SelectMany(poly => poly).ToArray());
                    }
                }

            foreach (Bone bone in result.bones)
            {
                FbxSharp.Matrix m = NodeByBone[bone].EvaluateGlobalTransform();
                Matrix4 mm = new Matrix4((float)m.M00, (float)m.M10, (float)m.M20, (float)m.M30,
                                         (float)m.M01, (float)m.M11, (float)m.M21, (float)m.M31,
                                         (float)m.M02, (float)m.M12, (float)m.M22, (float)m.M32,
                                         (float)m.M03, (float)m.M13, (float)m.M23, (float)m.M33);
                bone.gt = mm;
            }

            /*            foreach (Node node in sc.Nodes)
                        {
                            Bone bone = Bone.FromNode(node);
                            if (node == sc.GetRootNode())
                            {
                                result.bones.Insert(0, bone);
                                bone.TransposeTransform();
                            }
                            else
                                result.bones.Add(bone);
                        }*/

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

        public int GetBonesCount()
        {
            return bones.Count;
        }
    }
}
