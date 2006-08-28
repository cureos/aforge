// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;
	using System.Collections;

	/// <summary>
	/// Elite selection method
	/// </summary>
	public class EliteSelection : ISelectionMethod
	{
		private double	epsilon = 0;

		/// <summary>
		/// Percent of random chromosomes in new generation
		/// </summary>
		public double Epsilon
		{
			get { return epsilon; }
			set { epsilon = Math.Max( 0.0, Math.Min( 0.5, value ) ); }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public EliteSelection( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public EliteSelection( double epsilon )
		{
			this.epsilon = epsilon;
		}

		/// <summary>
		/// Apply selection to the population
		/// </summary>
		public void ApplySelection( ArrayList chromosomes, int size )
		{
			// amount of random chromosomes in the new population
			int randomAmount = (int)( epsilon * size );

			// sort chromosomes
			chromosomes.Sort( );

			size -= randomAmount;

			// remove bad chromosomes
			chromosomes.RemoveRange( size, chromosomes.Count - size );

			// add random chromosomes
			if ( randomAmount > 0 )
			{
				IChromosome ancestor = (IChromosome) chromosomes[0];

				for ( int i = 0; i < randomAmount; i++ )
				{
					chromosomes.Add( ancestor.CreateOffspring( ) );
				}
			}
		}
	}
}
