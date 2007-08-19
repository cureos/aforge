// AForge Image Processing Library
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
    /// Hit-And-Miss operator from Mathematical Morphology.
    /// </summary>
    /// 
    /// <remarks>
    /// Structuring element contains:
    ///  1 - foreground;
    ///  0 - background;
    /// -1 - don't care.
    /// </remarks>
    /// 
    public class HitAndMiss : FilterGrayToGrayUsingCopy
    {
        /// <summary>
        /// Hit and Miss modes.
        /// </summary>
        public enum Modes
        {
            /// <summary>
            /// Hit and miss mode.
            /// </summary>
            HitAndMiss = 0,

            /// <summary>
            /// Thinning mode.
            /// </summary>
            Thinning = 1,

            /// <summary>
            /// Thickening mode.
            /// </summary>
            Thickening = 2
        }

        // structuring element
        private short[,] se;
        private int size;

        // operation mode
        private Modes mode = Modes.HitAndMiss;

        /// <summary>
        /// Operation mode.
        /// </summary>
        /// 
        /// <remarks>Default mode is <see cref="Modes.HitAndMiss"/>.</remarks>
        /// 
        public Modes Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitAndMiss"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        ///
        public HitAndMiss( short[,] se )
        {
            int s = se.GetLength( 0 );

            // check structuring element size
            if ( ( s != se.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
                throw new ArgumentException( );

            this.se = se;
            this.size = s;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HitAndMiss"/> class.
        /// </summary>
        /// 
        /// <param name="se">Structuring element.</param>
        /// <param name="mode">Operation mode.</param>
        /// 
        public HitAndMiss( short[,] se, Modes mode )
            : this( se )
        {
            this.mode = mode;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Pointer to source image data (first scan line).</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData )
        {
            // get image size
            int width   = destinationData.Width;
            int height  = destinationData.Height;
            int stride  = destinationData.Stride;
            int offset  = stride - width;
            // loop and array indexes
            int ir, jr, i, j;
            // structuring element's radius
            int r = size >> 1;
            // pixel value
            byte dstValue, v;
            // structuring element's value
            short sv;

            // mode values
            byte[] hitValue = new byte[3] { 255, 0, 255 };
            byte[] missValue = new byte[3] { 0, 0, 0 };
            int modeIndex = (int) mode;

            // do the job
            byte* src = (byte*) sourceData.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                // for each pixel
                for ( int x = 0; x < width; x++, src++, dst++ )
                {
                    missValue[1] = missValue[2] = *src;
                    dstValue = 255;

                    // for each structuring element's row
                    for ( i = 0; i < size; i++ )
                    {
                        ir = i - r;

                        // for each structuring element's column
                        for ( j = 0; j < size; j++ )
                        {
                            jr = j - r;

                            // get structuring element's value
                            sv = se[i, j];

                            // skip "don't care" values
                            if ( sv == -1 )
                                continue;

                            // check, if we outside
                            if (
                                ( y + ir < 0 ) || ( y + ir >= height ) ||
                                ( x + jr < 0 ) || ( x + jr >= width )
                                )
                            {
                                // if it so, the result is zero,
                                // because it was required pixel
                                dstValue = 0;
                                break;
                            }

                            // get source image value
                            v = src[ir * stride + jr];

                            if (
                                ( ( sv != 0 ) || ( v != 0 ) ) &&
                                ( ( sv != 1 ) || ( v != 255 ) )
                                )
                            {
                                // failed structuring element mutch
                                dstValue = 0;
                                break;
                            }
                        }

                        if ( dstValue == 0 )
                            break;
                    }
                    // result pixel
                    *dst = ( dstValue == 255 ) ? hitValue[modeIndex] : missValue[modeIndex];
                }
                src += offset;
                dst += offset;
            }
        }
    }
}
