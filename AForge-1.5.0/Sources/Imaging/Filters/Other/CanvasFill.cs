// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fill areas iniside of specified region.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter fills areas inside of specified region using the specified color.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CanvasFill filter = new CanvasFill( new Rectangle(
    ///                         5, 5, image.Width - 10, image.Height - 10 ), Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// </remarks>
    /// 
    public class CanvasFill : FilterAnyToAny
    {
        // RGB fill color
        private byte fillRed = 255;
        private byte fillGreen = 255;
        private byte fillBlue = 255;
        // gray fill color
        private byte fillGray = 255;
        // region to keep
        private Rectangle region;

        /// <summary>
        /// RGB fill color.
        /// </summary>
        /// 
        /// <remarks>The color is used to fill areas inside of specified region in color images. Default value
        /// is white - RGB(255, 255, 255).</remarks>
        /// 
        public Color FillColorRGB
        {
            get { return Color.FromArgb( fillRed, fillGreen, fillBlue ); }
            set
            {
                fillRed     = value.R;
                fillGreen   = value.G;
                fillBlue    = value.B;
            }
        }

        /// <summary>
        /// Gray fill color.
        /// </summary>
        /// 
        /// <remarks>The color is used to fill areas inside of specified region in grayscale images. Default value
        /// is white - 255.</remarks>
        /// 
        public byte FillColorGray
        {
            get { return fillGray; }
            set { fillGray = value; }
        }

        /// <summary>
        /// Region to fill.
        /// </summary>
        /// 
        /// <remarks>Pixel inside of the specified region will be filled with specified color.</remarks>
        /// 
        public Rectangle Region
        {
            get { return region; }
            set { region = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// 
        public CanvasFill( Rectangle  region )
        {
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas inside of specified region in color images.</param>
        /// 
        public CanvasFill( Rectangle region, Color fillColorRGB )
        {
            this.region     = region;
            this.fillRed    = fillColorRGB.R;
            this.fillGreen  = fillColorRGB.G;
            this.fillBlue   = fillColorRGB.B;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas inside of specified region in grayscale images.</param>
        /// 
        public CanvasFill( Rectangle region, byte fillColorGray )
        {
            this.region = region;
            this.fillGray = fillColorGray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasFill"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to fill.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas inside of specified region in color images.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas inside of specified region in grayscale images.</param>
        /// 
        public CanvasFill( Rectangle region, Color fillColorRGB, byte fillColorGray )
        {
            this.region     = region;
            this.fillRed    = fillColorRGB.R;
            this.fillGreen  = fillColorRGB.G;
            this.fillBlue   = fillColorRGB.B;
            this.fillGray   = fillColorGray;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            // get image width and height
            int width = imageData.Width;
            int height = imageData.Height;

            // start (X, Y) point of filling region
            int startX = Math.Max( 0, region.X );
            int startY = Math.Max( 0, region.Y );

            // check if there is nothing to do
            if ( ( startX >= width ) || ( startY >= height ) )
                return;

            // stop (X, Y) point of filling region
            int stopX = Math.Min( width, startX + region.Width - ( startX - region.X ) );
            int stopY = Math.Min( height, startY + region.Height - ( startY - region.Y ) );

            // check if there is nothing to do
            if ( ( stopX <= startX ) || ( stopY <= startY ) )
                return;

            int stride = imageData.Stride;
            int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( ) + startY * stride + startX * pixelSize;

            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                int fillWidth = stopX - startX;

                for ( int y = startY; y < stopY; y++ )
                {
                    AForge.Win32.memset( ptr, fillGray, fillWidth );
                    ptr += stride;
                }
            }
            else
            {
                // color image
                int offset = stride - ( stopX - startX ) * pixelSize;

                for ( int y = startY; y < stopY; y++ )
                {
                    for ( int x = startX; x < stopX; x++, ptr += 3 )
                    {
                        ptr[RGB.R] = fillRed;
                        ptr[RGB.G] = fillGreen;
                        ptr[RGB.B] = fillBlue;
                    }
                    ptr += offset;
                }
            }
        }
    }
}
