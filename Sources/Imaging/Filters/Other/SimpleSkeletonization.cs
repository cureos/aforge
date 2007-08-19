// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Simple skeletonization filter.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class SimpleSkeletonization : FilterGrayToGrayUsingCopy
    {
        private byte bg = 0;
        private byte fg = 255;

        /// <summary>
        /// Background pixel color.
        /// </summary>
        /// 
        /// <remarks>Defalt value is 0 - black.</remarks>
        /// 
        public byte Background
        {
            get { return bg; }
            set { bg = value; }
        }

        /// <summary>
        /// Foreground pixel color.
        /// </summary>
        /// 
        /// <remarks>Defalt value is 255 - white.</remarks>
        /// 
        public byte Foreground
        {
            get { return fg; }
            set { fg = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSkeletonization"/> class.
        /// </summary>
        public SimpleSkeletonization( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSkeletonization"/> class.
        /// </summary>
        /// 
        /// <param name="bg">Background pixel color.</param>
        /// <param name="fg">Foreground pixel color.</param>
        /// 
        public SimpleSkeletonization( byte bg, byte fg )
        {
            this.bg = bg;
            this.fg = fg;
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
            int start;

            // make destination image filled with background color
            Win32.memset( destinationData.Scan0, bg, stride * height );

            // do the job
            byte* src0 = (byte*) sourceData.ToPointer( );
            byte* dst0 = (byte*) destinationData.Scan0.ToPointer( );
            byte* src = src0;
            byte* dst = dst0;

            // horizontal pass

            // for each line
            for ( int y = 0; y < height; y++ )
            {
                start = -1;
                // for each pixel
                for ( int x = 0; x < width; x++, src++ )
                {
                    // looking for foreground pixel
                    if ( start == -1 )
                    {
                        if ( *src == fg )
                            start = x;
                        continue;
                    }

                    // looking for non foreground pixel
                    if ( *src != fg )
                    {
                        dst[start + ( ( x - start ) >> 1 )] = (byte) fg;
                        start = -1;
                    }
                }
                if ( start != -1 )
                {
                    dst[start + ( ( width - start ) >> 1 )] = (byte) fg;
                }
                src += offset;
                dst += stride;
            }

            // vertical pass

            // for each column
            for ( int x = 0; x < width; x++ )
            {
                src = src0 + x;
                dst = dst0 + x;

                start = -1;
                // for each row
                for ( int y = 0; y < height; y++, src += stride )
                {
                    // looking for foreground pixel
                    if ( start == -1 )
                    {
                        if ( *src == fg )
                            start = y;
                        continue;
                    }

                    // looking for non foreground pixel
                    if ( *src != fg )
                    {
                        dst[stride * ( start + ( ( y - start ) >> 1 ) )] = (byte) fg;
                        start = -1;
                    }
                }
                if ( start != -1 )
                {
                    dst[stride * ( start + ( ( height - start ) >> 1 ) )] = (byte) fg;
                }
            }
        }
    }
}
