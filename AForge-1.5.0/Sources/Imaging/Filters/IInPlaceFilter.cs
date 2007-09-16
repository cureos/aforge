// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// In-place filter interface.
    /// </summary>
    /// 
    /// <remarks>The interface defines the set of methods, which should be
    /// implemented by filters, which are capable to do image processing
    /// directly on the source image. Not all of image processing filters
    /// can be applied directly to the source image - only filter, which do not
    /// change image's dimension and pixel format, can be applied directly to the
    /// source image.</remarks>
    /// 
    public interface IInPlaceFilter
    {
        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image.</remarks>
        /// 
        void ApplyInPlace( Bitmap image );

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image data.</remarks>
        /// 
        void ApplyInPlace( BitmapData imageData );
    }
}

