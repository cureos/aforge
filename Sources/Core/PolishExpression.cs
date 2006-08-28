// AForge Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;
	using System.Collections;

	// Quick and dirty implementation of polish expression evaluator

	/// <summary>
	/// Evaluator of expression written in polish notation.
	/// </summary>
	public class PolishExpression
	{
		/// <summary>
		/// Evaluate specified expression
		/// </summary>
		public static double Evaluate( string expression, double[] variables )
		{
			// split expression to separate tokens, which represent functions ans variables
			string[]	tokens = expression.Trim( ).Split( ' ' );
			// arguments stack
			Stack		arguments = new Stack( );

			// walk through all tokens
			foreach ( string token in tokens )
			{
				// check for token type
				if ( char.IsDigit( token[0] ) )
				{
					// the token in numeric argument
					arguments.Push( double.Parse( token ) );
				}
				else if ( token[0] == '$' )
				{
					// the token is variable
					arguments.Push( variables[ int.Parse( token.Substring( 1 ) ) ] );
				}
				else
				{
					// each function has at least one argument, so let's get the top one
					// argument from stack
					double v = (double) arguments.Pop( );

					// check for function
					switch ( token )
					{
						case "+":			// addition
							arguments.Push( (double) arguments.Pop( ) + v );
							break;

						case "-":			// subtraction
							arguments.Push( (double) arguments.Pop( ) - v );
							break;

						case "*":			// multiplication
							arguments.Push( (double) arguments.Pop( ) * v );
							break;

						case "/":			// division
							arguments.Push( (double) arguments.Pop( ) / v );
							break;

						case "sin":			// sine
							arguments.Push( Math.Sin( v ) );
							break;

						case "cos":			// cosine
							arguments.Push( Math.Cos( v ) );
							break;

						case "ln":			// natural logarithm
							arguments.Push( Math.Log( v ) );
							break;

						case "exp":			// exponent
							arguments.Push( Math.Exp( v ) );
							break;

						case "sqrt":		// square root
							arguments.Push( Math.Sqrt( v ) );
							break;

						default:
							// throw exception informing about undefined function
							throw new ArgumentException( "Undefined function: " + token );
					}
				}
			}

			// check stack size
			if ( arguments.Count != 1 )
			{
				throw new ArgumentException( "Incorrect expression" );
			}

			// return the only value from stack
			return (double) arguments.Pop( );
		}
	}
}
