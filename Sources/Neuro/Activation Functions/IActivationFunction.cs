// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Activation function interface.
	/// </summary>
	public interface IActivationFunction
	{
		/// <summary>
		/// Calculate function value at point X
		/// </summary>
		double Output( double x );

		/// <summary>
		/// Calculate function's differential at point X
		/// </summary>
		double Prime( double x );

		/// <summary>
		/// Calculate function's differential. The input to the function is not
		/// the X value, which was passed to Output() method, but the Y value,
		/// which was returned from the Output() value.
		/// </summary>
		double Prime2( double y );
	}
}
