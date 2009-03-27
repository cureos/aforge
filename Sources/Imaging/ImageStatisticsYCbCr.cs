// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Math;

    /// <summary>
    /// Gather statistics about image in YCbCr color space.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to accumulate statistical values about images,
    /// like histogram, mean, standard deviation, etc. for each <b>YCbCr</b> color channel.</para>
    /// 
    /// <para>The class accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // gather statistics
    /// ImageStatisticsYCbCr stat = new ImageStatisticsYCbCr( image );
    /// // get Y channel's histogram
    /// ContinuousHistogram y = stat.Y;
    /// // check mean value of Y channel
    /// if ( y.Mean > 0.5 )
    /// {
    ///     // do further processing
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="AForge.Math.ContinuousHistogram"/>
    /// 
    public class ImageStatisticsYCbCr
    {
        private ContinuousHistogram yHistogram;
        private ContinuousHistogram cbHistogram;
        private ContinuousHistogram crHistogram;

        private ContinuousHistogram yHistogramWithoutBlack;
        private ContinuousHistogram cbHistogramWithoutBlack;
        private ContinuousHistogram crHistogramWithoutBlack;

        private int pixels;
        private int pixelsWithoutBlack;

        /// <summary>
        /// Histogram of Y channel.
        /// </summary>
        /// 
        public ContinuousHistogram Y
        {
            get { return yHistogram; }
        }

        /// <summary>
        /// Histogram of Cb channel.
        /// </summary>
        /// 
        public ContinuousHistogram Cb
        {
            get { return cbHistogram; }
        }

        /// <summary>
        /// Histogram of Cr channel.
        /// </summary>
        /// 
        public ContinuousHistogram Cr
        {
            get { return crHistogram; }
        }

        /// <summary>
        /// Histogram of Y channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about Y channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// </remarks>
        /// 
        public ContinuousHistogram YWithoutBlack
        {
            get { return yHistogramWithoutBlack; }
        }

        /// <summary>
        /// Histogram of Cb channel excluding black pixels
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about Cb channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// </remarks>
        /// 
        public ContinuousHistogram CbWithoutBlack
        {
            get { return cbHistogramWithoutBlack; }
        }

        /// <summary>
        /// Histogram of Cr channel excluding black pixels
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about Cr channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// </remarks>
        /// 
        public ContinuousHistogram CrWithoutBlack
        {
            get { return crHistogramWithoutBlack; }
        }

        /// <summary>
        /// Total pixels count in the processed image.
        /// </summary>
        /// 
        public int PixelsCount
        {
            get { return pixels; }
        }

        /// <summary>
        /// Total pixels count in the processed image excluding black pixels.
        /// </summary>
        /// 
        public int PixelsCountWithoutBlack
        {
            get { return pixelsWithoutBlack; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsYCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormat">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsYCbCr( Bitmap image )
        {
            CheckSourceFormat( image.PixelFormat );

            // lock bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            try
            {
                // gather statistics
                ProcessImage( new UnmanagedImage( imageData ) );
            }
            finally
            {
                // unlock image
                image.UnlockBits( imageData );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsYCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormat">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsYCbCr( BitmapData imageData )
        {
            ProcessImage( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsYCbCr"/> class.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormat">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsYCbCr( UnmanagedImage image )
        {
            ProcessImage( image );
        }

        /// <summary>
        /// Gather statistics about specified image.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to process.</param>
        /// 
        private void ProcessImage( UnmanagedImage image )
        {
            CheckSourceFormat( image.PixelFormat );

            // get image dimension
            int width  = image.Width;
            int height = image.Height;

            pixels = pixelsWithoutBlack = 0;

            int[] yhisto  = new int[256];
            int[] cbhisto = new int[256];
            int[] crhisto = new int[256];

            int[] yhistoWB	= new int[256];
            int[] cbhistoWB	= new int[256];
            int[] crhistoWB	= new int[256];

            RGB   rgb   = new RGB( );
            YCbCr ycbcr = new YCbCr( );

            int pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
            int offset = image.Stride - width * pixelSize;

            // do the job
            unsafe
            {
                byte * p = (byte*) image.ImageData.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, p += pixelSize )
                    {
                        rgb.Red   = p[RGB.R];
                        rgb.Green = p[RGB.G];
                        rgb.Blue  = p[RGB.B];

                        // convert to YCbCr color space
                        AForge.Imaging.YCbCr.FromRGB( rgb, ycbcr );

                        yhisto [(int) ( ycbcr.Y * 255 )]++;
                        cbhisto[(int) ( ( ycbcr.Cb + 0.5 ) * 255 )]++;
                        crhisto[(int) ( ( ycbcr.Cr + 0.5 ) * 255 )]++;

                        pixels++;

                        if ( ( ycbcr.Y != 0.0 ) || ( ycbcr.Cb != 0.0 ) || ( ycbcr.Cr != 0.0 ) )
                        {
                            yhistoWB [(int) ( ycbcr.Y * 255 )]++;
                            cbhistoWB[(int) ( ( ycbcr.Cb + 0.5 ) * 255 )]++;
                            crhistoWB[(int) ( ( ycbcr.Cr + 0.5 ) * 255 )]++;

                            pixelsWithoutBlack++;
                        }
                    }
                    p += offset;
                }
            }

            // create histograms
            yHistogram  = new ContinuousHistogram( yhisto,  new DoubleRange(  0.0, 1.0 ) );
            cbHistogram = new ContinuousHistogram( cbhisto, new DoubleRange( -0.5, 0.5 ) );
            crHistogram = new ContinuousHistogram( crhisto, new DoubleRange( -0.5, 0.5 ) );

            yHistogramWithoutBlack  = new ContinuousHistogram( yhistoWB,  new DoubleRange(  0.0, 1.0 ) );
            cbHistogramWithoutBlack = new ContinuousHistogram( cbhistoWB, new DoubleRange( -0.5, 0.5 ) );
            crHistogramWithoutBlack = new ContinuousHistogram( crhistoWB, new DoubleRange( -0.5, 0.5 ) );
        }

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if (
                ( pixelFormat != PixelFormat.Format24bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppArgb ) )
            {
                throw new UnsupportedImageFormat( "Source pixel format is not supported." );
            }
        }
    }
}
