// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Sigmoid activation function
	/// </summary>
	///
	/// <remarks>The class represents sigmoid activation function with
	/// the next expression:<br />
	/// <code>
	///                1
	/// f(x) = ------------------
	///        1 + exp(-alpha * x)
	///
	///           alpha * exp(-alpha * x )
	/// f'(x) = ---------------------------- = alpha * f(x) * (1 - f(x))
	///           (1 + exp(-alpha * x))^2
	/// </code>
	/// Output range of the function: <b>[0, 1]</b><br /><br />
	/// Functions graph:<br />
	/// <img src="sigmoid.bmp" width="242" height="172" />
	/// </remarks>
	public class SigmoidFunction : IActivationFunction
	{
		// sigmoid's alpha value
		private double alpha = 2;

		/// <summary>
		/// Sigmoid's alpha value
		/// </summary>
		/// 
		/// <remarks>The value determines steepness of the function. Default value: <b>2</b>.
		/// </remarks>
		public double Alpha
		{
			get { return alpha; }
			set { alpha = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SigmoidFunction">SigmoidFunction</see> class
		/// </summary>
		public SigmoidFunction( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SigmoidFunction">SigmoidFunction</see> class
		/// </summary>
		/// 
		/// <param name="alpha">Sigmoid's alpha value</param>
		public SigmoidFunction( double alpha )
		{
			this.alpha = alpha;
		}


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
		public double Function( double x )
		{
			return ( 1 / ( 1 + Math.Exp( -alpha * x ) ) );
		}

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
		public double Derivative( double x )
		{
			double y = Function( x );

			return ( alpha * y * ( 1 - y ) );
		}

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
		public double Derivative2( double y )
		{
			return ( alpha * y * ( 1 - y ) );
		}	
	}
}
