// AForge Image Processing Library
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
	/// Crop an image
	/// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class Crop : FilterAnyToAnyNew
	{
		private Rectangle rect;

        /// <summary>
        /// Rectangle to crop
        /// </summary>
		public Rectangle Rectangle
		{
			get { return rect; }
			set { rect = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Crop"/> class
        /// </summary>
        /// 
        /// <param name="rect">Rectangle to crop</param>
        /// 
        public Crop( Rectangle rect )
		{
			this.rect = rect;
		}

        /// <summary>
        /// Calculates new image size
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// 
        /// <returns>New image size</returns>
        /// 
        protected override System.Drawing.Size CalculateNewImageSize( BitmapData sourceData )
        {
            return new Size( rect.Width, rect.Height );
        }

        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data</param>
        /// <param name="destinationData">Destination image data</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // destination image dimension
            int xmin = Math.Max( 0, Math.Min( width - 1, rect.Left ) );
            int ymin = Math.Max( 0, Math.Min( height - 1, rect.Top ) );
            int xmax = Math.Min( width - 1, xmin + rect.Width - 1 + ( ( rect.Left < 0 ) ? rect.Left : 0 ) );
            int ymax = Math.Min( height - 1, ymin + rect.Height - 1 + ( ( rect.Top < 0 ) ? rect.Top : 0 ) );
            int dstWidth = xmax - xmin + 1;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int pixelSize = ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int copySize = dstWidth * pixelSize;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // for each line
            for ( int y = ymin; y <= ymax; y++ )
            {
                Win32.memcpy( dst, src, copySize );
                src += srcStride;
                dst += dstStride;
            }
        }

		// Apply filter
/*		public Bitmap Apply(Bitmap srcImg)
		{



		}*/
	}
}
