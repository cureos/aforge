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
	/// Base class for two dimenstional functions to optimize.
	/// Evaluate only Binary Chromosomes.
	/// </summary>
	public abstract class OptimizationFunction2D : IFitnessFunction
	{
		// optimization ranges
		private DoubleRange	rangeX = new DoubleRange( 0, 1 );
		private DoubleRange	rangeY = new DoubleRange( 0, 1 );

		/// <summary>
		/// X optimization range
		/// </summary>
		public DoubleRange RangeX
		{
			get { return rangeX; }
			set { rangeX = value; }
		}

		/// <summary>
		/// Y optimization range
		/// </summary>
		public DoubleRange RangeY
		{
			get { return rangeY; }
			set { rangeY = value; }
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public OptimizationFunction2D( DoubleRange rangeX, DoubleRange rangeÍ )
		{
			this.rangeX = rangeX;
			this.rangeY = rangeY;
		}


		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		public double Evaluate( IChromosome chromosome )
		{
			double[] xy;

			// do native translation first
			xy = TranslateNative( chromosome );

			// return optimization function 
			return OptimizationFunction( xy[0], xy[1] );
		}

		/// <summary>
		/// Translate genotype to phenotype.
		/// Returns phenotype in form of object value.
		/// </summary>
		public object Translate( IChromosome chromosome )
		{
			// do native translation first
			return TranslateNative( chromosome );
		}

		/// <summary>
		/// Translate genotype to phenotype.
		/// Returns phenotype in native representation.
		/// </summary>
		public double[] TranslateNative( IChromosome chromosome )
		{
			// get chromosome's value
			ulong	val = ((BinaryChromosome) chromosome).Value;
			// chromosome's length
			int		length = ((BinaryChromosome) chromosome).Length;
			// length of X component
			int		xLength = length / 2;
			// length of Y component
			int		yLength = length - xLength;
			// X maximum value - equal to X mask
			ulong	xMax = 0xFFFFFFFFFFFFFFFF >> ( 64 - xLength );
			// Y maximum value
			ulong	yMax = 0xFFFFFFFFFFFFFFFF >> ( 64 - yLength );
			// X component
			double	xPart = val & xMax;
			// Y component;
			double	yPart = val >> xLength;

			// translate to optimization's funtion space
			double[] ret = new double[2];

			ret[0] = xPart * rangeX.Length / xMax + rangeX.Min;
			ret[1] = yPart * rangeY.Length / yMax + rangeY.Min;

			return ret;
		}

		/// <summary>
		/// Function to be optimized
		/// </summary>
		public abstract double OptimizationFunction( double x, double y );
	}
}
