// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Markus Falkensteiner, 2007
// mfalkensteiner@gmx.at
//
// Andrew Kirillov
// andrew.kirillov@gmail.com
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
    ///	<item>calculate new thresghold: ( µB + µO ) / 2;</item>
    /// <item>if Abs(oldThreshold - newThreshold) is less than a given manimum allowed error, then stop iteration process
    /// and create the binary image with the new threshold.</item>item>
    /// </list>
    /// </para>
    /// <para>See also: Digital Image Processing, Gonzalez/Woods. Ch.10 page:599.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// IterativeThreshold filter = new IterativeThreshold( 2, 128 );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    public class IterativeThreshold : Threshold
    {
        private byte minError = 0;

        /// <summary>
        /// Minimum error, value when iterative threshold search is stopped.
        /// </summary>
        /// 
        /// <remarks>Default value is 0.</remarks>
        /// 
        public byte MinimumError
        {
            get { return minError; }
            set { minError = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class.
        /// </summary>
        /// 
        public IterativeThreshold( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IterativeThreshold"/> class.
        /// </summary>
        /// 
        /// <param name="minError">Minimum allowed error, that ends the iteration process.</param>
        /// 
        public IterativeThreshold( byte minError )
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
        public IterativeThreshold( byte minError, byte threshold )
        {
            this.minError = minError;
            this.threshold = threshold;
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

            // amount of object pixels and their summary value
            int objectValue = 0;
            int objectPixels = 0;

            // amount of background pixels and their summary value
            int backgroundValue = 0;
            int backgroundPixels = 0;

            // new threshold value
            byte newThreshold = 0;

            bool firstPass = true;

            do
            {
                objectValue = 0;
                objectPixels = 0;
                backgroundValue = 0;
                backgroundPixels = 0;

                // do the job
                byte* ptr = (byte*)imageData.Scan0.ToPointer( );

                // allign pointer to the first pixel to process
                ptr += ( startY * imageData.Stride + startX );

                if ( !firstPass )
                    threshold = newThreshold;

                firstPass = false;

                // for each line	
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, ptr++ )
                    {
                        if ( *ptr >= threshold )
                        {
                            objectValue += *ptr;
                            objectPixels++;
                        }
                        else
                        {
                            backgroundValue += *ptr;
                            backgroundPixels++;
                        }
                    }
                    ptr += offset;
                }

                // mean object and background values
				double meanObject = ( objectPixels == 0 ) ? 0 : (double) objectValue / objectPixels;
				double meanBackground = ( backgroundPixels == 0 ) ? 0 : (double) backgroundValue / backgroundPixels;
				// calculate new threshold value
                newThreshold = (byte) ( ( meanBackground + meanObject ) / 2 );
            }
            while ( Math.Abs( threshold - newThreshold ) > minError );

            threshold = newThreshold;

            // process image data using base filter
            base.ProcessFilter( imageData, rect );
        }
    }
}
