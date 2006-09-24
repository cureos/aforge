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
	/// Evaluates fitness of GP chromosomes for time series prediction task
	/// </summary>
	public class TimeSeriesPredictionFitness : IFitnessFunction
	{
		// time series data
		private double[]	data;
		// varibles
		private double[]	variables;
		// window size
		private int			windowSize;
		// prediction size
		private int			predictionSize;

		/// <summary>
		/// Constructor
		/// </summary>
		public TimeSeriesPredictionFitness( double[] data, int windowSize, int predictionSize, double[] constants )
		{
			// check for correct parameters
			if ( windowSize >= data.Length )
				throw new ArgumentException( "Window size should be less then data amount" );
			if ( data.Length - windowSize - predictionSize < 1 )
				throw new ArgumentException( "Data size should be enough for window and prediction" );
			// save parameters
			this.data			= data;
			this.windowSize		= windowSize;
			this.predictionSize	= predictionSize;
			// copy constants
			variables = new double[constants.Length + windowSize];
			Array.Copy( constants, 0, variables, windowSize, constants.Length );
		}


		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		public double Evaluate( IChromosome chromosome )
		{
			// get function in polish notation
			string function = chromosome.ToString( );

			// go through all the data
			double error = 0.0;
			for ( int i = 0, n = data.Length - windowSize - predictionSize; i < n; i++ )
			{
				// put values from current window as variables
				for ( int j = 0, b = i + windowSize - 1; j < windowSize; j++ )
				{
					variables[j] = data[b - j];
				}

				// avoid evaluation errors
				try
				{
					// evalue the function
					double y = PolishExpression.Evaluate( function, variables );
					// check for correct numeric value
					if ( double.IsNaN( y ) )
						return 0;
					// get the difference between evaluated value and
					// next value after the window, and sum error
					error += Math.Abs( y - data[i + windowSize] );
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
