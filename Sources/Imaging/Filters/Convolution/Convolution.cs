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
	/// Convolution filter.
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
    public class Convolution : FilterAnyToAnyUsingCopyPartial
	{
        // convolution kernel
        private int[,] kernel;
        // division factor
        private int divisor = 1;
        // kernel size
        private int size;
        // use dynamic divisor for edges
        private bool dynamicDivisorForEdges = true;

        /// <summary>
        /// Convolution kernel.
        /// </summary>
        /// 
        /// <remarks>
        /// <para><note>Convolution kernel must be square and its width/height
        /// should be odd and should be in the range [3, 25].</note></para>
        /// 
        /// <para><note>Setting convolution kernel through this property does not
        /// affect <see cref="Divisor"/> - it is not recalculated automatically.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified.</exception>
        /// 
        public int[,] Kernel
        {
            get { return kernel; }
            set
            {
                int s = value.GetLength( 0 );

                // check kernel size
                if ( ( s != value.GetLength( 1 ) ) || ( s < 3 ) || ( s > 25 ) || ( s % 2 == 0 ) )
                    throw new ArgumentException( "Invalid kernel size" );

                this.kernel = value;
                this.size = s;
            }
        }

        /// <summary>
        /// Division factor.
        /// </summary>
        /// 
        /// <remarks><para>The value is used to divide convolution - weighted sum
        /// of pixels is divided by this value.</para>
        /// 
        /// <para><note>The value may calculated automatically in the case if constructor
        /// with one parameter is used (<see cref="Convolution( int[,] )"/>).</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Divisor can not be equal to zero.</exception>
        /// 
        public int Divisor
        {
            get { return divisor; }
            set
            {
                if ( value == 0 )
                    throw new ArgumentException( "Divisor can not be equal to zero" );
                divisor = value;
            }
        }

        /// <summary>
        /// Use dynamic divisor for edges or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies how to handle edges. If it is set to
        /// <b>false</b>, then the same divisor (which is specified by <see cref="Divisor"/>
        /// property or calculated automatically) will be applied both for non-edge regions
        /// and for edge regions. If the value is set to <b>true</b>, then dynamically
        /// calculated divisor will be used for edge regions, which is sum of those kernel
        /// elements, which are taken into account for particular processed image
        /// (are not outside image).</para>
        /// 
        /// <para>Default value is set to <b>true</b>.</para>
        /// </remarks>
        /// 
        public bool DynamicDivisorForEdges
        {
            get { return dynamicDivisorForEdges; }
            set { dynamicDivisorForEdges = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution"/> class.
        /// </summary>
        /// 
        /// <param name="kernel">Convolution kernel.</param>
        /// 
        /// <remarks><para>Using this constructor (specifying only convolution kernel),
        /// <see cref="Divisor">division factor</see> will be calculated automatically
        /// summing all kernel values. In the case if kernel's sum equals to zero,
        /// division factor will be assigned to 1.</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified. Kernel must be
        /// square, its width/height should be odd and should be in the range [3, 25].</exception>
        /// 
        public Convolution( int[,] kernel ) :
            this( kernel, 1 )
        {
            divisor = 0;

            // calculate divisor
            for ( int i = 0, n = kernel.GetLength( 0 ); i < n; i++ )
            {
                for ( int j = 0, k = kernel.GetLength( 1 ); j < k; j++ )
                {
                    divisor += kernel[i, j];
                }
            }
            if ( divisor == 0 )
                divisor = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Convolution"/> class.
        /// </summary>
        /// 
        /// <param name="kernel">Convolution kernel.</param>
        /// <param name="divisor">Divisor, used used to divide wheighted sum.</param>
        /// 
        /// <exception cref="ArgumentException">Invalid kernel size is specified. Kernel must be
        /// square, its width/height should be odd and should be in the range [3, 25].</exception>
        /// <exception cref="ArgumentException">Divisor can not be equal to zero.</exception>
        /// 
        public Convolution( int[,] kernel, int divisor )
        {
            Kernel = kernel;
            Divisor = divisor;
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
            int pixelSize = ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;

            // processing start and stop X,Y positions
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;

            int stride = destinationData.Stride;
            int offset = stride - rect.Width * pixelSize;

            // loop and array indexes
            int i, j, t, k, ir, jr;
            // kernel's radius
            int radius = size >> 1;
            // color sums
            long r, g, b, div;

            // kernel size
            int kernelSize = size * size;
            // number of kernel elements taken into account
            int processedKernelSize;

            byte* src = (byte*) sourceData.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer( );
            byte* p;

            // allign pointers to the first pixel to process
            src += ( startY * stride + startX * pixelSize );
            dst += ( startY * stride + startX * pixelSize );

            // do the processing job
            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src++, dst++ )
                    {
                        g = div = processedKernelSize = 0;

                        // for each kernel row
                        for ( i = 0; i < size; i++ )
                        {
                            ir = i - radius;
                            t = y + ir;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = 0; j < size; j++ )
                            {
                                jr = j - radius;
                                t = x + jr;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( t < stopX )
                                {
                                    k = kernel[i, j];

                                    div += k;
                                    g += k * src[ir * stride + jr];
                                    processedKernelSize++;
                                }
                            }
                        }

                        // check if all kernel elements were processed
                        if ( processedKernelSize == kernelSize )
                        {
                            // all kernel elements are processed - we are not on the edge
                            div = divisor;
                        }
                        else
                        {
                            // we are on edge. do we need to use dynamic divisor or not?
                            if ( !dynamicDivisorForEdges )
                            {
                                // do
                                div = divisor;
                            }
                        }

                        // check divider
                        if ( div != 0 )
                        {
                            g /= div;
                        }
                        *dst = ( g > 255 ) ? (byte) 255 : ( ( g < 0 ) ? (byte) 0 : (byte) g );
                    }
                    src += offset;
                    dst += offset;
                }
            }
            else
            {
                // RGB image

                // for each line
                for ( int y = startY; y < stopY; y++ )
                {
                    // for each pixel
                    for ( int x = startX; x < stopX; x++, src += 3, dst += 3 )
                    {
                        r = g = b = div = processedKernelSize = 0;

                        // for each kernel row
                        for ( i = 0; i < size; i++ )
                        {
                            ir = i - radius;
                            t = y + ir;

                            // skip row
                            if ( t < startY )
                                continue;
                            // break
                            if ( t >= stopY )
                                break;

                            // for each kernel column
                            for ( j = 0; j < size; j++ )
                            {
                                jr = j - radius;
                                t = x + jr;

                                // skip column
                                if ( t < startX )
                                    continue;

                                if ( t < stopX )
                                {
                                    k = kernel[i, j];
                                    p = &src[ir * stride + jr * 3];

                                    div += k;

                                    r += k * p[RGB.R];
                                    g += k * p[RGB.G];
                                    b += k * p[RGB.B];

                                    processedKernelSize++;
                                }
                            }
                        }

                        // check if all kernel elements were processed
                        if ( processedKernelSize == kernelSize )
                        {
                            // all kernel elements are processed - we are not on the edge
                            div = divisor;
                        }
                        else
                        {
                            // we are on edge. do we need to use dynamic divisor or not?
                            if ( !dynamicDivisorForEdges )
                            {
                                // do
                                div = divisor;
                            }
                        }

                        // check divider
                        if ( div != 0 )
                        {
                            r /= div;
                            g /= div;
                            b /= div;
                        }
                        dst[RGB.R] = ( r > 255 ) ? (byte) 255 : ( ( r < 0 ) ? (byte) 0 : (byte) r );
                        dst[RGB.G] = ( g > 255 ) ? (byte) 255 : ( ( g < 0 ) ? (byte) 0 : (byte) g );
                        dst[RGB.B] = ( b > 255 ) ? (byte) 255 : ( ( b < 0 ) ? (byte) 0 : (byte) b );
                    }
                    src += offset;
                    dst += offset;
                }
            }
        }
	}
}
