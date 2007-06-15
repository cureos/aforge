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
	/// Threshold binarization
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
    public class Threshold : FilterGrayToGray
	{
        /// <summary>
        /// Threshold value.
        /// </summary>
		protected byte threshold = 128;

		/// <summary>
		/// Threshold value (default is 128)
		/// </summary>
		public byte ThresholdValue
		{
			get { return threshold; }
			set { threshold = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Threshold"/> class
		/// </summary>
		/// 
		public Threshold( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Threshold"/> class
		/// </summary>
		/// 
		/// <param name="threshold">Threshold value</param>
		/// 
		public Threshold( byte threshold )
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

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // for each line	
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, ptr++ )
                {
                    *ptr = (byte) ( ( *ptr >= threshold ) ? 255 : 0 );
                }
                ptr += offset;
            }
        }
    }
}
