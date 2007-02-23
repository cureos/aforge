// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
    using AForge;
	using AForge.Math;

	/// <summary>
	/// Gather statistics about the image in YCbCr color space
	/// </summary>
    /// 
    /// <remarks></remarks>
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
        /// Histogram of Y channel
        /// </summary>
        /// 
        public ContinuousHistogram Y
		{
			get { return yHistogram; }
		}

        /// <summary>
        /// Histogram of Cb channel
        /// </summary>
        /// 
        public ContinuousHistogram Cb
		{
			get { return cbHistogram; }
		}

        /// <summary>
        /// Histogram of Cr channel
        /// </summary>
        /// 
        public ContinuousHistogram Cr
		{
			get { return crHistogram; }
		}

        /// <summary>
        /// Histogram of Y channel excluding black pixels
        /// </summary>
        /// 
        public ContinuousHistogram YWithoutBlack
		{
			get { return yHistogramWithoutBlack; }
		}

        /// <summary>
        /// Histogram of Cb channel excluding black pixels
        /// </summary>
        /// 
        public ContinuousHistogram CbWithoutBlack
		{
			get { return cbHistogramWithoutBlack; }
		}

        /// <summary>
        /// Histogram of Cr channel excluding black pixels
        /// </summary>
        /// 
        public ContinuousHistogram CrWithoutBlack
		{
			get { return crHistogramWithoutBlack; }
		}

        /// <summary>
        /// Total pixel count of the image
        /// </summary>
        /// 
        public int PixelsCount
		{
			get { return pixels; }
		}

        /// <summary>
        /// Total pixel count of the image excluding black pixels
        /// </summary>
        /// 
        public int PixelsCountWithoutBlack
		{
			get { return pixelsWithoutBlack; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsYCbCr"/> class
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel images are supported only.</remarks>
        /// 
        public ImageStatisticsYCbCr( Bitmap image )
		{
			if ( image.PixelFormat != PixelFormat.Format24bppRgb )
				throw new ArgumentException( );

			// lock bitmap data
			BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

			// gather statistics
            ProcessImage( imageData );

			// unlock image
            image.UnlockBits( imageData );
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatisticsYCbCr"/> class
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel images are supported only.</remarks>
        /// 
        public ImageStatisticsYCbCr( BitmapData imageData )
		{
			ProcessImage( imageData );
		}

        /// <summary>
        /// Gather statistics about specified image
        /// </summary>
        /// 
        /// <param name="imageData">Image data</param>
        /// 
        private void ProcessImage( BitmapData imageData )
		{
            // get image dimension
            int width = imageData.Width;
            int height = imageData.Height;
            
            pixels = pixelsWithoutBlack = 0;

			int[]	yhisto  = new int[256];
			int[]	cbhisto = new int[256];
			int[]	crhisto = new int[256];

			int[]	yhistoWB	= new int[256];
			int[]	cbhistoWB	= new int[256];
			int[]	crhistoWB	= new int[256];

			RGB		rgb = new RGB( );
			YCbCr	ycbcr = new YCbCr( );

			int offset = imageData.Stride - width * 3;

			// do the job
			unsafe
			{
				byte * p = (byte *) imageData.Scan0.ToPointer( );

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < width; x++, p += 3 )
					{
						rgb.Red		= p[RGB.R];
						rgb.Green	= p[RGB.G];
						rgb.Blue	= p[RGB.B];

						// convert to YCbCr color space
						AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

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
            yHistogram  = new ContinuousHistogram( yhisto, new DoubleRange( 0.0, 1.0 ) );
            cbHistogram = new ContinuousHistogram( cbhisto, new DoubleRange( -0.5, 0.5 ) );
            crHistogram = new ContinuousHistogram( crhisto, new DoubleRange( -0.5, 0.5 ) );

            yHistogramWithoutBlack  = new ContinuousHistogram( yhistoWB, new DoubleRange( 0.0, 1.0 ) );
            cbHistogramWithoutBlack = new ContinuousHistogram( cbhistoWB, new DoubleRange( -0.5, 0.5 ) );
            crHistogramWithoutBlack = new ContinuousHistogram( crhistoWB, new DoubleRange( -0.5, 0.5 ) );
		}
	}
}
