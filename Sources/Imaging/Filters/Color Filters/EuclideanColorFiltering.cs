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
    /// Euclidean color filtering.
    /// </summary>
    /// 
    /// <remarks><para>The filter filters pixels, which color is inside/outside
    /// of RGB sphere with specified center and radius - it keeps pixels with
    /// colors inside/outside of the specified sphere and fills the rest with
    /// <see cref="FillColor">specified color</see>.</para>
    /// 
    /// <para>The filter accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// EuclideanColorFiltering filter = new EuclideanColorFiltering( );
    /// // set center colol and radius
    /// filter.CenterColor = Color.FromArgb( 215, 30, 30 );
    /// filter.Radius = 100;
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    ///
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/euclidean_filtering.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    /// <seealso cref="ColorFiltering"/>
    /// 
    public class EuclideanColorFiltering : BaseInPlacePartialFilter
    {
        private short radius = 100;
        private Color center = Color.FromArgb( 255, 255, 255 );
        private Color fill   = Color.FromArgb( 0, 0, 0 );
        private bool fillOutside = true;

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
        /// RGB sphere's radius, [0, 450].
        /// </summary>
        /// 
        /// <remarks>Default value is 100.</remarks>
        /// 
        public short Radius
        {
            get { return radius; }
            set
            {
                radius = System.Math.Max( (short) 0, System.Math.Min( (short) 450, value ) );
            }
        }

        /// <summary>
        /// RGB sphere's center.
        /// </summary>
        /// 
        /// <remarks>Default value is (255, 255, 255) - white color.</remarks>
        /// 
        public Color CenterColor
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Fill color used to fill filtered pixels.
        /// </summary>
        public Color FillColor
        {
            get { return fill; }
            set { fill = value; }
        }

        /// <summary>
        /// Determines, if pixels should be filled inside or outside specified
        /// RGB sphere.
        /// </summary>
        public bool FillOutside
        {
            get { return fillOutside; }
            set { fillOutside = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanColorFiltering"/> class.
        /// </summary>
        /// 
        public EuclideanColorFiltering()
        {
            formatTransalations[PixelFormat.Format24bppRgb]  = PixelFormat.Format24bppRgb;
            formatTransalations[PixelFormat.Format32bppRgb]  = PixelFormat.Format32bppRgb;
            formatTransalations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanColorFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="center">RGB sphere's center.</param>
        /// <param name="radius">RGB sphere's radius.</param>
        /// 
        public EuclideanColorFiltering( Color center, short radius ) :
            this( )
        {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image, Rectangle rect )
        {
            // get pixel size
            int pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;

            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = image.Stride - rect.Width * pixelSize;

            byte r, g, b;
            // sphere's center
            byte cR = center.R;
            byte cG = center.G;
            byte cB = center.B;
            // fill color
            byte fR = fill.R;
            byte fG = fill.G;
            byte fB = fill.B;

            // do the job
            byte* ptr = (byte*) image.ImageData.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * image.Stride + startX * pixelSize );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += pixelSize )
                {
                    r = ptr[RGB.R];
                    g = ptr[RGB.G];
                    b = ptr[RGB.B];

                    // calculate the distance
                    if ( (int) Math.Sqrt(
                        Math.Pow( (int) r - (int) cR, 2 ) +
                        Math.Pow( (int) g - (int) cG, 2 ) +
                        Math.Pow( (int) b - (int) cB, 2 ) ) <= radius )
                    {
                        // inside sphere
                        if ( !fillOutside )
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;
                        }
                    }
                    else
                    {
                        // outside sphere
                        if ( fillOutside )
                        {
                            ptr[RGB.R] = fR;
                            ptr[RGB.G] = fG;
                            ptr[RGB.B] = fB;
                        }
                    }
                }
                ptr += offset;
            }
        }
    }
}
