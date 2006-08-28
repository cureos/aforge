// AForge Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;

	/// <summary>
	/// Represents an integer range with min and max values
	/// </summary>
	public class IntRange
	{
		private int min, max;

		/// <summary>
		/// Min value
		/// </summary>
		public int Min
		{
			get { return min; }
			set { min = value; }
		}

		/// <summary>
		/// Max value
		/// </summary>
		public int Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Length of the range (deffirence between max and min values)
		/// </summary>
		public int Length
		{
			get { return max - min; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public IntRange( int min, int max )
		{
			this.min = min;
			this.max = max;
		}
	}
}
