using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using OpenTK;

namespace FBXCube
{
    public struct Motion
    {
        public Vector3 translation;
        public Vector3 rotation;
        //            public Matrix4 transform;
    }

    public class BvhPart
    {
        public enum channelTypes { Xpos, Ypos, Zpos, Zrot, Xrot, Yrot };
        public string name;

        public BvhPart()
        {
        }

        public Vector3 offset=new Vector3();

//        public List<Matrix4> motion=new List<Matrix4>();
        public List<Motion> motion = new List<Motion>();

        public BvhPart parent;
        public List<channelTypes> channels= new List<channelTypes>();
        public List<BvhPart> child= new List<BvhPart>();
    }

    static class eMatrix4
    {
        public static void Translate(ref Matrix4 matrix, Vector3 vector)
        {
          /*  matrix.M41 += (matrix.M11 * vector.X) + (matrix.M21 * vector.Y) + (matrix.M31 * vector.Z);
            matrix.M42 += (matrix.M12 * vector.X) + (matrix.M22 * vector.Y) + (matrix.M32 * vector.Z);
            matrix.M43 += (matrix.M13 * vector.X) + (matrix.M23 * vector.Y) + (matrix.M33 * vector.Z);*/
        }

        public static void Translate(ref Matrix4 matrix, float x, float y, float z)
        {
            matrix = matrix * Matrix4.CreateTranslation(x, y, z);

//            matrix.M41 = (matrix.M11 * x) + (matrix.M21 * y) + (matrix.M31 * z);
//            matrix.M42 = (matrix.M12 * x) + (matrix.M22 * y) + (matrix.M32 * z);
//            matrix.M43 = (matrix.M13 * x) + (matrix.M23 * y) + (matrix.M33 * z);
        }

        public static void RotateX(ref Matrix4 matrix, float angle)
        {
            float tempX = matrix.M41;
            float tempY = matrix.M42;
            float tempZ = matrix.M43;

//            matrix.M41 = 0;
//            matrix.M42 = 0;
//            matrix.M43 = 0;

            Matrix4 temp = Matrix4.CreateRotationX(angle);
            temp.Transpose();
            //            temp.LoadIdentity();
            //            temp.matrix[5] = (float)Math.Cos(rad); temp.matrix[9] = (float)Math.Sin(rad);
            //            temp.matrix[6] = (float)-Math.Sin(rad); temp.matrix[10] = (float)Math.Cos(rad);

            temp = matrix * temp;
            matrix.Row0 = temp.Row0;
            matrix.Row1 = temp.Row1;
            matrix.Row2 = temp.Row2;
            matrix.Row3 = temp.Row3;

            //            this.Set((this) * temp);

            // restore x,y,z so rotation won't affect them - doesn't appear to work
//            matrix.M41 = tempX;
//            matrix.M42 = tempY;
//            matrix.M43 = tempZ;
        }

        public static void RotateY(ref Matrix4 matrix, float angle)
        {
            float tempX = matrix.M41;
            float tempY = matrix.M42;
            float tempZ = matrix.M43;

//            matrix.M41 = 0;
//            matrix.M42 = 0;
//            matrix.M43 = 0;

            Matrix4 temp = Matrix4.CreateRotationY(angle);
            temp.Transpose();
            //            temp.LoadIdentity();
            //            temp.matrix[5] = (float)Math.Cos(rad); temp.matrix[9] = (float)Math.Sin(rad);
            //            temp.matrix[6] = (float)-Math.Sin(rad); temp.matrix[10] = (float)Math.Cos(rad);

            temp = matrix * temp;
            matrix.Row0 = temp.Row0;
            matrix.Row1 = temp.Row1;
            matrix.Row2 = temp.Row2;
            matrix.Row3 = temp.Row3;
            //            this.Set((this) * temp);

            // restore x,y,z so rotation won't affect them - doesn't appear to work
//            matrix.M41 = tempX;
//            matrix.M42 = tempY;
//            matrix.M43 = tempZ;
        }

        public static void RotateZ(ref Matrix4 matrix, float angle)
        {
            float tempX = matrix.M41;
            float tempY = matrix.M42;
            float tempZ = matrix.M43;

//            matrix.M41 = 0;
//            matrix.M42 = 0;
//            matrix.M43 = 0;

            Matrix4 temp = Matrix4.CreateRotationZ(angle);
            temp.Transpose();
            //            temp.LoadIdentity();
            //            temp.matrix[5] = (float)Math.Cos(rad); temp.matrix[9] = (float)Math.Sin(rad);
            //            temp.matrix[6] = (float)-Math.Sin(rad); temp.matrix[10] = (float)Math.Cos(rad);

            temp = matrix * temp;
            matrix.Row0 = temp.Row0;
            matrix.Row1 = temp.Row1;
            matrix.Row2 = temp.Row2;
            matrix.Row3 = temp.Row3;
            //            this.Set((this) * temp);

            // restore x,y,z so rotation won't affect them - doesn't appear to work
//            matrix.M41 = tempX;
//            matrix.M42 = tempY;
//            matrix.M43 = tempZ;
        }
    }

    public class Bvh
    {
        public bool verbose;

        public float Scale = 1.0f;

        public enum modeT { NONE, OFFSET, CHANNELS, JOINT, ROOT, End, Site, MOTION, Frames, Frame, Time, MOTIONDATA };


        public BvhPart root;
        private bool subroot;
        public float frameTime;
        public BvhPart current;
        public List<BvhPart> BvhPartsLinear=new List<BvhPart>();

        public modeT theMode;
        public int vertIndex;

        public int channelIndex;
        public int partIndex;



        public int data;
        /// the channels vector will store it's own size, so this is just for error checking.
        public int channelsNum;

        private Motion tempMotion;

//        public Matrix4 tempMotion  = Matrix4.Identity;
//        public Matrix4 tempMotionY = Matrix4.Identity;
//        public Matrix4 tempMotionX = Matrix4.Identity;
//        public Matrix4 tempMotionZ = Matrix4.Identity;

        public int framesNum;

        public Bvh(string bvhFile, bool SubtractRootOffset = false, float Scale = 1.0f)
        {
            this.Scale = Scale;
            this.subroot = SubtractRootOffset;
//            verbose = true;

            try { init(bvhFile); }
            catch (Exception e)
            {
//                Config.fileNotFound(e);
            }

            return;
        }

        public void process(string line)
        {
            //bool switched = false;	
            if (line == "OFFSET")
            {
                //	if (theMode != Site) {	
                vertIndex = 0;
                theMode = modeT.OFFSET;
                //	}
            }
            else if (line == "ROOT")
            {
                root = new BvhPart();
                root.parent = null;
                current = root;

                theMode = modeT.ROOT;
            }
            else if (line == "{")
            {

            }
            else if (line == "}")
            {
                //	if (theMode == Site) { theMode = NONE; }
                //	else 
                if (current != root)
                {
                    current = current.parent;
                    theMode = modeT.NONE;
                }
            }
            else if (line == "JOINT")
            {
                BvhPart temp = new BvhPart();
                temp.parent = current;
                current.child.Add(temp);
                current = temp;
                theMode = modeT.JOINT;
            }
            else if (line == "CHANNELS")
            {
                theMode = modeT.CHANNELS;
            }
            else if (line == "End")
            {
                theMode = modeT.End;
            }
            else if (line == "Site")
            {
                BvhPart temp = new BvhPart();
                temp.parent = current;
                current.child.Add(temp);
                current = temp;
                theMode = modeT.NONE; //Site;
            }
            else if (line == "MOTION")
            {
                theMode = modeT.MOTION;
            }
            else if (line == "Frames:")
            {
                theMode = modeT.Frames;
            }
            else if (line == "Frame")
            {
                theMode = modeT.Frame;
            }
            else if (line == "Time:")
            {
                theMode = modeT.Time;
            }
            else
            {

                switch (theMode)
                {
                    case (modeT.ROOT):
                        root.name = line;

                        theMode = modeT.NONE;
                        break;

                    case (modeT.JOINT):
                        current.name = line;

                        //	cout << line << "\n";
                        theMode = modeT.NONE;
                        break;

                    case (modeT.OFFSET):
                        float val = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture);
                        switch (vertIndex)
                        {
                            case 0:
                                current.offset.X = val;
                                break;
                            case 1:
                                current.offset.Y = val;
                                break;
                            case 2:
                                current.offset.Z = val;
                                break;

                        }
                        vertIndex++;
                        if (vertIndex > 2)
                        {
                            vertIndex = 0;
                            theMode = modeT.NONE;
                        }
                        break;

                    case (modeT.CHANNELS):
                        // assume channelsNum == 0 in the .bvh .is impossible
                        if (channelsNum == 0)
                        {
                            channelsNum = int.Parse(line);
                        }
                        else if (line == "Xposition")
                        {
                            current.channels.Add(BvhPart.channelTypes.Xpos);
                        }
                        else if (line == "Yposition")
                        {
                            current.channels.Add(BvhPart.channelTypes.Ypos);
                        }
                        else if (line == "Zposition")
                        {
                            current.channels.Add(BvhPart.channelTypes.Zpos);
                        }
                        else if (line == "Zrotation")
                        {
                            current.channels.Add(BvhPart.channelTypes.Zrot);
                        }
                        else if (line == "Yrotation")
                        {
                            current.channels.Add(BvhPart.channelTypes.Yrot);
                        }
                        else if (line == "Xrotation")
                        {
                            current.channels.Add(BvhPart.channelTypes.Xrot);
                        }

                        // if there are additional channel types in error, they'll be ignored(?)
                        if (current.channels.Count == channelsNum)
                        {
                            theMode = modeT.NONE;
                            channelsNum = 0;
                        }
                        break;

                    case (modeT.Frames):
                        framesNum = int.Parse(line);
                        theMode = modeT.NONE;

                        break;

                    case (modeT.Frame):
                        break;

                    case (modeT.Time):
                        frameTime = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture);
                        theMode = modeT.MOTIONDATA;
                        current = root;
                        recurs(root);

                        break;

                    case (modeT.MOTIONDATA):

                        data++;
                        switch (BvhPartsLinear[partIndex].channels[channelIndex])
                        {
                            case (BvhPart.channelTypes.Xpos):
                                //eMatrix4.Translate(ref tempMotion, float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture)*Scale, 0, 0);
                                tempMotion.translation.X = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture) * Scale;
                                channelIndex++;
                                break;
                            case (BvhPart.channelTypes.Ypos):
                                //eMatrix4.Translate(ref tempMotion, 0, float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture)*Scale, 0);
                                tempMotion.translation.Y = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture) * Scale;
                                channelIndex++;
                                break;
                            case (BvhPart.channelTypes.Zpos):
                                //eMatrix4.Translate(ref tempMotion, 0, 0, float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture)*Scale);
                                tempMotion.translation.Z = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture) * Scale;
                                channelIndex++;
                                break;
                            case (BvhPart.channelTypes.Zrot):
                                // it seems like some limbs need a negative, and out
                                // limbs don't
                                //eMatrix4.RotateZ(ref tempMotionZ,  (float)(float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture)*Math.PI/180.0));
                                //tempMotionZ.RotateZ((float)-Mat.DEG_TO_RAD(float.Parse(line, Config.estilo, Config.cult)));
                                tempMotion.rotation.Z = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture);
                                channelIndex++;
                                break;
                            case (BvhPart.channelTypes.Yrot):
                                //if (partIndex == 3) cout << atof(line.c_str()) << "\n";
                                //eMatrix4.RotateY(ref tempMotionY, -(float)(float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture) * Math.PI / 180.0));
                                //tempMotionY.RotateY((float)-Mat.DEG_TO_RAD(float.Parse(line, Config.estilo, Config.cult)));
                                //tempMotion.print();
                                tempMotion.rotation.Y = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture);
                                channelIndex++;
                                break;
                            case (BvhPart.channelTypes.Xrot):
                                //if (partIndex == 3) cout << atof(line.c_str()) << "\n";
                                //eMatrix4.RotateX(ref tempMotionX, -(float)(float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture) * Math.PI / 180.0));
                                //tempMotionX.RotateX((float)-Mat.DEG_TO_RAD(float.Parse(line, Config.estilo, Config.cult)));
                                tempMotion.rotation.X = float.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture);
                                channelIndex++;
                                break;
                        }

                        if (channelIndex >= BvhPartsLinear[partIndex].channels.Count)
                        {
                            // store tempMotion and move to next part
                            // Y * Z * X

                            if (partIndex == 0)
                                tempMotion.translation -= BvhPartsLinear[partIndex].offset;

                            BvhPartsLinear[partIndex].motion.Add(tempMotion);

                            tempMotion = new Motion();

                            channelIndex = 0;
                            partIndex++;
                        }
                        if (partIndex >= BvhPartsLinear.Count)
                        {
                            // this should be the end of one line of motion data
                            partIndex = 0;
                        }
                        break;

                    case (modeT.NONE):
                    case (modeT.End):
                    case (modeT.Site):
                    case (modeT.MOTION):
                        break;

                }
            }
            /*
            if (mode == 1) {
                v_ind++;
                if (v_ind == 2) v_ind = 0;
            }*/
        }

        public void recurs(BvhPart some)
        {
            //            Matrix4 motion0 = Matrix4.CreateTranslation(some.offset);
            //            Matrix4 motion0 = Matrix4.Identity;
            Motion motion0 = new Motion();
            //	motion0.matrix[10] = -1.0f; 
            some.motion.Add(motion0);
            if (some.child.Count != 0) BvhPartsLinear.Add(some);
            //	cout << some.name << some.offset.vertex[0] << " " <<
            //				some.offset.vertex[1] << " " <<
            //				some.offset.vertex[2] << "\n";
            int i;
            for (i = 0; i < some.child.Count; i++)
                recurs(some.child[i]);

        }

        public void init(string bvhFile)
        {
            data = 0;
            partIndex = 0;
            channelIndex = 0;

            tempMotion = new Motion();

            StreamReader bvhStream = new StreamReader(bvhFile);
            if (bvhStream == null)
            {
                //cout << "File \"" << bvhFile << "\" not found.\n";
                throw new Exception();
            }
            //if (verbose) cout << "File \"" << bvhFile << "\" found.\n";



            //	for_each(lines.begin(),lines.end(),&process);


            while (bvhStream.Peek() >= 0)
            {
                string linea = bvhStream.ReadLine();
                linea=linea.Replace("\t", " ");
                linea=linea.Trim();
                string[] palabras=linea.Split(' ');
                for (int i = 0; i < palabras.Count<string>();i++ )
                {
                    if (palabras[0] == "")
                        break;
                    if (palabras[i].Substring(0, 1) == "#")
                        break;
                    try { process(palabras[i]); }
                    catch (Exception e) {  return; }
                }
                
            }

            bvhStream.Close();

            framesNum = BvhPartsLinear[0].motion.Count;

            //if (verbose) cout << "\n\n";
            //if (verbose) cout << "framesNum = " << framesNum << "\nframetime = " << frameTime << "\n"; 
            //if (verbose) cout << data/(framesNum*3)-1 << " bodyparts\n";

            //for (i =0; i< root.motion.size(); i++)
            //	root.child[0].child[0].motion[i].print();
            /*
                for (i = 0; i < BvhPartsLinear[1].motion.size(); i++) {
                    BvhPartsLinear[1].motion[i].print();		
                }
            */
            //return;	
        }

    }
}
