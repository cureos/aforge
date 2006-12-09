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
	/// Linear correction of YCbCr channels
	/// </summary>
	/// 
	/// <remarks>The filter operates in <b>YCbCr</b> color space and provides
	/// with the facility of linear correction of its channels.</remarks>
	/// 
	public class YCbCrLinear : FilterColorToColor
	{
		private DoubleRange inY = new DoubleRange( 0.0, 1.0 );
		private DoubleRange inCb = new DoubleRange( -0.5, 0.5 );
		private DoubleRange inCr = new DoubleRange( -0.5, 0.5 );

		private DoubleRange outY = new DoubleRange( 0.0, 1.0 );
		private DoubleRange outCb = new DoubleRange( -0.5, 0.5 );
		private DoubleRange outCr = new DoubleRange( -0.5, 0.5 );

		#region Public Propertis

		/// <summary>
		/// Y component's input range
		/// </summary>
		/// 
		/// <remarks>Y component is measured in the range of [0, 1].</remarks>
		///
		public DoubleRange InY
		{
			get { return inY; }
			set { inY = value; }
		}

		/// <summary>
		/// Cb component's input range
		/// </summary>
		/// 
		/// <remarks>Cb component is measured in the range of [-0.5, 0.5].</remarks>
		///
		public DoubleRange InCb
		{
			get { return inCb; }
			set { inCb = value; }
		}

		/// <summary>
		/// Cr component's input range
		/// </summary>
		/// 
		/// <remarks>Cr component is measured in the range of [-0.5, 0.5].</remarks>
		///
		public DoubleRange InCr
		{
			get { return inCr; }
			set { inCr = value; }
		}

		/// <summary>
		/// Y component's output range
		/// </summary>
		/// 
		/// <remarks>Y component is measured in the range of [0, 1].</remarks>
		///
		public DoubleRange OutY
		{
			get { return outY; }
			set { outY = value; }
		}

		/// <summary>
		/// Cb component's output range
		/// </summary>
		/// 
		/// <remarks>Cb component is measured in the range of [-0.5, 0.5].</remarks>
		///
		public DoubleRange OutCb
		{
			get { return outCb; }
			set { outCb = value; }
		}

		/// <summary>
		/// Cr component's output range
		/// </summary>
		/// 
		/// <remarks>Cr component is measured in the range of [-0.5, 0.5].</remarks>
		///
		public DoubleRange OutCr
		{
			get { return outCr; }
			set { outCr = value; }
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

			RGB		rgb = new RGB( );
			YCbCr	ycbcr = new YCbCr( );

			double	ky  = 0, by  = 0;
			double	kcb = 0, bcb = 0;
			double	kcr = 0, bcr = 0;

			// Y line parameters
			if ( inY.Max != inY.Min )
			{
				ky = ( outY.Max - outY.Min ) / ( inY.Max - inY.Min );
				by = outY.Min - ky * inY.Min;
			}
			// Cb line parameters
			if ( inCb.Max != inCb.Min )
			{
				kcb = ( outCb.Max - outCb.Min ) / ( inCb.Max - inCb.Min );
				bcb = outCb.Min - kcb * inCb.Min;
			}
			// Cr line parameters
			if ( inCr.Max != inCr.Min )
			{
				kcr = ( outCr.Max - outCr.Min ) / ( inCr.Max - inCr.Min );
				bcr = outCr.Min - kcr * inCr.Min;
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

					// convert to YCbCr
					AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

					// correct Y
					if ( ycbcr.Y >= inY.Max )
						ycbcr.Y = outY.Max;
					else if ( ycbcr.Y <= inY.Min )
						ycbcr.Y = outY.Min;
					else
						ycbcr.Y = ky * ycbcr.Y + by;

					// correct Cb
					if ( ycbcr.Cb >= inCb.Max )
						ycbcr.Cb = outCb.Max;
					else if ( ycbcr.Cb <= inCb.Min )
						ycbcr.Cb = outCb.Min;
					else
						ycbcr.Cb = kcb * ycbcr.Cb + bcb;

					// correct Cr
					if ( ycbcr.Cr >= inCr.Max )
						ycbcr.Cr = outCr.Max;
					else if ( ycbcr.Cr <= inCr.Min )
						ycbcr.Cr = outCr.Min;
					else
						ycbcr.Cr = kcr * ycbcr.Cr + bcr;

					// convert back to RGB
					AForge.Imaging.ColorConverter.YCbCr2RGB( ycbcr, rgb );

					ptr[RGB.R] = rgb.Red;
					ptr[RGB.G] = rgb.Green;
					ptr[RGB.B] = rgb.Blue;
				}
				ptr += offset;
			}
		}
	}
}
