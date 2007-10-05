// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
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
    /// <para><b>Warning</b>: Since this algorithm is based on recursion, it is
    /// required to be careful with its application to big images with big blob,
    /// because in this case recursion will require big stack size.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an instance of blob counter algorithm
    /// RecursiveBlobCounter bc = new RecursiveBlobCounter( );
    /// // process binary image
    /// bc.ProcessImage( image );
    /// Rectangle[] rects = bc.GetObjectRectangles( );
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
        /// Initializes a new instance of the <see cref="BlobCounter"/> class.
        /// </summary>
        /// 
        /// <remarks>Creates new instance of the <see cref="BlobCounter"/> class with
        /// an empty objects map. Before using <see cref="BlobCounterBase.GetObjects(Bitmap)"/> or 
        /// <see cref="BlobCounterBase.GetObjectRectangles"/> methods, the
        /// <see cref="BlobCounterBase.ProcessImage(Bitmap)"/>
        /// method should be called to collect objects map.</remarks>
        /// 
        public RecursiveBlobCounter( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="image">Binary image to look for objects in.</param>
        /// 
        public RecursiveBlobCounter( Bitmap image ) : base( image ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCounter"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Binary image data to look for objects in.</param>
        /// 
        public RecursiveBlobCounter( BitmapData imageData ) : base( imageData ) { }

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        /// 
        /// <param name="rawImageData">Raw image data.</param>
        /// <param name="stride">Length of one image line in bytes.</param>
        /// 
        protected override void BuildObjectsMap( IntPtr rawImageData, int stride )
        {
            this.stride = stride;
            int offset = stride - imageWidth;

            // allocate temporary labels array
            tempLabels = new int[ ( imageWidth + 2 )  * ( imageHeight + 2 ) ];
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
                byte* src = (byte*) rawImageData.ToPointer( );
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
                Array.Copy( tempLabels, ( y + 1 )* ( imageWidth + 2 ) + 1, objectLabels, y * imageWidth, imageWidth );
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
