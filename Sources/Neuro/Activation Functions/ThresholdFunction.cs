// AForge Neural Net Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
    using System;

    /// <summary>
    /// Threshold activation function.
    /// </summary>
    ///
    /// <remarks>The class represents threshold activation function with
    /// the next expression:<br />
    /// <code>
    /// f(x) = 1, if x >= 0, otherwise 0
    /// </code>
    /// Output range of the function: <b>[0, 1]</b><br /><br />
    /// Functions graph:<br />
    /// <img src="threshold.bmp" width="242" height="172" />
    /// </remarks>
    ///
    [Serializable]
    public class ThresholdFunction : IActivationFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThresholdFunction"/> class.
        /// </summary>
        public ThresholdFunction( ) { }

        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <b>x</b>.</remarks>
        ///
        public double Function( double x )
        {
            return ( x >= 0 ) ? 1 : 0;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// 
        /// <param name="x">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</remarks>
        ///
        public double Derivative( double x )
        {
            double y = Function( x );

            return 0;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// 
        /// <param name="y">Input value.</param>
        /// 
        /// <returns>Always returns 0.</returns>
        /// 
        /// <remarks>The method is not supported, because it is not possible to
        /// calculate derivative of the function.</remarks>
        /// 
        public double Derivative2( double y )
        {
            return 0;
        }
    }
}
