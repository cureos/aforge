// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

    /// <summary>
    /// Texture tools
    /// </summary>
    /// 
    /// <remarks>The class represents collection of different texture tools.</remarks>
    /// 
    public class Texture
    {
		// Avoid class instantiation
        private Texture( ) { }

        /// <summary>
        /// Convert texture to grayscale bitmap
        /// </summary>
        /// 
        /// <param name="texture">Texture to convert to bitmap</param>
        /// 
        /// <returns>Returns bitmap of the texture</returns>
        /// 
        public static Bitmap ToBitmap( float[,] texture )
        {
            // get texture dimension
            int width = texture.GetLength( 1 );
            int height = texture.GetLength( 0 );

            // create new grawscale image
            Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            // do the job
            unsafe
            {
                byte* dst = (byte*) dstData.Scan0.ToPointer( );
                int offset = dstData.Stride - width;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) ( texture[y, x] * 255.0f );
                    }
                    dst += offset;
                }
            }

            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Convert grayscale bitmap to texture
        /// </summary>
        /// 
        /// <param name="image">Image to convert to texture</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <remarks>Only 8 bit per pixel indexed images are supported.</remarks>
        /// 
        public static float[,] FromBitmap( Bitmap image )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // process the image
            float[,] texture = FromBitmap( imageData );

            // unlock source image
            image.UnlockBits( imageData );

            return texture;
        }

        /// <summary>
        /// Convert grayscale bitmap to texture
        /// </summary>
        /// 
        /// <param name="imageData">Image data to convert to texture</param>
        /// 
        /// <returns>Returns texture as 2D float array.</returns>
        /// 
        /// <remarks>Only 8 bit per pixel indexed images are supported.</remarks>
        /// 
        public static float[,] FromBitmap( BitmapData imageData )
        {
            // check source image
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "8 bit per pixel indexed image expected" );

            // get source image dimension
            int width = imageData.Width;
            int height = imageData.Height;

            // create texture array
            float[,] texture = new float[height, width];

            // do the job
            unsafe
            {
                byte* src = (byte*) imageData.Scan0.ToPointer( );
                int offset = imageData.Stride - width;

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, src++ )
                    {
                        texture[y, x] = (float) *src / 255.0f;
                    }
                    src += offset;
                }
            }

            return texture;
        }
    }
}