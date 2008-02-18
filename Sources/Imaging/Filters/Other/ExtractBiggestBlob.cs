// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Extract the biggest blob from image.
    /// </summary>
    /// 
    /// <remarks><para>The filter locates the biggest blob on the binary source image and extracts it.
    /// The filter also can use the binary source for the biggest blob's location only, but extract it from
    /// another image, which is set using <see cref="OriginalImage"/> property. The original image 
    /// potentially may be the source of the binary image and user may be interested in the biggest blob
    /// from the original image, but not from the binary.</para>
    /// 
    /// <para><note>The class accepts only binary (8 bpp indexed) images as filter's input.
    /// <see cref="OriginalImage"/> may be as grayscale (8 bpp indexed), as
    /// color (24 bpp) image.</note></para>
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
    /// <img src="sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="biggest_blob.jpg" width="141" height="226" />
    /// </remarks>
    /// 
    public class ExtractBiggestBlob : IFilter
    {
        private Bitmap originalImage = null;

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
        public Bitmap Apply( Bitmap image )
        {
            // check image format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new ArgumentException( "The filter can be applied to binary/graysclae (8bpp indexed) image only" );
            }

            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

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
        /// <param name="imageData">Source image to get biggest blob from.</param>
        /// 
        /// <returns>Returns image of the biggest blob.</returns>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new ArgumentException( "The filter can be applied to binary/graysclae (8bpp indexed) image only" );
            }

            // locate blobs on the source image
            BlobCounter blobCounter = new BlobCounter( imageData );
            // get information about blobs
            Blob[] blobs = blobCounter.GetObjectInformation( );
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
                blobCounter.ExtractBlobsImage( imageData, biggestBlob );
            }
            else
            {
                // check original image's format
                if (
                    ( originalImage.PixelFormat != PixelFormat.Format24bppRgb ) &&
                    ( originalImage.PixelFormat != PixelFormat.Format8bppIndexed )
                    )
                {
                    throw new ArgumentException( "Original image must graysclae (8bpp indexed) or color (24bpp) image only" );
                }

                // check its size
                if ( ( originalImage.Width != imageData.Width ) || ( originalImage.Height != imageData.Height ) )
                {
                    throw new ArgumentException( "Original image must have the same size as passed source image" );
                }

                blobCounter.ExtractBlobsImage( originalImage, biggestBlob );
            }

            return biggestBlob.Image;
        }
    }
}
