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
    /// <para><note>All methods of this class support drawing only on color 24 bpp images and
    /// on grayscale 8 bpp indexed images.</note></para></remarks>
    /// 
    public class Drawing
    {
        // Private constructor to avoid instantiation.
        private Drawing( ) { }

        /// <summary>
        /// Fill rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        /// <param name="rectangle">Rectangle's coordinates to fill.</param>
        /// <param name="color">Rectangle's color.</param>
        /// 
        public static unsafe void FillRectangle( BitmapData imageData, Rectangle rectangle, Color color )
        {
            // check pixel format
            if (
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                )
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

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

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + startY * stride + startX *
                ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3 );

            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                byte gray = (byte) ( 0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B );

                int fillWidth = stopX - startX + 1;

                for ( int y = startY; y <= stopY; y++ )
                {
                    AForge.Win32.memset( ptr, gray, fillWidth );
                    ptr += stride;
                }
            }
            else
            {
                // color image
                byte red    = color.R;
                byte green  = color.G;
                byte blue   = color.B;

                int offset = stride - ( stopX - startX + 1) * 3;

                for ( int y = startY; y <= stopY; y++ )
                {
                    for ( int x = startX; x <= stopX; x++, ptr += 3 )
                    {
                        ptr[RGB.R] = red;
                        ptr[RGB.G] = green;
                        ptr[RGB.B] = blue;
                    }
                    ptr += offset;
                }
            }
        }

        /// <summary>
        /// Draw rectangle on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
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
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

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

                    AForge.Win32.memset( ptr, gray, stopX - startX );
                }

                // draw bottom horizontal line
                if ( rectY2 < imageHeight )
                {
                    byte* ptr = (byte*) imageData.Scan0.ToPointer( ) + rectY2 * stride + startX ;

                    AForge.Win32.memset( ptr, gray, stopX - startX );
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

        /// <summary>
        /// Draw a line on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        /// <param name="pt1">The first point to connect.</param>
        /// <param name="pt2">The second point to connect.</param>
        /// <param name="color">Line's color.</param>
        /// 
        public static unsafe void Line(BitmapData imageData, Point pt1, Point pt2, Color color)
        {
            // check pixel format
            if (
                (imageData.PixelFormat != PixelFormat.Format24bppRgb) &&
                (imageData.PixelFormat != PixelFormat.Format8bppIndexed)
                )
            {
                throw new ArgumentException("Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only");
            }

            // image dimension
            int imageWidth = imageData.Width;
            int imageHeight = imageData.Height;
            int stride = imageData.Stride;

            
            
            // TODO: use a segment/rectangle intersection algorithm.
            // for now : not all cases are handled.
            if (    ((pt1.X < 0 ) && (pt2.X < 0)) || 
                    ((pt1.Y < 0 ) && (pt2.Y < 0)) ||
                    ((pt1.X >= imageWidth ) && (pt2.X >= imageWidth)) ||
                    ((pt1.Y >= imageHeight ) && (pt2.Y >= imageHeight))  )
            {
                // nothing to draw
                return;
            }


            // TODO: Clip the line so the coordinates are within image.
            // For now : if outside, project the point on the closest border
            int startX  = Math.Min(imageWidth - 1, Math.Max(0, pt1.X));
            int startY  = Math.Min(imageHeight - 1, Math.Max(0, pt1.Y));
            int stopX   = Math.Min(imageWidth - 1, Math.Max(0, pt2.X));
            int stopY   = Math.Min(imageHeight - 1, Math.Max(0, pt2.Y));
      


            // Compute pixel for grayscale image
            byte gray = 0;
            if (imageData.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                gray = (byte)(0.2125 * color.R + 0.7154 * color.G + 0.0721 * color.B);
            }
 

            // Draw the line
            int dx = stopX - startX;
            int dy = stopY - startY;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                // The line is more horizontal, we'll plot along the x axis.
                float slope = (float)dy / (float)dx;
                int coeff = (dx > 0) ? 1 : -1;
                
                for (int x = 0; x != dx; x += coeff)
                {
                    int px = startX + x;
                    int py = (int)((float)startY + (slope * (float)x));

                    if (imageData.PixelFormat == PixelFormat.Format8bppIndexed)
                    {
                        byte* ptr = (byte*)imageData.Scan0.ToPointer() + py * stride + px;
                        
                        *ptr = gray;
                    }
                    else
                    {
                        byte* ptr = (byte*)imageData.Scan0.ToPointer() + py * stride + px * 3;

                        ptr[RGB.R] = color.R;
                        ptr[RGB.G] = color.G;
                        ptr[RGB.B] = color.B;
                    }
                }
            }
            else
            {
                // The line is more vertical, we'll plot along the y axis.
                float slope = (float)dx / (float)dy;
                int coeff = (dy > 0) ? 1 : -1;

                for (int y = 0; y != dy; y += coeff)
                {
                    int px = (int)((float)startX + (slope * (float)y));
                    int py = startY + y;

                    if (imageData.PixelFormat == PixelFormat.Format8bppIndexed)
                    {
                        byte* ptr = (byte*)imageData.Scan0.ToPointer() + py * stride + px;
                        
                        *ptr = gray;
                    }
                    else
                    {
                        byte* ptr = (byte*)imageData.Scan0.ToPointer() + py * stride + px * 3;

                        ptr[RGB.R] = color.R;
                        ptr[RGB.G] = color.G;
                        ptr[RGB.B] = color.B;
                    }
                }
            }
        }

        // TODO:
        // DrawLines, DrawPolygon, DrawRectangles
        // DrawArc, DrawCurve, DrawEllipse
        // DrawString,
        // FillEllipse, FillPolygon...
    
    }
}
