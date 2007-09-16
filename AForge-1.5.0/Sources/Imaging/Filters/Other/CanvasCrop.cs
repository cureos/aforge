// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Volodymyr Goncharov, 2007
// volodymyr.goncharov@gmail.com
//
// Andrew Kirillov
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Fill areas outiside of specified region.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter fills areas outside of specified region using the specified color.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CanvasCrop filter = new CanvasCrop( new Rectangle(
    ///                         5, 5, image.Width - 10, image.Height - 10 ), Color.Red );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="canvas_crop.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class CanvasCrop : FilterAnyToAny
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
        /// <remarks>The color is used to fill areas out of specified region in color images. Default value
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
        /// <remarks>The color is used to fill areas out of specified region in grayscale images. Default value
        /// is white - 255.</remarks>
        /// 
        public byte FillColorGray
        {
            get { return fillGray; }
            set { fillGray = value; }
        }

        /// <summary>
        /// Region to keep.
        /// </summary>
        /// 
        /// <remarks>Pixel inside of the specified region will keep their value, but
        /// pixels outside of the region will be filled with specified color.</remarks>
        /// 
        public Rectangle Region
        {
            get { return region; }
            set { region = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// 
        public CanvasCrop( Rectangle  region )
        {
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas outside of specified region in color images.</param>
        /// 
        public CanvasCrop( Rectangle region, Color fillColorRGB )
        {
            this.region     = region;
            this.fillRed    = fillColorRGB.R;
            this.fillGreen  = fillColorRGB.G;
            this.fillBlue   = fillColorRGB.B;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas outside of specified region in grayscale images.</param>
        /// 
        public CanvasCrop( Rectangle region, byte fillColorGray )
        {
            this.region = region;
            this.fillGray = fillColorGray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasCrop"/> class.
        /// </summary>
        /// 
        /// <param name="region">Region to keep.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas outside of specified region in color images.</param>
        /// <param name="fillColorGray">Gray color to use for filling areas outside of specified region in grayscale images.</param>
        /// 
        public CanvasCrop( Rectangle region, Color fillColorRGB, byte fillColorGray )
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
			int offset = imageData.Stride - ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );

            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr++ )
                    {
                        if ( !region.Contains( x, y ) )
                        {
                            *ptr = fillGray;
                        }
                    }
                    ptr += offset;
                }
            }
            else
            {
                // color image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr += 3 )
                    {
                        if ( !region.Contains( x, y ) )
                        {
                            // red
                            ptr[RGB.R] = fillRed;
                            // green
                            ptr[RGB.G] = fillGreen;
                            // blue
                            ptr[RGB.B] = fillBlue;
                        }
                    }
                    ptr += offset;
                }
            }
        }
    }
}