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
	/// Evaluator of expressions written in polish notation
	/// </summary>
	/// 
	/// <remarks>The class evaluates expressions writen in post fix polish notation.
	/// The list of supported functuins is:
	/// <list type="bullet">
	/// <item><b>Arithmetic functions</b>: +, -, *, /;</item>
	/// <item><b>sin</b> - sine;</item>
	/// <item><b>cos</b> - cosine;</item>
	/// <item><b>ln</b> - natural logarithm;</item>
	/// <item><b>exp</b> - exponent;</item>
	/// <item><b>sqrt</b> - square root.</item>
	/// </list>
	/// Arguments for these functions could be as usual constants, written as numbers, as variables,
	/// writen as $&lt;var_number&gt; (<b>$2</b>, for example). The variable number is zero based index
	/// of variables array.
	/// </remarks>
	/// 
	/// <example>The following sample illustrates the usage of <c>PolishExpression</c>
	/// <code>
	/// // expression written in polish notation
	/// string expression = "2 $0 / 3 $1 * +";
	/// // variables for the expression
	/// double[] vars = new double[] { 3, 4 };
	/// // expression evaluation
	/// double result = PolishExpression.Evaluate( expression, vars );
	/// </code>
	/// </example>
	/// 
	public class PolishExpression
	{
		/// <summary>
		/// Constructor (the class should not be instantiated at this moment)
		/// </summary>
		private PolishExpression ( ) { }

		/// <summary>
		/// Evaluates specified expression
		/// </summary>
		///
		/// <param name="expression">Expression written in polish notation</param>
		/// <param name="variables">Variables for the expression</param>
		/// 
		/// <returns>Evaluated value of the expression</returns>
		///
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
