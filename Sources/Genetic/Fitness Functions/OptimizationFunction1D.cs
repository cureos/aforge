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
	/// Base class for one dimenstional functions to optimize.
	/// Evaluate only Binary Chromosomes.
	/// </summary>
	public abstract class OptimizationFunction1D : IFitnessFunction
	{
		/// <summary>
		/// Optimization modes
		/// </summary>
		public enum Modes
		{
			Maximization,
			Minimization
		}
		
		// optimization range
		private DoubleRange	range = new DoubleRange( 0, 1 );
		// optimization mode
		private Modes mode = Modes.Maximization;

		/// <summary>
		/// Optimization range
		/// </summary>
		public DoubleRange Range
		{
			get { return range; }
			set { range = value; }
		}

		/// <summary>
		/// Optimization mode
		/// </summary>
		public Modes Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public OptimizationFunction1D( DoubleRange range )
		{
			this.range = range;
		}

		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		public double Evaluate( IChromosome chromosome )
		{
			double functionValue = OptimizationFunction( TranslateNative( chromosome ) );
			// fitness value
			return ( mode == Modes.Maximization ) ? functionValue : 1 / functionValue;
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
		public double TranslateNative( IChromosome chromosome )
		{
			// get chromosome's value and max value
			double val = ((BinaryChromosome) chromosome).Value;
			double max = ((BinaryChromosome) chromosome).MaxValue;

			// translate to optimization's funtion space
			return val * range.Length / max + range.Min;
		}

		/// <summary>
		/// Function to be optimized
		/// </summary>
		public abstract double OptimizationFunction( double x );
	}
}
