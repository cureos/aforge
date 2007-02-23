// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
    using System;

    /// <summary>
    /// Histogram for continuous random values
    /// </summary>
    /// 
    /// <remarks>The class also works with integer arrays as <see cref="Histogram"/> class.
    /// But it also takes as parameter a range of random values. ... </remarks>
    /// 
    public class ContinuousHistogram
    {
		private int[] values;
		private DoubleRange	range;

		private double	mean;
		private double	stdDev;
		private double	median;
		private double	min;
		private double	max;
		private int		total;

        /// <summary>
        /// Values of the histogram
        /// </summary>
        /// 
        public int[] Values
		{
			get { return values; }
		}

        /// <summary>
        /// Range of random values
        /// </summary>
        /// 
        public DoubleRange Range
		{
			get { return range; }
		}

        /// <summary>
        /// Mean value
        /// </summary>
        /// 
        public double Mean
		{
			get { return mean; }
		}

        /// <summary>
        /// Standard deviation
        /// </summary>
        /// 
        public double StdDev
		{
			get { return stdDev; }
		}

        /// <summary>
        /// Median value
        /// </summary>
        /// 
        public double Median
		{
			get { return median; }
		}

        /// <summary>
        /// Minimum value
        /// </summary>
        /// 
        /// <remarks>Minimum value of the histogram with non zero
        /// hits count.</remarks>
        /// 
        public double Min
		{
			get { return min; }
		}

        /// <summary>
        /// Maximum value
        /// </summary>
        /// 
        /// <remarks>Maximum value of the histogram with non zero
        /// hits count.</remarks>
        /// 
        public double Max
		{
			get { return max; }
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousHistogram"/> class
        /// </summary>
        /// 
        /// <param name="values">Values of the histogram</param>
        /// <param name="range">Range of random values</param>
        /// 
        public ContinuousHistogram( int[] values, DoubleRange range )
		{
			this.values = values;
			this.range  = range;

            int hits;
			int i, n = values.Length;
			int nM1 = n - 1;

            double rangeLength = range.Length;
            double rangeMin = range.Min;

			max = 0;
			min = n;

			// calculate mean, min, max
			for ( i = 0; i < n; i++ )
			{
                hits = values[i];

                if ( hits != 0 )
				{
					// max
					if ( i > max )
						max = i;
					// min
					if ( i < min )
						min = i;
				}

				// accumulate total value
                total += hits;
				// accumulate mean value
                mean += ( ( (double) i / nM1 ) * rangeLength + rangeMin ) * hits;
			}
			mean /= total;

            min = ( min / nM1 ) * rangeLength + rangeMin;
            max = ( max / nM1 ) * rangeLength + rangeMin;

			// calculate stadard deviation
			for ( i = 0; i < n; i++ )
			{
				hits = values[i];
				stdDev += Math.Pow( ( ( (double) i / nM1 ) * rangeLength + rangeMin ) - mean, 2 ) * hits;
			}
			stdDev = Math.Sqrt( stdDev / total );
			
			// calculate median
			int m, halfTotal = total / 2;

			for ( m = 0, hits = 0; m < n; m++ )
			{
				hits += values[m];
				if ( hits >= halfTotal )
					break;
			}
			median = ( (double) m / nM1 ) * rangeLength + rangeMin;
		}

        /// <summary>
        /// Get range around median containing specified percentage of values
        /// </summary>
        /// 
        /// <param name="percent">Values percentage around median</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage
        /// of values.</returns>
        /// 
		public DoubleRange GetRange( double percent )
		{
			int min, max, hits;
			int h = (int)( total * ( percent + ( 1 - percent ) / 2 ) );
			int n = values.Length;
			int nM1 = n - 1;

			// skip left portion
			for ( min = 0, hits = total; min < n; min++ )
			{
				hits -= values[min];
				if ( hits < h )
					break;
			}
			// skip right portion
			for ( max = nM1, hits = total;  max >= 0; max-- )
			{
				hits -= values[max];
				if ( hits < h )
					break;
			}
			// return range between left and right boundaries
			return new DoubleRange(
                ( (double) min / nM1 ) * range.Length + range.Min,
                ( (double) max / nM1 ) * range.Length + range.Min );
		}
    }
}
