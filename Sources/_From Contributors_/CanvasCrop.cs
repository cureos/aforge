/*
 * Created by: Volodymyr Goncharov aka avov
 * mail-to: volodymyr.goncharov@gmail.com
 * Created: 5 may 2007 y.
 */

using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters.Transform
{
    /// <summary>
    /// Filter, which retains canvas at specified Region and
    /// fills cut off area with RGBColor or GrayColor
    /// </summary>
    public class CanvasCrop : FilterAnyToAny
    {
        private Color _rGBColor = Color.White;
        private byte _grayColor = 0;
        private Rectangle _region;

        /// <summary>
        /// Constructor
        /// Values by default:
        /// RGBColor = Color.White;
        /// GrayColor = 0;
        /// </summary>
        /// <param name="_region">Region which remains after crop</param>
        public CanvasCrop(Rectangle _region)
        {
            this._region = _region;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_rGBColor">Color for filling in case of RGB image</param>
        /// <param name="_region">Region which remains after crop</param>
        public CanvasCrop(Color _rGBColor, Rectangle _region)
        {
            this._rGBColor = _rGBColor;
            this._region = _region;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_grayColor">Color for filling in case of gray image</param>
        /// <param name="_region">Region which remains after crop</param>
        public CanvasCrop(byte _grayColor, Rectangle _region)
        {
            this._grayColor = _grayColor;
            this._region = _region;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_rGBColor">Color for filling in case of RGB image</param>
        /// <param name="_grayColor">Color for filling in case of gray image</param>
        /// <param name="_region">Region which remains after crop</param>
        public CanvasCrop(Color _rGBColor, byte _grayColor, Rectangle _region)
        {
            this._rGBColor = _rGBColor;
            this._grayColor = _grayColor;
            this._region = _region;
        }

        /// <summary>
        /// Color for filling in case of RGB image
        /// </summary>
        public Color RGBColor
        {
            get
            {
                return _rGBColor;
            }
            set
            {
                _rGBColor = value;
            }
        }

        /// <summary>
        /// Color for filling in case of gray image
        /// </summary>
        public byte GrayColor
        {
            get
            {
                return _grayColor;
            }
            set
            {
                _grayColor = value;
            }
        }

        /// <summary>
        /// Region which remains after crop
        /// </summary>
        public Rectangle Region
        {
            get
            {
                return _region;
            }
            set
            {
                _region = value;
            }
        }

        protected override unsafe void ProcessFilter(BitmapData imageData)
        {
            // get image width and height
            int width = imageData.Width;
            int height = imageData.Height;

            byte* src = (byte*) imageData.Scan0.ToPointer();
            byte* pixel;
            
            int pixelSize = (imageData.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int yScale = imageData.Stride;
            int xScale = pixelSize;

            for (int y = 0; y < height; y++, src += yScale)
            {
                pixel = src;
                
                for (int x = 0; x < width; x++, pixel += xScale)
                {
                    if(!_region.Contains(x, y)) // pixel for cutting off
                    {
                        // fills pixel
                        int color = (pixelSize == 1) ? _grayColor : _rGBColor.ToArgb();
                        byte* data = pixel;
                        for (int i = 0; i < pixelSize; i++, data++)
                        {
                            *data = (byte) ((color >> i*8) & 255);
                        }
                    }
                }
            }
        }
    }
}