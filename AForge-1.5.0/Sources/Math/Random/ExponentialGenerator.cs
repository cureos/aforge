// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Random
{
    using System;

    /// <summary>
    /// Exponential random numbers generator
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates exponential
    /// random numbers with specified rate value (lambda).</para>
    /// <para>The generator uses <see cref="UniformOneGenerator"/> generator
    /// to generate random numbers.</para></remarks>
    /// 
    public class ExponentialGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        private double rate = 0;

        /// <summary>
        /// Rate value (inverse scale)
        /// </summary>
        /// 
        /// <remarks>The rate value should be positive and non zero.</remarks>
        /// 
        public double Rate
        {
            get { return rate; }
        }

        /// <summary>
        /// Mean value of generator
        /// </summary>
        /// 
        public double Mean
        {
            get { return 1 / rate; }
        }

        /// <summary>
        /// Variance value of generator
        /// </summary>
        ///
        public double Variance
        {
            get { return 1 / ( rate * rate ); }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialGenerator"/> class
        /// </summary>
        /// 
        /// <param name="rate">Rate value</param>
        /// 
        public ExponentialGenerator( double rate ) :
            this( rate, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialGenerator"/> class
        /// </summary>
        /// 
        /// <param name="rate">Rate value</param>
        /// <param name="seed">Seed value to initialize random numbers generator</param>
        /// 
        public ExponentialGenerator( double rate, int seed )
        {
            // check rate value
            if ( rate <= 0 )
                throw new ArgumentException( "Rate value should be positive and non zero" );

            this.rand = new UniformOneGenerator( seed );
            this.rate = rate;
        }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number</returns>
        /// 
        public double Next( )
        {
            return - Math.Log( rand.Next( ) ) / rate;
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
            rand = new UniformOneGenerator( seed );
        }
    }
}
