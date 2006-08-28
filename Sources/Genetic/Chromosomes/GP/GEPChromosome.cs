// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;
	using System.Collections;
	using System.Text;

	/// <summary>
	/// The chromosome represents a Gene Expression. It is used for
	/// different tasks of Genetic Expression Programming.
	/// </summary>
	public class GEPChromosome : IChromosome
	{
		// head length
		protected int			headLength;
		// chromosome's length
		protected int			length;
		// chromosome's genes
		protected IGPGene[]		genes;
		
		// chromosome's fitness
		protected double		fitness = 0;

		// random number generator for chromosoms generation
		protected static Random	rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Chromosome's fintess value
		/// </summary>
		public double Fitness
		{
			get { return fitness; }
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public GEPChromosome( IGPGene ancestor, int headLength )
		{
			// store head length
			this.headLength = headLength;
			// calculate chromosome's length
			length = headLength + headLength * ( ancestor.MaxArgumentsCount - 1 ) + 1;
			// allocate genes array
			genes = new IGPGene[length];
			// save ancestor as a temporary head
			genes[0] = ancestor;
			// generate the chromosome
			Generate( );
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected GEPChromosome( GEPChromosome source )
		{
			headLength	= source.headLength;
			length		= source.length;
			fitness		= source.fitness;
			// allocate genes array
			genes = new IGPGene[length];
			// copy genes
			for ( int i = 0; i < length; i++ )
				genes[i] = source.genes[i].Clone( );
		}


		/// <summary>
		/// Get string representation of the chromosome. Returns the chromosome
		/// in reverse polish notation (postfix notation).
		/// </summary>
		public override string ToString( )
		{
			// return string representation of the chromosomes tree
			return GetTree( ).ToString( );
		}

		/// <summary>
		/// Get string representation of the chromosome. Returns the chromosome
		/// in native linear representation. The method is used for debugging
		/// mostly.
		/// </summary>
		public string ToStringNative( )
		{
			StringBuilder sb = new StringBuilder( );

			foreach ( IGPGene gene in genes )
			{
				sb.Append( gene.ToString( ) );
				sb.Append( " " );
			}
			return sb.ToString( );
		}

		/// <summary>
		/// Compare two chromosomes
		/// </summary>
		public int CompareTo( object o )
		{
			double f = ((GEPChromosome) o).fitness;

			return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
		}

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		public virtual void Generate( )
		{
			// randomize the root
			genes[0].Generate( );
			// generate the rest of the head
			for ( int i = 1; i < headLength; i++ )
			{
				genes[i] = genes[0].CreateNew( );
			}
			// generate the tail
			for ( int i = headLength; i < length; i++ )
			{
				genes[i] = genes[0].CreateNew( GPGeneType.Argument );
			}
		}

		/// <summary>
		/// Get tree representation of the chromosome. For GEP chromosomes
		/// it is required to build the tree from linear representation, since
		/// linear form is native for GEP.
		/// </summary>
		protected GPTreeNode GetTree( )
		{
			// function node queue. the queue contains function node,
			// which requires children. when a function node receives
			// all children, it will be removed from the queue
			Queue functionNodes = new Queue( );

			// create root node
			GPTreeNode root = new GPTreeNode( genes[0] );

			// check children amount of the root node
			if ( root.Gene.ArgumentsCount != 0 )
			{
				root.Children = new ArrayList( );
				// place the root to the queue
				functionNodes.Enqueue( root );

				// go through genes
				for ( int i = 1; i < length; i++ )
				{
					// create new node
					GPTreeNode node = new GPTreeNode( genes[i] );

					// if next gene represents function, place it to the queue
					if ( genes[i].GeneType == GPGeneType.Function )
					{
						node.Children = new ArrayList( );
						functionNodes.Enqueue( node );
					}

					// get function node from the top of the queue
					GPTreeNode parent = (GPTreeNode) functionNodes.Peek( );

					// add new node to children of the parent node
					parent.Children.Add( node );

					// remove the parent node from the queue, if it is
					// already complete
					if ( parent.Children.Count == parent.Gene.ArgumentsCount )
					{
						functionNodes.Dequeue( );

						// check the queue if it is empty
						if ( functionNodes.Count == 0 )
							break;
					}
				}
			}
			// return formed tree
			return root;
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public virtual IChromosome CreateOffspring( )
		{
			return new GEPChromosome( genes[0].Clone( ), headLength );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public virtual IChromosome Clone( )
		{
			return new GEPChromosome( this );
		}

		/// <summary>
		/// Mutation operator
		/// </summary>
		public virtual void Mutate( )
		{
			// randomly choose mutation method
			switch ( rand.Next( 3 ) )
			{
				case 0:		// ordinary gene mutation
					MutateGene( );
					break;

				case 1:		// IS transposition
					TransposeIS( );
					break;

				case 2:		// root transposition
					TransposeRoot( );
					break;
			}
		}


		/// <summary>
		/// Usual gene mutation
		/// </summary>
		public void MutateGene( )
		{
			// select random point of mutation
			int mutationPoint = rand.Next( length );

			if ( mutationPoint < headLength )
			{
				// genes from head can be randomized freely (type may change)
				genes[mutationPoint].Generate( );
			}
			else
			{
				// genes from tail cannot change their type - they
				// should be always arguments
				genes[mutationPoint].Generate( GPGeneType.Argument );
			}
		}

		/// <summary>
		/// Transposition of IS elements (insertion sequence)
		/// </summary>
		public virtual void TransposeIS( )
		{
			// select source point (may be any point of the chromosome)
			int sourcePoint = rand.Next( length );
			// calculate maxim source length
			int maxSourceLength = length - sourcePoint;
			// select tartget insertion point in the head (except first position)
			int targetPoint = rand.Next( headLength - 1 ) + 1;
			// calculate maximum target length
			int maxTargetLength = headLength - targetPoint;
			// select randomly transposon length
			int transposonLength = rand.Next( Math.Min( maxTargetLength, maxSourceLength ) ) + 1;
			// genes copy
			IGPGene[] genesCopy = new IGPGene[transposonLength];

			// copy genes from source point
			for ( int i = sourcePoint, j = 0; j < transposonLength; i++, j++ )
			{
				genesCopy[j] = genes[i].Clone( );
			}

			// copy genes to target point
			for ( int i = targetPoint, j = 0; j < transposonLength; i++, j++ )
			{
				genes[i] = genesCopy[j];
			}
		}

		/// <summary>
		/// Root transposition
		/// </summary>
		public virtual void TransposeRoot( )
		{
			// select source point (may be any point in the head of the chromosome)
			int sourcePoint = rand.Next( headLength );
			// scan downsrteam the head searching for function gene
			while ( ( genes[sourcePoint].GeneType != GPGeneType.Function ) && ( sourcePoint < headLength ) )
			{
				sourcePoint++;
			}
			// return (do nothing) if function gene was not found
			if ( sourcePoint == headLength )
				return;

			// calculate maxim source length
			int maxSourceLength = headLength - sourcePoint;
			// select randomly transposon length
			int transposonLength = rand.Next( maxSourceLength ) + 1;
			// genes copy
			IGPGene[] genesCopy = new IGPGene[transposonLength];

			// copy genes from source point
			for ( int i = sourcePoint, j = 0; j < transposonLength; i++, j++ )
			{
				genesCopy[j] = genes[i].Clone( );
			}

			// shift the head
			for ( int i = headLength - 1; i >= transposonLength; i-- )
			{
				genes[i] = genes[i - transposonLength];
			}

			// put new root
			for ( int i = 0; i < transposonLength; i++ )
			{
				genes[i] = genesCopy[i];
			}
		}

		
		/// <summary>
		/// Crossover operator
		/// </summary>
		public virtual void Crossover( IChromosome pair )
		{
			GEPChromosome p = (GEPChromosome) pair;

			// check for correct chromosome
			if ( p != null )
			{
				// choose recombination method
				if ( rand.Next( 2 ) == 0 )
				{
					RecombinationOnePoint( p );
				}
				else
				{
					RecombinationTwoPoint( p );
				}
			}
		}

		/// <summary>
		/// One point recombination
		/// </summary>
		public void RecombinationOnePoint( GEPChromosome pair )
		{
			// check for correct pair
			if ( ( pair.length == length ) )
			{
				// crossover point
				int crossOverPoint = rand.Next( length - 1 ) + 1;
				// length of chromosome to be crossed
				int crossOverLength = length - crossOverPoint;

				// swap parts of chromosomes
				Recombine( genes, pair.genes, crossOverPoint, crossOverLength );
			}
		}

		/// <summary>
		/// Two point recombination
		/// </summary>
		public void RecombinationTwoPoint( GEPChromosome pair )
		{
			// check for correct pair
			if ( ( pair.length == length ) )
			{
				// crossover point
				int crossOverPoint = rand.Next( length - 1 ) + 1;
				// length of chromosome to be crossed
				int crossOverLength = length - crossOverPoint;

				// if crossover length already equals to 1, then it becomes
				// usual one point crossover. otherwise crossover length
				// also randomly chosen
				if ( crossOverLength != 1 )
				{
					crossOverLength = rand.Next( crossOverLength - 1 ) + 1;
				}

				// swap parts of chromosomes
				Recombine( genes, pair.genes, crossOverPoint, crossOverLength );
			}
		}

		/// <summary>
		/// Swap parts of two chromosomes
		/// </summary>
		protected static void Recombine( IGPGene[] src1, IGPGene[] src2, int point, int length )
		{
			// temporary array
			IGPGene[] temp = new IGPGene[length];

			// copy part of first chromosome to temp
			Array.Copy( src1, point, temp, 0, length );
			// copy part of second chromosome to the first
			Array.Copy( src2, point, src1, point, length );
			// copy temp to the second
			Array.Copy( temp, 0, src2, point, length );
		}


		/// <summary>
		/// Evaluate chromosome with specified fitness function
		/// </summary>
		public void Evaluate( IFitnessFunction function )
		{
			fitness = function.Evaluate( this );
		}
	}
}
