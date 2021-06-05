using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Tree
{
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
        public static Texture GetTexture(int index)
        {
            return textures[index];
        }
    }
}
