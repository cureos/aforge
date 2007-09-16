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
	using AForge.Math;

	/// <summary>
	/// Gather statistics about the image in RGB color space
	/// </summary>
    /// 
    /// <remarks></remarks>
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
        /// Histogram of red channel
        /// </summary>
        /// 
		public Histogram Red
		{
			get { return red; }
		}

        /// <summary>
        /// Histogram of green channel
        /// </summary>
        /// 
        public Histogram Green
		{
			get { return green; }
		}

        /// <summary>
        /// Histogram of blue channel
        /// </summary>
        /// 
        public Histogram Blue
		{
			get { return blue; }
		}

        /// <summary>
        /// Histogram of gray channel (intensities)
        /// </summary>
        /// 
        public Histogram Gray
		{
			get { return gray; }
		}

        /// <summary>
        /// Histogram of the channel excluding black pixels
        /// </summary>
        /// 
        public Histogram RedWithoutBlack
		{
			get { return redWithoutBlack; }
		}

        /// <summary>
        /// Histogram of green channel excluding black pixels
        /// </summary>
        /// 
        public Histogram GreenWithoutBlack
		{
			get { return greenWithoutBlack; }
		}

        /// <summary>
        /// Histogram of blue channel excluding black pixels
        /// </summary>
        /// 
        public Histogram BlueWithoutBlack
		{
			get { return blueWithoutBlack; }
		}

        /// <summary>
        /// Histogram of gray channel (intensities) channel excluding black pixels
        /// </summary>
        /// 
        public Histogram GrayWithoutBlack
		{
			get { return grayWithoutBlack; }
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
        /// Value wich specifies if the image is color or grayscale
        /// </summary>
        /// 
        public bool IsGrayscale
		{
			get { return grayscale; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatistics"/> class
        /// </summary>
        /// 
        /// <param name="image">Image to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel or 8 bit indexed (grayscale) images
        /// are supported only.</remarks>
        /// 
        public ImageStatistics( Bitmap image )
		{
			PixelFormat fmt = ( image.PixelFormat == PixelFormat.Format8bppIndexed ) ?
				PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;

			// lock bitmap data
            BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, fmt );

			// gather statistics
            ProcessImage( imageData );

			// unlock image
            image.UnlockBits( imageData );
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageStatistics"/> class
        /// </summary>
        /// 
        /// <param name="imageData">Image data to gather statistics about</param>
        /// 
        /// <remarks>24 bit per pixel or 8 bit indexed (grayscale) images
        /// are supported only.</remarks>
        /// 
		public ImageStatistics( BitmapData imageData )
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

            // check pixel format
			if ( grayscale = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) )
			{
				// alloc arrays
				int[] g = new int[256];
				int[] gwb = new int[256];

				byte    value;
				int     offset = imageData.Stride - width;

				// do the job
				unsafe
				{
					byte * p = (byte *) imageData.Scan0.ToPointer( );

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

				byte	rValue, gValue, bValue;
				int     offset = imageData.Stride - width * 3;

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
				red		= new Histogram( r );
				green	= new Histogram( g );
				blue	= new Histogram( b );

				redWithoutBlack		= new Histogram( rwb );
				greenWithoutBlack	= new Histogram( gwb );
				blueWithoutBlack	= new Histogram( bwb );
			}
		}
	}
}
