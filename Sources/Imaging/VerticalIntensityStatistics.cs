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
    /// Vertical intensity statistics.
    /// </summary>
    /// 
    /// <remarks><para>The class provides information about vertical distribution
    /// of pixel intensities, which may be used to locate objects, their centers, etc.
    /// </para>
    /// <para><note>The class processes only grayscale (8 bpp indexed) and color (24 bpp) images.</note></para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // collect statistics
    /// VerticalIntensityStatistics vis = new VerticalIntensityStatistics( sourceImage );
    /// // get gray histogram (for grayscale image)
    /// Histogram histogram = vis.Gray;
    /// // output some histogram's information
    /// System.Diagnostics.Debug.WriteLine( "Mean = " + histogram.Mean );
    /// System.Diagnostics.Debug.WriteLine( "Min = " + histogram.Min );
    /// System.Diagnostics.Debug.WriteLine( "Max = " + histogram.Max );
    /// </code>
    /// <para><b>Sample grayscale image with its vertical intensity histogram:</b></para>
    /// <img src="ver_histogram.jpg" width="450" height="240" />
    /// </remarks>
    /// 
    public class VerticalIntensityStatistics
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
        /// Initializes a new instance of the <see cref="VerticalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="image">Source image.</param>
        ///
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public VerticalIntensityStatistics( Bitmap image )
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
        /// Initializes a new instance of the <see cref="VerticalIntensityStatistics"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Source image data.</param>
        ///
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        public VerticalIntensityStatistics( BitmapData imageData )
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
        /// Gather vertical intensity statistics for specified image.
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
                    int[] g = new int[height];

					// for each pixel
                    for ( int y = 0; y < height; y++ )
                    {
                        int lineSum = 0;

                        // for each pixel
                        for ( int x = 0; x < width; x++, p++ )
                        {
                            lineSum += *p;
                        }
                        g[y] = lineSum;

                        p += offset;
                    }

                    // create historgram for gray level
                    gray = new Histogram( g );
                }
                else
                {
                    int offset = imageData.Stride - width * 3;

                    // histogram arrays
                    int[] r = new int[height];
                    int[] g = new int[height];
                    int[] b = new int[height];

                    // for each line
                    for ( int y = 0; y < height; y++ )
                    {
                        int lineRSum = 0;
                        int lineGSum = 0;
                        int lineBSum = 0;

                        // for each pixel
                        for ( int x = 0; x < width; x++, p += 3 )
                        {
                            lineRSum += p[RGB.R];
                            lineGSum += p[RGB.G];
                            lineBSum += p[RGB.B];
                        }
                        r[y] = lineRSum;
                        g[y] = lineGSum;
                        b[y] = lineBSum;

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
