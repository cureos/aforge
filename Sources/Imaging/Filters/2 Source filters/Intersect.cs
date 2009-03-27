// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Intersect filter - get MIN of pixels in two images.
    /// </summary>
    /// 
    /// <remarks><para>The intersect filter takes two images (source and overlay images)
    /// of the same size and pixel format and produces an image, where each pixel equals
    /// to the minimum value of corresponding pixels from provided images.</para>
    /// 
    /// <para>The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp
    /// color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// Intersect filter = new Intersect( overlayImage );
    /// // apply the filter
    /// Bitmap resultImage = filter.Apply( sourceImage );
    /// </code>
    /// 
    /// <para><b>Source image:</b></para>
    /// <img src="img/imaging/sample6.png" width="320" height="240" />
    /// <para><b>Overlay image:</b></para>
    /// <img src="img/imaging/sample7.png" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/intersect.png" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="Merge"/>
    /// <seealso cref="Difference"/>
    /// <seealso cref="Add"/>
    /// <seealso cref="Subtract"/>
    /// 
    public sealed class Intersect : BaseInPlaceFilter2
    {
        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Merge"/> class.
        /// </summary>
        public Intersect( )
        {
            InitFormatTransalations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Merge"/> class.
        /// </summary>
        /// 
        /// <param name="overlayImage">Overlay image.</param>
        /// 
        public Intersect( Bitmap overlayImage )
            : base( overlayImage )
        {
            InitFormatTransalations( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Intersect"/> class.
        /// </summary>
        /// 
        /// <param name="unmanagedOverlayImage">Unmanaged overlay image.</param>
        /// 
        public Intersect( UnmanagedImage unmanagedOverlayImage )
            : base( unmanagedOverlayImage )
        {
            InitFormatTransalations( );
        }

        // Initialize format translation dictionary
        private void InitFormatTransalations( )
        {
            formatTransalations[PixelFormat.Format8bppIndexed]    = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format24bppRgb]       = PixelFormat.Format24bppRgb;
            formatTransalations[PixelFormat.Format32bppRgb]       = PixelFormat.Format32bppRgb;
            formatTransalations[PixelFormat.Format32bppArgb]      = PixelFormat.Format32bppArgb;
            formatTransalations[PixelFormat.Format16bppGrayScale] = PixelFormat.Format16bppGrayScale;
            formatTransalations[PixelFormat.Format48bppRgb]       = PixelFormat.Format48bppRgb;
            formatTransalations[PixelFormat.Format64bppArgb]      = PixelFormat.Format64bppArgb;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="overlay">Overlay image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, UnmanagedImage overlay )
        {
            PixelFormat pixelFormat = image.PixelFormat;
            // get image dimension
            int width  = image.Width;
            int height = image.Height;

            if (
                ( pixelFormat == PixelFormat.Format8bppIndexed ) ||
                ( pixelFormat == PixelFormat.Format24bppRgb ) ||
                ( pixelFormat == PixelFormat.Format32bppRgb ) ||
                ( pixelFormat == PixelFormat.Format32bppArgb ) )
            {
                // initialize other variables
                int pixelSize = ( pixelFormat == PixelFormat.Format8bppIndexed ) ? 1 :
                    ( pixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
                int lineSize  = width * pixelSize;
                int srcOffset = image.Stride - lineSize;
                int ovrOffset = overlay.Stride - lineSize;

                // do the job
                byte * ptr = (byte*) image.ImageData.ToPointer( );
                byte * ovr = (byte*) overlay.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
                    {
                        if ( *ovr < *ptr )
                            *ptr = *ovr;
                    }
                    ptr += srcOffset;
                    ovr += ovrOffset;
                }
            }
            else
            {
                // initialize other variables
                int pixelSize = ( pixelFormat == PixelFormat.Format16bppGrayScale ) ? 1 :
                    ( pixelFormat == PixelFormat.Format48bppRgb ) ? 3 : 4;
                int lineSize  = width * pixelSize;
                int srcStride = image.Stride;
                int ovrStride = overlay.Stride;

                // do the job
                int basePtr = (int) image.ImageData.ToPointer( );
                int baseOvr = (int) overlay.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    ushort * ptr = (ushort*) ( basePtr + y * srcStride );
                    ushort * ovr = (ushort*) ( baseOvr + y * ovrStride );

                    // for each pixel
                    for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
                    {
                        if ( *ovr < *ptr )
                            *ptr = *ovr;
                    }
                }
            }
        }
    }
}
