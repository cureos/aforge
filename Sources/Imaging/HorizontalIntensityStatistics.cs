// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge.Math;

    /// <summary>
    /// Horizontal intensity statistics.
    /// </summary>
    /// 
    /// <remarks><para>The class provides information about horizontal distribution
    /// of pixel intensities, which may be used to locate objects, their centers, etc.
    /// </para>
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // collect statistics
    /// HorizontalIntensityStatistics his = new HorizontalIntensityStatistics( sourceImage );
    /// // get gray histogram (for grayscale image)
    /// Histogram histogram = his.Gray;
    /// // output some histogram's information
    /// System.Diagnostics.Debug.WriteLine( "Mean = " + histogram.Mean );
    /// System.Diagnostics.Debug.WriteLine( "Min = " + histogram.Min );
    /// System.Diagnostics.Debug.WriteLine( "Max = " + histogram.Max );
    /// </code>
    /// <para><b>Sample grayscale image with its horizontal intensity histogram:</b></para>
    /// <img src="hor_histogram.jpg" width="320" height="338" />
    /// </remarks>
    /// 
    public class HorizontalIntensityStatistics
    {
        // histograms for RGB channgels
        private Histogram red = null;
        private Histogram green = null;
        private Histogram blue = null;
        // grayscale histogram
        private Histogram gray = null;

        /// <summary>
        /// Histogram for red channel.
        /// </summary>
        /// 
        public Histogram Red
        {
            get { return red; }
        }

        /// <summary>
        /// Histogram for green channel.
        /// </summary>
        /// 
        public Histogram Green
        {
            get { return green; }
        }

        /// <summary>
        /// Histogram for blue channel.
        /// </summary>
        /// 
        public Histogram Blue
        {
            get { return blue; }
        }

        /// <summary>
        /// Histogram for gray channel (intensities).
        /// </summary>
        /// 
        public Histogram Gray
        {
            get { return gray; }
        }

        /// <summary>
        /// Value wich specifies if the processed image was color or grayscale.
        /// </summary>
        /// 
        public bool IsGrayscale
        {
            get { return ( gray == null ); }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Source image.</param>
        ///
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public HorizontalIntensityStatistics( Bitmap image )
        {
            // check image format
            if (
                ( image.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( image.PixelFormat != PixelFormat.Format24bppRgb )
                )
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

            // lock bitmap data
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            // gather statistics
            ProcessImage( imageData );

            // unlock image
            image.UnlockBits( imageData );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        ///
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public HorizontalIntensityStatistics( BitmapData imageData )
        {
            // check image format
            if (
                ( imageData.PixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( imageData.PixelFormat != PixelFormat.Format24bppRgb )
                )
            {
                throw new ArgumentException( "Source image can be graysclae (8 bpp indexed) or color (24 bpp) image only" );
            }

            ProcessImage( imageData );
        }

        /// <summary>
        /// Gather horizontal intensity statistics for specified image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        /// 
        private void ProcessImage( BitmapData imageData )
        {
            // get image dimension
            int width  = imageData.Width;
            int height = imageData.Height;

            // do the job
            unsafe
            {
                byte* p = (byte*) imageData.Scan0.ToPointer( );

                // check pixel format
                if ( imageData.PixelFormat == PixelFormat.Format8bppIndexed )
                {
                    int offset = imageData.Stride - width;

                    // histogram array
                    int[] g = new int[width];

					// for each pixel
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, p++ )
                        {
                            g[x] += *p;
                        }
                        p += offset;
                    }

                    // create historgram for gray level
                    gray = new Histogram( g );
                }
                else
                {
                    int offset = imageData.Stride - width * 3;

                    // histogram arrays
                    int[] r = new int[width];
                    int[] g = new int[width];
                    int[] b = new int[width];

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        // for each pixel
                        for ( int x = 0; x < width; x++, p += 3 )
                        {
                            r[x] += p[RGB.R];
                            g[x] += p[RGB.G];
                            b[x] += p[RGB.B];
                        }
                        p += offset;
                    }

                    // create histograms
                    red   = new Histogram( r );
                    green = new Histogram( g );
                    blue  = new Histogram( b );
                }
            }
        }
    }
}
