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
    /// Connected components labeling.
    /// </summary>
    /// 
    /// <remarks><para>The filter performs labeling of objects in binary image. It colors
    /// each separate object using different color.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// ConnectedComponentsLabeling filter = new ConnectedComponentsLabeling( );
    /// // apply the filter
    /// Bitmap newImage = filter.Apply( image );
    /// // check objects count
    /// int objectCount = filter.ObjectCount;
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample2.jpg" width="320" height="240" />
    /// <para><b>Result image:</b></para>
    /// <img src="labeling.jpg" width="320" height="240" />
    /// </remarks>
    /// 
    public class ConnectedComponentsLabeling : FilterGrayToColor
    {
        // Color table for coloring objects
        private static Color[] colorTable = new Color[]
		{
			Color.Red,		Color.Green,	Color.Blue,			Color.Yellow,
			Color.Violet,	Color.Brown,	Color.Olive,		Color.Cyan,

			Color.Magenta,	Color.Gold,		Color.Indigo,		Color.Ivory,
			Color.HotPink,	Color.DarkRed,	Color.DarkGreen,	Color.DarkBlue,

			Color.DarkSeaGreen,	Color.Gray,	Color.DarkKhaki,	Color.DarkGray,
			Color.LimeGreen, Color.Tomato,	Color.SteelBlue,	Color.SkyBlue,

			Color.Silver,	Color.Salmon,	Color.SaddleBrown,	Color.RosyBrown,
            Color.PowderBlue, Color.Plum,	Color.PapayaWhip,	Color.Orange
		};

        // blob counter
        private BlobCounter blobCounter = new BlobCounter( );

        /// <summary>
        /// Colors used to color the binary image.
        /// </summary>
        public static Color[] ColorTable
        {
            get { return colorTable; }
            set { colorTable = value; }
        }

        /// <summary>
        /// Specifies if blobs should be filtered.
        /// </summary>
        /// 
        /// <remarks><para>See documentation for <see cref="BlobCounterBase.FilterBlobs"/> property
        /// of <see cref="BlobCounterBase"/> class for more information.</para></remarks>
        /// 
        public bool FilterBlobs
        {
            get { return blobCounter.FilterBlobs; }
            set { blobCounter.FilterBlobs = value; }
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
        /// Objects count.
        /// </summary>
        /// 
        /// <remarks>The amount of objects found in the last processed image.</remarks>
        /// 
        public int ObjectCount
        {
            get { return blobCounter.ObjectsCount; }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // process the image
            blobCounter.ProcessImage( sourceData );

            // get object labels
            int[] labels = blobCounter.ObjectLabels;

            // get width and height
            int width = sourceData.Width;
            int height = sourceData.Height;

            int dstOffset = destinationData.Stride - width * 3;

            // do the job
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            int p = 0;

            // for each row
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, dst += 3, p++ )
                {
                    if ( labels[p] != 0 )
                    {
                        Color c = colorTable[( labels[p] - 1 ) % colorTable.Length];

                        dst[RGB.R] = c.R;
                        dst[RGB.G] = c.G;
                        dst[RGB.B] = c.B;
                    }
                }
                dst += dstOffset;
            }
        }
    }
}
