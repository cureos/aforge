// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Math
{
    using System;

    /// <summary>
    /// Histogram for continuous random values.
    /// </summary>
    /// 
    /// <remarks><para>The class wraps histogram for continuous stochastic function, which is represented
    /// by integer array and range of the function. Values of the integer array are treated
    /// as total amount of hits on the corresponding subranges, which are calculated by splitting the
    /// specified range into required amount of consequent ranges.</para>
    /// 
    /// <para>For example, if the integer array is equal to { 1, 2, 4, 8, 16 } and the range is set
    /// to [0, 1], then the histogram consists of next subranges:
    /// <list type="bullet">
    /// <item>[0.0, 0.2] - 1 hit;</item>
    /// <item>[0.2, 0.4] - 2 hits;</item>
    /// <item>[0.4, 0.6] - 4 hits;</item>
    /// <item>[0.6, 0.8] - 8 hits;</item>
    /// <item>[0.8, 1.0] - 16 hits.</item>
    /// </list>
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create histogram
    /// ContinuousHistogram histogram = new ContinuousHistogram(
    ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
    /// // get mean and standard deviation values
    /// System.Diagnostics.Debug.WriteLine( "mean = " + histogram.Mean + ", std.dev = " + histogram.StdDev );
    /// </code>
    /// </remarks>
    /// 
    public class ContinuousHistogram
    {
        private int[] values;
        private DoubleRange range;

        private double  mean;
        private double  stdDev;
        private double  median;
        private double  min;
        private double  max;
        private int     total;

        /// <summary>
        /// Values of the histogram.
        /// </summary>
        /// 
        public int[] Values
        {
            get { return values; }
        }

        /// <summary>
        /// Range of random values.
        /// </summary>
        /// 
        public DoubleRange Range
        {
            get { return range; }
        }

        /// <summary>
        /// Mean value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve mean value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get mean value (= 0.505 )
        /// System.Diagnostics.Debug.WriteLine( "mean = " + histogram.Mean.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double Mean
        {
            get { return mean; }
        }

        /// <summary>
        /// Standard deviation.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve standard deviation value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get std.dev. value (= 0.215)
        /// System.Diagnostics.Debug.WriteLine( "std.dev. = " + histogram.StdDev.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Median value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve median value of the histogram.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get median value (= 0.500)
        /// System.Diagnostics.Debug.WriteLine( "median = " + histogram.Median.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double Median
        {
            get { return median; }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve minimum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get min value (= 0.250)
        /// System.Diagnostics.Debug.WriteLine( "min = " + histogram.Min.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        public double Min
        {
            get { return min; }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// 
        /// <remarks><para>The property allows to retrieve maximum value of the histogram with non zero
        /// hits count.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get max value (= 0.875)
        /// System.Diagnostics.Debug.WriteLine( "max = " + histogram.Max.ToString( "F3" ) );
        /// </code>
        /// </remarks>
        /// 
        public double Max
        {
            get { return max; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousHistogram"/> class.
        /// </summary>
        /// 
        /// <param name="values">Values of the histogram.</param>
        /// <param name="range">Range of random values.</param>
        /// 
        /// <remarks>Values of the integer array are treated as total amount of hits on the
        /// corresponding subranges, which are calculated by splitting the specified range into
        /// required amount of consequent ranges (see <see cref="ContinuousHistogram"/> class
        /// description for more information).
        /// </remarks>
        /// 
        public ContinuousHistogram( int[] values, DoubleRange range )
        {
            this.values = values;
            this.range  = range;

            Update( );
        }

        /// <summary>
        /// Get range around median containing specified percentage of values.
        /// </summary>
        /// 
        /// <param name="percent">Values percentage around median.</param>
        /// 
        /// <returns>Returns the range which containes specifies percentage of values.</returns>
        /// 
        /// <remarks><para>The method calculates range of stochastic variable, which summary probability
        /// comprises the specified percentage of histogram's hits.</para>
        /// 
        /// <para>Sample usage:</para>
        /// <code>
        /// // create histogram
        /// ContinuousHistogram histogram = new ContinuousHistogram(
        ///     new int[] { 0, 0, 8, 4, 2, 4, 7, 1, 0 }, new DoubleRange( 0.0, 1.0 ) );
        /// // get 50% range
        /// DoubleRange range = histogram.GetRange( 0.5 );
        /// // show the range ([0.25, 0.75])
        /// System.Diagnostics.Debug.WriteLine( "50% range = [" + range.Min + ", " + range.Max + "]" );
        /// </code>
        /// </remarks>
        /// 
        public DoubleRange GetRange( double percent )
        {
            int min, max, hits;
            int h = (int) ( total * ( percent + ( 1 - percent ) / 2 ) );
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
            for ( max = nM1, hits = total; max >= 0; max-- )
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
            int hits;
            int i, n = values.Length;
            int nM1 = n - 1;

            double rangeLength = range.Length;
            double rangeMin = range.Min;

            max    = 0;
            min    = n;
            mean   = 0;
            stdDev = 0;
            total  = 0;

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
    }
}
