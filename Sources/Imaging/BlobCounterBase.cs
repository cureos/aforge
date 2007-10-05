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
    /// Base class for differnt blob counting algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The class is abstract and serves as a base for different blob counting algorithms.
    /// Classes, which inherit from this base class, require to implement <see cref="BuildObjectsMap"/>
    /// method, which does actual building objects labels map.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create an instance of blob counter algorithm
    /// BlobCounterBase bc = new ...
    /// // process binary image
    /// bc.ProcessImage( image );
    /// Blob[] blobs = bc.GetObjects( image );
    /// // process blobs
    /// foreach ( Blob blob in blobs )
    /// {
    ///     // ...
    ///     // blob.Location - location of the blob
    ///     // blob.Image - blob`s image
    /// }
    /// </code>
    /// </remarks>
    /// 
    public abstract class BlobCounterBase
    {
        /// <summary>
        /// Objects count.
        /// </summary>
        protected int objectsCount;

        /// <summary>
        /// Objects' labels.
        /// </summary>
        protected int[] objectLabels;

        /// <summary>
        /// Width of processed image.
        /// </summary>
        protected int imageWidth;

        /// <summary>
        /// Height of processed image.
        /// </summary>
        protected int imageHeight;

        /// <summary>
        /// Objects count.
        /// </summary>
        public int ObjectsCount
        {
            get { return objectsCount; }
        }

        /// <summary>
        /// Objects' labels.
        /// </summary>
        /// 
        /// <remarks>The array of <b>width</b> * <b>height</b> size, which holds
        /// labels for all objects. The background is represented with <b>0</b> value,
        /// but objects are represented with labels starting from <b>1</b>.</remarks>
        /// 
        public int[] ObjectLabels
        {
            get { return objectLabels; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCounterBase"/> class.
        /// </summary>
        /// 
        /// <remarks>Creates new instance of the <see cref="BlobCounterBase"/> class with
        /// an empty objects map. Before using <see cref="GetObjects(Bitmap)"/> or 
        /// <see cref="GetObjectRectangles"/> methods, the <see cref="ProcessImage(Bitmap)"/>
        /// method should be called to collect objects map.</remarks>
        /// 
        public BlobCounterBase( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCounterBase"/> class.
        /// </summary>
        /// 
        /// <param name="image">Binary image to look for objects in.</param>
        /// 
        /// <remarks>Creates new instance of the <see cref="BlobCounterBase"/> class with
        /// initialized objects map built by calling <see cref="ProcessImage(Bitmap)"/> method.</remarks>
        /// 
        public BlobCounterBase( Bitmap image )
        {
            ProcessImage( image );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobCounterBase"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Binary image data to look for objects in.</param>
        /// 
        /// <remarks>Creates new instance of the <see cref="BlobCounterBase"/> class with
        /// initialized objects map built by calling <see cref="ProcessImage(BitmapData)"/> method.</remarks>
        /// 
        public BlobCounterBase( BitmapData imageData )
        {
            ProcessImage( imageData );
        }

        /// <summary>
        /// Build objects map.
        /// </summary>
        /// 
        /// <param name="image">Source binary image.</param>
        /// 
        /// <remarks>Processes the image and builds objects map, which is used later to extracts blobs.</remarks>
        /// 
        public void ProcessImage( Bitmap image )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // process image
            ProcessImage( imageData );

            // unlock source images
            image.UnlockBits( imageData );
        }

		/// <summary>
		/// Build objects map.
		/// </summary>
		/// 
		/// <param name="imageData">Source binary image data.</param>
		/// 
		/// <remarks>Processes the image and builds objects map, which is used later to extracts blobs.</remarks>
		/// 
        public void ProcessImage( BitmapData imageData )
        {
            // check for grayscale image
            // actually we need binary image, but binary images are
            // represented as grayscale
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "Only binary (8bpp indexed grayscale) images are supported" );

            // get source image size
            imageWidth = imageData.Width;
            imageHeight = imageData.Height;

            // do actual objects map building
            BuildObjectsMap( imageData.Scan0, imageData.Stride );
        }

        /// <summary>
        /// Build object map from raw image data.
        /// </summary>
        /// 
        /// <param name="rawImageData">Raw image data.</param>
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="stride">Length of one image line in bytes.</param>
        /// 
        /// <remarks><para>Processes the image and builds objects map, which is used later to extracts blobs.
        /// This method works with raw image data kept in unmanaged memory.</para>
        /// <para>Warning</para>: it is on user's responsibility to ensure that memory buffer contains
        /// binary (or grayscale) image with 1 byte per pixel.</remarks>
        /// 
        public void ProcessImage( IntPtr rawImageData, int width, int height, int stride )
        {
            imageWidth = width;
            imageHeight = height;

            // do actual objects map building
            BuildObjectsMap( rawImageData, stride );
        }

        /// <summary>
        /// Get objects rectangles.
        /// </summary>
        /// 
        /// <returns>Returns array of objects rectangles.</returns>
        /// 
        /// <remarks>The method returns array of objects rectangles. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be called, which will build objects map.</remarks>
        /// 
        public Rectangle[] GetObjectRectangles( )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collec objects map" );

            int i = 0, label;

            // create object coordinates arrays
            int[] x1 = new int[objectsCount + 1];
            int[] y1 = new int[objectsCount + 1];
            int[] x2 = new int[objectsCount + 1];
            int[] y2 = new int[objectsCount + 1];

            for ( int j = 1; j <= objectsCount; j++ )
            {
                x1[j] = imageWidth;
                y1[j] = imageHeight;
            }

            // walk through labels array
            for ( int y = 0; y < imageHeight; y++ )
            {
                for ( int x = 0; x < imageWidth; x++, i++ )
                {
                    // get current label
                    label = objectLabels[i];

                    // skip unlabeled pixels
                    if ( label == 0 )
                        continue;

                    // check and update all coordinates

                    if ( x < x1[label] )
                    {
                        x1[label] = x;
                    }
                    if ( x > x2[label] )
                    {
                        x2[label] = x;
                    }
                    if ( y < y1[label] )
                    {
                        y1[label] = y;
                    }
                    if ( y > y2[label] )
                    {
                        y2[label] = y;
                    }
                }
            }

            // create rectangles
            Rectangle[] rects = new Rectangle[objectsCount];

            for ( int j = 1; j <= objectsCount; j++ )
            {
                rects[j - 1] = new Rectangle( x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1 );
            }

            return rects;
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="image">Source image.</param>
        /// 
        /// <returns>Returns array of blobs.</returns>
        /// 
        /// <remarks>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be called, which will build objects map.</remarks>
        /// 
        public Blob[] GetObjects( Bitmap image )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // process image
            Blob[] blobs = GetObjects( imageData );

            // unlock source images
            image.UnlockBits( imageData );

            return blobs;
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        /// 
        /// <returns>Returns array of blobs.</returns>
        /// 
        /// <remarks>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be called, which will build objects map.</remarks>
        /// 
        public Blob[] GetObjects( BitmapData imageData )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collec objects map" );

            // image size
            int width = imageData.Width;
            int height = imageData.Height;
            int i = 0, label;

            // --- STEP 1 - find each objects coordinates

            // create object coordinates arrays
            int[] x1 = new int[objectsCount + 1];
            int[] y1 = new int[objectsCount + 1];
            int[] x2 = new int[objectsCount + 1];
            int[] y2 = new int[objectsCount + 1];

            for ( int k = 1; k <= objectsCount; k++ )
            {
                x1[k] = width;
                y1[k] = height;
            }

            // walk through labels array
            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++, i++ )
                {
                    // get current label
                    label = objectLabels[i];

                    // skip unlabeled pixels
                    if ( label == 0 )
                        continue;

                    // check and update all coordinates

                    if ( x < x1[label] )
                    {
                        x1[label] = x;
                    }
                    if ( x > x2[label] )
                    {
                        x2[label] = x;
                    }
                    if ( y < y1[label] )
                    {
                        y1[label] = y;
                    }
                    if ( y > y2[label] )
                    {
                        y2[label] = y;
                    }
                }
            }

            // --- STEP 2 - get each object
            Blob[] objects = new Blob[objectsCount];

            int srcStride = imageData.Stride;

            // create each image
            for ( int k = 1; k <= objectsCount; k++ )
            {
                int xmin = x1[k];
                int xmax = x2[k];
                int ymin = y1[k];
                int ymax = y2[k];
                int objectWidth = xmax - xmin + 1;
                int objectHeight = ymax - ymin + 1;

                // create new image
                Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage( objectWidth, objectHeight );

                // lock destination bitmap data
                BitmapData dstData = dstImg.LockBits(
                    new Rectangle( 0, 0, objectWidth, objectHeight ),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

                // copy image
                unsafe
                {
                    byte* src = (byte*) imageData.Scan0.ToPointer( ) + ymin * srcStride + xmin;
                    byte* dst = (byte*) dstData.Scan0.ToPointer( );
                    int p = ymin * width + xmin;

                    int srcOffset = srcStride - objectWidth;
                    int dstOffset = dstData.Stride - objectWidth;
                    int labelsOffset = width - objectWidth;

                    // for each line
                    for ( int y = ymin; y <= ymax; y++ )
                    {
                        // copy each pixel
                        for ( int x = xmin; x <= xmax; x++, src++, dst++, p++ )
                        {
                            if ( objectLabels[p] == k )
                                *dst = *src;
                        }
                        src += srcOffset;
                        dst += dstOffset;
                        p += labelsOffset;
                    }
                }
                // unlock destination image
                dstImg.UnlockBits( dstData );

                objects[k - 1] = new Blob( dstImg, new Point( xmin, ymin ) );
            }

            return objects;
        }

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        /// 
        /// <param name="rawImageData">Raw image data.</param>
        /// <param name="stride">Length of one image line in bytes.</param>
        /// 
        /// <remarks>By the time this method is called, bitmap's pixel format is already
        /// checked as well as <see cref="imageWidth"/> and <see cref="imageHeight"/>
        /// members are initialized.</remarks>
        /// 
        protected abstract void BuildObjectsMap( IntPtr rawImageData, int stride );
    }
}
