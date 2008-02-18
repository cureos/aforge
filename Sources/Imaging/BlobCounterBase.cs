// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
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
    /// // set filtering options
    /// bc.FilterBlobs = true;
    /// bc.MinWidth  = 5;
    /// bc.MinHeight = 5;
    /// // process binary image
    /// bc.ProcessImage( image );
    /// Blob[] blobs = bc.GetObjects( image );
    /// // process blobs
    /// foreach ( Blob blob in blobs )
    /// {
    ///     // ...
    ///     // blob.Rectangle - blob's rectangle
    ///     // blob.Image - blob's image
    /// }
    /// </code>
    /// </remarks>
    /// 
    public abstract class BlobCounterBase
    {
        // blobs' rectangles
        Rectangle[] blobsRectangles = null;

        // filtering by size is required or nor
        private bool filterBlobs = false;

        // coupled size filtering or not
        private bool coupledSizeFiltering = false;

        // blobs' minimal and maximal size
        private int minWidth    = 1;
        private int minHeight   = 1;
        private int maxWidth    = int.MaxValue;
        private int maxHeight   = int.MaxValue;

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
        /// Specifies if blobs should be filtered.
        /// </summary>
        /// 
        /// <remarks><para>If the property is equal to <b>false</b>, then there is no any additional
        /// post processing after image was processed. If the property is set to <b>true</b>, then
        /// blobs filtering is done right after image processing routine. Blobs are filtered according
        /// to dimensions specified in <see cref="MinWidth"/>, <see cref="MinHeight"/>, <see cref="MaxWidth"/>
        /// and <see cref="MaxHeight"/> properties.</para>
        /// <para>Default value is <b>false</b>.</para></remarks>
        /// 
        public bool FilterBlobs
        {
            get { return filterBlobs; }
            set { filterBlobs = value; }
        }

        /// <summary>
        /// Specifies if size filetering should be coupled or not.
        /// </summary>
        /// 
        /// <remarks><para>In uncoupled filtering mode, objects are filtered out in the case if
        /// their width is smaller than <see cref="MinWidth"/> or height is smaller than 
        /// <see cref="MinHeight"/>. But in coupled filtering mode, objects are filtered out in
        /// the case if their width is smaller than <see cref="MinWidth"/> <b>and</b> height is
        /// smaller than <see cref="MinHeight"/>. In both modes the idea with filtering by objects'
        /// maximum size is the same as filtering by objects' minimum size.</para>
        /// <para>Default value is set to <b>false</b>, what means uncoupled filtering by size.</para>
        /// </remarks>
        /// 
        public bool CoupledSizeFiltering
        {
            get { return coupledSizeFiltering; }
            set { coupledSizeFiltering = value; }
        }

        /// <summary>
        /// Minimum allowed width of blob.
        /// </summary>
        /// 
        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        /// <summary>
        /// Minimum allowed height of blob.
        /// </summary>
        /// 
        public int MinHeight
        {
            get { return minHeight; }
            set { minHeight = value; }
        }

        /// <summary>
        /// Maximum allowed width of blob.
        /// </summary>
        /// 
        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        /// <summary>
        /// Maximum allowed height of blob.
        /// </summary>
        /// 
        public int MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
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

            // do actual objects map building
            ProcessImage( imageData.Scan0, imageData.Width, imageData.Height, imageData.Stride );
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

            // free old blobs' rectangles
            blobsRectangles = null;

            // do actual objects map building
            BuildObjectsMap( rawImageData, stride );

            // filter blobs by size if required
            if ( filterBlobs )
            {
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

                // labels remapping array
                int[] labelsMap = new int[objectsCount + 1];
                for ( int j = 1; j <= objectsCount; j++ )
                {
                    labelsMap[j] = j;
                }

                // check dimension of all objects and filter them
                int objectsToRemove = 0;

                for ( int j = 1; j <= objectsCount; j++ )
                {
                    int blobWidth  = x2[j] - x1[j] + 1;
                    int blobHeight = y2[j] - y1[j] + 1;

                    if ( coupledSizeFiltering == false )
                    {
                        // uncoupled filtering
                        if (
                            ( blobWidth < minWidth ) || ( blobHeight < minHeight ) ||
                            ( blobWidth > maxWidth ) || ( blobHeight > maxHeight ) )
                        {
                            labelsMap[j] = 0;
                            objectsToRemove++;
                        }
                    }
                    else
                    {
                        // coupled filtering
                        if (
                            ( ( blobWidth < minWidth ) && ( blobHeight < minHeight ) ) ||
                            ( ( blobWidth > maxWidth ) && ( blobHeight > maxHeight ) ) )
                        {
                            labelsMap[j] = 0;
                            objectsToRemove++;
                        }
                    }
                }

                // 1) update labels remapping array
                // 2) collect remaining rectangles
                blobsRectangles = new Rectangle[objectsCount - objectsToRemove];

                label = 0;
                for ( int j = 1; j <= objectsCount; j++ )
                {
                    if ( labelsMap[j] != 0 )
                    {
                        // collect blob
                        blobsRectangles[label] = new Rectangle( x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1 );
                        label++;
                        // update remapping array
                        labelsMap[j] = label;
                    }
                }

                // repair object labels
                for ( int j = 0, n = objectLabels.Length; j < n; j++ )
                {
                    objectLabels[j] = labelsMap[objectLabels[j]];
                }

                objectsCount -= objectsToRemove;
            }
        }

        /// <summary>
        /// Get objects' rectangles.
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
                throw new ApplicationException( "Image should be processed before to collect objects map" );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            return blobsRectangles;
        }

        /// <summary>
        /// Get objects' information.
        /// </summary>
        /// 
        /// <returns>Returns array of partially initialized blobs (without <see cref="Blob.Image"/> property initialized.</returns>
        /// 
        /// <remarks><para>By the amount of provided information, the method is between <see cref="GetObjectRectangles"/> and
        /// <see cref="GetObjects( BitmapData )"/> methods. The method provides array of blobs without initialized their image.
        /// Blob's image may be extracted later using <see cref="ExtractBlobsImage( Bitmap, Blob )"/>
        /// or <see cref="ExtractBlobsImage( BitmapData, Blob )"/> method.
        /// </para></remarks>
        /// 
        public Blob[] GetObjectInformation( )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map" );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            Blob[] blobs = new Blob[objectsCount];

            // create each blob
            for ( int k = 0; k < objectsCount; k++ )
            {
                blobs[k] = new Blob( k + 1, blobsRectangles[k] );
            }

            return blobs;
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="image">Source image (24 bpp color or 8 bpp indexed grayscale).</param>
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
                ImageLockMode.ReadOnly,
                ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                    PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb );

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
        /// <param name="imageData">Source image data (24 bpp color or 8 bpp indexed grayscale).</param>
        /// 
        /// <returns>Returns array of blobs.</returns>
        /// 
        /// <remarks><para>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be called, which will build objects map.</para></remarks>
        /// 
        public Blob[] GetObjects( BitmapData imageData )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map" );

            if (
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new ArgumentException( "The method can be applied to graysclae (8bpp indexed) or color (24bpp) image only" );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            // image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int srcStride = imageData.Stride;
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            Blob[] objects = new Blob[objectsCount];

            // create each image
            for ( int k = 0; k < objectsCount; k++ )
            {
                int objectWidth  = blobsRectangles[k].Width;
                int objectHeight = blobsRectangles[k].Height;

                int xmin = blobsRectangles[k].X;
                int xmax = xmin + objectWidth - 1;
                int ymin = blobsRectangles[k].Y;
                int ymax = ymin + objectHeight - 1;

                int label = k + 1;

                // create new image
                Bitmap dstImg = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                    AForge.Imaging.Image.CreateGrayscaleImage( objectWidth, objectHeight ) :
                    new Bitmap( objectWidth, objectHeight, PixelFormat.Format24bppRgb );

                // lock destination bitmap data
                BitmapData dstData = dstImg.LockBits(
                    new Rectangle( 0, 0, objectWidth, objectHeight ),
                    ImageLockMode.ReadWrite, imageData.PixelFormat );

                // copy image
                unsafe
                {
                    byte* src = (byte*) imageData.Scan0.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
                    byte* dst = (byte*) dstData.Scan0.ToPointer( );
                    int p = ymin * width + xmin;

                    int srcOffset = srcStride - objectWidth * pixelSize;
                    int dstOffset = dstData.Stride - objectWidth * pixelSize;
                    int labelsOffset = width - objectWidth;

                    // for each line
                    for ( int y = ymin; y <= ymax; y++ )
                    {
                        // copy each pixel
                        for ( int x = xmin; x <= xmax; x++, p++, dst += pixelSize, src += pixelSize )
                        {
                            if ( objectLabels[p] == label )
                            {
                                // copy pixel
                                *dst = *src;

                                if ( pixelSize > 1 )
                                {
                                    dst[1] = src[1];
                                    dst[2] = src[2];
                                }
                            }
                        }
                        src += srcOffset;
                        dst += dstOffset;
                        p += labelsOffset;
                    }
                }
                // unlock destination image
                dstImg.UnlockBits( dstData );

                objects[k] = new Blob( label, new Rectangle( xmin, ymin, objectWidth, objectHeight ), dstImg );
            }
            return objects;
        }

        /// <summary>
        /// Extract blob's image.
        /// </summary>
        /// 
        /// <param name="image">Source image to extract blob's image from.</param>
        /// <param name="blob">Blob which is required to extracå image of.</param>
        /// 
        /// <remarks><para>The method is used to extract image of partially initialized blob, which
        /// was provided by <see cref="GetObjectInformation"/> method.</para></remarks>
        /// 
        public void ExtractBlobsImage( Bitmap image, Blob blob )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly,
                ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                    PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb );

            // process image
            ExtractBlobsImage( imageData, blob );

            // unlock source images
            image.UnlockBits( imageData );
        }

        /// <summary>
        /// Extract blob's image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to extract blob's image from.</param>
        /// <param name="blob">Blob which is required to extracå image of.</param>
        /// 
        /// <remarks><para>The method is used to extract image of partially initialized blob, which
        /// was provided by <see cref="GetObjectInformation"/> method.</para></remarks>
        /// 
        public void ExtractBlobsImage( BitmapData imageData, Blob blob )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map" );

            if (
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new ArgumentException( "The method can be applied to graysclae (8bpp indexed) or color (24bpp) image only" );

            // image size
            int width  = imageData.Width;
            int height = imageData.Height;
            int srcStride = imageData.Stride;
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            int objectWidth  = blob.Rectangle.Width;
            int objectHeight = blob.Rectangle.Height;

            int xmin = blob.Rectangle.Left;
            int xmax = xmin + objectWidth - 1;
            int ymin = blob.Rectangle.Top;
            int ymax = ymin + objectHeight - 1;

            int label = blob.ID;

            // create new image
            blob.Image = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                AForge.Imaging.Image.CreateGrayscaleImage( objectWidth, objectHeight ) :
                new Bitmap( objectWidth, objectHeight, PixelFormat.Format24bppRgb );

            // lock destination bitmap data
            BitmapData dstData = blob.Image.LockBits(
                new Rectangle( 0, 0, objectWidth, objectHeight ),
                ImageLockMode.ReadWrite, imageData.PixelFormat );

            // copy image
            unsafe
            {
                byte* src = (byte*) imageData.Scan0.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
                byte* dst = (byte*) dstData.Scan0.ToPointer( );
                int p = ymin * width + xmin;

                int srcOffset = srcStride - objectWidth * pixelSize;
                int dstOffset = dstData.Stride - objectWidth * pixelSize;
                int labelsOffset = width - objectWidth;

                // for each line
                for ( int y = ymin; y <= ymax; y++ )
                {
                    // copy each pixel
                    for ( int x = xmin; x <= xmax; x++, p++, dst += pixelSize, src += pixelSize )
                    {
                        if ( objectLabels[p] == label )
                        {
                            // copy pixel
                            *dst = *src;

                            if ( pixelSize > 1 )
                            {
                                dst[1] = src[1];
                                dst[2] = src[2];
                            }
                        }
                    }
                    src += srcOffset;
                    dst += dstOffset;
                    p += labelsOffset;
                }
            }
            // unlock destination image
            blob.Image.UnlockBits( dstData );
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


        #region Private Methods - Collecting objects' rectangles

        // Collect objects' rectangles
        private void CollectObjectsRectangles( )
        {
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
            blobsRectangles = new Rectangle[objectsCount];

            for ( int j = 1; j <= objectsCount; j++ )
            {
                blobsRectangles[j - 1] = new Rectangle( x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1 );
            }
        }
        #endregion
    }
}
