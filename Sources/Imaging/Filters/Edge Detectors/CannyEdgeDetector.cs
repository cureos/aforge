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
    /// Canny edge detector.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class CannyEdgeDetector : FilterColorToGray
    {
        private IFilter grayscaleFilter = new GrayscaleBT709( );
        private GaussianBlur gaussianFilter = new GaussianBlur( );
        private byte lowThreshold = 20;
        private byte highThreshold = 100;

        // Sobel kernels
        private static int[,] xKernel = new int[,]
		{
			{ -1,  0,  1 },
			{ -2,  0,  2 },
			{ -1,  0,  1 }
		};
        private static int[,] yKernel = new int[,]
		{
			{  1,  2,  1 },
			{  0,  0,  0 },
			{ -1, -2, -1 }
		};

        /// <summary>
        /// Low threshold.
        /// </summary>
        /// 
        public byte LowThreshold
        {
            get { return lowThreshold; }
            set { lowThreshold = value; }
        }

        /// <summary>
        /// High threshold.
        /// </summary>
        /// 
        public byte HighThreshold
        {
            get { return highThreshold; }
            set { highThreshold = value; }
        }

        /// <summary>
        /// Gaussian sigma.
        /// </summary>
        /// 
        /// <remarks>The value is for Gaussian bluring.</remarks>
        /// 
        public double GaussianSigma
        {
            get { return gaussianFilter.Sigma; }
            set { gaussianFilter.Sigma = value; }
        }

        /// <summary>
        /// Gaussian size.
        /// </summary>
        /// 
        /// <remarks>Size of Gaussian kernel.</remarks>
        /// 
        public int GaussianSize
        {
            get { return gaussianFilter.Size; }
            set { gaussianFilter.Size = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        public CannyEdgeDetector( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        /// <param name="lowThreshold">Low threshold.</param>
        /// <param name="highThreshold">High threshold.</param>
        /// 
        public CannyEdgeDetector( byte lowThreshold, byte highThreshold )
        {
            this.lowThreshold = lowThreshold;
            this.highThreshold = highThreshold;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        /// 
        /// <param name="lowThreshold">Low threshold.</param>
        /// <param name="highThreshold">High threshold.</param>
        /// <param name="sigma">Gaussian sigma.</param>
        /// 
        public CannyEdgeDetector( byte lowThreshold, byte highThreshold, double sigma )
        {
            this.lowThreshold = lowThreshold;
            this.highThreshold = highThreshold;
            gaussianFilter.Sigma = sigma;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData sourceData, BitmapData destinationData )
        {
            // get width and height
            int width    = sourceData.Width;
            int height   = sourceData.Height;
            int widthM1  = width - 1;
            int heightM1 = height - 1;
            int stride   = destinationData.Stride;
            int offset   = stride - width;

            // loop and array indexes
            int i, j, ir;
            // pixel's value and gradients
            double v, gx, gy;
            //
            double orientation, toAngle = 180.0 / System.Math.PI;
            float leftPixel = 0, rightPixel = 0;

            // orientation array
            byte[] orients = new byte[width * height];

            // STEP 1 - convert image to grayscale if it is color
            Bitmap grayImage = null;
            BitmapData grayData = null;

            if ( sourceData.PixelFormat != PixelFormat.Format8bppIndexed )
            {
                // do the processing
                grayImage = grayscaleFilter.Apply( sourceData );
                // lock the image
                grayData = grayImage.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );
                // substitute the source
                sourceData = grayData;
            }

            // STEP 2 - blur image
            Bitmap blurredImage = gaussianFilter.Apply( sourceData );
            BitmapData blurredData = blurredImage.LockBits(
                    new Rectangle( 0, 0, width, height ),
                    ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // gradients array
            float[,] gradients = new float[width, height];
            float maxGradient = float.NegativeInfinity;

            // do the job
            byte* src = (byte*) blurredData.Scan0.ToPointer( ) + stride;
            int p = width;

            // STEP 3 - calculate magnitude and edge orientation

            // for each line
            for ( int y = 1; y < heightM1; y++ )
            {
                src++;
                p++;

                // for each pixel
                for ( int x = 1; x < widthM1; x++, src++, p++ )
                {
                    gx = gy = 0;
                    // for each kernel row
                    for ( i = 0; i < 3; i++ )
                    {
                        ir = i - 1;
                        // for each kernel column
                        for ( j = 0; j < 3; j++ )
                        {
                            // source value
                            v = src[ir * stride + j - 1];

                            gx += v * xKernel[i, j];
                            gy += v * yKernel[i, j];
                        }
                    }
                    // get gradient value
                    gradients[x, y] = (float) Math.Sqrt( gx * gx + gy * gy );
                    maxGradient = Math.Max( maxGradient, gradients[x, y] );

                    // --- get orientation
                    if ( gx == 0 )
                    {
                        // can not divide by zero
                        orientation = ( gy == 0 ) ? 0 : 90;
                    }
                    else
                    {
                        double div = gy / gx;

                        // handle angles of the 2nd and 4th quads
                        if ( div < 0 )
                        {
                            orientation = 180 - System.Math.Atan( -div ) * toAngle;
                        }
                        // handle angles of the 1st and 3rd quads
                        else
                        {
                            orientation = System.Math.Atan( div ) * toAngle;
                        }

                        // get closest angle from 0, 45, 90, 135 set
                        if ( orientation < 22.5 )
                            orientation = 0;
                        else if ( orientation < 67.5 )
                            orientation = 45;
                        else if ( orientation < 112.5 )
                            orientation = 90;
                        else if ( orientation < 157.5 )
                            orientation = 135;
                        else orientation = 0;
                    }

                    // save orientation
                    orients[p] = (byte) orientation;
                }
                src += ( offset + 1 );
                p++;
            }

            // STEP 4 - suppres non maximums
            byte* dst = (byte*) destinationData.Scan0.ToPointer( ) + stride;
            p = width;

            // for each line
            for ( int y = 1; y < heightM1; y++ )
            {
                dst++;
                p++;

                // for each pixel
                for ( int x = 1; x < widthM1; x++, dst++, p++ )
                {
                    // get two adjacent pixels
                    switch ( orients[p] )
                    {
                        case 0:
                            leftPixel  = gradients[x - 1, y];
                            rightPixel = gradients[x + 1, y];
                            break;
                        case 45:
                            leftPixel  = gradients[x - 1, y + 1];
                            rightPixel = gradients[x + 1, y - 1];
                            break;
                        case 90:
                            leftPixel  = gradients[x, y + 1];
                            rightPixel = gradients[x, y - 1];
                            break;
                        case 135:
                            leftPixel  = gradients[x + 1, y + 1];
                            rightPixel = gradients[x - 1, y - 1];
                            break;
                    }
                    // compare current pixels value with adjacent pixels
                    if ( ( gradients[x, y] < leftPixel ) || ( gradients[x, y] < rightPixel ) )
                    {
                        *dst = 0;
                    }
                    else
                    {
                        *dst = (byte) ( gradients[x, y] / maxGradient * 255 );
                    }
                }
                dst += ( offset + 1 );
                p++;
            }

            // STEP 5 - hysteresis
            dst = (byte*) destinationData.Scan0.ToPointer( ) + stride;
            p = width;

            // for each line
            for ( int y = 1; y < heightM1; y++ )
            {
                dst++;
                p++;

                // for each pixel
                for ( int x = 1; x < widthM1; x++, dst++, p++ )
                {
                    if ( *dst < highThreshold )
                    {
                        if ( *dst < lowThreshold )
                        {
                            // non edge
                            *dst = 0;
                        }
                        else
                        {
                            // check 8 neighboring pixels
                            if ( ( dst[-1] < highThreshold ) &&
                                ( dst[1] < highThreshold ) &&
                                ( dst[-stride - 1] < highThreshold ) &&
                                ( dst[-stride] < highThreshold ) &&
                                ( dst[-stride + 1] < highThreshold ) &&
                                ( dst[stride - 1] < highThreshold ) &&
                                ( dst[stride] < highThreshold ) &&
                                ( dst[stride + 1] < highThreshold ) )
                            {
                                *dst = 0;
                            }
                        }
                    }
                }
                dst += ( offset + 1 );
                p++;
            }

            // release blurred image
            blurredImage.UnlockBits( blurredData );
            blurredImage.Dispose( );

            // release gray image, if there was conversion
            if ( grayData != null )
            {
                grayImage.UnlockBits( grayData );
                grayImage.Dispose( );
            }
        }
    }
}
