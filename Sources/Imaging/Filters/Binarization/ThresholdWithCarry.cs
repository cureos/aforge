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
	/// Threshold binarization with error carry
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
    public class ThresholdWithCarry : FilterGrayToGray
	{
		private byte threshold = 128;

		/// <summary>
		/// Threshold value (default is 128)
		/// </summary>
		public byte ThresholdValue
		{
			get { return threshold; }
			set { threshold = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class
		/// </summary>
		/// 
		public ThresholdWithCarry( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ThresholdWithCarry"/> class
		/// </summary>
		/// 
		/// <param name="threshold">Threshold value</param>
		/// 
		public ThresholdWithCarry( byte threshold )
		{
			this.threshold = threshold;
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
            int offset = imageData.Stride - width;

            // value which is caried from pixel to pixel
            short carry = 0;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // for each line	
            for ( int y = 0; y < height; y++ )
            {
                carry = 0;

                // for each pixel
                for ( int x = 0; x < width; x++, ptr++ )
                {
                    carry += *ptr;

                    if ( carry >= threshold )
                    {
                        *ptr = (byte) 255;
                        carry -= 255;
                    }
                    else
                    {
                        *ptr = (byte) 0;
                    }
                }
                ptr += offset;
            }
        }
	}
}
