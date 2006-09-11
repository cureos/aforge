// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;
	using AForge;

	/// <summary>
	/// Evaluates fitness of GP chromosomes for symbolic regression task
	/// </summary>
	public class SymbolicRegressionFitness : IFitnessFunction
	{
		// regression data
		private double[,]	data;
		// varibles
		private double[]	variables;
		// last evaluation error
		private double		error = 0;

		/// <summary>
		/// Last evaluation error
		/// </summary>
		public double Error
		{
			get { return error; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public SymbolicRegressionFitness( double[,] data, double[] constants )
		{
			this.data = data;
			// copy constants
			variables = new double[constants.Length + 1];
			Array.Copy( constants, 0, variables, 1, constants.Length );
		}

		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		public double Evaluate( IChromosome chromosome )
		{
			// get function in polish notation
			string function = chromosome.ToString( );

			// go through all the data
			error = 0.0;
			for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				// put next X value to variables list
				variables[0] = data[i, 0];
				// avoid evaluation errors
				try
				{
					// evalue the function
					double y = PolishExpression.Evaluate( function, variables );
					// check for correct numeric value
					if ( double.IsNaN( y ) )
						return 0;
					// get the difference between evaluated Y and real Y
					// and sum error
					error += Math.Abs( y - data[i, 1] );
				}
				catch
				{
					return 0;
				}
			}

			// return optimization function value
			return 100.0 / ( error + 1 );
		}

		/// <summary>
		/// Translate genotype to phenotype.
		/// Returns phenotype in form of object value.
		/// </summary>
		public object Translate( IChromosome chromosome )
		{
			return TranslateNative( chromosome );
		}

		/// <summary>
		/// Translate genotype to phenotype.
		/// Returns phenotype in native representation.
		/// </summary>
		public string TranslateNative( IChromosome chromosome )
		{
			// return polish notation for now ...
			return chromosome.ToString( );
		}
	}
}
