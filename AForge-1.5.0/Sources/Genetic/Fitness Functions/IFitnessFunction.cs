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
	/// 
	/// <remarks>The interface should be implemented by all fitness function
	/// classes, which are supposed to be used for calculation of chromosomes
	/// fitness values. All fitness functions should return positive (<b>greater
	/// then zero</b>) value, which indicates how good is the evaluated chromosome - 
	/// the greater the value, the better the chromosome.
	/// </remarks>
	public interface IFitnessFunction
	{
		/// <summary>
		/// Evaluates chromosome
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome to evaluate</param>
		/// 
		/// <returns>Returns chromosome's fitness value</returns>
		///
		/// <remarks>The method calculates fitness value of the specified
		/// chromosome.</remarks>
		///
		double Evaluate( IChromosome chromosome );

		/// <summary>
		/// Translates genotype to phenotype 
		/// </summary>
		/// 
		/// <param name="chromosome">Chromosome, which genoteype should be
		/// translated to phenotype</param>
		///
		/// <returns>Returns chromosome's fenotype - the actual solution
		/// encoded by the chromosome</returns> 
		/// 
		/// <remarks>The return value may be a string, a number or any other
		/// object, which is supposed to represent the problem solution.</remarks>
		///
		object Translate( IChromosome chromosome );
	}
}