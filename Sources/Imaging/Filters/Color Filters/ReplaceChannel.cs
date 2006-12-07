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
	/// Replace RGB channel of color imgae
	/// </summary>
	/// 
	/// <remarks>Replaces specified RGB channel of color image with
	/// specified grayscale imge.</remarks>
	/// 
	public class ReplaceChannel : FilterColorToColor
	{
		private short channel = RGB.R;
		private Bitmap channelImage;
		
		/// <summary>
		/// RGB channel to replace
		/// </summary>
		public short Channel
		{
			get { return channel; }
			set
			{
				if (
					( value != RGB.R ) &&
					( value != RGB.G ) &&
					( value != RGB.B )
					)
				{
					throw new ArgumentException( );
				}
				channel = value;
			}
		}

		/// <summary>
		/// Grayscale image to use for channel replacement
		/// </summary>
		public Bitmap ChannelImage
		{
			get { return channelImage; }
			set
			{
				// check for not null
				if ( value == null )
					throw new NullReferenceException( );
				// check for valid format
				if ( value.PixelFormat != PixelFormat.Format8bppIndexed )
					throw new ArgumentException( );

				channelImage = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReplaceChannel"/> class
		/// </summary>
		/// 
		/// <param name="channel">RGB channel to replace</param>
		/// <param name="channelImage">Channel image to use for replacement</param>
		/// 
		public ReplaceChannel( short channel, Bitmap channelImage )
		{
			this.Channel = channel;
			this.ChannelImage = channelImage;
		}

		/// <summary>
		/// Process the filter on the specified image
		/// </summary>
		/// 
		/// <param name="imageData">Image data</param>
		/// 
		protected override unsafe void ProcessFilter( BitmapData imageData )
		{
			int width	= imageData.Width;
			int height	= imageData.Height;
			int offset	= imageData.Stride - width * 3;

			// check channel's image dimension
			if ( ( width != channelImage.Width ) || ( height != channelImage.Height ) )
				throw new ArgumentException( "Channel image size does not match source image size" );

			// lock channel image
			BitmapData chData = channelImage.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			int offsetCh = chData.Stride - width;

			// do the job
			byte * dst = (byte *) imageData.Scan0.ToPointer( );
			byte * ch = (byte *) chData.Scan0.ToPointer( );

			// for each line
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, dst += 3, ch ++ )
				{
					dst[channel] = *ch;
				}
				dst += offset;
				ch += offsetCh;
			}
			// unlock
			channelImage.UnlockBits( chData );
		}
	}
}
