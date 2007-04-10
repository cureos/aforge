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
    /// Top-hat operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Top-hat morphological operator <see cref="Subtract">subtracts</see>
    /// result of <see cref="Opening">morphological opening</see> on the the input image
    /// from the input image itself.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// TopHat filter = new TopHat( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    public class TopHat : IFilter
    {
        private Opening opening = new Opening( );
        private Subtract subtract = new Subtract( );

        /// <summary>
        /// Initializes a new instance of the <see cref="TopHat"/> class.
        /// </summary>
        /// 
        public TopHat( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopHat"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// 
        public TopHat( short[,] se )
        {
            opening = new Opening( se );
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
            // morphological opening
            Bitmap tempImage = opening.Apply( image );

            // subtraction from original image
            subtract.OverlayImage = tempImage;
            Bitmap destImage = subtract.Apply( image );

            tempImage.Dispose( );

            return destImage;
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
        /// <remarks>The filter accepts birmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            // morphological opening
            Bitmap tempImage = opening.Apply( imageData );

            // subtraction from original image
            subtract.OverlayImage = tempImage;
            Bitmap destImage = subtract.Apply( imageData );

            tempImage.Dispose( );

            return destImage;
        }
    }
}
