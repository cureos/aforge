// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Blobs filtering by size.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs filtering of blobs by their size in binary image.</para>
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
    /// <para><b>Initial image:</b></para>
    /// <img src="sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="blobs_filtering.jpg" width="320" height="240" />
    /// </remarks>
    ///
    public class BlobsFiltering : FilterGrayToGray
    {
        private BlobCounter blobCounter = new BlobCounter( );

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
        public BlobsFiltering( int minWidth, int minHeight, int maxWidth, int maxHeight, bool coupledSizeFiltering )
        {
            blobCounter.MinWidth  = minWidth;
            blobCounter.MinHeight = minHeight;
            blobCounter.MaxWidth  = maxWidth;
            blobCounter.MaxHeight = maxHeight;
            blobCounter.CoupledSizeFiltering = coupledSizeFiltering;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            // use blob counter to build objects map and filter them
            blobCounter.ProcessImage( imageData );
            int[] objectsMap = blobCounter.ObjectLabels;

            // get image width and height
            int width  = imageData.Width;
            int height = imageData.Height;
            int offset = imageData.Stride - width;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

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
