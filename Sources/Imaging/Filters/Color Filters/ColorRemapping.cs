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
    /// Color remapping.
    /// </summary>
    /// 
    /// <remarks><para>The filter allows to remap color of the image. Unlike <see cref="LevelsLinear"/> filter
    /// the filter allow to do non-linear remapping.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create map
    /// byte[] map = new byte[256];
    /// for ( int i = 0; i &lt; 256; i++ )
    /// {
    ///     map[i] = (byte) Math.Min( 255, Math.Pow( 2, (double) i / 32 ) );
    /// }
    /// // create filter
    /// ColorRemapping filter = new ColorRemapping( map, map, map );
    /// // apply the filter
    /// filter.ApplyInPlace( image );
    /// </code>
    /// <para><b>Initial image:</b></para>
    /// <img src="sample1.jpg" width="480" height="361" />
    /// <para><b>Result image:</b></para>
    /// <img src="color_remapping.jpg" width="480" height="361" />
    /// </remarks>
    /// 
    public class ColorRemapping : FilterAnyToAny
    {
        // color maps
        private byte[] redMap;
        private byte[] greenMap;
        private byte[] blueMap;
        private byte[] grayMap;

        /// <summary>
        /// Remapping array for red color plane.
        /// </summary>
        /// 
        /// <remarks>The remapping array should contain 256 remapping values.</remarks>
        /// 
        public byte[] RedMap
        {
            get { return redMap; }
            set
            {
                // check the map
                if ( ( value == null ) || ( value.Length != 256 ) )
                    throw new ArgumentException( "Red map should be array with 256 value" );

                redMap = value;
            }
        }

        /// <summary>
        /// Remapping array for green color plane.
        /// </summary>
        /// 
        /// <remarks>The remapping array should contain 256 remapping values.</remarks>
        /// 
        public byte[] GreenMap
        {
            get { return greenMap; }
            set
            {
                // check the map
                if ( ( value == null ) || ( value.Length != 256 ) )
                    throw new ArgumentException( "Green map should be array with 256 value" );

                greenMap = value;
            }
        }

        /// <summary>
        /// Remapping array for blue color plane.
        /// </summary>
        /// 
        /// <remarks>The remapping array should contain 256 remapping values.</remarks>
        /// 
        public byte[] BlueMap
        {
            get { return blueMap; }
            set
            {
                // check the map
                if ( ( value == null ) || ( value.Length != 256 ) )
                    throw new ArgumentException( "Blue map should be array with 256 value" );

                blueMap = value;
            }
        }

        /// <summary>
        /// Remapping array for gray color.
        /// </summary>
        /// 
        /// <remarks><para>The remapping array should contain 256 remapping values.</para>
        /// <para>The gray map is for grayscale images.</para></remarks>
        /// 
        public byte[] GrayMap
        {
            get { return grayMap; }
            set
            {
                // check the map
                if ( ( value == null ) || ( value.Length != 256 ) )
                    throw new ArgumentException( "Gray map should be array with 256 value" );

                grayMap = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRemapping"/> class.
        /// </summary>
        /// 
        /// <remarks>Initializes the filter without any remapping. Any
        /// pixel value is mapped to the same value.</remarks>
        /// 
        public ColorRemapping( )
        {
            redMap      = new byte[256];
            greenMap    = new byte[256];
            blueMap     = new byte[256];
            grayMap     = new byte[256];

            // fill the maps
            for ( int i = 0; i < 256; i++ )
            {
                redMap[i] = greenMap[i] = blueMap[i] = grayMap[i] = (byte) i;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRemapping"/> class.
        /// </summary>
        /// 
        /// <param name="redMap">Red map.</param>
        /// <param name="greenMap">Green map.</param>
        /// <param name="blueMap">Blue map.</param>
        /// 
        public ColorRemapping( byte[] redMap, byte[] greenMap, byte[] blueMap )
        {
            RedMap      = redMap;
            GreenMap    = greenMap;
            BlueMap     = blueMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRemapping"/> class.
        /// </summary>
        /// 
        /// <param name="grayMap">Gray map.</param>
        /// 
        /// <remarks>This constructor is supposed for grayscale images.</remarks>
        /// 
        public ColorRemapping( byte[] grayMap )
        {
            GrayMap = grayMap;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData )
        {
            int width = imageData.Width;
            int height = imageData.Height;

            int offset = imageData.Stride - ( ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr++ )
                    {
                        // gray
                        *ptr = grayMap[*ptr];
                    }
                    ptr += offset;
                }
            }
            else
            {
                // RGB image
                for ( int y = 0; y < height; y++ )
                {
                    for ( int x = 0; x < width; x++, ptr += 3 )
                    {
                        // red
                        ptr[RGB.R] = redMap[ptr[RGB.R]];
                        // green
                        ptr[RGB.G] = greenMap[ptr[RGB.G]];
                        // blue
                        ptr[RGB.B] = blueMap[ptr[RGB.B]];
                    }
                    ptr += offset;
                }
            }
        }
    }
}
