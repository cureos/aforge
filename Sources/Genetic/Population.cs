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
	/// Population of chromosomes
	/// </summary>
	public class Population
	{
		private IFitnessFunction fitnessFunction;
		private ISelectionMethod selectionMethod;
		private ArrayList	population = new ArrayList( );
		private int			size;

		// population parameters
		private double		crossOverRate	= 0.75;
		private double		mutationRate	= 0.10;

		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		//
		private double		fitnessMax = 0;
		private double		fitnessSum = 0;
		private double		fitnessAvg = 0;
		private IChromosome	bestChromosome = null;

		/// <summary>
		/// Maximum fitness of the population
		/// </summary>
		public double FitnessMax
		{
			get { return fitnessMax; }
		}

		/// <summary>
		/// Summary fitness of the population
		/// </summary>
		public double FitnessSum
		{
			get { return fitnessSum; }
		}

		/// <summary>
		/// Average fitness of the population
		/// </summary>
		public double FitnessAvg
		{
			get { return fitnessAvg; }
		}

		/// <sumary>
		/// Best chromosome of the population
		/// </sumary>
		public IChromosome BestChromosome
		{
			get { return bestChromosome; }
		}

		/// <sumary>
		/// Size of the population
		/// </sumary>
		public int Size
		{
			get { return size; }
		}

		/// <sumary>
		/// Get chromosome with specified index
		/// </sumary>
		public IChromosome this[int index]
		{
			get { return (IChromosome) population[index]; }
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public Population( int size,
							IChromosome ancestor,
							IFitnessFunction fitnessFunction,
							ISelectionMethod selectionMethod )
		{
			this.fitnessFunction = fitnessFunction;
			this.selectionMethod = selectionMethod;
			this.size	= size;

			// add ancestor to the population
			population.Add( ancestor );
			// add more chromosomes to the population
			for ( int i = 1; i < size; i++ )
			{
				population.Add( ancestor.CreateOffspring( ) );
			}
		}

		/// <summary>
		/// Regenerate population - feel it with random chromosomes
		/// </summary>
		public void Regenerate( )
		{
			IChromosome ancestor = (IChromosome) population[0];

			// clear population
			population.Clear( );
			// add chromosomes to the population
			for ( int i = 0; i < size; i++ )
			{
				population.Add( ancestor.CreateOffspring( ) );
			}
		}

		/// <summary>
		/// Do crossover in the population
		/// </summary>
		public virtual void Crossover( )
		{
			// crossover
			for ( int i = 1; i < size; i += 2 )
			{
				// generate next random number and check if we need to do crossover
				if ( rand.NextDouble( ) <= crossOverRate )
				{
					// clone both ancestors
					IChromosome c1 = ((IChromosome) population[i - 1]).Clone( );
					IChromosome c2 = ((IChromosome) population[i]).Clone( );

					// do crossover
					c1.Crossover( c2 );

					// add two new offsprings to the population
					population.Add( c1 );
					population.Add( c2 );
				}
			}
		}

		/// <summary>
		/// Do mutation in the population
		/// </summary>
		public virtual void Mutate( )
		{
			// mutate
			for ( int i = 0; i < size; i++ )
			{
				// generate next random number and check if we need to do mutation
				if ( rand.NextDouble( ) <= mutationRate )
				{
					// clone the chromosome
					IChromosome c = ((IChromosome) population[i]).Clone( );

					// mutate it
					c.Mutate( );

					// add mutant to the population
					population.Add( c );
				}
			}
		}

		/// <summary>
		/// Do selection
		/// </summary>
		public virtual void Selection( )
		{
			// calculate fitness for the population
			foreach ( IChromosome c in population )
			{
				c.Evaluate( fitnessFunction );
			}

			// do selection
			selectionMethod.ApplySelection( population, size );

			// find best chromosome
			fitnessMax = 0;
			fitnessSum = 0;

			foreach ( IChromosome c in population )
			{
				double fitness = c.Fitness;

				// accumulate summary value
				fitnessSum += fitness;

				// check for max
				if ( fitness > fitnessMax )
				{
					fitnessMax = fitness;
					bestChromosome = c;
				}
			}
			fitnessAvg = fitnessSum / size;
		
		}

		/// <summary>
		/// Run one epoch of the population - crossover, mutation and selection
		/// </summary>
		public void RunEpoch( )
		{
			Crossover( );
			Mutate( );
			Selection( );
		}
		
		public void Trace( )
		{
			System.Diagnostics.Debug.WriteLine( "Max = " + fitnessMax );
			System.Diagnostics.Debug.WriteLine( "Sum = " + fitnessSum );
			System.Diagnostics.Debug.WriteLine( "Avg = " + fitnessAvg );
			System.Diagnostics.Debug.WriteLine( "--------------------------" );
			foreach ( IChromosome c in population )
			{
				System.Diagnostics.Debug.WriteLine( "genotype = " + c.ToString( ) +
					", phenotype = " + fitnessFunction.Translate( c ) +
					" , fitness = " + c.Fitness );
			}
			System.Diagnostics.Debug.WriteLine( "==========================" );
		}
	}
}
