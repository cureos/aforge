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
	/// 
	/// <remarks>The class represents population - collection of individuals (chromosomes)
	/// and provides functionality for common population's life cycle - population growing
	/// with help of genetic operators and selection of chromosomes to new generation
	/// with help of selection algorithm. The class may work with any type of chromosomes
	/// implementing <see cref="IChromosome"/> interface, use any type of fitness functions
	/// implementing <see cref="IFitnessFunction"/> interface and use any type of selection
	/// algorithms implementing <see cref="ISelectionMethod"/> interface.</remarks>
	/// 
	public class Population
	{
		private IFitnessFunction fitnessFunction;
		private ISelectionMethod selectionMethod;
		private ArrayList	population = new ArrayList( );
		private int			size;
		private double		randomSelectionPortion = 0.0;

		// population parameters
		private double		crossoverRate	= 0.75;
		private double		mutationRate	= 0.10;

		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		//
		private double		fitnessMax = 0;
		private double		fitnessSum = 0;
		private double		fitnessAvg = 0;
		private IChromosome	bestChromosome = null;

		/// <summary>
		/// Crossover rate
		/// </summary>
		/// 
		/// <remarks>The value determines the amount of chromosomes which participate
		/// in crossover. The value is measured in the range of [0.1, 1]. Default value
		/// is 0.75.</remarks>
		/// 
		public double CrossoverRate
		{
			get { return crossoverRate; }
			set
			{
				crossoverRate = Math.Max( 0.1, Math.Min( 1.0, value ) );
			}
		}

		/// <summary>
		/// Mutation rate
		/// </summary>
		/// 
		/// <remarks>The value determines the amount of chromosomes which participate
		/// in mutation. The value is measured in the range of [0.1, 1]. Defaul value
		/// is 0.1.</remarks>
		/// 
		public double MutationRate
		{
			get { return mutationRate; }
			set
			{
				mutationRate = Math.Max( 0.1, Math.Min( 1.0, value ) );
			}
		}

		/// <summary>
		/// Random selection portion
		/// </summary>
		/// 
		/// <remarks>The value determines the amount of chromosomes which will be
		/// randomly generated for the new population. The value is measured in the
		/// range of [0, 0.9]. Default value is 0.</remarks>
		/// 
		public double RandomSelectionPortion
		{
			get { return randomSelectionPortion; }
			set
			{
				randomSelectionPortion = Math.Max( 0, Math.Min( 0.9, value ) );
			}
		}

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

		/// <summary>
		/// Best chromosome of the population
		/// </summary>
		public IChromosome BestChromosome
		{
			get { return bestChromosome; }
		}

		/// <summary>
		/// Size of the population
		/// </summary>
		/// 
		/// <remarks>The property returns initial (minimal) size of population.
		/// Population always returns to this size after using <see cref="Selection"/>
		/// or <see cref="RunEpoch"/> methods.</remarks>
		/// 
		public int Size
		{
			get { return size; }
		}

		/// <summary>
		/// Get chromosome with specified index
		/// </summary>
		/// 
		/// <param name="index">Chromosome's index</param>
		/// 
		/// <remarks>Allows to access individuals of the population.</remarks>
		/// 
		public IChromosome this[int index]
		{
			get { return (IChromosome) population[index]; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Population"/> class
		/// </summary>
		/// 
		/// <param name="size">Initial size of population</param>
		/// <param name="ancestor">Ancestor chromosome to use for population creatioin</param>
		/// <param name="fitnessFunction">Fitness function to use for calculating
		/// chromosome's fitness values</param>
		/// <param name="selectionMethod">Selection algorithm to use for selection
		/// chromosome's to new generation</param>
		/// 
		/// <remarks>Creates new population of specified size. The specified ancestor
		/// becomes first member of the population and is used to create other members
		/// with same parameters, which were used for ancestor's creation.</remarks>
		///
		public Population( int size,
							IChromosome ancestor,
							IFitnessFunction fitnessFunction,
							ISelectionMethod selectionMethod )
		{
			this.fitnessFunction = fitnessFunction;
			this.selectionMethod = selectionMethod;
			this.size = size;

			// add ancestor to the population
			ancestor.Evaluate( fitnessFunction );
			population.Add( ancestor );
			// add more chromosomes to the population
			for ( int i = 1; i < size; i++ )
			{
				// create new chromosome
				IChromosome c = ancestor.CreateOffspring( );
				// calculate it's fitness
				c.Evaluate( fitnessFunction );
				// add it to population
				population.Add( c );
			}
		}

		/// <summary>
		/// Regenerate population
		/// </summary>
		/// 
		/// <remarks>The method regenerates population filling it with random chromosomes.</remarks>
		/// 
		public void Regenerate( )
		{
			IChromosome ancestor = (IChromosome) population[0];

			// clear population
			population.Clear( );
			// add chromosomes to the population
			for ( int i = 0; i < size; i++ )
			{
				// create new chromosome
				IChromosome c = ancestor.CreateOffspring( );
				// calculate it's fitness
				c.Evaluate( fitnessFunction );
				// add it to population
				population.Add( c );
			}
		}

		/// <summary>
		/// Do crossover in the population
		/// </summary>
		/// 
		/// <remarks>The method walks through the population and performs crossover operator
		/// taking each two chromosomes in the order of their presence in the population.
		/// The total amount of paired chromosomes is determined by
		/// <see cref="CrossoverRate">crossover rate</see>.</remarks>
		/// 
		public virtual void Crossover( )
		{
			// crossover
			for ( int i = 1; i < size; i += 2 )
			{
				// generate next random number and check if we need to do crossover
				if ( rand.NextDouble( ) <= crossoverRate )
				{
					// clone both ancestors
					IChromosome c1 = ((IChromosome) population[i - 1]).Clone( );
					IChromosome c2 = ((IChromosome) population[i]).Clone( );

					// do crossover
					c1.Crossover( c2 );

					// calculate fitness of these two offsprings
					c1.Evaluate( fitnessFunction );
					c2.Evaluate( fitnessFunction );

					// add two new offsprings to the population
					population.Add( c1 );
					population.Add( c2 );
				}
			}
		}

		/// <summary>
		/// Do mutation in the population
		/// </summary>
		/// 
		/// <remarks>The method walks through the population and performs mutation operator
		/// taking each chromosome one by one. The total amount of mutated chromosomes is
		/// determined by <see cref="MutationRate">mutation rate</see>.</remarks>
		/// 
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
					// calculate fitness of the mutant
					c.Evaluate( fitnessFunction );
					// add mutant to the population
					population.Add( c );
				}
			}
		}

		/// <summary>
		/// Do selection
		/// </summary>
		/// 
		/// <remarks>The method applies selection operator to the current population. Using
		/// specified selection algorithm it selects members to the new generation from current
		/// generates and adds certain amount of random members, if is required
		/// (see <see cref="RandomSelectionPortion"/>).</remarks>
		/// 
		public virtual void Selection( )
		{
			// amount of random chromosomes in the new population
			int randomAmount = (int)( randomSelectionPortion * size );

			// do selection
			selectionMethod.ApplySelection( population, size - randomAmount );

			// add random chromosomes
			if ( randomAmount > 0 )
			{
				IChromosome ancestor = (IChromosome) population[0];

				for ( int i = 0; i < randomAmount; i++ )
				{
					// create new chromosome
					IChromosome c = ancestor.CreateOffspring( );
					// calculate it's fitness
					c.Evaluate( fitnessFunction );
					// add it to population
					population.Add( c );
				}
			}

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
		/// Run one epoch of the population
		/// </summary>
		/// 
		/// <remarks>The method runs one epoch of the population, doing crossover, mutation
		/// and selection.</remarks>
		/// 
		public void RunEpoch( )
		{
			Crossover( );
			Mutate( );
			Selection( );
		}
		
/*		public void Trace( )
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
		}*/
	}
}
