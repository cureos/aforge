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
	/// Add fillter - add overlay image to the source image
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public sealed class Add : FilterAnyToAny
	{
		private Bitmap	overlayImage;
		private Point	overlayPos = new Point( 0, 0 );

		/// <summary>
		/// Overlay image
		/// </summary>
		public Bitmap OverlayImage
		{
			get { return overlayImage; }
			set { overlayImage = value; }
		}

		/// <summary>
		/// Overlay position
		/// </summary>
		public Point OverlayPos
		{
			get { return overlayPos; }
			set { overlayPos = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Add"/> class
		/// </summary>
		public Add( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Add"/> class
		/// </summary>
		/// 
		/// <param name="overlayImage">Overlay image</param>
		/// 
		public Add( Bitmap overlayImage )
		{
			this.overlayImage = overlayImage;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Add"/> class
		/// </summary>
		/// 
		/// <param name="overlayImage">Overlay image</param>
		/// <param name="position">Overlay position</param>
		/// 
		public Add( Bitmap overlayImage, Point position )
		{
			this.overlayImage = overlayImage;
			this.overlayPos = position;
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

			// overlay position and dimension
			int ovrX = overlayPos.X;
			int ovrY = overlayPos.Y;
			int ovrW = overlayImage.Width;
			int ovrH = overlayImage.Height;

			// lock overlay image
			BitmapData ovrData = overlayImage.LockBits(
				new Rectangle( 0, 0, ovrW, ovrH ),
				ImageLockMode.ReadOnly, imageData.PixelFormat );

			// initialize other variables
			int pixelSize = ( imageData.PixelFormat == PixelFormat.Format8bppIndexed ) ? 1 : 3;
			int stride = imageData.Stride;
			int offset = stride - pixelSize * width;
			int ovrStide = ovrData.Stride;
			int ovrOffset, lineSize;
			// pixel's value
			int v;

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );
			byte * ovr = (byte *) ovrData.Scan0.ToPointer( );

			if ( ( width == ovrW ) && ( height == ovrH ) && ( ovrX == 0 ) && ( ovrY == 0 ) )
			{
				// overlay image has the same size as the source image and its position is (0, 0)
				lineSize = width * pixelSize;

				// for each line
				for ( int y = 0; y < height; y++ )
				{
					// for each pixel
					for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
					{
						v = (int) *ptr + (int) *ovr;
						*ptr = ( v > 255 ) ? (byte) 255 : (byte) v;
					}
					ptr += offset;
					ovr += offset;
				}
			}
			else
			{
				// align Y
				if ( ovrY >= 0 )
				{
					ptr += stride * ovrY;
				}
				else
				{
					ovr -= ovrStide * ovrY;
					ovrH += ovrY;
					ovrY = 0;
				}

				// align X
				if ( ovrX >= 0 )
				{
					ptr += pixelSize * ovrX;
				}
				else
				{
					ovr -= pixelSize * ovrX;
					ovrW += ovrX;
					ovrX = 0;
				}

				// update overlay width and height
				ovrW = Math.Min( ovrW, width - ovrX );
				ovrH = Math.Min( ovrH, height - ovrY );

				// update offset
				ovrOffset = ovrStide - ovrW * pixelSize;
				offset = stride - ovrW * pixelSize;

				if ( ( ovrW > 0 ) && ( ovrH > 0 ) && ( ovrX < width ) && ( ovrY < height ) )
				{
					lineSize = pixelSize * ovrW;

					// for each line
					for ( int y = 0; y < ovrH; y++ )
					{
						// for each pixel
						for ( int x = 0; x < lineSize; x++, ptr++, ovr++ )
						{
							v = (int) *ptr + (int) *ovr;
							*ptr = ( v > 255 ) ? (byte) 255 : (byte) v;
						}
						ptr += offset;
						ovr += ovrOffset;
					}
				}
			}

			// unlock overlay image
			overlayImage.UnlockBits( ovrData );
		}
	}
}
