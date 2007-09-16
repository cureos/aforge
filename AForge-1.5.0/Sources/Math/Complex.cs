// AForge Math Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math
{
	using System;
	
	/// <summary>
	/// Complex number
	/// </summary>
	/// 
	/// <remarks>The class encapsulate complex number and provides
	/// basic complex operators.</remarks>
	/// 
	public struct Complex
	{
		/// <summary>
		/// Real part of the complex number
		/// </summary>
		public double Re;

		/// <summary>
		/// Imaginary part of the complex number
		/// </summary>
		public double Im;
		
		/// <summary>
		/// Represents complex zero
		/// </summary>
		/// 
		/// <remarks>Represents complex number with both real and imaginary
		/// parts equal to zero.</remarks>
		/// 
		public static Complex Zero
		{
			get { return new Complex( 0, 0 ); }
		}
		
		/// <summary>
		/// Magnitude value of the complex number
		/// </summary>
		public double Magnitude
		{
			get { return System.Math.Sqrt( Re * Re + Im * Im ); }
		}

		/// <summary>
		/// Phase value of the complex number
		/// </summary>
		public double Phase
		{
			get { return System.Math.Atan( Im / Re ); }
		}
		
		/// <summary>
		/// Squared magnitude value of the complex number
		/// </summary>
		public double SquaredMagnitude
		{
			get { return ( Re * Re + Im * Im ); }
		}
	
	
		/// <summary>
		/// Initializes a new instance of the <see cref="Complex"/> class
		/// </summary>
		/// 
		/// <param name="re">Real part</param>
		/// <param name="im">Imaginary part</param>
		/// 
		public Complex( double re, double im )
		{
			this.Re = re;
			this.Im = im;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Complex"/> class
		/// </summary>
		/// 
		/// <param name="c">Source complex number</param>
		/// 
		public Complex( Complex c )
		{
			this.Re = c.Re;
			this.Im = c.Im;
		}
		
		/// <summary>
		/// Returns string representation of the complex number
		/// </summary>
		/// 
		/// <returns>String representation of the complex number</returns>
		/// 
		public override string ToString( )
		{
			return String.Format( "{0}{1}{2}i", Re, ( ( Im < 0 ) ? '-' : '+' ), Math.Abs( Im ) );
		}
		
		/// <summary>
		/// Addition operator
		/// </summary>
		/// 
		/// <param name="a">Left complex operand</param>
		/// <param name="b">Right complex operand</param>
		/// 
		/// <returns>Result complex number</returns>
		/// 
		public static Complex operator+( Complex a, Complex b )
		{
			return new Complex( a.Re + b.Re, a.Im + b.Im );
		}

		/// <summary>
		/// Subtraction operator
		/// </summary>
		/// 
		/// <param name="a">Left complex operand</param>
		/// <param name="b">Right complex operand</param>
		/// 
		/// <returns>Result complex number</returns>
		/// 
		public static Complex operator-( Complex a, Complex b )
		{
			return new Complex( a.Re - b.Re, a.Im - b.Im );
		}
		
		/// <summary>
		/// Multiplication operator
		/// </summary>
		/// 
		/// <param name="a">Left complex operand</param>
		/// <param name="b">Right complex operand</param>
		/// 
		/// <returns>Result complex number</returns>
		/// 
		public static Complex operator*( Complex a, Complex b )
		{
			return new Complex(
				a.Re * b.Re - a.Im * b.Im,
				a.Re * b.Im + a.Im * b.Re );
		}

		/// <summary>
		/// Division operator
		/// </summary>
		/// 
		/// <param name="a">Left complex operand</param>
		/// <param name="b">Right complex operand</param>
		/// 
		/// <returns>Result complex number</returns>
		/// 
		public static Complex operator/( Complex a, Complex b )
		{
			double divider = b.Re * b.Re + b.Im * b.Im;
			
			if ( divider == 0 )
				throw new DivideByZeroException( );
		
			return new Complex(
				( a.Re * b.Re + a.Im * b.Im ) / divider,
				( a.Im * b.Re - a.Re * b.Im ) / divider );
		}
	}
}