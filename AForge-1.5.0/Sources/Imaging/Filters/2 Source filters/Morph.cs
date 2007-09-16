// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Morph filter
	/// </summary>
	/// 
	/// <remarks>Gets specified percentage of source image and the rest from overlay image.</remarks>
	/// 
	public class Morph : FilterAnyToAny
	{
		private double	percent = 0.50;
		private Bitmap	overlayImage;

		/// <summary>
		/// Percent of source image to keep
		/// </summary>
		public double Percent
		{
			get { return percent; }
			set { percent = Math.Max( 0.0, Math.Min( 1.0, value ) ); }
		}

		/// <summary>
		/// Overlay image
		/// </summary>
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Morph"/> class
		/// </summary>
		public Morph( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Morph"/> class
		/// </summary>
		/// 
		/// <param name="overlayImage">Overlay image</param>
		/// 
		public Morph( Bitmap overlayImage )
		{
			this.overlayImage = overlayImage;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Morph"/> class
		/// </summary>
		/// 
		/// <param name="overlayImage">Overlay image</param>
		/// <param name="percent">Percent of source image to keep</param>
		/// 
		public Morph( Bitmap overlayImage, double percent )
		{
			this.overlayImage = overlayImage;
			Percent = percent;
		}

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			// source image and overlay must have same pixel format
			if ( imageData.PixelFormat != overlayImage.PixelFormat )
				throw new ArgumentException( "Source and overlay images must have same pixel format " );

			// get image dimension
			int width	= imageData.Width;
			int height	= imageData.Height;

			// check overlay image size
			if ( ( width != overlayImage.Width ) || ( height != overlayImage.Height ) )
				throw new ArgumentException( "Overlay image size should be equal to source image size" );

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, imageData.PixelFormat );

			// initialize other variables
			int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int lineSize = width * pixelSize;
			int offset = imageData.Stride - lineSize;
			// percentage of overlay image
			double q = 1.0 - percent;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
				{
					*ptr = (byte) ( ( percent * *ptr ) + ( q * *ovr ) );
				}
				ptr += offset;
				ovr += offset;
			}

			// unlock overlay image
			overlayImage.UnlockBits( ovrData );
		}
	}
}