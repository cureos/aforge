// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Volodymyr Goncharov, 2007
// volodymyr.goncharov@gmail.com
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging.Filters
{
    using System.Drawing;
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    /// <summary>
    /// Move canvas to the specified point.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The filter moves canvas to the specified area filling unused empty areas with specified color.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create filter
    /// CanvasMove filter = new CanvasMove( new Point( -50, -50 ), Color.Green );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// 
    /// <para><b>Initial image:</b></para>
    /// <img src="img/imaging/sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="img/imaging/canvas_move.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class CanvasMove : BaseInPlaceFilter
    {
        // RGB fill color
        private byte fillRed   = 255;
        private byte fillGreen = 255;
        private byte fillBlue  = 255;
        // gray fill color
        private byte fillGray = 255;
        // point to move to
        private Point movePoint;

        // private format translation dictionary
        private Dictionary<PixelFormat, PixelFormat> formatTransalations = new Dictionary<PixelFormat, PixelFormat>( );

        /// <summary>
        /// Format translations dictionary.
        /// </summary>
        /// 
        /// <remarks><para>See <see cref="IFilterInformation.FormatTransalations"/>
        /// documentation for additional information.</para></remarks>
        /// 
        public override Dictionary<PixelFormat, PixelFormat> FormatTransalations
        {
            get { return formatTransalations; }
        }

        /// <summary>
        /// RGB fill color.
        /// </summary>
        /// 
        /// <remarks><para>The color is used to fill empty areas in color images.</para>
        /// 
        /// <para>Default value is set to white - RGB(255, 255, 255).</para></remarks>
        /// 
        public Color FillColorRGB
        {
            get { return Color.FromArgb( fillRed, fillGreen, fillBlue ); }
            set
            {
                fillRed = value.R;
                fillGreen = value.G;
                fillBlue = value.B;
            }
        }

        /// <summary>
        /// Gray fill color.
        /// </summary>
        /// 
        /// <remarks><para>The color is used to fill empty areas in grayscale images.</para>
        /// 
        /// <para>Default value is set to white - 255.</para></remarks>
        ///
        public byte FillColorGray
        {
            get { return fillGray; }
            set { fillGray = value; }
        }

        /// <summary>
        /// Point to move the canvas to.
        /// </summary>
        /// 
        public Point MovePoint
        {
            get { return movePoint; }
            set { movePoint = value; }
        }

        // Private constructor to do common initialization
        private CanvasMove( )
        {
            formatTransalations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTransalations[PixelFormat.Format24bppRgb]    = PixelFormat.Format24bppRgb;
            formatTransalations[PixelFormat.Format32bppArgb]   = PixelFormat.Format32bppArgb;
            formatTransalations[PixelFormat.Format32bppRgb]    = PixelFormat.Format32bppRgb;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasMove"/> class.
        /// </summary>
        /// 
        /// <param name="movePoint">Point to move the canvas to.</param>
        /// 
        public CanvasMove( Point movePoint ) : this( )
        {
            this.movePoint = movePoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasMove"/> class.
        /// </summary>
        /// 
        /// <param name="movePoint">Point to move the canvas.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas empty areas in color images.</param>
        /// 
        public CanvasMove( Point movePoint, Color fillColorRGB )
            : this( )
        {
            this.movePoint  = movePoint;
            this.fillRed    = fillColorRGB.R;
            this.fillGreen  = fillColorRGB.G;
            this.fillBlue   = fillColorRGB.B;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasMove"/> class.
        /// </summary>
        /// 
        /// <param name="movePoint">Point to move the canvas.</param>
        /// <param name="fillColorGray">Gray color to use for filling empty areas in grayscale images.</param>
        /// 
        public CanvasMove( Point movePoint, byte fillColorGray )
            : this( )
        {
            this.movePoint = movePoint;
            this.fillGray = fillColorGray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasMove"/> class.
        /// </summary>
        /// 
        /// <param name="movePoint">Point to move the canvas.</param>
        /// <param name="fillColorRGB">RGB color to use for filling areas empty areas in color images.</param>
        /// <param name="fillColorGray">Gray color to use for filling empty areas in grayscale images.</param>
        /// 
        public CanvasMove( Point movePoint, Color fillColorRGB, byte fillColorGray )
            : this( )
        {
            this.movePoint  = movePoint;
            this.fillRed    = fillColorRGB.R;
            this.fillGreen  = fillColorRGB.G;
            this.fillBlue   = fillColorRGB.B;
            this.fillGray   = fillColorGray;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="image">Source image data.</param>
        ///
        protected override unsafe void ProcessFilter( UnmanagedImage image )
        {
            int pixelSize = Image.GetPixelFormatSize( image.PixelFormat ) / 8;

            // get image width and height
            int width  = image.Width;
            int height = image.Height;
            int stride = image.Stride;

            int movePointX = movePoint.X;
            int movePointY = movePoint.Y;

            // intersection rectangle
            Rectangle intersect = Rectangle.Intersect(
                new Rectangle( 0, 0, width, height ),
                new Rectangle( movePointX, movePointY, width, height ) );

            // start, stop and step for X adn Y
            int yStart  = 0;
            int yStop   = height;
            int yStep   = 1;
            int xStart  = 0;
            int xStop   = width;
            int xStep   = 1;

            if ( movePointY > 0 )
            {
                yStart  = height - 1;
                yStop   = -1;
                yStep   = -1;
            }
            if ( movePointX > 0 )
            {
                xStart = width - 1;
                xStop = -1;
                xStep = -1;
            }

            // do the job
            byte* src = (byte*) image.ImageData.ToPointer( );
            byte* pixel, moved;

            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int y = yStart; y != yStop; y += yStep )
                {
                    for ( int x = xStart; x != xStop; x += xStep )
                    {
                        // current pixel
                        pixel = src + y * stride + x;

                        if ( intersect.Contains( x, y ) )
                        {
                            moved = src + ( y - movePointY ) * stride + ( x - movePointX );

                            *pixel = *moved;
                        }
                        else
                        {
                            *pixel = fillGray;
                        }
                    }
                }
            }
            else
            {
                // color image
                for ( int y = yStart; y != yStop; y += yStep )
                {
                    for ( int x = xStart; x != xStop; x += xStep )
                    {
                        // current pixel
                        pixel = src + y * stride + x * pixelSize;

                        if ( intersect.Contains( x, y ) )
                        {
                            moved = src + ( y - movePointY ) * stride + ( x - movePointX ) * pixelSize;

                            pixel[RGB.R] = moved[RGB.R];
                            pixel[RGB.G] = moved[RGB.G];
                            pixel[RGB.B] = moved[RGB.B];
                        }
                        else
                        {
                            pixel[RGB.R] = fillRed;
                            pixel[RGB.G] = fillGreen;
                            pixel[RGB.B] = fillBlue;
                        }
                    }
                }
            }
        }
    }
}