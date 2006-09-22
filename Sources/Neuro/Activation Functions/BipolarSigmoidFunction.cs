// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Bipolar Sigmoid activation function
	///
	///                1
	/// f(x) = ------------------ - 0.5
	///        1 + exp(-alfa * x)
	///
	/// Output range: [-0.5, 0.5]
	///
	/// </summary>
	public class BipolarSigmoidFunction
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
		public BipolarSigmoidFunction( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public BipolarSigmoidFunction( double alfa )
		{
			this.alfa = alfa;
		}

		/// <summary>
		/// Calculate function value at point X
		/// </summary>
		public double Output( double x )
		{
			return ( ( 1 / ( 1 + Math.Exp( -alfa * x ) ) ) - 0.5 );
		}

		/// <summary>
		/// Calculate function's differential at point X
		/// </summary>
		public double Prime( double x )
		{
			double y = Output( x );

			return ( alfa * ( 0.25 - y * y ) );
		}

		/// <summary>
		/// Calculate function's differential. The input to the function is not
		/// the X value, which was passed to Output() method, but the Y value,
		/// which was returned from the Output() value.
		/// </summary>
		public double Prime2( double y )
		{
			return ( alfa * ( 0.25 - y * y ) );
		}	
	}
}
