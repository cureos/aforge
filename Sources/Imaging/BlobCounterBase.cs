// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Frank Nagl, 2009
// admin@franknagl.de
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Possible object orders.
    /// </summary>
    /// 
    /// <remarks>The enumeration defines possible sorting orders of objects, found by blob
    /// counting classes.</remarks>
    /// 
    public enum ObjectsOrder
    {
        /// <summary>
        /// Unsorted order (as it is collected by algorithm).
        /// </summary>
        None,

        /// <summary>
        /// Objects are sorted by size in descending order (big objects go first).
        /// </summary>
        Size,

        /// <summary>
        /// Objects are sorted by Y coordinate, then by X coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        YX,

        /// <summary>
        /// Objects are sorted by X coordinate, then by Y coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        XY
    }

    /// <summary>
    /// Base class for different blob counting algorithms.
    /// </summary>
    /// 
    /// <remarks><para>The class is abstract and serves as a base for different blob counting algorithms.
    /// Classes, which inherit from this base class, require to implement <see cref="BuildObjectsMap"/>
    /// method, which does actual building of object's label's map.</para>
    /// 
    /// <para>For blobs' searcing usually all inherited classes accept binary images, which are actually
    /// grayscale thresholded images. But the exact supported format should be checked in particular class,
    /// inheriting from the base class. For blobs' extraction the class supports grayscale (8 bpp indexed)
    /// and color images (24 bpp).</para>
    /// 
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

        // objects' sort order
        private ObjectsOrder objectsOrder = ObjectsOrder.None;

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
        /// Objects sort order.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies objects' sort order, which are provided
        /// by <see cref="GetObjectsRectangles"/>, <see cref="GetObjectsInformation"/>, etc.
        /// </para></remarks>
        /// 
        public ObjectsOrder ObjectsOrder
        {
            get { return objectsOrder; }
            set { objectsOrder = value; }
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
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para></remarks>
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
        /// 
        /// <para>Default value is set to <see langword="false"/>, what means uncoupled filtering by size.</para>
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
        /// <remarks><para>The property specifies minimum object's width acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
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
        /// <remarks><para>The property specifies minimum object's height acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
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
        /// <remarks><para>The property specifies maximum object's width acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
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
        /// <remarks><para>The property specifies maximum object's height acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
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
        /// an empty objects map. Before using methods, which provide information about blobs
        /// or extract them, the <see cref="ProcessImage(Bitmap)"/>,
        /// <see cref="ProcessImage(BitmapData)"/> or <see cref="ProcessImage(UnmanagedImage)"/>
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
        /// Initializes a new instance of the <see cref="BlobCounterBase"/> class.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged binary image to look for objects in.</param>
        /// 
        /// <remarks>Creates new instance of the <see cref="BlobCounterBase"/> class with
        /// initialized objects map built by calling <see cref="ProcessImage(UnmanagedImage)"/> method.</remarks>
        /// 
        public BlobCounterBase( UnmanagedImage image )
        {
            ProcessImage( image );
        }

        /// <summary>
        /// Build objects map.
        /// </summary>
        /// 
        /// <param name="image">Source binary image.</param>
        /// 
        /// <remarks><para>Processes the image and builds objects map, which is used later to extracts blobs.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( Bitmap image )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                // process image
                ProcessImage( imageData );
            }
            finally
            {
                // unlock source image
                image.UnlockBits( imageData );
            }
        }

        /// <summary>
        /// Build objects map.
        /// </summary>
        /// 
        /// <param name="imageData">Source binary image data.</param>
        /// 
        /// <remarks><para>Processes the image and builds objects map, which is used later to extracts blobs.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( BitmapData imageData )
        {
            // do actual objects map building
            ProcessImage( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Build object map from raw image data.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged binary image data.</param>
        /// 
        /// <remarks><para>Processes the image and builds objects map, which is used later to extracts blobs.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the source image.</exception>
        /// 
        public void ProcessImage( UnmanagedImage image )
        {
            imageWidth  = image.Width;
            imageHeight = image.Height;

            // free old blobs' rectangles
            blobsRectangles = null;

            // do actual objects map building
            BuildObjectsMap( image );

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
        /// <returns>Returns array of objects' rectangles.</returns>
        /// 
        /// <remarks>The method returns array of objects rectangles. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will
        /// build objects map.</remarks>
        /// 
        /// <exception cref="ApplicationException">No image was processed before, so objects' rectangles
        /// can not be collected.</exception>
        /// 
        public Rectangle[] GetObjectsRectangles( )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map." );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            // do we need to sort the list?
            if ( objectsOrder != ObjectsOrder.None )
            {
                Rectangle[] rects = (Rectangle[]) blobsRectangles.Clone( );
                Array.Sort( rects, new RectanglesSorter( objectsOrder ) );
                return rects;
            }

            return blobsRectangles;
        }

        /// <summary>
        /// Get objects' information.
        /// </summary>
        /// 
        /// <returns>Returns array of partially initialized blobs (without <see cref="Blob.Image"/> property initialized).</returns>
        /// 
        /// <remarks><para>By the amount of provided information, the method is between <see cref="GetObjectsRectangles"/> and
        /// <see cref="GetObjects( BitmapData, bool )"/> methods. The method provides array of blobs without initialized their image.
        /// Blob's image may be extracted later using <see cref="ExtractBlobsImage( Bitmap, Blob, bool )"/>,
        /// <see cref="ExtractBlobsImage( BitmapData, Blob, bool )"/> or <see cref="ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.
        /// </para></remarks>
        /// 
        /// <example>
        /// <code>
        /// // create blob counter and process image
        /// BlobCounter bc = new BlobCounter( sourceImage );
        /// // specify sort order
        /// bc.ObjectsOrder = ObjectsOrder.Size;
        /// // get objects' information (blobs without image)
        /// Blob[] blobs = bc.GetObjectInformation( );
        /// // process blobs
        /// foreach ( Blob blob in blobs )
        /// {
        ///     // check blob's properties
        ///     if ( blob.Rectangle.Width > 50 )
        ///     {
        ///         // the blob looks interesting, let's extract it
        ///         bc.ExtractBlobsImage( sourceImage, blob );
        ///     }
        /// }
        /// </code>
        /// </example>
        /// 
        /// <exception cref="ApplicationException">No image was processed before, so objects' rectangles
        /// can not be collected.</exception>
        /// 
        public Blob[] GetObjectsInformation( )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map." );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            Blob[] blobs = new Blob[objectsCount];

            // create each blob
            for ( int k = 0; k < objectsCount; k++ )
            {
                blobs[k] = new Blob( k + 1, blobsRectangles[k] );
            }

            // sort blobs
            if ( objectsOrder != ObjectsOrder.None )
            {
                Array.Sort( blobs, new RectanglesSorter( objectsOrder ) );
            }

            return blobs;
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="image">Source image to extract objects from.</param>
        /// 
        /// <returns>Returns array of blobs.</returns>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        ///
        /// <remarks><para>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so objects
        /// can not be collected.</exception>
        /// 
        public Blob[] GetObjects( Bitmap image, bool extractInOriginalSize )
        {
            Blob[] blobs = null;
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                // process image
                blobs = GetObjects( imageData, extractInOriginalSize );
            }
            finally
            {
                // unlock source images
                image.UnlockBits( imageData );
            }
            return blobs;
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to extract objects from.</param>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        ///
        /// <returns>Returns array of blobs.</returns>
        /// 
        /// <remarks><para>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so objects
        /// can not be collected.</exception>
        /// 
        public Blob[] GetObjects( BitmapData imageData, bool extractInOriginalSize )
        {
            return GetObjects( new UnmanagedImage( imageData ), extractInOriginalSize );
        }

        /// <summary>
        /// Get blobs.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to extract objects from.</param>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        /// 
        /// <returns>Returns array of blobs.</returns>
        /// 
        /// <remarks><para>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        ///
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so objects
        /// can not be collected.</exception>
        /// 
        public Blob[] GetObjects( UnmanagedImage image, bool extractInOriginalSize )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map." );

            if (
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new UnsupportedImageFormat( "Unsupported pixel format of the provided image." );

            // collect rectangles, if they are not collected yet
            if ( blobsRectangles == null )
                CollectObjectsRectangles( );

            // image size
            int width  = image.Width;
            int height = image.Height;
            int srcStride = image.Stride;
            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            Blob[] objects = new Blob[objectsCount];

            // create each image
            for ( int k = 0; k < objectsCount; k++ )
            {
                int objectWidth  = blobsRectangles[k].Width;
                int objectHeight = blobsRectangles[k].Height;

                int blobImageWidth  = ( extractInOriginalSize ) ? width : objectWidth;
                int blobImageHeight = ( extractInOriginalSize ) ? height : objectHeight;

                int xmin = blobsRectangles[k].X;
                int xmax = xmin + objectWidth - 1;
                int ymin = blobsRectangles[k].Y;
                int ymax = ymin + objectHeight - 1;

                int label = k + 1;

                // create new image
                Bitmap dstImg = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                    AForge.Imaging.Image.CreateGrayscaleImage( blobImageWidth, blobImageHeight ) :
                    new Bitmap( blobImageWidth, blobImageHeight, PixelFormat.Format24bppRgb );

                // lock destination bitmap data
                BitmapData dstData = dstImg.LockBits(
                    new Rectangle( 0, 0, blobImageWidth, blobImageHeight ),
                    ImageLockMode.ReadWrite, image.PixelFormat );

                // copy image
                unsafe
                {
                    byte* src = (byte*) image.ImageData.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
                    byte* dst = (byte*) dstData.Scan0.ToPointer( );
                    int p = ymin * width + xmin;

                    if ( extractInOriginalSize )
                    {
                        // allign destination pointer also
                        dst += ymin * dstData.Stride + xmin * pixelSize;
                    }

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

                objects[k] = new Blob( label, new Rectangle( xmin, ymin, objectWidth, objectHeight ), dstImg, extractInOriginalSize );
            }

            // sort blobs
            if ( objectsOrder != ObjectsOrder.None )
            {
                Array.Sort( objects, new RectanglesSorter( objectsOrder ) );
            }

            return objects;
        }

        /// <summary>
        /// Extract blob's image.
        /// </summary>
        /// 
        /// <param name="image">Source image to extract blob's image from.</param>
        /// <param name="blob">Blob which is required to be extracted.</param>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        ///
        /// <remarks><para>The method is used to extract image of partially initialized blob, which
        /// was provided by <see cref="GetObjectsInformation"/> method. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so blob
        /// can not be extracted.</exception>
        /// 
        public void ExtractBlobsImage( Bitmap image, Blob blob, bool extractInOriginalSize )
        {
            // lock source bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                // process image
                ExtractBlobsImage( imageData, blob, extractInOriginalSize );
            }
            finally
            {
                // unlock source images
                image.UnlockBits( imageData );
            }
        }

        /// <summary>
        /// Extract blob's image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data to extract blob's image from.</param>
        /// <param name="blob">Blob which is required to be extracted.</param>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        ///
        /// <remarks><para>The method is used to extract image of partially initialized blob, which
        /// was provided by <see cref="GetObjectsInformation"/> method. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so blob
        /// can not be extracted.</exception>
        /// 
        public void ExtractBlobsImage( BitmapData imageData, Blob blob, bool extractInOriginalSize )
        {
            ExtractBlobsImage( new UnmanagedImage( imageData ), blob, extractInOriginalSize );
        }

        /// <summary>
        /// Extract blob's image.
        /// </summary>
        /// 
        /// <param name="image">Source unmanaged image to extract blob's image from.</param>
        /// <param name="blob">Blob which is required to be extracted.</param>
        /// <param name="extractInOriginalSize">Specifies size of blobs' image to extract.
        /// If set to <see langword="true"/> each blobs' image will have the same size as
        /// the specified image. If set to <see langword="false"/> each blobs' image will
        /// have the size of its blob.</param>
        ///
        /// <remarks><para>The method is used to extract image of partially initialized blob, which
        /// was provided by <see cref="GetObjectsInformation"/> method. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/>, <see cref="ProcessImage(BitmapData)"/>
        /// or <see cref="ProcessImage(UnmanagedImage)"/> method should be called, which will build
        /// objects map.</para>
        /// 
        /// <para>The method supports 24 bpp color and 8 bpp indexed grayscale images.</para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormat">Unsupported pixel format of the provided image.</exception>
        /// <exception cref="ApplicationException">No image was processed before, so blob
        /// can not be extracted.</exception>
        /// 
        public void ExtractBlobsImage( UnmanagedImage image, Blob blob, bool extractInOriginalSize )
        {
            // check if objects map was collected
            if ( objectLabels == null )
                throw new ApplicationException( "Image should be processed before to collect objects map." );

            if (
                ( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
                ( image.PixelFormat != PixelFormat.Format8bppIndexed )
                )
                throw new UnsupportedImageFormat( "Unsupported pixel format of the provided image." );

            // image size
            int width  = image.Width;
            int height = image.Height;
            int srcStride = image.Stride;
            int pixelSize = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            int objectWidth  = blob.Rectangle.Width;
            int objectHeight = blob.Rectangle.Height;

            int blobImageWidth  = ( extractInOriginalSize ) ? width : objectWidth;
            int blobImageHeight = ( extractInOriginalSize ) ? height : objectHeight;

            int xmin = blob.Rectangle.Left;
            int xmax = xmin + objectWidth - 1;
            int ymin = blob.Rectangle.Top;
            int ymax = ymin + objectHeight - 1;

            int label = blob.ID;

            // create new image
            blob.Image = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
                AForge.Imaging.Image.CreateGrayscaleImage( blobImageWidth, blobImageHeight ) :
                new Bitmap( blobImageWidth, blobImageHeight, PixelFormat.Format24bppRgb );
            blob.OriginalSize = extractInOriginalSize;

            // lock destination bitmap data
            BitmapData dstData = blob.Image.LockBits(
                new Rectangle( 0, 0, blobImageWidth, blobImageHeight ),
                ImageLockMode.ReadWrite, image.PixelFormat );

            // copy image
            unsafe
            {
                byte* src = (byte*) image.ImageData.ToPointer( ) + ymin * srcStride + xmin * pixelSize;
                byte* dst = (byte*) dstData.Scan0.ToPointer( );
                int p = ymin * width + xmin;

                if ( extractInOriginalSize )
                {
                    // allign destination pointer also
                    dst += ymin * dstData.Stride + xmin * pixelSize;
                }

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
        /// <param name="image">Unmanaged image to process.</param>
        /// 
        /// <remarks><note>By the time this method is called bitmap's pixel format is not
        /// yet checked, so this should be done by the class inheriting from the base class.
        /// <see cref="imageWidth"/> and <see cref="imageHeight"/> members are initialized
        /// before the method is called, so these members may be used safely.</note></remarks>
        /// 
        protected abstract void BuildObjectsMap( UnmanagedImage image );


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

        // Rectangles' and blobs' sorter
        private class RectanglesSorter : System.Collections.IComparer
        {
            private ObjectsOrder order;

            public RectanglesSorter( ObjectsOrder order )
            {
                this.order = order;
            }

            public int Compare( Object x, Object y )
            {
                Rectangle xRect = ( x is Rectangle ) ? (Rectangle) x : ( (Blob) x ).Rectangle;
                Rectangle yRect = ( y is Rectangle ) ? (Rectangle) y : ( (Blob) y ).Rectangle;

                switch ( order )
                {
                    case ObjectsOrder.Size: // sort by size

                        // the order is changed to descending
                        return ( yRect.Width * yRect.Height - xRect.Width * xRect.Height );

                    case ObjectsOrder.YX:   // YX order

                        return ( ( xRect.Y * 100000 + xRect.X ) - ( yRect.Y * 100000 + yRect.X ) );

                    case ObjectsOrder.XY:   // XY order

                        return ( ( xRect.X * 100000 + xRect.Y ) - ( yRect.X * 100000 + yRect.Y ) );
                }
                return 0;
            }
        }

        #endregion
    }
}
