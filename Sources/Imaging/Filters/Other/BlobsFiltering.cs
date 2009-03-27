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
    /// Blobs filtering by size.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs filtering of blobs by their size in binary image - all
    /// blobs, which are smaller or bigger then specified limits, are removed from source image.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// BlobsFiltering filter = new BlobsFiltering( );
    /// // configure filter
    /// filter.CoupledSizeFiltering = true;
    /// filter.MinWidth  = 70;
    /// filter.MinHeight = 70;
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/blobs_filtering.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    /// <seealso cref="BlobCounter"/>
    /// <seealso cref="BlobCounterBase"/>
    ///
    public class BlobsFiltering : BaseInPlaceFilter
    {
        private BlobCounter blobCounter = new BlobCounter( );

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
        /// Specifies if size filetering should be coupled or not.
        /// </summary>
        /// 
        /// <remarks><para>See documentation for <see cref="BlobCounterBase.CoupledSizeFiltering"/> property
        /// of <see cref="BlobCounterBase"/> class for more information.</para></remarks>
        /// 
        public bool CoupledSizeFiltering
        {
            get { return blobCounter.CoupledSizeFiltering; }
            set { blobCounter.CoupledSizeFiltering = value; }
        }

        /// <summary>
        /// Minimum allowed width of blob.
        /// </summary>
        /// 
        public int MinWidth
        {
            get { return blobCounter.MinWidth; }
            set { blobCounter.MinWidth = value; }
        }

        /// <summary>
        /// Minimum allowed height of blob.
        /// </summary>
        /// 
        public int MinHeight
        {
            get { return blobCounter.MinHeight; }
            set { blobCounter.MinHeight = value; }
        }

        /// <summary>
        /// Maximum allowed width of blob.
        /// </summary>
        /// 
        public int MaxWidth
        {
            get { return blobCounter.MaxWidth; }
            set { blobCounter.MaxWidth = value; }
        }

        /// <summary>
        /// Maximum allowed height of blob.
        /// </summary>
        /// 
        public int MaxHeight
        {
            get { return blobCounter.MaxHeight; }
            set { blobCounter.MaxHeight = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobsFiltering"/> class.
        /// </summary>
        /// 
        public BlobsFiltering( )
            : this( 1, 1, int.MaxValue, int.MaxValue, false )
        {
            blobCounter.FilterBlobs = true;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobsFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="minWidth">Minimum allowed width of blob.</param>
        /// <param name="minHeight">Minimum allowed height of blob.</param>
        /// <param name="maxWidth">Maximum allowed width of blob.</param>
        /// <param name="maxHeight">Maximum allowed height of blob.</param>
        /// 
        /// <remarks>This constructor creates an instance of <see cref="BlobsFiltering"/> class
        /// with <see cref="CoupledSizeFiltering"/> property set to <b>false</b>.</remarks>
        /// 
        public BlobsFiltering( int minWidth, int minHeight, int maxWidth, int maxHeight )
            : this( minWidth, minHeight, maxWidth, maxHeight, false ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobsFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="minWidth">Minimum allowed width of blob.</param>
        /// <param name="minHeight">Minimum allowed height of blob.</param>
        /// <param name="maxWidth">Maximum allowed width of blob.</param>
        /// <param name="maxHeight">Maximum allowed height of blob.</param>
        /// <param name="coupledSizeFiltering">Specifies if size filetering should be coupled or not.</param>
        /// 
        /// <remarks><para>For information about coupled filtering mode see documentation for
        /// <see cref="BlobCounterBase.CoupledSizeFiltering"/> property of <see cref="BlobCounterBase"/>
        /// class.</para></remarks>
        /// 
        public BlobsFiltering( int minWidth, int minHeight, int maxWidth, int maxHeight, bool coupledSizeFiltering )
        {
            blobCounter.FilterBlobs = true;
            blobCounter.MinWidth  = minWidth;
            blobCounter.MinHeight = minHeight;
            blobCounter.MaxWidth  = maxWidth;
            blobCounter.MaxHeight = maxHeight;
            blobCounter.CoupledSizeFiltering = coupledSizeFiltering;

            formatTransalations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            // use blob counter to build objects map and filter them
            blobCounter.ProcessImage( image );
            int[] objectsMap = blobCounter.ObjectLabels;

            // get image width and height
            int width  = image.Width;
            int height = image.Height;
            int offset = image.Stride - width;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            for ( int y = 0, p = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++, ptr++, p++ )
                {
                    if ( objectsMap[p] == 0 )
                    {
                        *ptr = 0;
                    }
                }
                ptr += offset;
            }
        }
    }
}
