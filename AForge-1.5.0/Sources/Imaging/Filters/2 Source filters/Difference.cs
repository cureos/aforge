// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Difference filter - get the difference of overlay and source images
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Difference : FilterAnyToAny
	{
		private Bitmap	overlayImage;

		/// <summary>
		/// Overlay image
		/// </summary>
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Difference"/> class
		/// </summary>
		public Difference( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Difference"/> class
		/// </summary>
		/// 
		/// <param name="overlayImage">Overlay image</param>
		/// 
		public Difference( Bitmap overlayImage )
		{
			this.overlayImage = overlayImage;
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
				throw new ArgumentException( "Source and overlay images must have same pixel format" );

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
			// pixel value
			int v;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
				{
					// abs(sub)
					v = (int) *ptr - (int) *ovr;
					*ptr = ( v < 0 ) ? (byte) -v : (byte) v;
				}
				ptr += offset;
				ovr += offset;
			}

			// unlock overlay image
			overlayImage.UnlockBits( ovrData );
		}
	}
}
