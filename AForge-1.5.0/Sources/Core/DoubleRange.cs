// AForge Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;

	/// <summary>
	/// Represents a double range with minimum and maximum values
	/// </summary>
	public class DoubleRange
	{
		private double min, max;

		/// <summary>
		/// Minimum value
		/// </summary>
		public double Min
		{
			get { return min; }
			set { min = value; }
		}

		/// <summary>
		/// Maximum value
		/// </summary>
		public double Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Length of the range (deffirence between maximum and minimum values)
		/// </summary>
		public double Length
		{
			get { return max - min; }
		}

		
		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleRange"/> class
		/// </summary>
		/// 
		/// <param name="min">Minimum value of the range</param>
		/// <param name="max">Maximum value of the range</param>
		public DoubleRange( double min, double max )
		{
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Check if the specified value is inside this range
		/// </summary>
		/// 
		/// <param name="x">Value to check</param>
		/// 
		/// <returns><b>True</b> if the specified value is inside this range or
		/// <b>false</b> otherwise.</returns>
		/// 
		public bool IsInside( double x )
		{
			return ( ( x >= min ) && ( x <= max ) );
		}

		/// <summary>
		/// Check if the specified range is inside this range
		/// </summary>
		/// 
		/// <param name="range">Range to check</param>
		/// 
		/// <returns><b>True</b> if the specified range is inside this range or
		/// <b>false</b> otherwise.</returns>
		/// 
		public bool IsInside( DoubleRange range )
		{
			return ( ( IsInside( range.min ) ) && ( IsInside( range.max ) ) );
		}

		/// <summary>
		/// Check if the specified range overlaps with this range
		/// </summary>
		/// 
		/// <param name="range">Range to check for overlapping</param>
		/// 
		/// <returns><b>True</b> if the specified range overlaps with this range or
		/// <b>false</b> otherwise.</returns>
		/// 
		public bool IsOverlapping( DoubleRange range )
		{
			return ( ( IsInside( range.min ) ) || ( IsInside( range.max ) ) );
		}
	}
}
