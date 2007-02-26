// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Random
{
    using System;
    using AForge;

    /// <summary>
    /// Uniform random numbers generator
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates unformaly
    /// distributed numbers in the specified range.</para>
    /// <para>The generator uses <see cref="UniformOneGenerator"/> generator
    /// to generate random numbers.</para></remarks>
    /// 
    public class UniformGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        // generator's range
        private double min;
        private double length;

        /// <summary>
        /// Random numbers range
        /// </summary>
        /// 
        public DoubleRange Range
        {
            get { return new DoubleRange( min, min + length ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class
        /// </summary>
        /// 
        /// <param name="range">Random numbers range</param>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformGenerator( DoubleRange range ) :
            this( range, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class
        /// </summary>
        /// 
        /// <param name="range">Random numbers range</param>
        /// <param name="seed">Seed value to initialize random numbers generator</param>
        /// 
        public UniformGenerator( DoubleRange range, int seed )
        {
            rand = new UniformOneGenerator( seed );

            min     = range.Min;
            length  = range.Length;
        }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number</returns>
        /// 
        public double Next( )
        {
            return rand.Next( ) * length + min;
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
