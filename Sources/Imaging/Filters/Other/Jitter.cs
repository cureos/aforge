// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//
// Original idea from CxImage
// http://www.codeproject.com/bitmap/cximage.asp
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Jitter filter.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class Jitter : FilterAnyToAnyUsingCopyPartial
    {
        private int radius = 2;

        // random number generator
        private Random rand = new Random( );

        /// <summary>
        /// Jittering radius.
        /// </summary>
        /// 
        /// <remarks>Determines radius in which pixels can move. Default value is 2.
        /// Minimum value is 1. Maximum value is 10.</remarks>
        /// 
        public int Radius
        {
            get { return radius; }
            set { radius = Math.Max( 1, Math.Min( 10, value ) ); }
        }

        /// <summary>
        /// Determines if source image should be copied to destination image before
        /// starting the jittering.
        /// </summary>
        /// 
        [Obsolete( "Now the property is set to true by default" )]
        public bool CopyBefore
        {
            get { return true; }
            set { }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jitter"/> class.
        /// </summary>
        public Jitter( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jitter"/> class.
        /// </summary>
        /// 
        /// <param name="radius">Jittering radius.</param>
        /// 
        public Jitter( int radius )
        {
            Radius = radius;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Pointer to source image data (first scan line).</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData, Rectangle rect )
        {
            int pixelSize = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop X,Y positions
            int startX = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int stride = destinationData.Stride;
            int offset = stride - rect.Width * pixelSize;

            // new pixel's position
            int ox, oy;

            // maximum value for random number generator
            int max = radius * 2 + 1;

            byte* src = (byte*) sourceData.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            byte* p;

            // copy source to destination before
            Win32.memcpy( dst, src, stride * destinationData.Height );

            // allign pointer to the first pixel to process
            dst += ( startY * stride + startX * pixelSize );

            // Note:
            // It is possible to speed-up this filter creating separate
            // loops for RGB and grayscale images.

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++ )
                {
                    // generate radnom pixel's position
                    ox = x + rand.Next( max ) - radius;
                    oy = y + rand.Next( max ) - radius;

                    // check if the random pixel is inside our image
                    if ( ( ox >= startX ) && ( oy >= startY ) && ( ox < stopX ) && ( oy < stopY ) )
                    {
                        p = src + oy * stride + ox * pixelSize;

                        for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                        {
                            *dst = *p;
                        }
                    }
                    else
                    {
                        dst += pixelSize;
                    }
                }
                dst += offset;
            }
        }
    }
}
