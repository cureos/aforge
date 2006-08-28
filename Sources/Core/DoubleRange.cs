// AForge Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;

	/// <summary>
	/// Represents a double range with min and max values
	/// </summary>
	public class DoubleRange
	{
		private double min, max;

		/// <summary>
		/// Min value
		/// </summary>
		public double Min
		{
			get { return min; }
			set { min = value; }
		}

		/// <summary>
		/// Max value
		/// </summary>
		public double Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Length of the range (deffirence between max and min values)
		/// </summary>
		public double Length
		{
			get { return max - min; }
		}

		
		/// <summary>
		/// Constructor
		/// </summary>
		public DoubleRange( double min, double max )
		{
			this.min = min;
			this.max = max;
		}
	}
}
