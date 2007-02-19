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
    /// Resize image using nearest neighbor algorithm
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ResizeNearestNeighbor : FilterAnyToAnyNew
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
        /// Initializes a new instance of the <see cref="ResizeNearestNeighbor"/> class
        /// </summary>
        /// 
        /// <param name="newWidth">Width of new image</param>
        /// <param name="newHeight">Height of new image</param>
        /// 
		public ResizeNearestNeighbor( int newWidth, int newHeight )
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
            byte* p;

            // cooridinaes of nearest point
            int ox, oy;

            // for each line
            for ( int y = 0; y < newHeight; y++ )
            {
                // Y coordinate of the nearest point
                oy = (int) ( y * yFactor );

                // for each pixel
                for ( int x = 0; x < newWidth; x++ )
                {
                    // X coordinate of the nearest point
                    ox = (int) ( x * xFactor );

                    p = src + oy * srcStride + ox * pixelSize;

                    for ( int i = 0; i < pixelSize; i++, dst++, p++ )
                    {
                        *dst = *p;
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
