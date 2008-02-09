// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
    using System;

    /// <summary>
    /// Histogram for discrete random values.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class Histogram
    {
        private int[]   values;
        private double  mean = 0;
        private double  stdDev = 0;
        private int     median = 0;
        private int     min;
        private int     max;

        /// <summary>
        /// Values of the histogram.
        /// </summary>
        /// 
        public int[] Values
        {
            get { return values; }
        }

        /// <summary>
        /// Mean value.
        /// </summary>
        /// 
        public double Mean
        {
            get { return mean; }
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// 
        public double StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Median value.
        /// </summary>
        /// 
        public int Median
        {
            get { return median; }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// 
        /// <remarks>Minimum value of the histogram with non zero
        /// hits count.</remarks>
        /// 
        public int Min
        {
            get { return min; }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// 
        /// <remarks>Maximum value of the histogram with non zero
        /// hits count.</remarks>
        /// 
        public int Max
        {
            get { return max; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Histogram"/> class.
        /// </summary>
        /// 
        /// <param name="values">Values of the histogram.</param>
        /// 
        public Histogram( int[] values )
        {
            this.values = values;
            Update( );
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage
        /// of values.</returns>
        /// 
        public IntRange GetRange( double percent )
        {
            return Statistics.GetRange( values, percent );
        }

        /// <summary>
        /// Update statistical value of the histogram.
        /// </summary>
        /// 
        /// <remarks>The method recalculates statistical values of the histogram, like mean,
        /// standard deviation, etc. The method should be called only in the case if histogram
        /// values were retrieved through <see cref="Values"/> property and updated after that.
        /// </remarks>
        /// 
        public void Update( )
        {
            int i, n = values.Length;

            max = 0;
            min = n;

            // calculate min and max
            for ( i = 0; i < n; i++ )
            {
                if ( values[i] != 0 )
                {
                    // max
                    if ( i > max )
                        max = i;
                    // min
                    if ( i < min )
                        min = i;
                }
            }

            mean   = Statistics.Mean( values );
            stdDev = Statistics.StdDev( values );
            median = Statistics.Median( values );
        }
    }
}
