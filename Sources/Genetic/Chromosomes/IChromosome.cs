// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Chromosome interface
	/// </summary>
	public interface IChromosome : IComparable
	{
		/// <summary>
		/// Chromosome's fintess value
		/// </summary>
		double Fitness { get; }

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		void Generate( );

		/// <summary>
		/// Create new random chromosome with same parameters (factory method)
		/// </summary>
		IChromosome CreateOffspring( );

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		IChromosome Clone( );

		/// <summary>
		/// Mutation operator
		/// </summary>
		void Mutate( );

		/// <summary>
		/// Crossover operator
		/// </summary>
		void Crossover( IChromosome pair );

		/// <summary>
		/// Evaluate chromosome with specified fitness function
		/// </summary>
		void Evaluate( IFitnessFunction function );
	}
}
