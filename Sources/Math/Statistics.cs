// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;
    using AForge;

	/// <summary>
	/// Set of statistics functions
	/// </summary>
    /// 
    /// <remarks>The class represents collection of functions used
    /// in statistics</remarks>
    /// 
	public class Statistics
	{
        /// <summary>
        /// Calculate mean value
        /// </summary>
        /// 
        /// <param name="values">Histogram array</param>
        /// 
        /// <returns>Returns mean value</returns>
        /// 
        /// <remarks>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</remarks>
        /// 
		public static double Mean( int[] values )
		{
			int hits;
			int mean = 0;
			int total = 0;

			// for all values
			for ( int i = 0, n = values.Length; i < n; i++ )
			{
                hits = values[i];
				// accumulate mean
                mean += i * hits;
				// accumalate total
                total += hits;
			}
			return (double) mean / total;
		}

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// 
        /// <param name="values">Histogram array</param>
        /// 
        /// <returns>Returns value of standard deviation</returns>
        /// 
        /// <remarks>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</remarks>
        /// 
        public static double StdDev( int[] values )
		{
			double  mean = Mean( values );
			double  stddev = 0;
			double	centeredValue;
            int     hits;
			int		total = 0;

			// for all values
			for ( int i = 0, n = values.Length; i < n; i++ )
			{
                hits = values[i];
                centeredValue = (double) i - mean;

				// accumulate mean
                stddev += centeredValue * centeredValue * hits;
				// accumalate total
                total += hits;
			}

			return Math.Sqrt( stddev / total );
		}

        /// <summary>
        /// Calculate median value
        /// </summary>
        /// 
        /// <param name="values">Histogram array</param>
        /// 
        /// <returns>Returns value of median</returns>
        /// 
        /// <remarks>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</remarks>
        /// 
        public static int Median( int[] values )
		{
			int total = 0, n = values.Length;

			// for all values
			for ( int i = 0; i < n; i++ )
			{
				// accumalate total
				total += values[i];
			}

			int halfTotal = total / 2;
			int median = 0, v = 0;

			// find median value
			for ( ; median < n; median++ )
			{
				v += values[median];
				if ( v >= halfTotal )
					break;
			}

			return median;
		}

        /// <summary>
        /// Get range around median containing specified percentage of values
        /// </summary>
        /// 
        /// <param name="values">Histogram array</param>
        /// <param name="percent">Values percentage around median</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage
        /// of values.</returns>
        /// 
		public static IntRange GetRange( int[] values, double percent )
		{
			int total = 0, n = values.Length;

			// for all values
			for ( int i = 0; i < n; i++ )
			{
				// accumalate total
				total += values[i];
			}

			int min, max, hits;
			int h = (int)( total * ( percent + ( 1 - percent ) / 2 ) );

			// get range min value
            for ( min = 0, hits = total; min < n; min++ )
			{
                hits -= values[min];
                if ( hits < h )
					break;
			}
			// get range max value
            for ( max = n - 1, hits = total; max >= 0; max-- )
			{
                hits -= values[max];
                if ( hits < h )
					break;
			}
			return new IntRange( min, max );
		}

        /// <summary>
        /// Calculate an entropy
        /// </summary>
        /// 
        /// <param name="values">Histogram array</param>
        /// 
        /// <returns>Returns entropy value</returns>
        /// 
        /// <remarks>The input array is treated as histogram, i.e. its
        /// indexes are treated as values of stochastic function, but
        /// array values are treated as "probabilities" (total amount of
        /// hits).</remarks>
        /// 
        public static double Entropy( int[] values )
		{
            int     n = values.Length;
            int     total = 0;
            double  entropy = 0;
            double  p;

            // calculate total amount of hits
			for ( int i = 0; i < n; i++ )
			{
				total += values[i];
			}

            // for all values
            for ( int i = 0; i < n; i++ )
            {
                // get item's probability
                p = (double) values[i] / total;
                // calculate entropy
                if ( p != 0 )
                    entropy += ( -p * Math.Log( p, 2 ) );
            }
            return entropy;
		}
	}
}
