// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Euclidean color filtering.
    /// </summary>
    /// 
    /// <remarks>The filter filters pixel, which color is inside or outside
    /// of RGB sphere with specified center and radius.</remarks>
    /// 
    public class EuclideanColorFiltering : FilterColorToColorPartial
    {
        private short radius = 100;
        private Color center = Color.FromArgb( 255, 255, 255 );
        private Color fill = Color.FromArgb( 0, 0, 0 );
        private bool fillOutside = true;

        /// <summary>
        /// RGB sphere's radius.
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
        /// Fill color.
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
        public EuclideanColorFiltering( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EuclideanColorFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="center">RGB sphere's center.</param>
        /// <param name="radius">RGB sphere's radius.</param>
        /// 
        public EuclideanColorFiltering( Color center, short radius )
        {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = imageData.Stride - rect.Width * 3;

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
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * imageData.Stride + startX * 3 );

            // for each row
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, ptr += 3 )
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
