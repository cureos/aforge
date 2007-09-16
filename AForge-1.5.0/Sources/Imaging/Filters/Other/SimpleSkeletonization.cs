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

    /// <summary>
    /// Simple skeletonization filter.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class SimpleSkeletonization : FilterGrayToGrayUsingCopyPartial
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
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData, Rectangle rect )
        {
            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int stride = destinationData.Stride;
            int offset = stride - rect.Width;

            // get image size
//            int width   = destinationData.Width;
//            int height  = destinationData.Height;
            int start;

            // do the job
            byte* src0 = (byte*) sourceData.ToPointer( );
            byte* dst0 = (byte*) destinationData.Scan0.ToPointer( );
            byte* src = src0;
            byte* dst = dst0;

            // horizontal pass

            // allign pointers to the first pixel to process
            src += ( startY * stride + startX );
            dst += ( startY * stride );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // make destination image filled with background color
                Win32.memset( dst + startX, bg, stopX - startX );
                
                start = -1;
                // for each pixel
                for ( int x = startX; x < stopX; x++, src++ )
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
                    dst[start + ( ( stopX - start ) >> 1 )] = (byte) fg;
                }
                src += offset;
                dst += stride;
            }

            // vertical pass

            // allign pointer to the first line to process
            src0 += ( startY * stride );

            // for each column
            for ( int x = startX; x < stopX; x++ )
            {
                src = src0 + x;
                dst = dst0 + x;

                start = -1;
                // for each row
                for ( int y = startY; y < stopY; y++, src += stride )
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
                    dst[stride * ( start + ( ( stopY - start ) >> 1 ) )] = (byte) fg;
                }
            }
        }
    }
}
