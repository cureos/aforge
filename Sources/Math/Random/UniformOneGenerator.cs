// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Random
{
    using System;

    /// <summary>
    /// Uniform random numbers generator in the range of [0, 1]
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates unformaly
    /// distributed numbers in the range of [0, 1].</para>
    /// <para><b>Note</b>: At this point the generator is based on the
    /// internal .NET generator, but is going to be rewriten to
    /// use faster generation algorithm.</para></remarks>
    /// 
    public class UniformOneGenerator : IRandomNumberGenerator
    {
        // .NET random generator as a base
        private Random rand = null;

        /// <summary>
		/// Initializes a new instance of the <see cref="UniformOneGenerator"/> class
        /// </summary>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformOneGenerator( )
        {
            rand = new Random( 0 );
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="UniformOneGenerator"/> class
        /// </summary>
        /// 
        /// <param name="seed">Seed value to initialize random numbers generator</param>
        /// 
        public UniformOneGenerator( int seed )
        {
            rand = new Random( seed );
        }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number</returns>
        /// 
        public double Next( )
        {
            return rand.NextDouble( );
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
            rand = new Random( seed );
        }
    }
}
