using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace OpenTKTesting
{
    class TextureCubemap
    {
        public readonly int Handle;

        // Create texture from path.
        public TextureCubemap(List<string> imagePaths)
        {
            // Generate handle
            Handle = GL.GenTexture();

            // Bind the handle
            UseCubemap();

            TextureTarget[] targets =
            {
               TextureTarget.TextureCubeMapNegativeX, TextureTarget.TextureCubeMapNegativeY,
               TextureTarget.TextureCubeMapNegativeZ, TextureTarget.TextureCubeMapPositiveX,
               TextureTarget.TextureCubeMapPositiveY, TextureTarget.TextureCubeMapPositiveZ
            };

            for (int i = 0; i < imagePaths.Count; i++)
            {
                // Load the image
                using (var image = new Bitmap(imagePaths[i]))
                {
                    Debug.WriteLine(imagePaths[i]);

                    var data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);


                    GL.TexImage2D(targets[i],
                        0,
                        PixelInternalFormat.Rgb,
                        image.Width,
                        image.Height,
                        0,
                        PixelFormat.Rgb,
                        PixelType.UnsignedByte,
                        data.Scan0);

                }
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);


        }


        public void UseCubemap(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }

    }
}