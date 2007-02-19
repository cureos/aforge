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
    /// Resize image using bilinear interpolation
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ResizeBilinear : FilterAnyToAnyNew
    {
        // dimension of the new image
        private int newWidth;
        private int newHeight;

        /// <summary>
        /// Width of new image
        /// </summary>
        /// 
        public int NewWidth
        {
            get { return newWidth; }
            set { newWidth = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Height of new image
        /// </summary>
        /// 
        public int NewHeight
        {
            get { return newHeight; }
            set { newHeight = Math.Max( 1, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeBilinear"/> class
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image</param>
        /// <param name="newHeight">Height of new image</param>
        /// 
		public ResizeBilinear( int newWidth, int newHeight )
		{
			this.newWidth   = newWidth;
			this.newHeight  = newHeight;
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
            return new Size( newWidth, newHeight );
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
            int width   = sourceData.Width;
            int height  = sourceData.Height;

            int pixelSize = ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
            int srcStride = sourceData.Stride;
            int dstOffset = destinationData.Stride - pixelSize * newWidth;
            double xFactor = (double) width / newWidth;
            double yFactor = (double) height / newHeight;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // coordinates of source points
            double  ox, oy, dx1, dy1, dx2, dy2;
            int     ox1, oy1, ox2, oy2;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointers
            byte* tp1, tp2;
            // point of 
            byte* p1, p2, p3, p4;

            // for each line
            for ( int y = 0; y < newHeight; y++ )
            {
                // Y coordinates
                oy  = (double) y * yFactor;
                oy1 = (int) oy;
                oy2 = ( oy1 == ymax ) ? oy1 : oy1 + 1;
                dy1 = oy - (double) oy1;
                dy2 = 1.0 - dy1;

                // get temp pointers
                tp1 = src + oy1 * srcStride;
                tp2 = src + oy2 * srcStride;

                // for each pixel
                for ( int x = 0; x < newWidth; x++ )
                {
                    // X coordinates
                    ox  = (double) x * xFactor;
                    ox1 = (int) ox;
                    ox2 = ( ox1 == xmax ) ? ox1 : ox1 + 1;
                    dx1 = ox - (double) ox1;
                    dx2 = 1.0 - dx1;

                    // get four points
                    p1 = tp1 + ox1 * pixelSize;
                    p2 = tp1 + ox2 * pixelSize;
                    p3 = tp2 + ox1 * pixelSize;
                    p4 = tp2 + ox2 * pixelSize;

                    // interpolate using 4 points
                    for ( int i = 0; i < pixelSize; i++, dst++, p1++, p2++, p3++, p4++ )
                    {
                        *dst = (byte) (
                            dy2 * ( dx2 * ( *p1 ) + dx1 * ( *p2 ) ) +
                            dy1 * ( dx2 * ( *p3 ) + dx1 * ( *p4 ) ) );
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
