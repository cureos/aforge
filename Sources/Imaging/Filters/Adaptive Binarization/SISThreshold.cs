// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Threshold using Simple Image Statistics (SIS).
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class SISThreshold : FilterGrayToGrayPartial
    {
        private byte threshold;

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The property is read only and represents the value, which
        /// was automaticaly calculated using image statistics.</remarks>
        /// 
        public byte ThresholdValue
        {
            get { return threshold; }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int stopXM1 = stopX - 1;
            int stopYM1 = stopY - 1;
            int stride  = imageData.Stride;
            int offset  = stride - rect.Width;

            // differences and weights
            double ex, ey, weight, weightTotal = 0, total = 0;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // --- 1st pass - collecting statistics

            // skip the first line for the first pass
            ptr += stride;

            // for each line
            for ( int y = startY + 1; y < stopYM1; y++ )
            {
                ptr++;
                // for each pixels
                for ( int x = startX + 1; x < stopXM1; x++, ptr++ )
                {
                    // the equations are:
                    // ex = | I(x + 1, y) - I(x - 1, y) |
                    // ey = | I(x, y + 1) - I(x, y - 1) |
                    // weight = max(ex, ey)
                    // weightTotal += weight
                    // total += weight * I(x, y)

                    ex = Math.Abs( ptr[1] - ptr[-1] );
                    ey = Math.Abs( ptr[stride] - ptr[-stride] );
                    weight = ( ex > ey ) ? ex : ey;
                    weightTotal += weight;
                    total += weight * ( *ptr );
                }
                ptr += offset + 1;
            }

            // calculate threshold
            threshold = ( weightTotal == 0 ) ? (byte) 0 : (byte) ( total / weightTotal );

            // --- 2nd pass - thresholding
            ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for all pixels
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    *ptr = (byte) ( ( *ptr >= threshold ) ? 255 : 0 );
                }
                ptr += offset;
            }
        }
    }
}
