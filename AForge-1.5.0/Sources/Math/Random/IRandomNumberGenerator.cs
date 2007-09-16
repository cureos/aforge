// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Random
{
    using System;

    /// <summary>
    /// Interface for random numbers generators
    /// </summary>
    /// 
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Mean value of generator
        /// </summary>
        /// 
        double Mean { get; }

        /// <summary>
        /// Variance value of generator
        /// </summary>
        /// 
        double Variance { get; }

        /// <summary>
        /// Generate next random number
        /// </summary>
        /// 
        /// <returns>Returns next random number</returns>
        /// 
        double Next( );

        /// <summary>
        /// Set seed of the random numbers generator
        /// </summary>
        /// 
        /// <param name="seed">Seed value</param>
        /// 
        void SetSeed( int seed );
    }
}
