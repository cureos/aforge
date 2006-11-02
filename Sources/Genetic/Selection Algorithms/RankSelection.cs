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
	/// Rank selection method
	/// </summary>
	/// 
	/// <remarks>The algorithm selects chromosomes to the new generation depending on
	/// their fitness values - the better fitness value chromosome has, the more chances
	/// it has to become member of the new generation. Each chromosome can be selected
	/// several times to the new generation. This algorithm is similar to
	/// <see cref="RouletteWheelSelection">Roulette Wheel Selection</see> algorithm, but
	/// a the difference in "wheel" and its sectors size calculation method. The size
	/// of the wheel equals to <b>size * ( size + 1 ) / 2</b>, where <b>size</b> is the
	/// current size of population. The worst chromosome has its sector's size equals to 1,
	/// the next chromosome has its sector's size equals to 2, etc.</remarks>
	/// 
	public class RankSelection : ISelectionMethod
	{
		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Initializes a new instance of the <see cref="RankSelection"/> class
		/// </summary>
		public RankSelection( ) { }

		/// <summary>
		/// Apply selection to the population
		/// </summary>
		/// 
		/// <param name="chromosomes">Population, which should be filtered</param>
		/// <param name="size">The amount of chromosomes to keep</param>
		/// 
		/// <remarks>Filters specified population according to the implemented
		/// algorithm</remarks>
		/// 
		public void ApplySelection( ArrayList chromosomes, int size )
		{
			// new population, initially empty
			ArrayList newPopulation = new ArrayList( );
			// size of current population
			int currentSize = chromosomes.Count;

			// sort current population
			chromosomes.Sort( );

			// calculate amount of ranges in the wheel
			double ranges = currentSize * ( currentSize + 1 ) / 2;

			// create wheel ranges
			double[]	rangeMax = new double[currentSize];
			double		s = 0;

			for ( int i = 0, n = currentSize; i < currentSize; i++, n-- )
			{
				s += ( (double) n / ranges );
				rangeMax[i] = s;
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
