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
    /// Opening operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks><para>Opening morphology operator equals to <see cref="Erosion">erosion</see> followed
    /// by <see cref="Dilatation">dilatation</see>.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Opening filter = new Opening( );
    /// // apply the filter
    /// filter.Apply( image );
    /// </code>
    /// </remarks>
    /// 
    public class Opening : IFilter, IInPlaceFilter
    {
        private Erosion     errosion = new Erosion( );
        private Dilatation  dilatation = new Dilatation( );

        /// <summary>
        /// Initializes a new instance of the <see cref="Opening"/> class.
        /// </summary>
        /// 
        public Opening( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Opening"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// 
        public Opening( short[,] se )
        {
            errosion = new Erosion( se );
            dilatation = new Dilatation( se );
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
            Bitmap tempImage = errosion.Apply( image );
            Bitmap destImage = dilatation.Apply( tempImage );

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
            Bitmap tempImage = errosion.Apply( imageData );
            Bitmap destImage = dilatation.Apply( tempImage );

            tempImage.Dispose( );

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
            errosion.ApplyInPlace( image );
            dilatation.ApplyInPlace( image );
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
            errosion.ApplyInPlace( imageData );
            dilatation.ApplyInPlace( imageData );
        }
    }
}
