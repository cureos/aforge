// AForge Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;

	/// <summary>
	/// Represents an integer range with minimum and maximum values
	/// </summary>
	public class IntRange
	{
		private int min, max;

		/// <summary>
		/// Minimum value
		/// </summary>
		public int Min
		{
			get { return min; }
			set { min = value; }
		}

		/// <summary>
		/// Maximum value
		/// </summary>
		public int Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Length of the range (deffirence between maximum and minimum values)
		/// </summary>
		public int Length
		{
			get { return max - min; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// 
		/// <param name="min">Minimum value of the range</param>
		/// <param name="max">Maximum value of the range</param>
		public IntRange( int min, int max )
		{
			this.min = min;
			this.max = max;
		}
	}
}
