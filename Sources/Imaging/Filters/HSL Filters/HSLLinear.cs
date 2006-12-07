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
	using AForge;

	/// <summary>
	/// Luminance and saturation linear correction
	/// </summary>
	/// 
	/// <remarks>The filter operates in <b>HSL</b> color space and provides
	/// with the facility of luminance and saturation linear correction.</remarks>
	/// 
	public class HSLLinear : FilterColorToColor
	{
		private DoubleRange inLuminance = new DoubleRange( 0.0, 1.0 );
		private DoubleRange inSaturation = new DoubleRange( 0.0, 1.0 );
		private DoubleRange outLuminance = new DoubleRange( 0.0, 1.0 );
		private DoubleRange outSaturation = new DoubleRange( 0.0, 1.0 );

		#region Public Propertis

		/// <summary>
		/// Luminance input range
		/// </summary>
		public DoubleRange InLuminance
		{
			get { return inLuminance; }
			set { inLuminance = value; }
		}

		/// <summary>
		/// Luminance output range
		/// </summary>
		public DoubleRange OutLuminance
		{
			get { return outLuminance; }
			set { outLuminance = value; }
		}

		/// <summary>
		/// Saturation input range
		/// </summary>
		public DoubleRange InSaturation
		{
			get { return inSaturation; }
			set { inSaturation = value; }
		}

		/// <summary>
		/// Saturation output range
		/// </summary>
		public DoubleRange OutSaturation
		{
			get { return outSaturation; }
			set { outSaturation = value; }
		}

		#endregion


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

			RGB rgb = new RGB( );
			HSL hsl = new HSL( );

			double	kl = 0, bl = 0;
			double	ks = 0, bs = 0;

			// luminance line parameters
			if ( inLuminance.Max != inLuminance.Min )
			{
				kl = ( outLuminance.Max - outLuminance.Min ) / ( inLuminance.Max - inLuminance.Min );
				bl = outLuminance.Min - kl * inLuminance.Min;
			}
			// saturation line parameters
			if ( inSaturation.Max != inSaturation.Min )
			{
				ks = ( outSaturation.Max - outSaturation.Min ) / ( inSaturation.Max - inSaturation.Min );
				bs = outSaturation.Min - ks * inSaturation.Min;
			}

			// do the job
			byte *ptr = (byte *) imageData.Scan0.ToPointer( );

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					rgb.Red		= ptr[RGB.R];
					rgb.Green	= ptr[RGB.G];
					rgb.Blue	= ptr[RGB.B];

					// convert to HSL
					AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

					// do luminance correction
					if ( hsl.Luminance >= inLuminance.Max )
						hsl.Luminance = outLuminance.Max;
					else if ( hsl.Luminance <= inLuminance.Min )
						hsl.Luminance = outLuminance.Min;
					else
						hsl.Luminance = kl * hsl.Luminance + bl;

					// do saturation correct correction
					if ( hsl.Saturation >= inSaturation.Max )
						hsl.Saturation = outSaturation.Max;
					else if ( hsl.Saturation <= inSaturation.Min )
						hsl.Saturation = outSaturation.Min;
					else
						hsl.Saturation = ks * hsl.Saturation + bs;

					// convert back to RGB
					AForge.Imaging.ColorConverter.HSL2RGB( hsl, rgb );

					ptr[RGB.R] = rgb.Red;
					ptr[RGB.G] = rgb.Green;
					ptr[RGB.B] = rgb.Blue;
				}
				ptr += offset;
			}
		}
	}
}
