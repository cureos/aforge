// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Hyperbolic Tangens activation function
	///
	///                         exp(alfa * x) - exp(-alfa * x)
	/// f(x) = tanh(alfa * x) = ------------------------------
	///                         exp(alfa * x) + exp(-alfa * x)
	///
	/// Output range: [-1, 1]
	///
	/// </summary>
	public class HyperbolicTangensFunction
	{
		// hyperboloid's alfa value
		private double alfa = 1;

		/// <summary>
		/// Hyperboloid's alfa value
		/// </summary>
		public double Alfa
		{
			get { return alfa; }
			set { alfa = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public HyperbolicTangensFunction( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public HyperbolicTangensFunction( double alfa )
		{
			this.alfa = alfa;
		}
	
		/// <summary>
		/// Calculate function value at point X
		/// </summary>
		public double Output( double x )
		{
			return ( Math.Tanh( alfa * x ) );
		}

		/// <summary>
		/// Calculate function's differential at point X
		/// </summary>
		public double Prime( double x )
		{
			double y = Output( x );

			return ( alfa * ( 1 - y * y ) );
		}

		/// <summary>
		/// Calculate function's differential. The input to the function is not
		/// the X value, which was passed to Output() method, but the Y value,
		/// which was returned from the Output() value.
		/// </summary>
		public double Prime2( double y )
		{
			return ( alfa * ( 1 - y * y ) );
		}	
	}
}
