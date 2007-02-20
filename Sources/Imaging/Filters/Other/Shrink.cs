// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Shrink an image
	/// </summary>
    /// 
    /// <remarks>Removes pixels with specified color from image bounds
    /// reducing image size.</remarks>
    /// 
    public class Shrink : FilterAnyToAnyNew
	{
		private Color colorToRemove = Color.FromArgb( 0, 0, 0 );
        // top-left coordinates of the object (calculated by CalculateNewImageSize())
        private int minX, minY;

        /// <summary>
        /// Color to remove
        /// </summary>
        /// 
		public Color ColorToRemove
		{
			get { return colorToRemove; }
			set { colorToRemove = value; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Shrink"/> class
        /// </summary>
        /// 
		public Shrink( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shrink"/> class
        /// </summary>
        /// 
        /// <param name="colorToRemove">Color to remove</param>
        /// 
        public Shrink( Color colorToRemove )
		{
			this.colorToRemove = colorToRemove;
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
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;
            int offset = sourceData.Stride -
                ( ( sourceData .PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

            // color to remove
            byte r = colorToRemove.R;
            byte g = colorToRemove.G;
            byte b = colorToRemove.B;

            minX = width;
            minY = height;
            int maxX = 0;
            int maxY = 0;

            // find rectangle which contains something except color to remove
            unsafe
            {
                byte* src = (byte*) sourceData.Scan0.ToPointer( );

                if ( sourceData.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    // grayscale
                    for ( int y = 0; y < height; y++ )
                    {
                        for ( int x = 0; x < width; x++, src++ )
                        {
                            if ( *src != g )
                            {
                                if ( x < minX )
                                    minX = x;
                                if ( x > maxX )
                                    maxX = x;
                                if ( y < minY )
                                    minY = y;
                                if ( y > maxY )
                                    maxY = y;
                            }
                        }
                        src += offset;
                    }
                }
                else
                {
                    // RGB
                    for ( int y = 0; y < height; y++ )
                    {
                        for ( int x = 0; x < width; x++, src += 3 )
                        {
                            if (
                                ( src[RGB.R] != r ) ||
                                ( src[RGB.G] != g ) ||
                                ( src[RGB.B] != b ) )
                            {
                                if ( x < minX )
                                    minX = x;
                                if ( x > maxX )
                                    maxX = x;
                                if ( y < minY )
                                    minY = y;
                                if ( y > maxY )
                                    maxY = y;
                            }
                        }
                        src += offset;
                    }
                }
            }

            // check
            if ( ( minX == width ) && ( minY == height ) && ( maxX == 0 ) && ( maxY == 0 ) )
            {
                minX = minY = 0;
            }

            return new Size( maxX - minX + 1, maxY - minY + 1 );
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
            // get destination image size
            int newWidth = destinationData.Width;
            int newHeight = destinationData.Height;

            int srcStride = sourceData.Stride;
            int dstStride = destinationData.Stride;
            int copySize = destinationData.Width;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            src += ( minY * srcStride );

            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                src += minX;
            }
            else
            {
                src += minX * 3;
                copySize *= 3;
            }

            // copy image
            for ( int y = 0; y < newHeight; y++ )
            {
                Win32.memcpy( dst, src, copySize );
                dst += dstStride;
                src += srcStride;
            }
        }
	}
}
