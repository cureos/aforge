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
    /// Extract the biggest blob from image.
    /// </summary>
    /// 
    /// <remarks><para>The filter locates the biggest blob in the binary source image and extracts it.
    /// The filter also can use the binary source for the biggest blob's location only, but extract it from
    /// another image, which is set using <see cref="OriginalImage"/> property. The original image 
    /// usually is the source of the binary image and user may be interested in the biggest blob
    /// from the original image, but not from the binary.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing as source image passed to
    /// <see cref="Apply( Bitmap )"/> method.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp color images for processing as original
    /// image passed to <see cref="OriginalImage"/> property.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ExtractBiggestBlob filter = new ExtractBiggestBlob( );
    /// // apply the filter
    /// Bitmap biggestBlobsImage = filter.Apply( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/biggest_blob.jpg" width="141" height="226" />
    /// </remarks>
    /// 
    public class ExtractBiggestBlob : IFilter, IFilterInformation
    {
        private Bitmap originalImage = null;

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>The dictionary defines, which pixel formats are supported for
        /// source images and which pixel format will be used for resulting image.
        /// </para>
        /// 
        /// <para>See <see cref="IFilterInformation.FormatTransalations"/> for more information.</para>
        /// </remarks>
        ///
        public Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get
            {
                Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

                // initialize format translation dictionary
                formatTransalations[PixelFormat.Format8bppIndexed] = ( originalImage == null ) ?
                    PixelFormat.Format8bppIndexed : originalImage.PixelFormat;

                return formatTransalations;
            }
        }

        /// <summary>
        /// Original image, which is the source of binary image where the biggest blob is searched for.
        /// </summary>
        public Bitmap OriginalImage
        {
            get { return originalImage; }
            set { originalImage = value; }
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to get biggest blob from.</param>
        /// 
        /// <returns>Returns image of the biggest blob.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        ///
        public Bitmap Apply( Bitmap image )
        {
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            Bitmap dstImage = null;

            try
            {
                // apply the filter
                dstImage = Apply( srcData );
            }
            finally
            {
                // unlock source image
                image.UnlockBits( srcData );
            }

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to get biggest blob from.</param>
        /// 
        /// <returns>Returns image of the biggest blob.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the original image.</exception>
        /// <exception cref="InvalidImagePropertiesException">Source and original images must have the same size.</exception>
        ///
        public Bitmap Apply( BitmapData imageData )
        {
            // check pixel format of the source image
            if ( !FormatTransalations.ContainsKey( imageData.PixelFormat ) )
                throw new UnsupportedImageFormatException( "Source pixel format is not supported by the filter." );

            // locate blobs in the source image
            BlobCounter blobCounter = new BlobCounter( imageData );
            // get information about blobs
            Blob[] blobs = blobCounter.GetObjectsInformation( );
            // find the biggest blob
            int  maxSize = 0;
            Blob biggestBlob = null;

            for ( int i = 0, n = blobs.Length; i < n; i++ )
            {
                int size = blobs[i].Rectangle.Width * blobs[i].Rectangle.Height;

                if ( size > maxSize )
                {
                    maxSize = size;
                    biggestBlob = blobs[i];
                }
            }

            // extract biggest blob's image
            if ( originalImage == null )
            {
                blobCounter.ExtractBlobsImage( imageData, biggestBlob, false );
            }
            else
            {
                // check original image's format
                if (
                    ( originalImage.PixelFormat != PixelFormat.Format24bppRgb ) &&
                    ( originalImage.PixelFormat != PixelFormat.Format8bppIndexed )
                    )
                {
                    throw new UnsupportedImageFormatException( "Original image may be grayscale (8bpp indexed) or color (24bpp) image only." );
                }

                // check its size
                if ( ( originalImage.Width != imageData.Width ) || ( originalImage.Height != imageData.Height ) )
                {
                    throw new InvalidImagePropertiesException( "Original image must have the same size as passed source image." );
                }

                blobCounter.ExtractBlobsImage( originalImage, biggestBlob, false );
            }

            return biggestBlob.Image;
        }

        /// <summary>
        /// Apply filter to an image (not implemented).
        /// </summary>
        /// 
        /// <param name="image">Image in unmanaged memory.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        /// 
        public UnmanagedImage Apply( UnmanagedImage image )
        {
            throw new NotImplementedException( "The method is not implemented for the filter." );
        }

        /// <summary>
        /// Apply filter to an image (not implemented).
        /// </summary>
        /// 
        /// <param name="sourceImage">Source image to be processed.</param>
        /// <param name="destinationImage">Destination image to store filter's result.</param>
        /// 
        /// <exception cref="NotImplementedException">The method is not implemented.</exception>
        /// 
        public void Apply( UnmanagedImage sourceImage, UnmanagedImage destinationImage )
        {
            throw new NotImplementedException( "The method is not implemented filter." );
        }
    }
}
