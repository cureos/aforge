// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Bipolar sigmoid activation function
	/// </summary>
	///
	/// <remarks>The class represents bipolar sigmoid activation function with
	/// the next expression:<br />
	/// <code>
	///                2
	/// f(x) = ------------------ - 1
	///        1 + exp(-alpha * x)
	///
	///           2 * alpha * exp(-alpha * x )
	/// f'(x) = -------------------------------- = alpha * (1 - f(x)^2) / 2
	///           (1 + exp(-alpha * x))^2
	/// </code>
	/// Output range of the function: <b>[-1, 1]</b><br /><br />
	/// Functions graph:<br />
	/// <img src="sigmoid_bipolar.bmp" width="242" height="172" />
	/// </remarks>
	public class BipolarSigmoidFunction : IActivationFunction
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
		/// Initializes a new instance of the <see cref="SigmoidFunction">BipolarSigmoidFunction</see> class
		/// </summary>
		public BipolarSigmoidFunction( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BipolarSigmoidFunction">BipolarSigmoidFunction</see> class
		/// </summary>
		/// 
		/// <param name="alpha">Sigmoid's alpha value</param>
		public BipolarSigmoidFunction( double alpha )
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
			return ( ( 2 / ( 1 + Math.Exp( -alpha * x ) ) ) - 1 );
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

			return ( alpha * ( 1 - y * y ) / 2 );
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
			return ( alpha * ( 1 - y * y ) / 2 );
		}	
	}
}
