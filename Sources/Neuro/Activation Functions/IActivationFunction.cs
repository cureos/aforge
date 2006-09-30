// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Activation function interface
	/// </summary>
	/// 
	/// <remarks>All activation functions, which are supposed to be used with
	/// neurons, which calculate their output as a function of weighted sum of
	/// their inputs, should implement this interfaces.
	/// </remarks>
	public interface IActivationFunction
	{
		/// <summary>
		/// Calculates function value
		/// </summary>
		///
		/// <param name="x">Function input value</param>
		/// 
		/// <returns>Function output value, <i>f(x)</i></returns>
		///
		/// <remarks>The method calculates function value at point <b>x</b>.</remarks>
		///
		double Function( double x );

		/// <summary>
		/// Calculates function derivative
		/// </summary>
		/// 
		/// <param name="x">Function input value</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i></returns>
		/// 
		/// <remarks>The method calculates function derivative at point <b>x</b>.</remarks>
		///
		double Derivative( double x );

		/// <summary>
		/// Calculates function derivative
		/// </summary>
		/// 
		/// <param name="y">Function output value - the value, which was obtained
		/// with the help of <see cref="Function">Function</see> method</param>
		/// 
		/// <returns>Function derivative, <i>f'(x)</i></returns>
		/// 
		/// <remarks>The method calculates the same derivative value as the
		/// <see cref="Derivative">Derivative</see> method, but it takes not the input <b>x</b> value
		/// itself, but the function value, which was calculated previously with
		/// the help of <see cref="Function">Function</see> method. <i>(Some applications require as
		/// function value, as derivative value, so they can seve the amount of
		/// calculations using this method to calculate derivative)</i></remarks>
		/// 
		double Derivative2( double y );
	}
}
