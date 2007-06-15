/*
 * Created by: Volodymyr Goncharov aka avov
 * mail-to: volodymyr.goncharov@gmail.com
 * Created: 1 may 2007 y.
 */

using System.Drawing;
using System.Drawing.Imaging;

namespace AForge.Imaging.Filters
{
    /// <summary>
    /// Filter, which moves canvas to specified point and
    /// fills unused area with RGBColor or GrayColor
    /// </summary>
    public class CanvasMove : FilterAnyToAny
    {
        private Color _rGBColor = Color.White;
        private byte _grayColor = 0;
        private Point _point = new Point(0, 0);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_rGBColor">Color for filling in case of RGB image</param>
        /// <param name="_point">Point to which canvas would be moved</param>
        public CanvasMove(Color _rGBColor, Point _point)
        {
            this._rGBColor = _rGBColor;
            this._point = _point;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_grayColor">Color for filling in case of gray image</param>
        /// <param name="_point">Point to which canvas would be moved</param>
        public CanvasMove(byte _grayColor, Point _point)
        {
            this._grayColor = _grayColor;
            this._point = _point;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_rGBColor">Color for filling in case of RGB image</param>
        /// <param name="_grayColor">Color for filling in case of gray image</param>
        /// <param name="_point">Point to which canvas would be moved</param>
        public CanvasMove(Color _rGBColor, byte _grayColor, Point _point)
        {
            this._rGBColor = _rGBColor;
            this._grayColor = _grayColor;
            this._point = _point;
        }

        /// <summary>
        /// Default constructor
        /// Values by default:
        /// RGBColor = Color.White;
        /// GrayColor = 0;
        /// Point = new Point(0, 0);
        /// </summary>
        public CanvasMove()
        {
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
        /// Point to which canvas would be moved
        /// </summary>
        public Point Point
        {
            get
            {
                return _point;
            }
            set
            {
                _point = value;
            }
        }

        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="imageData">image data</param>
        /// 
        protected override unsafe void ProcessFilter(BitmapData imageData)
        {
            // get image width and height
            int width = imageData.Width;
            int height = imageData.Height;

            Rectangle intersect = Rectangle.Intersect(new Rectangle(0, 0, width, height), new Rectangle(_point.X, _point.Y, width, height));

            byte* src = (byte*) imageData.Scan0.ToPointer();

            int pixelSize = (imageData.PixelFormat == PixelFormat.Format8bppIndexed) ? 1 : 3;
            int yScale = imageData.Stride;
            int xScale = pixelSize;


            for (int y = (_point.Y < 0) ? 0 : height - 1;
                 (_point.Y < 0 && y < height) || (_point.Y >= 0 && y >= 0);
                 y += (_point.Y < 0) ? 1 : -1) // line iteration
            {
                for (int x = (_point.X < 0) ? 0 : width - 1;
                     (_point.X < 0 && x < width) || (_point.X >= 0 && x >= 0);
                     x += (_point.X < 0) ? 1 : -1) // pixel iteration
                {
                    byte* pixel = src + y*yScale + x*xScale;

                    if(intersect.Contains(x, y))
                    { // moving pixel
                        
                        byte* moved = src + (y - _point.Y)*yScale + (x - _point.X)*xScale;

                        for (int i = 0; i < pixelSize; i++, pixel++, moved++)
                        {
                            *pixel = *moved;
                        }
                    }
                    else
                    { // fills with color for replacement
                        
                        int color = (pixelSize == 1) ? _grayColor : _rGBColor.ToArgb();

                        for (int i = 0; i < pixelSize; i++, pixel++)
                        {
                            *pixel = (byte) ((color >> i*8) & 255);
                        }
                    }
                }
            }
        }
    }
}