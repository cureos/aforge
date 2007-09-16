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
    /// Base class for error diffusion dithering.
    /// </summary>
    /// 
    /// <remarks>The class is the base class for binarization algorithms based on error diffusion.</remarks>
    /// 
    public abstract class ErrorDiffusionDithering : FilterGrayToGrayUsingCopyPartial
    {
        /// <summary>
        /// Current processing X coordinate.
        /// </summary>
        protected int x;

        /// <summary>
        /// Current processing Y coordinate.
        /// </summary>
        protected int y;

        /// <summary>
        /// Processing X start position.
        /// </summary>
        protected int startX;

        /// <summary>
        /// Processing Y start position.
        /// </summary>
        protected int startY;

        /// <summary>
        /// Processing X stop position.
        /// </summary>
        protected int stopX;

        /// <summary>
        /// Processing Y stop position.
        /// </summary>
        protected int stopY;

        /// <summary>
        /// Processing X stop position minus 1.
        /// </summary>
        protected int stopXM1;

        /// <summary>
        /// Processing Y stop position minus 1. 
        /// </summary>
        protected int stopYM1;

        /// <summary>
        /// Processing image's stride (line size).
        /// </summary>
        protected int stride;

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="error">Current error value.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized in protected members.</remarks>
        /// 
        protected abstract unsafe void Diffuse( int error, byte* ptr );

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
            startX  = rect.Left;
            startY  = rect.Top;
            stopX   = startX + rect.Width;
            stopY   = startY + rect.Height;
            stopXM1 = stopX - 1;
            stopYM1 = stopY - 1;
            stride  = destinationData.Stride;

            int offset = stride - rect.Width;

            // allocate memory for source copy
            IntPtr sourceCopy = Win32.LocalAlloc( Win32.MemoryFlags.Fixed, destinationData.Stride * destinationData.Height );

            // copy source image
            Win32.memcpy( sourceCopy, sourceData, destinationData.Stride * destinationData.Height );

            int v, error;

            // do the job
            byte* src = (byte*) sourceCopy.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );

            // allign pointers to the first pixel to process
            src += ( startY * stride + startX );
            dst += ( startY * stride + startX );

            // for each line
            for ( y = startY; y < stopY; y++ )
            {
                // for each pixels
                for ( x = startX; x < stopX; x++, src++, dst++ )
                {
                    v = *src;

                    // fill the next destination pixel
                    if ( v < 128 )
                    {
                        *dst = 0;
                        error = v;
                    }
                    else
                    {
                        *dst = 255;
                        error = v - 255;
                    }

                    // do error diffusion
                    Diffuse( error, src );
                }
                src += offset;
                dst += offset;
            }

            // free memory for image copy
            Win32.LocalFree( sourceCopy );
        }
    }
}
