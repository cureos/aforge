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
	/// Complex image
	/// </summary>
    /// 
    /// <remarks>Represents image in complex numbers sutable for Fourier
    /// transformations.</remarks>
    /// 
	public class ComplexImage : ICloneable
	{
        // image complex data
		private Complex[,]	data;
        // image dimension
		private int			width;
		private int			height;
        // current state of the image (transformed with Fourier ot not)
		private bool		fmode = false;

        /// <summary>
        /// Image width
        /// </summary>
        /// 
		public int Width
		{
			get { return width; }
		}

        /// <summary>
        /// Image height
        /// </summary>
        /// 
		public int Height
		{
			get { return height; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class
        /// </summary>
        /// 
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// 
        protected ComplexImage( int width, int height )
        {
            this.width  = width;
            this.height = height;
            this.data   = new Complex[height, width];
            this.fmode = false;
        }

        /// <summary>
        /// Clone the complex image
        /// </summary>
        /// 
        /// <returns>Returns copy of the image.</returns>
        /// 
		public object Clone( )
		{
			// create new complex image
			ComplexImage    dstImage = new ComplexImage( width, height );
            Complex[,]      data = dstImage.data;

			for ( int i = 0; i < height; i++ )
			{
				for ( int j = 0; j < width; j++ )
				{
					data[i, j] = this.data[i, j];
				}
			}

            return dstImage;
		}

        /// <summary>
        /// Creates complex image from bitmap
        /// </summary>
        /// 
        /// <param name="srcImage">Source bitmap</param>
        /// 
        /// <returns>Returns an instance of complex image.</returns>
        /// 
		public static ComplexImage FromBitmap( Bitmap srcImage )
		{
			// get source image size
            int width   = srcImage.Width;
            int height  = srcImage.Height;

			// check image size
			if ( 
				( !Tools.IsPowerOf2( width ) ) ||
				( !Tools.IsPowerOf2( height ) )
				)
			{
				throw new ArgumentException( "Image width and height should be power of 2" );
			}

			// create new complex image
			ComplexImage	dstImage = new ComplexImage( width, height );
			Complex[,]		data = dstImage.data;

			// lock source bitmap data
            BitmapData srcData = srcImage.LockBits(
				new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, srcImage.PixelFormat );

            int offset = srcData.Stride - ( ( srcImage.PixelFormat == PixelFormat.Format8bppIndexed ) ? width : width * 3 );

			// do the job
			unsafe
			{
				byte * src = (byte *) srcData.Scan0.ToPointer( );

                if ( srcImage.PixelFormat == PixelFormat.Format8bppIndexed )
				{
					// grayscale image

					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++, src++ )
						{
							data[y, x].Re = (float) *src / 255;
						}
						src += offset;
					}
				}
				else
				{
					// RGB image

					// for each line
					for ( int y = 0; y < height; y++ )
					{
						// for each pixel
						for ( int x = 0; x < width; x++, src += 3 )
						{
							data[y, x].Re = ( 0.2125 * src[RGB.R] + 0.7154 * src[RGB.G] + 0.0721 * src[RGB.B] ) / 255;
						}
						src += offset;
					}
				}
			}
			// unlock source images
			srcImage.UnlockBits( srcData );

            return dstImage;
		}

        /// <summary>
        /// Convert complex image to bitmap
        /// </summary>
        /// 
        /// <returns>Returns grayscale bitmap</returns>
        /// 
		public Bitmap ToBitmap( )
		{
			// create new image
			Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );
			
			// lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

			int offset = dstData.Stride - width;
			double scale = ( fmode ) ? Math.Sqrt( width * height ) : 1;

			// do the job
			unsafe
			{
				byte * dst = (byte *) dstData.Scan0.ToPointer( );

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++, dst ++ )
					{
						*dst = (byte) System.Math.Max( 0, System.Math.Min( 255, data[y, x].Magnitude * scale * 255 ) );
					}
					dst += offset;
				}
			}
			// unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
		}

        /// <summary>
        /// Applies forward fast Fourier transformation of the complex image
        /// </summary>
        /// 
		public void ForwardFourierTransform( )
		{
			if ( !fmode )
			{
				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++ )
					{
						if ( ( ( x + y ) & 0x1 ) != 0 )
						{
							data[y, x].Re *= -1;
							data[y, x].Im *= -1;
						}
					}
				}

                FourierTransform.FFT2( data, FourierTransform.Direction.Forward );
				fmode = true;
			}
		}

        /// <summary>
        /// Applies backward fast Fourier transformation of the complex image
        /// </summary>
        /// 
		public void BackwardFourierTransform( )
		{
			if ( fmode )
			{
				FourierTransform.FFT2( data, FourierTransform.Direction.Backward );
				fmode = false;

				for ( int y = 0; y < height; y++ )
				{
					for ( int x = 0; x < width; x++ )
					{
						if ( ( ( x + y ) & 0x1 ) != 0 )
						{
							data[y, x].Re *= -1;
							data[y, x].Im *= -1;
						}
					}
				}
			}
		}

        /// <summary>
        /// Performs frequency filter
        /// </summary>
        /// 
        /// <param name="range">Frequency range to keep</param>
        /// 
        /// <remarks>Frequency filter zeros all values which frequencies are
        /// outside of the spefied range.</remarks>
        /// 
		public void FrequencyFilter( IntRange range )
		{
			if ( fmode )
			{
				int hw = width >> 1;
				int hh = height >> 1;
				int min = range.Min;
				int max = range.Max;

				// process all data
				for ( int i = 0; i < height; i++ )
				{
					int y = i - hh;

					for ( int j = 0; j < width; j++ )
					{
						int	x = j - hw;
						int d = (int) Math.Sqrt( x * x + y * y );

                        // filter values outside the range
						if ( ( d > max ) || ( d < min ) )
						{
							data[i, j].Re = 0;
							data[i, j].Im = 0;
						}
					}
				}
			}
		}
	}
}
