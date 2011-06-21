// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Controls
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    internal static class Tools
    {
        // Convert high color depth image into lower color depth image
        public static Bitmap ConvertImage( Bitmap source )
        {
            Bitmap newImage = null;
            int layers = 0;

            // get image size
            int width  = source.Width;
            int height = source.Height;

            // create new image depending on source image format
            switch ( source.PixelFormat )
            {
                case PixelFormat.Format16bppGrayScale:
                    {
                        // create new grayscale image
                        newImage = new Bitmap( width, height, PixelFormat.Format8bppIndexed );
                        // get palette
                        ColorPalette cp = newImage.Palette;
                        // init palette with grayscale colors
                        for ( int i = 0; i < 256; i++ )
                        {
                            cp.Entries[i] = Color.FromArgb( i, i, i );
                        }
                        // set palette back
                        newImage.Palette = cp;

                        layers = 1;
                        break;
                    }

                case PixelFormat.Format48bppRgb:
                    {
                        // create new color 24 bpp image
                        newImage = new Bitmap( width, height, PixelFormat.Format24bppRgb );
                        layers = 3;
                        break;
                    }

                case PixelFormat.Format64bppArgb:
                    {
                        // create new color 32 bpp image
                        newImage = new Bitmap( width, height, PixelFormat.Format32bppArgb );
                        layers = 4;
                        break;
                    }

                default:
                    return null;
            }

            // lock both images
            BitmapData sourceData = source.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, source.PixelFormat );
            BitmapData newData = newImage.LockBits( new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, newImage.PixelFormat );

            unsafe
            {
                // base pointers
                byte* sourceBasePtr = (byte*) sourceData.Scan0.ToPointer( );
                byte* newBasePtr    = (byte*) newData.Scan0.ToPointer( );
                // image strides
                int sourceStride = sourceData.Stride;
                int newStride    = newData.Stride;

                for ( int y = 0; y < height; y++ )
                {
                    ushort* sourcePtr = (ushort*) ( sourceBasePtr + y * sourceStride );
                    byte*   newPtr    = (byte*) ( newBasePtr + y * newStride );

                    for ( int x = 0, lineSize = width * layers; x < lineSize; x++, sourcePtr++, newPtr++ )
                    {
                        *newPtr = (byte) ( *sourcePtr >> 8 );
                    }
                }
            }

            // unlock both image
            source.UnlockBits( sourceData );
            newImage.UnlockBits( newData );

            return newImage;
        }
    }
}
