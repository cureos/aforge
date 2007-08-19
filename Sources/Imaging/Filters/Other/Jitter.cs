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
    public class Jitter : FilterAnyToAnyUsingCopy
    {
        private int radius = 2;
        private bool copyBefore = true;

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
        public bool CopyBefore
        {
            get { return copyBefore; }
            set { copyBefore = value; }
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
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData )
        {
            // get source image size
            int width = destinationData.Width;
            int height = destinationData.Height;

            int pixelSize = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int stride = destinationData.Stride;
            int offset = stride - width * pixelSize;

            // new pixel's position
            int ox, oy;

            // maximum value for random number generator
            int max = radius * 2 + 1;

            byte* src = (byte*) sourceData.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            byte* p;

            if ( copyBefore )
                Win32.memcpy( dst, src, stride * height );

            // Note:
            // It is possible to speed-up this filter creating separate
            // loops for RGB and grayscale images.

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++ )
                {
                    // generate radnom pixel's position
                    ox = x + rand.Next( max ) - radius;
                    oy = y + rand.Next( max ) - radius;

                    // check if the random pixel is inside our image
                    if ( ( ox >= 0 ) && ( oy >= 0 ) && ( ox < width ) && ( oy < height ) )
                    {
                        p = src + oy * stride + ox * pixelSize;

                        for ( int i = 0; i < pixelSize; i++, dst++ )
                        {
                            *dst = p[i];
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
