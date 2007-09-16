// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Random
{
    using System;

    /// <summary>
    /// Gaussian random numbers generator
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates gaussian
    /// random numbers with specified mean and standard deviation values.</para>
    /// <para>The generator uses <see cref="StandardGenerator"/> generator
    /// to generate random numbers.</para></remarks>
    /// 
    public class GaussianGenerator : IRandomNumberGenerator
    {
        // standard numbers generator
        private StandardGenerator rand = null;
        // mean value
        private double mean;
        // standard deviation value
        private double stdDev;

        /// <summary>
        /// Mean value of generator
        /// </summary>
        ///
        public double Mean
        {
            get { return mean;  }
        }

        /// <summary>
        /// Variance value of generator
        /// </summary>
        ///
        public double Variance
        {
            get { return stdDev * stdDev; }
        }

        /// <summary>
        /// Standard deviation value
        /// </summary>
        ///
        public double StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class
        /// </summary>
        /// 
        /// <param name="mean">Mean value</param>
        /// <param name="stdDev">Standard deviation value</param>
        /// 
        public GaussianGenerator( double mean, double stdDev ) :
            this( mean, stdDev, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class
        /// </summary>
        /// 
        /// <param name="mean">Mean value</param>
        /// <param name="stdDev">Standard deviation value</param>
        /// <param name="seed">Seed value to initialize random numbers generator</param>
        /// 
        public GaussianGenerator( double mean, double stdDev, int seed )
        {
            this.mean   = mean;
            this.stdDev = stdDev;

            rand = new StandardGenerator( );
        }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number</returns>
        /// 
        public double Next( )
        {
            return rand.Next( ) * stdDev + mean;
        }

        /// <summary>
        /// Set seed of the random numbers generator
        /// </summary>
        /// 
        /// <param name="seed">Seed value</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed( int seed )
        {
            rand = new StandardGenerator( seed );
        }
    }
}
