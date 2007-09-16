// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Types of genes in genetic programming
	/// </summary>
	public enum GPGeneType
	{
		Function,
		Argument
	}


	/// <summary>
	/// Gene interface, which is used in Genetic Programming (GP) and
	/// Gene Expression Programming (GEP)
	/// </summary>
	public interface IGPGene
	{
		/// <summary>
		/// Gene type
		/// </summary>
		GPGeneType GeneType { get; }

		/// <summary>
		/// Arguments count
		/// </summary>
		int ArgumentsCount { get; }

		/// <summary>
		/// Maximum arguments count
		/// </summary>
		int MaxArgumentsCount { get; }

		/// <summary>
		/// Clone the gene
		/// </summary>
		IGPGene Clone( );

		/// <summary>
		/// Randomize gene with random type and value
		/// </summary>
		void Generate( );

		/// <summary>
		/// Randomize gene with random value
		/// </summary>
		void Generate( GPGeneType type );

		/// <summary>
		/// Creates new gene with random type
		/// </summary>
		IGPGene CreateNew( );

		/// <summary>
		/// Creates new gene with certain type
		/// </summary>
		IGPGene CreateNew( GPGeneType type );
	}
}
