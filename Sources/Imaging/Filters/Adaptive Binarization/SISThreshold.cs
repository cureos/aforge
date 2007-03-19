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
	/// Threshold using Simple Image Statistics (SIS)
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class SISThreshold : FilterGrayToGray
	{
		private byte threshold;

		/// <summary>
		/// Threshold value
		/// </summary>
		/// 
		/// <remarks>The property is read only and represents the value, which
		/// was automaticaly calculated using image statistics.</remarks>
		/// 
		public byte Threshold
		{
			get { return threshold; }
		}

        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="imageData">Image data</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            // get image width and height
            int width = imageData.Width;
            int height = imageData.Height;
            int widthM1 = width - 1;
            int heightM1 = height - 1;
            int stride = imageData.Stride;
            int offset = stride - width;
            // differences and weights
            double ex, ey, weight, weightTotal = 0, total = 0;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // --- 1st pass - collecting statistics

            // skip the first line for the first pass
            ptr += stride;

            // for each line
            for ( int y = 1; y < heightM1; y++ )
            {
                ptr++;
                // for each pixels
                for ( int x = 1; x < widthM1; x++, ptr++ )
                {
                    // the equations are:
                    // ex = I(x + 1, y) - I(x - 1, y)
                    // ey = I(x, y + 1) - I(x, y - 1)
                    // weight = max(ex, ey)
                    // weightTotal += weight
                    // total += weight * I(x, y)

                    ex = ptr[1] - ptr[-1];
                    ey = ptr[stride] - ptr[-stride];
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

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for all pixels
                for ( int x = 0; x < width; x++, ptr++ )
                {
                    *ptr = (byte) ( ( *ptr >= threshold ) ? 255 : 0 );
                }
                ptr += offset;
            }
        }
	}
}
