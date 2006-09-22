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
	///
	///                1
	/// f(x) = ------------------
	///        1 + exp(-alfa * x)
	///
	/// Output range: [0, 1]
	/// </summary>
	public class SigmoidFunction
	{
		// sigmoid's alfa value
		private double alfa = 2;

		/// <summary>
		/// Sigmoid's alfa value
		/// </summary>
		public double Alfa
		{
			get { return alfa; }
			set { alfa = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public SigmoidFunction( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public SigmoidFunction( double alfa )
		{
			this.alfa = alfa;
		}


		/// <summary>
		/// Calculate function value at point X
		/// </summary>
		public double Output( double x )
		{
			return ( 1 / ( 1 + Math.Exp( -alfa * x ) ) );
		}

		/// <summary>
		/// Calculate function's differential at point X
		/// </summary>
		public double Prime( double x )
		{
			double y = Output( x );

			return ( alfa * y * ( 1 - y ) );
		}

		/// <summary>
		/// Calculate function's differential. The input to the function is not
		/// the X value, which was passed to Output() method, but the Y value,
		/// which was returned from the Output() value.
		/// </summary>
		public double Prime2( double y )
		{
			return ( alfa * y * ( 1 - y ) );
		}	
	}
}
