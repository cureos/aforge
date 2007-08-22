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
    using AForge;

    /// <summary>
    /// Channels filters.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class ChannelFiltering : FilterColorToColorPartial
    {
        private IntRange red = new IntRange( 0, 255 );
        private IntRange green = new IntRange( 0, 255 );
        private IntRange blue = new IntRange( 0, 255 );

        private byte fillR = 0;
        private byte fillG = 0;
        private byte fillB = 0;

        private bool redFillOutsideRange = true;
        private bool greenFillOutsideRange = true;
        private bool blueFillOutsideRange = true;

        private byte[] mapRed = new byte[256];
        private byte[] mapGreen = new byte[256];
        private byte[] mapBlue = new byte[256];

        #region Public properties

        /// <summary>
        /// Red channel's range.
        /// </summary>
        public IntRange Red
        {
            get { return red; }
            set
            {
                red = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Red fill value.
        /// </summary>
        public byte FillRed
        {
            get { return fillR; }
            set
            {
                fillR = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Green channel's range.
        /// </summary>
        public IntRange Green
        {
            get { return green; }
            set
            {
                green = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Green fill value.
        /// </summary>
        public byte FillGreen
        {
            get { return fillG; }
            set
            {
                fillG = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Blue channel's range.
        /// </summary>
        public IntRange Blue
        {
            get { return blue; }
            set
            {
                blue = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        /// <summary>
        /// Blue fill value.
        /// </summary>
        public byte FillBlue
        {
            get { return fillB; }
            set
            {
                fillB = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        /// <summary>
        /// Determines, if red channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is <b>true</b>.</remarks>
        /// 
        public bool RedFillOutsideRange
        {
            get { return redFillOutsideRange; }
            set
            {
                redFillOutsideRange = value;
                CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            }
        }

        /// <summary>
        /// Determines, if green channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is <b>true</b>.</remarks>
        /// 
        public bool GreenFillOutsideRange
        {
            get { return greenFillOutsideRange; }
            set
            {
                greenFillOutsideRange = value;
                CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            }
        }

        /// <summary>
        /// Determines, if blue channel should be filled inside or outside filtering range.
        /// </summary>
        /// 
        /// <remarks>Default value is <b>true</b>.</remarks>
        ///
        public bool BlueFillOutsideRange
        {
            get { return blueFillOutsideRange; }
            set
            {
                blueFillOutsideRange = value;
                CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
            }
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFiltering"/> class.
        /// </summary>
        /// 
        public ChannelFiltering( )
        {
            CalculateMap( red, fillR, redFillOutsideRange, mapRed );
            CalculateMap( green, fillG, greenFillOutsideRange, mapGreen );
            CalculateMap( blue, fillB, blueFillOutsideRange, mapBlue );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelFiltering"/> class.
        /// </summary>
        /// 
        /// <param name="red">Red channel's filtering range.</param>
        /// <param name="green">Green channel's filtering range.</param>
        /// <param name="blue">Blue channel's filtering range.</param>
        /// 
        public ChannelFiltering( IntRange red, IntRange green, IntRange blue )
        {
            Red = red;
            Green = green;
            Blue = blue;
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
                    // red
                    ptr[RGB.R] = mapRed[ptr[RGB.R]];
                    // green
                    ptr[RGB.G] = mapGreen[ptr[RGB.G]];
                    // blue
                    ptr[RGB.B] = mapBlue[ptr[RGB.B]];
                }
                ptr += offset;
            }
        }

        /// <summary>
        /// Calculate filtering map.
        /// </summary>
        /// 
        /// <param name="range">Filtering range.</param>
        /// <param name="fill">Fillter value.</param>
        /// <param name="fillOutsideRange">Fill outside or inside the range.</param>
        /// <param name="map">Filtering map.</param>
        /// 
        private void CalculateMap( IntRange range, byte fill, bool fillOutsideRange, byte[] map )
        {
            for ( int i = 0; i < 256; i++ )
            {
                if ( ( i >= range.Min ) && ( i <= range.Max ) )
                {
                    map[i] = ( fillOutsideRange ) ? (byte) i : fill;
                }
                else
                {
                    map[i] = ( fillOutsideRange ) ? fill : (byte) i;
                }
            }
        }
    }
}
