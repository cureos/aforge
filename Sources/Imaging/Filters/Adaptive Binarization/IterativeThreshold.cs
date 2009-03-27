// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Markus Falkensteiner, 2007
// mfalkensteiner@gmx.at
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

	/// <summary>
	/// Iterative threshold search and binarization.
	/// </summary>
	/// 
	/// <remarks>
    /// <para>The algorithm works in the following way:
    /// <list type="bullet">
    /// <item>select any start threshold;</item>
    /// <item>compute average value of Background (µB) and Object (µO) values:
    /// 1) all pixels with a value that is below threshold, belong to the Background values;
    ///	2) all pixels greater or equal threshold, belong to the Object values.
    /// </item>
    ///	<item>calculate new thresghold: (µB + µO) / 2;</item>
    /// <item>if |oldThreshold - newThreshold| is less than a given manimum allowed error, then stop iteration process
    /// and create the binary image with the new threshold.</item>
    /// </list>
    /// </para>
    /// 
    /// <para>For additional information see <b>Digital Image Processing, Gonzalez/Woods. Ch.10 page:599</b>.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images for processing.</para>
    /// 
    /// <para><note>Since the filter can be applied as to 8 bpp and to 16 bpp images,
    /// the initial value of <see cref="Threshold.ThresholdValue"/> property should be set appropriately to the
    /// pixel format. In the case of 8 bpp images the threshold value is in the [0, 255] range, but
    /// in the case of 16 bpp images the threshold value is in the [0, 65535] range.</note></para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// IterativeThreshold filter = new IterativeThreshold( 2, 128 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample11.png" width="256" height="256" />
    /// <para><b>Result image (calculated threshold is 102):</b></para>
    /// <img src="img/imaging/iterative_threshold.png" width="256" height="256" />
    /// </remarks>
    /// 
    /// <seealso cref="OtsuThreshold"/>
    /// <seealso cref="SISThreshold"/>
    /// 
    public class IterativeThreshold : Threshold
    {
        private int minError = 0;

        /// <summary>
        /// Minimum error, value when iterative threshold search is stopped.
        /// </summary>
        /// 
        /// <remarks>Default value is set to <b>0</b>.</remarks>
        /// 
        public int MinimumError
        {
            get { return minError; }
            set { minError = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class.
        /// </summary>
        /// 
        public IterativeThreshold( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class.
        /// </summary>
        /// 
        /// <param name="minError">Minimum allowed error, that ends the iteration process.</param>
        /// 
        public IterativeThreshold( int minError )
        {
            this.minError = minError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class.
        /// </summary>
        /// 
        /// <param name="minError">Minimum allowed error, that ends the iteration process.</param>
        /// <param name="threshold">Initial threshold value.</param>
        /// 
        public IterativeThreshold( int minError, int threshold )
        {
            this.minError = minError;
            this.threshold = threshold;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            // get start and stop X-Y coordinates
            int startX = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            // histogram array
            int[] integerHistogram = null;
            int maxThreshold = 256;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                integerHistogram = new int[256];

                // collect histogram first
                byte* ptr = (byte*) image.ImageData.ToPointer( );
                int offset = image.Stride - rect.Width;

                // allign pointer to the first pixel to process
                ptr += ( startY * image.Stride + startX );

                // for each line	
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        integerHistogram[*ptr]++;
                    }
                    ptr += offset;
                }
            }
            else
            {
                integerHistogram = new int[65536];
                maxThreshold = 65536;

                // collect histogram first
                byte* basePtr = (byte*) image.ImageData.ToPointer( ) + startX * 2;
                int stride = image.Stride;

                // for each line	
                for ( int y = startY; y < stopY; y++ )
                {
                    ushort* ptr = (ushort*) ( basePtr + y * stride );

                    // for each pixel
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        integerHistogram[*ptr]++;
                    }
                }
            }

            // old threshold value
            int oldThreshold = 0;

            do
            {
                oldThreshold = threshold;

                // object's mean and amount of object's pixels
                double meanObject = 0;
                int objectPixels = 0;

                // background's mean and amount of background's pixels
                double meanBackground = 0;
                int backgroundPixels = 0;

                for ( int t = 0; t < threshold; t++ )
                {
                    meanBackground += (double) t * integerHistogram[t];
                    backgroundPixels += integerHistogram[t];
                }
                // calculate object pixels
                for ( int t = threshold; t < maxThreshold; t++ )
                {
                    meanObject += (double) t * integerHistogram[t];
                    objectPixels += integerHistogram[t];
                }
                meanBackground /= backgroundPixels;
                meanObject /= objectPixels;

                // calculate new threshold value
                if ( backgroundPixels == 0 )
                {
                    threshold = (int) meanObject;
                }
                else if ( objectPixels == 0 )
                {
                    threshold = (int) meanBackground;
                }
                else
                {
                    threshold = (int) ( ( meanBackground + meanObject ) / 2 );
                }
            }
            while ( Math.Abs( oldThreshold - threshold ) > minError );

            // process image data using base filter
            base.ProcessFilter( image, rect );
        }
    }
}
