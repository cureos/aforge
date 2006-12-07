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
	/// Hue modifier
	/// </summary>
	/// 
	/// <remarks>The filter operates in <b>HSL</b> color space and updates
	/// pixels hue value setting it the specified value.</remarks>
	/// 
	public class HueModifier : FilterColorToColor
	{
		private int hue = 0;

		/// <summary>
		/// Hue value to set in the range of [0, 359].
		/// </summary>
		public int Hue
		{
			get { return hue; }
			set { hue = Math.Max( 0, Math.Min( 359, value ) ); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HueModifier"/> class
		/// </summary>
		/// 
		public HueModifier( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="HueModifier"/> class
		/// </summary>
		/// 
		/// <param name="hue">Hue value to set</param>
		/// 
		public HueModifier( int hue )
		{
			this.hue = hue;
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

			RGB rgb = new RGB( );
			HSL hsl = new HSL( );

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

					// modify hue value
					hsl.Hue = hue;

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
