// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;
	using System.Text;

	/// <summary>
	/// Permutation Chromosome is a Short Array Chromosome, but with two
	/// restrictions:
	/// 1) all genes are unique, i.e. there are no two genes in such
	///    chromosome with the same value;
	/// 2) maximum value of each gene is equal to chromosome length - 1.
	/// 
	/// With these two restrictions the chromosome represents permutation.
	/// </summary>
	public class PermutationChromosome : ShortArrayChromosome
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public PermutationChromosome( int length ) : base( length, length - 1 ) { }

		/// <summary>
		/// Copy Constructor
		/// </summary>
		protected PermutationChromosome( PermutationChromosome source ) : base( source ) { }

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		public override void Generate( )
		{
			// create ascending permutation initially
			for ( int i = 0; i < length; i++ )
			{
				val[i] = (ushort) i;
			}

			// shufle the permutation
			for ( int i = 0, n = length >> 1; i < n; i++ )
			{
				ushort t;
				int j1 = rand.Next( length );
				int j2 = rand.Next( length );

				// swap values
				t		= val[j1];
				val[j1]	= val[j2];
				val[j2]	= t;
			}
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public override IChromosome CreateOffspring( )
		{
			return new PermutationChromosome( length );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public override IChromosome Clone( )
		{
			return new PermutationChromosome( this );
		}
		
		/// <summary>
		/// Mutation operator
		/// </summary>
		public override void Mutate( )
		{
			ushort t;
			int j1 = rand.Next( length );
			int j2 = rand.Next( length );

			// swap values
			t		= val[j1];
			val[j1]	= val[j2];
			val[j2]	= t;
		}

		/// <summary>
		/// Crossover operator
		/// </summary>
		public override void Crossover( IChromosome pair )
		{
			PermutationChromosome p = (PermutationChromosome) pair;

			// check for correct pair
			if ( ( p != null ) && ( p.length == length ) )
			{
				ushort[] child1 = new ushort[length];
				ushort[] child2 = new ushort[length];

				// create two children
				CreateChildUsingCrossover( this.val, p.val, child1 );
				CreateChildUsingCrossover( p.val, this.val, child2 );

				// replace parents with children
				this.val	= child1;
				p.val		= child2;
			}
		}

		// Produce new child applying crossover to two parents
		private void CreateChildUsingCrossover( ushort[] parent1, ushort[] parent2, ushort[] child )
		{
			// temporary array to specify if certain gene already
			// present in the child
			bool[]	geneIsBusy = new bool[length];
			// previous gene in the child and two next candidates
			ushort	prev, next1, next2;
			// candidates validness - candidate is valid, if it is not
			// yet in the child
			bool	valid1, valid2;

			int		j, k = length - 1;

			// first gene of the child is taken from the second parent
			prev = child[0] = parent2[0];
			geneIsBusy[prev] = true;

			// resolve all other genes of the child
			for ( int i = 1; i < length; i++ )
			{
				// find the next gene after PREV in both parents
				// 1
				for ( j = 0; j < k; j++ )
				{
					if ( parent1[j] == prev )
						break;
				}
				next1 = ( j == k ) ? parent1[0] : parent1[j + 1];
				// 2
				for ( j = 0; j < k; j++ )
				{
					if ( parent2[j] == prev )
						break;
				}
				next2 = ( j == k ) ? parent2[0] : parent2[j + 1];

				// check candidate genes for validness
				valid1 = !geneIsBusy[next1];
				valid2 = !geneIsBusy[next2];

				// select gene
				if ( valid1 && valid2 )
				{
					// both candidates are valid
					// select one of theme randomly
					prev = ( rand.Next( 2 ) == 0 ) ? next1 : next2;
				}
				else if ( !( valid1 || valid2 ) )
				{
					// none of candidates is valid, so
					// select random gene which is not in the child yet
					int r = j = rand.Next( length );

					// go down first
					while ( ( r < length ) && ( geneIsBusy[r] == true ) )
						r++;
					if ( r == length )
					{
						// not found, try to go up
						r = j - 1;
						while ( geneIsBusy[r] == true )	// && ( r >= 0 )
							r--;
					}
					prev = (ushort) r;
				}
				else
				{
					// one of candidates is valid
					prev = ( valid1 ) ? next1 : next2;
				}

				child[i] = prev;
				geneIsBusy[prev] = true;
			}
		}
	}
}
