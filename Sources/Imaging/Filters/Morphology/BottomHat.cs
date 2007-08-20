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
    /// Bottop-hat operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Bottom-hat morphological operator <see cref="Subtract">subtracts</see>
    /// input image from the result of <see cref="Closing">morphological closing</see> on the
    /// the input image.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BottomHat filter = new BottomHat( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    public class BottomHat : IFilter, IInPlaceFilter
    {
        private Closing closing = new Closing( );
        private Subtract subtract = new Subtract( );

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomHat"/> class.
        /// </summary>
        /// 
        public BottomHat( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomHat"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// 
        public BottomHat( short[,] se )
        {
            closing = new Closing( se );
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
            // morphological closing
            Bitmap tempImage = closing.Apply( image );

            // subtraction from original image
            subtract.OverlayImage = image;
            Bitmap destImage = subtract.Apply( tempImage );

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
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            // 2-source filters do not accept BitmapData as overlay, so
            // we need to make a copy of the image
            Bitmap source = AForge.Imaging.Image.Clone( imageData );

            // morphological closing
            Bitmap tempImage = closing.Apply( imageData );

            // subtraction from original image
            subtract.OverlayImage = source;
            Bitmap destImage = subtract.Apply( tempImage );

            tempImage.Dispose( );
            source.Dispose( );

            return destImage;
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
            // make a copy of the source image
            Bitmap source = AForge.Imaging.Image.Clone( image );

            // morphological closing
            closing.ApplyInPlace( image );

            // subtraction from original image
            subtract.OverlayImage = source;
            subtract.ApplyInPlace( image );

            source.Dispose( );
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
            // make a copy of the source image
            Bitmap source = AForge.Imaging.Image.Clone( imageData );

            // morphological closing
            closing.ApplyInPlace( imageData );

            // subtraction from original image
            subtract.OverlayImage = source;
            subtract.ApplyInPlace( imageData );

            source.Dispose( );
        }
    }
}
