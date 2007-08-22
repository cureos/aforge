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
    using AForge;

    /// <summary>
    /// Brightness adjusting in HSL color space.
    /// </summary>
    /// 
    /// <remarks>The filter operates in <b>HSL</b> color space and adjusts
    /// pixels brightness value using luminance value of HSL color space.</remarks>
    /// 
    public class BrightnessCorrection : IFilter, IInPlaceFilter, IInPlacePartialFilter
    {
        private HSLLinear baseFilter = new HSLLinear( );
        private double adjustValue;	// [-1, 1]

        /// <summary>
        /// Brightness adjust value in the range of [-1, 1].
        /// </summary>
        /// 
        /// <remarks>Default value is 0.1.</remarks>
        /// 
        public double AdjustValue
        {
            get { return adjustValue; }
            set
            {
                adjustValue = Math.Max( -1.0, Math.Min( 1.0, value ) );

                // create luminance filter
                if ( adjustValue > 0 )
                {
                    baseFilter.InLuminance = new DoubleRange( 0.0, 1.0 - adjustValue );
                    baseFilter.OutLuminance = new DoubleRange( adjustValue, 1.0 );
                }
                else
                {
                    baseFilter.InLuminance = new DoubleRange( -adjustValue, 1.0 );
                    baseFilter.OutLuminance = new DoubleRange( 0.0, 1.0 + adjustValue );
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
        /// </summary>
        /// 
        public BrightnessCorrection( )
        {
            AdjustValue = 0.1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrightnessCorrection"/> class.
        /// </summary>
        /// 
        /// <param name="adjustValue">Brightness adjust value.</param>
        /// 
        public BrightnessCorrection( double adjustValue )
        {
            AdjustValue = adjustValue;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns the
        /// the result of image processing filter as new image.</remarks> 
        ///
        public Bitmap Apply( Bitmap image )
        {
            return baseFilter.Apply( image );
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            return baseFilter.Apply( imageData );
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image.</remarks>
        /// 
        public void ApplyInPlace( Bitmap image )
        {
            baseFilter.ApplyInPlace( image );
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image data.</remarks>
        /// 
        public void ApplyInPlace( BitmapData imageData )
        {
            baseFilter.ApplyInPlace( imageData );
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image.</remarks>
        /// 
        public void ApplyInPlace( Bitmap image, Rectangle rect )
        {
            baseFilter.ApplyInPlace( image, rect );
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image data.</remarks>
        /// 
        public void ApplyInPlace( BitmapData imageData, Rectangle rect )
        {
            baseFilter.ApplyInPlace( imageData, rect );
        }
    }
}
