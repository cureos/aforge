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
    using AForge.Math;

    /// <summary>
    /// Gather statistics about image in RGB color space.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to accumulate statistical values about images,
    /// like histogram, mean, standard deviation, etc. for each color channel in RGB color
    /// space.</para>
    /// 
    /// <para>The class accepts 8 bpp grayscale and 24/32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // gather statistics
    /// ImageStatistics stat = new ImageStatistics( image );
    /// // get red channel's histogram
    /// Histogram red = stat.Red;
    /// // check mean value of red channel
    /// if ( red.Mean > 128 )
    /// {
    ///     // do further processing
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="AForge.Math.Histogram"/>
    /// 
    public class ImageStatistics
    {
        private Histogram red;
        private Histogram green;
        private Histogram blue;
        private Histogram gray;

        private Histogram redWithoutBlack;
        private Histogram greenWithoutBlack;
        private Histogram blueWithoutBlack;
        private Histogram grayWithoutBlack;

        private int pixels;
        private int pixelsWithoutBlack;
        private bool grayscale;

        /// <summary>
        /// Histogram of red channel.
        /// </summary>
        /// 
        /// <remarks><para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram Red
        {
            get { return red; }
        }

        /// <summary>
        /// Histogram of green channel.
        /// </summary>
        /// 
        /// <remarks><para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram Green
        {
            get { return green; }
        }

        /// <summary>
        /// Histogram of blue channel.
        /// </summary>
        /// 
        /// <remarks><para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram Blue
        {
            get { return blue; }
        }

        /// <summary>
        /// Histogram of gray channel.
        /// </summary>
        /// 
        /// <remarks><para><note>The property is valid only for grayscale images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram Gray
        {
            get { return gray; }
        }

        /// <summary>
        /// Histogram of red channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about red channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// 
        /// <para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram RedWithoutBlack
        {
            get { return redWithoutBlack; }
        }

        /// <summary>
        /// Histogram of green channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about green channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// 
        /// <para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram GreenWithoutBlack
        {
            get { return greenWithoutBlack; }
        }

        /// <summary>
        /// Histogram of blue channel excluding black pixels
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about blue channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// 
        /// <para><note>The property is valid only for color images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram BlueWithoutBlack
        {
            get { return blueWithoutBlack; }
        }

        /// <summary>
        /// Histogram of gray channel channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about gray channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// 
        /// <para><note>The property is valid only for grayscale images
        /// (see <see cref="IsGrayscale"/> property).</note></para></remarks>
        /// 
        public Histogram GrayWithoutBlack
        {
            get { return grayWithoutBlack; }
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
        /// Value wich specifies if the processed image was color or grayscale.
        /// </summary>
        /// 
        /// <remarks><para>If the value is set to <see langword="true"/> then <see cref="Gray"/>
        /// property should be used to get statistics information about image. Otherwise
        /// <see cref="Red"/>, <see cref="Green"/> and <see cref="Blue"/> properties should be used
        /// for color images.</para></remarks>
        /// 
        public bool IsGrayscale
        {
            get { return grayscale; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatistics( Bitmap image )
        {
            CheckSourceFormat( image.PixelFormat );

            // lock bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

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
        /// Initializes a new instance of the <see cref="ImageStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatistics( BitmapData imageData )
        {
            ProcessImage( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatistics( UnmanagedImage image )
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

            // check pixel format
            if ( grayscale = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) )
            {
                // alloc arrays
                int[] g   = new int[256];
                int[] gwb = new int[256];

                byte value;
                int  offset = image.Stride - width;

                // do the job
                unsafe
                {
                    byte * p = (byte*) image.ImageData.ToPointer( );

                    // for each pixel
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, p++ )
                        {
                            // get pixel value
                            value = *p;

                            g[value]++;
                            pixels++;

                            if ( value != 0 )
                            {
                                gwb[value]++;
                                pixelsWithoutBlack++;
                            }
                        }
                        p += offset;
                    }
                }

                // create historgram for gray level
                gray = new Histogram( g );
                grayWithoutBlack = new Histogram( gwb );
            }
            else
            {
                // alloc arrays
                int[]	r = new int[256];
                int[]	g = new int[256];
                int[]	b = new int[256];

                int[]	rwb = new int[256];
                int[]	gwb = new int[256];
                int[]	bwb = new int[256];

                byte rValue, gValue, bValue;
                int  pixelSize = ( image.PixelFormat == PixelFormat.Format24bppRgb ) ? 3 : 4;
                int  offset = image.Stride - width * pixelSize;

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
                            // get pixel values
                            rValue = p[RGB.R];
                            gValue = p[RGB.G];
                            bValue = p[RGB.B];

                            r[rValue]++;
                            g[gValue]++;
                            b[bValue]++;
                            pixels++;

                            if ( ( rValue != 0 ) || ( gValue != 0 ) || ( bValue != 0 ) )
                            {
                                rwb[rValue]++;
                                gwb[gValue]++;
                                bwb[bValue]++;
                                pixelsWithoutBlack++;
                            }
                        }
                        p += offset;
                    }
                }

                // create histograms
                red   = new Histogram( r );
                green = new Histogram( g );
                blue  = new Histogram( b );

                redWithoutBlack   = new Histogram( rwb );
                greenWithoutBlack = new Histogram( gwb );
                blueWithoutBlack  = new Histogram( bwb );
            }
        }

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if (
                ( pixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( pixelFormat != PixelFormat.Format24bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppArgb ) )
            {
                throw new UnsupportedImageFormatException( "Source pixel format is not supported." );
            }
        }
    }
}
