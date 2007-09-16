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
	/// Gather statistics about the image in HSL color space
	/// </summary>
    /// 
    /// <remarks></remarks>
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
        /// Histogram of saturation channel
        /// </summary>
        /// 
        public ContinuousHistogram Saturation
		{
			get { return saturation; }
		}

        /// <summary>
        /// Histogram of luminance channel
        /// </summary>
        /// 
        public ContinuousHistogram Luminance
		{
			get { return luminance; }
		}

        /// <summary>
        /// Histogram of saturation channel excluding black pixels
        /// </summary>
        /// 
        public ContinuousHistogram SaturationWithoutBlack
		{
			get { return saturationWithoutBlack; }
		}

        /// <summary>
        /// Histogram of luminance channel excluding black pixels
        /// </summary>
        /// 
        public ContinuousHistogram LuminanceWithoutBlack
		{
			get { return luminanceWithoutBlack; }
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
        /// Initializes a new instance of the <see cref="ImageStatisticsHSL"/> class
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel images are supported only.</remarks>
        /// 
		public ImageStatisticsHSL( Bitmap image )
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
        /// Initializes a new instance of the <see cref="ImageStatisticsHSL"/> class
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel images are supported only.</remarks>
        /// 
        public ImageStatisticsHSL( BitmapData imageData )
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

			int[]	s = new int[256];
			int[]	l = new int[256];
			int[]	swb = new int[256];
			int[]	lwb = new int[256];
			RGB		rgb = new RGB( );
			HSL		hsl = new HSL( );

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

						// convert to HSL color space
						AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

						s[(int) ( hsl.Saturation * 255 )]++;
						l[(int) ( hsl.Luminance * 255 )]++;
						pixels++;

						if ( ( hsl.Hue != 0.0 ) || ( hsl.Luminance != 0.0 ) || ( hsl.Saturation != 0.0 ) )
						{
							swb[(int) ( hsl.Saturation * 255 )]++;
							lwb[(int) ( hsl.Luminance * 255 )]++;
							pixelsWithoutBlack++;
						}
					}
					p += offset;
				}
			}

			// create histograms
			saturation = new ContinuousHistogram( s, new DoubleRange( 0, 1 ) );
            luminance = new ContinuousHistogram( l, new DoubleRange( 0, 1 ) );

            saturationWithoutBlack = new ContinuousHistogram( swb, new DoubleRange( 0, 1 ) );
            luminanceWithoutBlack = new ContinuousHistogram( lwb, new DoubleRange( 0, 1 ) );
		}
	}
}
