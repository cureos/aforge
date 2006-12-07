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
	/// Color filtering in HSL color space
	/// </summary>
	/// 
	/// <remarks>The filter operates in <b>HSL</b> color space and filters
	/// pixels, which color is inside or outside specified HSL range.</remarks>
	/// 
	public class HSLFiltering : FilterColorToColor
	{
		private IntRange hue			= new IntRange( 0, 359 );
		private DoubleRange saturation	= new DoubleRange( 0.0, 1.0 );
		private DoubleRange luminance	= new DoubleRange( 0.0, 1.0 );

		private int fillH = 0;
		private double fillS = 0.0;
		private double fillL = 0.0;
		private bool fillOutsideRange = true;

		private bool updateH = true;
		private bool updateS = true;
		private bool updateL = true;

		#region Public properties

		/// <summary>
		/// Range of hue component
		/// </summary>
		/// 
		/// <remarks>Because of hue values are cycled, the minimum value of the hue
		/// range may have bigger integer value then the maximum value.</remarks>
		/// 
		public IntRange Hue
		{
			get { return hue; }
			set { hue = value; }
		}

		/// <summary>
		/// Range of saturation component
		/// </summary>
		public DoubleRange Saturation
		{
			get { return saturation; }
			set { saturation = value; }
		}

		/// <summary>
		/// Range of luminance component
		/// </summary>
		public DoubleRange Luminance
		{
			get { return luminance; }
			set { luminance = value; }
		}

		/// <summary>
		/// Fill color used to fill filtered pixels
		/// </summary>
		public HSL FillColor
		{
			get { return new HSL( fillH, fillS, fillL ); }
			set
			{
				fillH = value.Hue;
				fillS = value.Saturation;
				fillL = value.Luminance;
			}
		}

		/// <summary>
		/// Determines, if pixels should be filled inside or outside specified
		/// color range
		/// </summary>
		public bool FillOutsideRange
		{
			get { return fillOutsideRange; }
			set { fillOutsideRange = value; }
		}

		/// <summary>
		/// Determines, if hue value of filtered pixels should be updated.
		/// <b>True</b> by default.
		/// </summary>
		public bool UpdateHue
		{
			get { return updateH; }
			set { updateH = value; }
		}

		/// <summary>
		/// Determines, if saturation value of filtered pixels should be updated
		/// <b>True</b> by default.
		/// </summary>
		public bool UpdateSaturation
		{
			get { return updateS; }
			set { updateS = value; }
		}

		/// <summary>
		/// Determines, if luminance value of filtered pixels should be updated
		/// <b>True</b> by default.
		/// </summary>
		public bool UpdateLuminance
		{
			get { return updateL; }
			set { updateL = value; }
		}

		#endregion

		
		/// <summary>
		/// Initializes a new instance of the <see cref="HSLFiltering"/> class
		/// </summary>
		public HSLFiltering( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="HSLFiltering"/> class
		/// </summary>
		/// 
		/// <param name="hue">Range of hue component</param>
		/// <param name="saturation">Range of saturation component</param>
		/// <param name="luminance">Range of luminance component</param>
		/// 
		public HSLFiltering( IntRange hue, DoubleRange saturation, DoubleRange luminance )
		{
			this.hue		= hue;
			this.saturation	= saturation;
			this.luminance	= luminance;
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

			bool updated;

			// do the job
			byte *ptr = (byte *) imageData.Scan0.ToPointer( );

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					updated		= false;
					rgb.Red		= ptr[RGB.R];
					rgb.Green	= ptr[RGB.G];
					rgb.Blue	= ptr[RGB.B];

					// convert to HSL
					AForge.Imaging.ColorConverter.RGB2HSL( rgb, hsl );

					// check HSL values
					if (
						( hsl.Saturation >= saturation.Min ) && ( hsl.Saturation <= saturation.Max ) &&
						( hsl.Luminance >= luminance.Min ) && ( hsl.Luminance <= luminance.Max ) &&
						(
						( ( hue.Min < hue.Max ) && ( hsl.Hue >= hue.Min ) && ( hsl.Hue <= hue.Max ) ) ||
						( ( hue.Min > hue.Max ) && ( ( hsl.Hue >= hue.Min ) || ( hsl.Hue <= hue.Max ) ) )
						)
						)
					{
						if ( !fillOutsideRange )
						{
							if ( updateH ) hsl.Hue			= fillH;
							if ( updateS ) hsl.Saturation	= fillS;
							if ( updateL ) hsl.Luminance	= fillL;

							updated = true;
						}
					}
					else
					{
						if ( fillOutsideRange )
						{
							if ( updateH ) hsl.Hue			= fillH;
							if ( updateS ) hsl.Saturation	= fillS;
							if ( updateL ) hsl.Luminance	= fillL;

							updated = true;
						}
					}

					if ( updated )
					{
						// convert back to RGB
						AForge.Imaging.ColorConverter.HSL2RGB( hsl, rgb );

						ptr[RGB.R] = rgb.Red;
						ptr[RGB.G] = rgb.Green;
						ptr[RGB.B] = rgb.Blue;
					}
				}
				ptr += offset;
			}
		}
	}
}
