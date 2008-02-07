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
    /// Filter to mark (highlight) corners of objects.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter highlights corners of objects on the image using provided corners
    /// detection algorithm.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create corner detector's instance
    /// MoravecCornersDetector mcd = new MoravecCornersDetector( );
    /// // create corner maker filter
    /// CornersMarker filter = new CornersMarker( mcd, Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// </remarks>
    /// 
    public class CornersMarker : FilterAnyToAny
    {
        // color used to mark corners
        private Color markerColor = Color.White;
        // algorithm used to detect corners
        private ICornersDetector detector = null;

        /// <summary>
        /// Color used to mark corners.
        /// </summary>
        public Color MarkerColor
        {
            get { return markerColor; }
            set { markerColor = value; }
        }

        /// <summary>
        /// Interface of corners' detection algorithm used to detect corners.
        /// </summary>
        public ICornersDetector Detector
        {
            get { return detector; }
            set { detector = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornersMarker"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Interface of corners' detection algorithm.</param>
        /// 
        public CornersMarker( ICornersDetector detector )
        {
            this.detector = detector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CornersMarker"/> class.
        /// </summary>
        /// 
        /// <param name="detector">Interface of corners' detection algorithm.</param>
        /// <param name="markerColor">Marker's color used to mark corner.</param>
        /// 
        public CornersMarker( ICornersDetector detector, Color markerColor )
        {
            this.detector    = detector;
            this.markerColor = markerColor;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            // get collection of corners
            Point[] corners = detector.ProcessImage( imageData );
            // mark all corners
            foreach ( Point corner in corners )
            {
                Drawing.FillRectangle( imageData, new Rectangle( corner.X - 1, corner.Y - 1, 3, 3 ), markerColor );
            }
        }
    }
}
