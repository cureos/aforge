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
	/// Color filtering
	/// </summary>
	/// 
	/// <remarks>The filter filters pixels inside or outside specified color range</remarks>
	/// 
	public class ColorFiltering : FilterColorToColor
	{
		private IntRange red	= new IntRange( 0, 255 );
		private IntRange green	= new IntRange( 0, 255 );
		private IntRange blue	= new IntRange( 0, 255 );

		private byte fillR = 0;
		private byte fillG = 0;
		private byte fillB = 0;
		private bool fillOutsideRange = true;

		#region Public properties

		/// <summary>
		/// Range of red color component
		/// </summary>
		public IntRange Red
		{
			get { return red; }
			set { red = value; }
		}

		/// <summary>
		/// Range of green color component
		/// </summary>
		public IntRange Green
		{
			get { return green; }
			set { green = value; }
		}

		/// <summary>
		/// Range of blue color component
		/// </summary>
		public IntRange Blue
		{
			get { return blue; }
			set { blue = value; }
		}

		/// <summary>
		/// Fill color used to fill filtered pixels
		/// </summary>
		public RGB FillColor
		{
			get { return new RGB( fillR, fillG, fillB ); }
			set
			{
				fillR = value.Red;
				fillG = value.Green;
				fillB = value.Blue;
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

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorFiltering"/> class
		/// </summary>
		/// 
		public ColorFiltering( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ColorFiltering"/> class
		/// </summary>
		/// 
		/// <param name="red">Red components filtering range</param>
		/// <param name="green">Green components filtering range</param>
		/// <param name="blue">Blue components filtering range</param>
		/// 
		public ColorFiltering( IntRange red, IntRange green, IntRange blue )
		{
			this.red	= red;
			this.green	= green;
			this.blue	= blue;
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

			// do the job
			byte * ptr = (byte *) imageData.Scan0.ToPointer( );
			byte r, g, b;

			// for each row
			for ( int y = 0; y < height; y++ )
			{
				// for each pixel
				for ( int x = 0; x < width; x++, ptr += 3 )
				{
					r = ptr[RGB.R];
					g = ptr[RGB.G];
					b = ptr[RGB.B];

					// check pixel
					if (
						( r >= red.Min ) && ( r <= red.Max ) &&
						( g >= green.Min ) && ( g <= green.Max ) &&
						( b >= blue.Min ) && ( b <= blue.Max)
						)
					{
						if ( !fillOutsideRange )
						{
							ptr[RGB.R] = fillR;
							ptr[RGB.G] = fillG;
							ptr[RGB.B] = fillB;
						}
					}
					else
					{
						if ( fillOutsideRange )
						{
							ptr[RGB.R] = fillR;
							ptr[RGB.G] = fillG;
							ptr[RGB.B] = fillB;
						}
					}
				}
				ptr += offset;
			}
		}
	}
}
