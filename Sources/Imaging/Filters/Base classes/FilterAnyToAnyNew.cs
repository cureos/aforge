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
    /// Base class for filtering any images without changing pixel format, but
    /// creating new image of new size.
    /// </summary>
    /// 
    /// <remarks>The abstract class is the base class for all filters, which can
    /// be applied to color and grayscale images without changing their pixel format
    /// but changing image dimension. The base class is used for filters, which can not be applied
    /// directly to the specified source image updating them. Instead of this, these
    /// filters return new image as a result of processing routine.</remarks>
    /// 
    public abstract class FilterAnyToAnyNew : IFilter
    {
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
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly,
                ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb );

            // apply the filter
            Bitmap dstImage = Apply( srcData );

            // unlock source image
            image.UnlockBits( srcData );

            return dstImage;
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
            if (
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new ArgumentException( "The filter can be applied to graysclae (8bpp indexed) or color (24bpp) image only" );

            // get new image dimension
            Size newSize = CalculateNewImageSize( imageData );

            // create new image
            Bitmap dstImage = ( imageData.PixelFormat == PixelFormat.Format24bppRgb ) ?
                new Bitmap( newSize.Width, newSize.Height, imageData.PixelFormat ) :
                AForge.Imaging.Image.CreateGrayscaleImage( newSize.Width, newSize.Height );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, newSize.Width, newSize.Height ),
                ImageLockMode.ReadWrite, imageData.PixelFormat );

            // process the filter
            ProcessFilter( imageData, dstData );

            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// 
        /// <returns>New image size.</returns>
        /// 
        protected abstract System.Drawing.Size CalculateNewImageSize( BitmapData sourceData );

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected abstract unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData );
    }
}
