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
    /// Binarization with thresholds matrix.
    /// </summary>
    /// 
    public class OrderedDithering : FilterGrayToGrayPartial
    {
        private int rows = 4;
        private int cols = 4;

        private byte[,] matrix = new byte[4, 4]
		{
			{  15, 143,  47, 175 },
			{ 207,  79, 239, 111 },
			{  63, 191,  31, 159 },
			{ 255, 127, 223,  95 }
		};

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDithering"/> class.
        /// </summary>
        /// 
        public OrderedDithering( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedDithering"/> class.
        /// </summary>
        /// 
        /// <param name="matrix">Thresholds matrix.</param>
        /// 
        public OrderedDithering( byte[,] matrix )
        {
            rows = matrix.GetLength( 0 );
            cols = matrix.GetLength( 1 );

            this.matrix = matrix;
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
            int offset  = imageData.Stride - rect.Width;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX );

            // for each line	
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr++ )
                {
                    *ptr = (byte) ( ( *ptr <= matrix[( y % rows ), ( x % cols )] ) ? 0 : 255 );
                }
                ptr += offset;
            }
        }
    }
}
