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
	/// Roulette Wheel selection method
	/// </summary>
	public class RouletteWheelSelection : ISelectionMethod
	{
		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Default constructor
		/// </summary>
		public RouletteWheelSelection( ) { }

		/// <summary>
		/// Apply selection to the population
		/// </summary>
		public void ApplySelection( ArrayList chromosomes, int size )
		{
			// new population, initially empty
			ArrayList newPopulation = new ArrayList( );
			// size of current population
			int currentSize = chromosomes.Count;

			// calculate summary fitness of current population
			double fitnessSum = 0;
			foreach ( IChromosome c in chromosomes )
			{
				fitnessSum += c.Fitness;
			}

			// create wheel ranges
			double[]	rangeMax = new double[currentSize];
			double		s = 0;
			int			k = 0;

			foreach ( IChromosome c in chromosomes )
			{
				// cumulative normalized fitness
				s += ( c.Fitness / fitnessSum );
				rangeMax[k++] = s;
			}

			// select chromosomes from old population to the new population
			for ( int j = 0; j < size; j++ )
			{
				// get wheel value
				double wheelValue = rand.NextDouble( );
				// find the chromosome for the wheel value
				for ( int i = 0; i < currentSize; i++ )
				{
					if ( wheelValue <= rangeMax[i] )
					{
						// add the chromosome to the new population
						newPopulation.Add( ((IChromosome) chromosomes[i]).Clone( ) );
						break;
					}
				}
			}

			// empty current population
			chromosomes.Clear( );

			// move elements from new to current population
			// !!! moving is done to reduce objects cloning
			for ( int i = 0; i < size; i++ )
			{
				chromosomes.Add( newPopulation[0] );
				newPopulation.RemoveAt( 0 );
			}
		}
	}
}
