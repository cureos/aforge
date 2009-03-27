// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Blob counter based on recursion.
    /// </summary>
    /// 
    /// <remarks><para>The class counts and extracts stand alone objects in
    /// binary images using recursive version of connected components labeling
    /// algorithm.</para>
    /// 
    /// <para><note>The algorithm treats all black pixels as background, but not an object.
    /// This means that all objects, which could be located be the algorithm, should have other
    /// than black color.</note></para>
    /// 
    /// <para><note>Since this algorithm is based on recursion, it is
    /// required to be careful with its application to big images with big blobs,
    /// because in this case recursion will require big stack size and may lead
    /// to stack overflow. The recursive version may be applied (and may be even
    /// faster than <see cref="BlobCounter"/>) to an image with small blobs -
    /// "star sky" image (or small cells, for example, etc).</note></para>
    /// 
    /// <para>For blobs' searching the class supports only 8 bpp indexed grayscale images. 
    /// See documentation about <see cref="BlobCounterBase"/> for information about which
    /// pixel formats are supported for extraction of blobs.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an instance of blob counter algorithm
    /// RecursiveBlobCounter bc = new RecursiveBlobCounter( );
    /// // process binary image
    /// bc.ProcessImage( image );
    /// Rectangle[] rects = bc.GetObjectsRectangles( );
    /// // process blobs
    /// foreach ( Rectangle rect in rects )
    /// {
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class RecursiveBlobCounter : BlobCounterBase
    {
        // temporary variable
        private int[] tempLabels;
        private int stride;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <remarks>Creates new instance of the <see cref="RecursiveBlobCounter"/> class with
        /// an empty objects map. Before using methods, which provide information about blobs
        /// or extract them, the <see cref="BlobCounterBase.ProcessImage(Bitmap)"/>,
        /// <see cref="BlobCounterBase.ProcessImage(BitmapData)"/> or <see cref="BlobCounterBase.ProcessImage(UnmanagedImage)"/>
        /// method should be called to collect objects map.</remarks>
        /// 
        public RecursiveBlobCounter( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="image">Binary image to look for objects in.</param>
        /// 
        public RecursiveBlobCounter( Bitmap image ) : base( image ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Binary image data to look for objects in.</param>
        /// 
        public RecursiveBlobCounter( BitmapData imageData ) : base( imageData ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveBlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="image">Binary unmanaged image to look for objects in.</param>
        /// 
        public RecursiveBlobCounter( UnmanagedImage image ) : base( image ) { }

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to process.</param>
        /// 
        /// <remarks>The method supports only 8 bpp indexed grayscale image.</remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the source image.</exception>
        /// 
        protected override void BuildObjectsMap( UnmanagedImage image )
        {
            this.stride = image.Stride;
            int offset = stride - imageWidth;

            // check pixel format
            if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                throw new UnsupportedImageFormat( "Unsupported pixel format of the source image." );
            }

            // allocate temporary labels array
            tempLabels = new int[( imageWidth + 2 ) * ( imageHeight + 2 )];
            // fill boundaries with reserved value
            for ( int x = 0, mx = imageWidth + 2; x < mx; x++ )
            {
                tempLabels[x] = -1;
                tempLabels[x + ( imageHeight + 1 ) * ( imageWidth + 2 )] = -1;
            }
            for ( int y = 0, my = imageHeight + 2; y < my; y++ )
            {
                tempLabels[y * ( imageWidth + 2 )] = -1;
                tempLabels[y * ( imageWidth + 2 ) + imageWidth + 1] = -1;
            }

            // initial objects count
            objectsCount = 0;

            // do the job
            unsafe
            {
                byte* src = (byte*) image.ImageData.ToPointer( );
                int p = imageWidth + 2 + 1;

                // for each line
                for ( int y = 0; y < imageHeight; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < imageWidth; x++, src++, p++ )
                    {
                        // check for non-labeled pixel
                        if ( ( *src != 0 ) && ( tempLabels[p] == 0 ) )
                        {
                            objectsCount++;
                            LabelPixel( src, p );
                        }
                    }
                    src += offset;
                    p += 2;
                }

            }

            // allocate labels array
            objectLabels = new int[imageWidth * imageHeight];

            for ( int y = 0; y < imageHeight; y++ )
            {
                Array.Copy( tempLabels, ( y + 1 ) * ( imageWidth + 2 ) + 1, objectLabels, y * imageWidth, imageWidth );
            }
        }

        private unsafe void LabelPixel( byte* pixel, int labelPointer )
        {
            if ( ( tempLabels[labelPointer] == 0 ) && ( *pixel != 0 ) )
            {
                tempLabels[labelPointer] = objectsCount;

                LabelPixel( pixel + 1, labelPointer + 1 );                              // x + 1, y
                LabelPixel( pixel + 1 + stride, labelPointer + 1 + 2 + imageWidth );    // x + 1, y + 1
                LabelPixel( pixel + stride, labelPointer + 2 + imageWidth );            // x    , y + 1
                LabelPixel( pixel - 1 + stride, labelPointer - 1 + 2 + imageWidth );    // x - 1, y + 1
                LabelPixel( pixel - 1, labelPointer - 1 );                              // x - 1, y
                LabelPixel( pixel - 1 - stride, labelPointer - 1 - 2 - imageWidth );    // x - 1, y - 1
                LabelPixel( pixel - stride, labelPointer - 2 - imageWidth );            // x    , y - 1
                LabelPixel( pixel + 1 - stride, labelPointer + 1 - 2 - imageWidth );    // x + 1, y - 1
            }
        }
    }
}
