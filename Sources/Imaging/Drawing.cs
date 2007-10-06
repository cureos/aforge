// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Drawing primitives.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to drawing of some primitives directly on
    /// locked image data.</para>
    /// <note>All methods of this class support drawing only on color 24 bpp images and
    /// on grayscale 8 bpp indexed images.</note></remarks>
    /// 
    public class Drawing
    {
        // Private constructor to avoid instantiation.
        private Drawing( ) { }

        /// <summary>
        /// Draw rectangle on image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rectangle">Rectangle's coordinates to draw.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        public static unsafe void Rectangle( BitmapData imageData, Rectangle rectangle, Color color )
        {
            // check pixel format
            if (
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new ArgumentException( "The function can be applied to graysclae (8bpp indexed) or color (24bpp) image only" );

            // image dimension
            int imageWidth  = imageData.Width;
            int imageHeight = imageData.Height;
            int stride      = imageData.Stride;

            // rectangle dimension and position
            int rectX1 = rectangle.X;
            int rectY1 = rectangle.Y;
            int rectX2 = rectangle.X + rectangle.Width - 1;
            int rectY2 = rectangle.Y + rectangle.Height - 1;

            // check if rectangle is in the image
            if ( ( rectX1 >= imageWidth ) || ( rectY1 >= imageHeight ) || ( rectX2 < 0 ) || ( rectY2 < 0 ) )
            {
                // nothing to draw
                return;
            }

            int startX  = Math.Max( 0, rectX1 );
            int stopX   = Math.Min( imageWidth - 1, rectX2 );
            int startY  = Math.Max( 0, rectY1 );
            int stopY   = Math.Min( imageHeight - 1, rectY2 );
           
            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                byte gray = (byte) ( 0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B );

                // draw top horizontal line
                if ( rectY1 >= 0 )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + rectY1 * stride + startX;

                    for ( int x = startX; x <= stopX; x++, ptr++ )
                    {
                        *ptr = gray;
                    }
                }

                // draw bottom horizontal line
                if ( rectY2 < imageHeight )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + rectY2 * stride + startX ;

                    for ( int x = startX; x <= stopX; x++, ptr++ )
                    {
                        *ptr = gray;
                    }
                }

                // draw left vertical line
                if ( rectX1 >= 0 )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + startY * stride + rectX1;

                    for ( int y = startY; y <= stopY; y++, ptr += stride )
                    {
                        *ptr = gray;
                    }
                }

                // draw right vertical line
                if ( rectX2 < imageWidth )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + startY * stride + rectX2;

                    for ( int y = startY; y <= stopY; y++, ptr += stride )
                    {
                        *ptr = gray;
                    }
                }
            }
            else
            {
                // color image
                byte red    = color.R;
                byte green  = color.G;
                byte blue   = color.B;

                // draw top horizontal line
                if ( rectY1 >= 0 )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + rectY1 * stride + startX * 3;

                    for ( int x = startX; x <= stopX; x++, ptr += 3 )
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw bottom horizontal line
                if ( rectY2 < imageHeight )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + rectY2 * stride + startX * 3;

                    for ( int x = startX; x <= stopX; x++, ptr += 3 )
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw left vertical line
                if ( rectX1 >= 0 )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + startY * stride + rectX1 * 3;

                    for ( int y = startY; y <= stopY; y++, ptr += stride )
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }

                // draw right vertical line
                if ( rectX2 < imageWidth )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + startY * stride + rectX2 * 3;

                    for ( int y = startY; y <= stopY; y++, ptr += stride )
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                }
            }
        }
    }
}
