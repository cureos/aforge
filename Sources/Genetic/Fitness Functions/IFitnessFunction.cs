// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Fitness function interface
	/// </summary>
	public interface IFitnessFunction
	{
		/// <summary>
		/// Evaluate chromosome - calculates its fitness value
		/// </summary>
		double Evaluate( IChromosome chromosome );

		/// <summary>
		/// Translate genotype to phenotype 
		/// </summary>
		object Translate( IChromosome chromosome );
	}
}