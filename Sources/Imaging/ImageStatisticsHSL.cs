// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;
    using AForge.Math;

    /// <summary>
    /// Gather statistics about image in HSL color space.
    /// </summary>
    /// 
    /// <remarks><para>The class is used to accumulate statistical values about images,
    /// like histogram, mean, standard deviation, etc. for each <b>HSL</b> color channel.</para>
    /// 
    /// <para>The class accepts 24 and 32 bpp color images for processing.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // gather statistics
    /// ImageStatisticsHSL stat = new ImageStatisticsHSL( image );
    /// // get saturation channel's histogram
    /// ContinuousHistogram saturation = stat.Saturation;
    /// // check mean value of saturation channel
    /// if ( saturation.Mean > 0.5 )
    /// {
    ///     // do further processing
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="AForge.Math.ContinuousHistogram"/>
    /// 
    public class ImageStatisticsHSL
    {
        private ContinuousHistogram luminance;
        private ContinuousHistogram saturation;

        private ContinuousHistogram luminanceWithoutBlack;
        private ContinuousHistogram saturationWithoutBlack;

        private int pixels;
        private int pixelsWithoutBlack;

        /// <summary>
        /// Histogram of saturation channel.
        /// </summary>
        /// 
        public ContinuousHistogram Saturation
        {
            get { return saturation; }
        }

        /// <summary>
        /// Histogram of luminance channel.
        /// </summary>
        /// 
        public ContinuousHistogram Luminance
        {
            get { return luminance; }
        }

        /// <summary>
        /// Histogram of saturation channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about saturation channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// </remarks>
        /// 
        public ContinuousHistogram SaturationWithoutBlack
        {
            get { return saturationWithoutBlack; }
        }

        /// <summary>
        /// Histogram of luminance channel excluding black pixels.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps statistics about luminance channel, which
        /// excludes all black pixels, what affects mean, standard deviation, etc.</para>
        /// </remarks>
        /// 
        public ContinuousHistogram LuminanceWithoutBlack
        {
            get { return luminanceWithoutBlack; }
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
        /// Initializes a new instance of the <see cref="ImageStatisticsHSL"/> class.
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsHSL( Bitmap image )
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
        /// Initializes a new instance of the <see cref="ImageStatisticsHSL"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsHSL( BitmapData imageData )
        {
            ProcessImage( new UnmanagedImage( imageData ) );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsHSL"/> class.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to gather statistics about.</param>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Source pixel format is not supported.</exception>
        /// 
        public ImageStatisticsHSL( UnmanagedImage image )
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

            int[] s   = new int[256];
            int[] l   = new int[256];
            int[] swb = new int[256];
            int[] lwb = new int[256];
            RGB   rgb = new RGB( );
            HSL   hsl = new HSL( );

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
                        rgb.Red = p[RGB.R];
                        rgb.Green = p[RGB.G];
                        rgb.Blue = p[RGB.B];

                        // convert to HSL color space
                        AForge.Imaging.HSL.FromRGB( rgb, hsl );

                        s[(int) ( hsl.Saturation * 255 )]++;
                        l[(int) ( hsl.Luminance  * 255 )]++;
                        pixels++;

                        if ( hsl.Luminance != 0.0 )
                        {
                            swb[(int) ( hsl.Saturation * 255 )]++;
                            lwb[(int) ( hsl.Luminance  * 255 )]++;
                            pixelsWithoutBlack++;
                        }
                    }
                    p += offset;
                }
            }

            // create histograms
            saturation = new ContinuousHistogram( s, new Range( 0, 1 ) );
            luminance  = new ContinuousHistogram( l, new Range( 0, 1 ) );

            saturationWithoutBlack = new ContinuousHistogram( swb, new Range( 0, 1 ) );
            luminanceWithoutBlack  = new ContinuousHistogram( lwb, new Range( 0, 1 ) );
        }

        // Check pixel format of the source image
        private void CheckSourceFormat( PixelFormat pixelFormat )
        {
            if (
                ( pixelFormat != PixelFormat.Format24bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppArgb ) )
            {
                throw new UnsupportedImageFormatException( "Source pixel format is not supported." );
            }
        }
    }
}
