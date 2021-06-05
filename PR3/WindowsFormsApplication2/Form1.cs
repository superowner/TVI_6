using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            buffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            gfx = Graphics.FromImage(buffer);
        }
        struct Vertex 
        {
            public Vector4 position;
            public Color color;
            public Vector3 normal;
        }
        Vertex[,] cube = new Vertex[,] {

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)} },

                                             { new Vertex {position = new Vector4( 1,-1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color.Orange, normal = new Vector3(0,0,-1)} }, //1

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)},
                                               new Vertex {position = new Vector4( 1,-1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)} },

                                             { new Vertex {position = new Vector4( 1,-1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)},
                                               new Vertex {position = new Vector4( 1, 1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color.Violet, normal = new Vector3(0,0,1)} }, //2

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color.Blue, normal = new Vector3(-1,0,0)},
                                               new Vertex {position = new Vector4(-1,-1, 1,1), color = Color.Blue, normal = new Vector3(-1,0,0)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color.Blue, normal = new Vector3(-1,0,0)} },

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color.Blue, normal = new Vector3(-1,0,0)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color.Blue, normal = new Vector3(-1,0,0)},
                                               new Vertex {position = new Vector4(-1, 1,-1,1), color = Color.Blue, normal = new Vector3(-1,0,0)} }, //3

                                             { new Vertex {position = new Vector4(1,-1,-1,1), color = Color.Pink, normal = new Vector3(1,0,0)},
                                               new Vertex {position = new Vector4(1,-1, 1,1), color = Color.Pink, normal = new Vector3(1,0,0)},
                                               new Vertex {position = new Vector4(1, 1,-1,1), color = Color.Pink, normal = new Vector3(1,0,0)} },

                                             { new Vertex {position = new Vector4(1,-1, 1,1), color = Color.Pink, normal = new Vector3(1,0,0)},
                                               new Vertex {position = new Vector4(1, 1, 1,1), color = Color.Pink, normal = new Vector3(1,0,0)},
                                               new Vertex {position = new Vector4(1, 1,-1,1), color = Color.Pink, normal = new Vector3(1,0,0)} }, //4

                                             { new Vertex {position = new Vector4(-1,-1,-1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)},
                                               new Vertex {position = new Vector4(-1,-1, 1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)} },

                                             { new Vertex {position = new Vector4(-1,-1, 1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)},
                                               new Vertex {position = new Vector4( 1,-1, 1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)},
                                               new Vertex {position = new Vector4( 1,-1,-1,1), color = Color.Magenta, normal = new Vector3(0,-1,0)} }, //5

                                             { new Vertex {position = new Vector4(-1, 1,-1,1), color = Color.Red, normal = new Vector3(0,1,0)},
                                               new Vertex {position = new Vector4(-1, 1, 1,1), color = Color.Red, normal = new Vector3(0,1,0)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color.Red, normal = new Vector3(0,1,0)} },

                                             { new Vertex {position = new Vector4(-1, 1, 1,1), color = Color.Red, normal = new Vector3(0,1,0)},
                                               new Vertex {position = new Vector4( 1, 1, 1,1), color = Color.Red, normal = new Vector3(0,1,0)},
                                               new Vertex {position = new Vector4( 1, 1,-1,1), color = Color.Red, normal = new Vector3(0,1,0)} }, //6
                                          };

        Vertex[,] back1 = new Vertex[,] {
                                             { new Vertex {position = new Vector4(-5,-5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 5,-5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-5, 5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)} },

                                             { new Vertex {position = new Vector4( 5,-5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 5, 5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-5, 5, 0,1),  color = Color.FromArgb(0, 128, 255), normal = new Vector3(0,0,-1)} }
                                        };

        Vertex[,] back2 = new Vertex[,] {
                                             { new Vertex {position = new Vector4(-5,-5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 5,-5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-5, 5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)} },

                                             { new Vertex {position = new Vector4( 5,-5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4( 5, 5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)},
                                               new Vertex {position = new Vector4(-5, 5, 0,1),  color = Color.FromArgb(224, 224, 224), normal = new Vector3(0,0,-1)} }
                                        };


        float rotx = 0.0f;
        float roty = 0.0f;
        float rotz = 0.0f;
        Bitmap buffer;
        Graphics gfx;

        private Vertex[,] ApplyTransform(Vertex[,] model, Matrix4x4 transform)
        {
            Vertex[,] mdl = (Vertex[,])model.Clone();
            for (int pindex = 0; pindex < model.GetLength(0); pindex++)
                for (int i = 0; i < model.GetLength(1); i++)
                {
                    mdl[pindex, i].position = Vector4.Transform(mdl[pindex, i].position, transform);
                    mdl[pindex, i].normal = Vector3.TransformNormal(mdl[pindex, i].normal, transform);
                }
            return mdl;

        }

        private void DrawModel(Vertex[,] model)
        {
            Point[] poly = new Point[3];
            for (int pindex = 0; pindex < model.GetLength(0); pindex++)
            {
                for (int i = 0; i < model.GetLength(1); i++)
                {
                    poly[i].X = (int)(model[pindex, i].position.X / model[pindex, i].position.W);
                    poly[i].Y = (int)(model[pindex, i].position.Y / model[pindex, i].position.W);
                }
                gfx.FillPolygon(new SolidBrush(model[pindex, 0].color), poly);
            }
        }

        private Vertex[,] Aggregate(Vertex[,] model1, Vertex[,] model2)
        {
            Vertex[,] result = new Vertex[model1.GetLength(0) + model2.GetLength(0), model1.GetLength(1)];

            int index = 0;
            for (int i = 0; i < model1.GetLength(0); i++, index++)
                for (int j = 0; j < model1.GetLength(1); j++)
                    result[index, j] = model1[i, j];

            for (int i = 0; i < model2.GetLength(0); i++, index++)
                for (int j = 0; j < model2.GetLength(1); j++)
                    result[index, j] = model2[i, j];

            return result;
        }

        private static double Scalar(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        private static Vector3 ToVector(Vector4 v1, Vector4 v2)
        {
            return new Vector3(v2.X / v2.W - v1.X / v1.W, v2.Y / v2.W - v1.Y / v1.W, v2.Z / v2.W - v1.Z / v1.W);
        }

        private static Vector3 GetNormal(Vertex[] triangle)
        {
            Vector3 v1 = ToVector(triangle[0].position, triangle[1].position);
            Vector3 v2 = ToVector(triangle[0].position, triangle[2].position);
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        //private static bool IsCovered(Vertex[] triangle1, Vertex[] triangle2)
        //{
        //    int isPositive = 2;
        //    bool breaked = false;
        //    for (int i = 0; i < triangle1.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < triangle2.GetLength(0); j++)
        //        {
        //            float scalar = Scalar(ToVector(triangle1[i].position, triangle2[j].position), triangle1[i].normal);
        //            if (Math.Abs(scalar) > 0f)
        //            {
        //                if (isPositive == 2)
        //                    if (scalar > 0)
        //                        isPositive = 1;
        //                    else
        //                        isPositive = 0;
        //                if ((isPositive == 0 && scalar > 0) || (isPositive == 1 && scalar < 0))
        //                {
        //                    breaked = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (breaked)
        //            break;
        //    }
        //    if (!breaked)
        //        return (isPositive == 0) && (Scalar(triangle1[0].normal, new Vector3(0.0f, 0.0f, -1.0f)) < 0) || (isPositive != 0) && (Scalar(triangle1[0].normal, new Vector3(0.0f, 0.0f, -1.0f)) > 0);

        //    isPositive = 2;
        //    breaked = false;
        //    for (int i = 0; i < triangle2.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < triangle1.GetLength(0); j++)
        //        {
        //            float scalar = Scalar(ToVector(triangle2[i].position, triangle1[j].position), triangle2[i].normal);
        //            if (Math.Abs(scalar) > 0f)
        //            {
        //                if (isPositive == 2)
        //                    if (scalar > 0)
        //                        isPositive = 1;
        //                    else
        //                        isPositive = 0;
        //                if ((isPositive == 0 && scalar > 0) || (isPositive == 1 && scalar < 0))
        //                {
        //                    breaked = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (breaked)
        //            break;
        //    }
        //    return !((isPositive == 0) && (Scalar(triangle2[0].normal, new Vector3(0.0f, 0.0f, -1.0f)) < 0) || (isPositive != 0) && (Scalar(triangle2[0].normal, new Vector3(0.0f, 0.0f, -1.0f)) > 0));
        //}

        private bool IsCovered(Vertex[] triangle1, Vertex[] triangle2, bool called=false)
        {
            
            Vector3 normal = GetNormal(triangle1);
            int isPositive = 2;
            bool breaked = false;
            for (int i = 0; i < triangle1.GetLength(0); i++)
            {
                for (int j = 0; j < triangle2.GetLength(0); j++)
                {
                    double scalar = Scalar(normal, ToVector(triangle1[i].position, triangle2[j].position));
                    if (Math.Abs(scalar) > 16f)
                        if (isPositive == 2)
                        {
                            isPositive = scalar > 0 ? 1 : 0;
                        }
                        else if ((scalar > 0) ^ (isPositive == 1))
                        {
                            breaked = true;
                            isPositive = 2;
                            break;
                        }
                }
                if (breaked)
                    break;
            }
            if (breaked && !called)
            {
                return !IsCovered(triangle2, triangle1, true);
            } 

            if (isPositive == 2)
            {
                //float dist1 = Math.Abs(getTriangleMZ(triangle1));
                //float dist2 = Math.Abs(getTriangleMZ(triangle2));
                //return dist1 > dist2;
                //throw new Exception("PIZDEC");
            }

            return !((isPositive == 1) ^ Scalar(normal, new Vector3(0, 0, -1)) > 0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            rotx += 15.0f;
            roty += 7.5f;
            rotz += 2.5f;
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 3.0), (float)buffer.Width / buffer.Height, 1.0f, 100.0f);
            Matrix4x4 windowScale = Matrix4x4.CreateScale(buffer.Width / 2, -buffer.Height / 2, 1.0f) * Matrix4x4.CreateTranslation(buffer.Width / 2, buffer.Height / 2, 0.0f);
            Matrix4x4 transform = Matrix4x4.CreateRotationX((float)(rotx / 180.0 * Math.PI)) *
                                  Matrix4x4.CreateRotationY((float)(roty / 180.0 * Math.PI)) *
                                  Matrix4x4.CreateRotationZ((float)(rotz / 180.0 * Math.PI)) *
                
                Matrix4x4.CreateTranslation(0.0f, 0.0f, -5.0f) * projection * windowScale;
            gfx.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, buffer.Width, buffer.Height));
            Vertex[,] model = ApplyTransform(cube, transform);

            var b1 = ApplyTransform(back1, Matrix4x4.CreateRotationX((float)(trackBar1.Value*Math.PI/180.0))*Matrix4x4.CreateTranslation(0.0f, 1.0f, -8.0f)*projection*windowScale);
            var b2 = ApplyTransform(back2, Matrix4x4.CreateRotationX((float)(trackBar1.Value * Math.PI / 180.0)) * Matrix4x4.CreateTranslation(0.0f, -1.0f, -8.0f) * projection * windowScale);
            var back = Aggregate(b1, b2);
            SortTriangles(back);
            DrawModel(back);

            SortTriangles(model);
            DrawModel(model);

            pictureBox1.Image = buffer;
        }
        
        private Vertex[] getTriangle(Vertex[,] model, int index)
        {
            return new Vertex[3] { 
            model[index, 0],
            model[index,1],
            model[index,2]
            };
        }
        private void setTriangle(Vertex[,] model, int index, Vertex[] triangle)
        {
            model[index, 0] = triangle[0];
            model[index, 1] = triangle[1];
            model[index, 2] = triangle[2];
        }
        private void swapTriangles(Vertex[,] model, int index1, int index2)
        {
            Vertex[] t = getTriangle(model, index1);
            setTriangle(model, index1, getTriangle(model, index2));
            setTriangle(model, index2, t);
        }
        float getTriangleMZ(Vertex[] triangle)
        {
            return (triangle[0].position.Z/
                    triangle[0].position.W+
                    triangle[1].position.Z/
                    triangle[1].position.W+
                    triangle[2].position.Z/
                    triangle[2].position.W)/3.0f;
        }
        void SortTriangles(Vertex[,] model)
        {
            for (int i=0; i < model.GetLength(0); i++)
                for (int j=1; j < model.GetLength(0); j++)
                    if (!IsCovered(getTriangle(model, j - 1), getTriangle(model, j)))
                        swapTriangles(model, j - 1, j);
        }
    }
}

