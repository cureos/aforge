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
		private double	epsilon = 0;
		// random number generator
		static Random	rand = new Random( (int) DateTime.Now.Ticks );

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
		public RouletteWheelSelection( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public RouletteWheelSelection( double epsilon )
		{
			this.epsilon = epsilon;
		}

		/// <summary>
		/// Apply selection to the population
		/// </summary>
		public void ApplySelection( ArrayList chromosomes, int size )
		{
			// new population, initially empty
			ArrayList newPopulation = new ArrayList( );
			// size of current population
			int currentSize = chromosomes.Count;
			// amount of random chromosomes in the new population
			int randomAmount = (int)( epsilon * size );

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
			size -= randomAmount;
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

			// add random chromosomes
			if ( randomAmount > 0 )
			{
				IChromosome ancestor = (IChromosome) newPopulation[0];

				for ( int i = 0; i < randomAmount; i++ )
				{
					newPopulation.Add( ancestor.CreateOffspring( ) );
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
