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
	/// Tree chromosome represents a tree of genes. It is used for
	/// different tasks of Genetic Programming.
	/// </summary>
	public class GPTreeChromosome : IChromosome
	{
		// tree root
		private GPTreeNode		root = new GPTreeNode( );
		// chromosome's fitness
		protected double		fitness = 0;


		// maximum initial level of the tree
		private static int		maxInitialLevel = 3;
		// maximum level of the tree
		private static int		maxLevel = 5;

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
		/// Maximum initial level of genetic trees
		/// </summary>
		public static int MaxInitialLevel
		{
			get { return maxInitialLevel; }
			set { maxInitialLevel = Math.Max( 1, Math.Min( 25, value ) ); }
		}

		/// <summary>
		/// Maximum level of genetic trees
		/// </summary>
		public static int MaxLevel
		{
			get { return maxLevel; }
			set { maxLevel = Math.Max( 1, Math.Min( 50, value ) ); }
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public GPTreeChromosome( IGPGene ancestor )
		{
			// make the ancestor gene to be as temporary root of the tree
			root.Gene = ancestor;
			// call tree regeneration function
			Generate( );
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected GPTreeChromosome( GPTreeChromosome source )
		{
			root	= (GPTreeNode) source.root.Clone( );
			fitness	= source.fitness;
		}

		/// <summary>
		/// Get string representation of the chromosome. Return the chromosome
		/// in reverse polish notation (postfix notation).
		/// </summary>
		public override string ToString( )
		{
			return root.ToString( );
		}

		/// <summary>
		/// Compare two chromosomes
		/// </summary>
		public int CompareTo( object o )
		{
			double f = ((GPTreeChromosome) o).fitness;

			return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
		}

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		public virtual void Generate( )
		{
			// randomize the root
			root.Gene.Generate( );
			// create children
			if ( root.Gene.ArgumentsCount != 0 )
			{
				root.Children = new ArrayList( );
				for ( int i = 0; i < root.Gene.ArgumentsCount; i++ )
				{
					// create new child
					GPTreeNode child = new GPTreeNode( );
					Generate( child, rand.Next( maxInitialLevel ) );
					// add the new child
					root.Children.Add( child );
				}
			}
		}

		/// <summary>
		/// Generate chromosome's subtree of specified level
		/// </summary>
		protected void Generate( GPTreeNode node, int level )
		{
			// create gene for the node
			if ( level == 0 )
			{
				// the gene should be an argument
				node.Gene = root.Gene.CreateNew( GPGeneType.Argument );
			}
			else
			{
				// the gene can be function or argument
				node.Gene = root.Gene.CreateNew( );
			}

			// add children
			if ( node.Gene.ArgumentsCount != 0 )
			{
				node.Children = new ArrayList( );
				for ( int i = 0; i < node.Gene.ArgumentsCount; i++ )
				{
					// create new child
					GPTreeNode child = new GPTreeNode( );
					Generate( child, level - 1 );
					// add the new child
					node.Children.Add( child );
				}
			}
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public virtual IChromosome CreateOffspring( )
		{
			return new GPTreeChromosome( root.Gene.Clone( ) );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public virtual IChromosome Clone( )
		{
			return new GPTreeChromosome( this );
		}

		/// <summary>
		/// Mutation operator
		/// </summary>
		public virtual void Mutate( )
		{
			// current tree level
			int			currentLevel = 0;
			// current node
			GPTreeNode	node = root;

			for ( ; ; )
			{
				// regenerate node if it does not have children
				if ( node.Children == null )
				{
					if ( currentLevel == maxLevel )
					{
						// we reached maximum possible level, so the gene
						// can be an argument only
						node.Gene.Generate( GPGeneType.Argument );
					}
					else
					{
						// generate subtree
						Generate( node, rand.Next( maxLevel - currentLevel ) );
					}
					break;
				}

				// if it is a function node, than we need to get a decision, about
				// mutation point - the node itself or one of its children
				int r = rand.Next( node.Gene.ArgumentsCount + 1 );

				if ( r == node.Gene.ArgumentsCount )
				{
					// node itself should be regenerated
					node.Gene.Generate( );

					// check current type
					if ( node.Gene.GeneType == GPGeneType.Argument )
					{
						node.Children = null;
					}
					else
					{
						// create children's list if it was absent
						if ( node.Children == null )
							node.Children = node.Children = new ArrayList( );

						// check for missing or extra children
						if ( node.Children.Count != node.Gene.ArgumentsCount )
						{
							if ( node.Children.Count > node.Gene.ArgumentsCount )
							{
								// remove extra children
								node.Children.RemoveRange( node.Gene.ArgumentsCount, node.Children.Count - node.Gene.ArgumentsCount );
							}
							else
							{
								// add missing children
								for ( int i = node.Children.Count; i < node.Gene.ArgumentsCount; i++ )
								{
									// create new child
									GPTreeNode child = new GPTreeNode( );
									Generate( child, rand.Next( maxLevel - currentLevel ) );
									// add the new child
									node.Children.Add( child );
								}
							}
						}
					}
					break;
				}

				// mutation goes further to one of the children
				node = (GPTreeNode) node.Children[ r ];
				currentLevel++;
			}
		}

		/// <summary>
		/// Crossover operator
		/// </summary>
		public virtual void Crossover( IChromosome pair )
		{
			GPTreeChromosome p = (GPTreeChromosome) pair;

			// check for correct pair
			if ( p != null )
			{
				// do we need to use root node for crossover ?
				if ( ( root.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
				{
					// give the root to the pair and use pair's part as a new root
					root = p.RandomSwap( root );
				}
				else
				{
					GPTreeNode node = root;

					for ( ; ; )
					{
						// choose random child
						int r = rand.Next( node.Gene.ArgumentsCount );
						GPTreeNode child = (GPTreeNode) node.Children[r];

						// swap the random node, if it is an end node or
						// random generator "selected" this node
						if ( ( child.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
						{
							// swap the node with pair's one
							node.Children[r] = p.RandomSwap( child );
							break;
						}

						// go further by tree
						node = child;
					}
				}
				// trim both of them
				Trim( root, maxLevel );
				Trim( p.root, maxLevel );
			}
		}

		/// <summary>
		/// Crossover helper routine - selects random node of chromosomes tree and
		/// swaps it with specified node.
		/// </summary>
		private GPTreeNode RandomSwap( GPTreeNode source )
		{
			GPTreeNode retNode = null;

			// swap root node ?
			if ( ( root.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
			{
				// replace current root and return it
				retNode	= root;
				root	= source;
			}
			else
			{
				GPTreeNode node = root;

				for ( ; ; )
				{
					// choose random child
					int r = rand.Next( node.Gene.ArgumentsCount );
					GPTreeNode child = (GPTreeNode) node.Children[r];

					// swap the random node, if it is an end node or
					// random generator "selected" this node
					if ( ( child.Children == null ) || ( rand.Next( maxLevel ) == 0 ) )
					{
						// swap the node with pair's one
						retNode = child;
						node.Children[r] = source;
						break;
					}

					// go further by tree
					node = child;
				}
			}
			return retNode;
		}

		/// <summary>
		/// Trim tree node, so its depth does not exceed specified level
		/// </summary>
		private static void Trim( GPTreeNode node, int level )
		{
			// check if the node has children
			if ( node.Children != null )
			{
				if ( level == 0 )
				{
					// remove all children
					node.Children = null;
					// and make the node of argument type
					node.Gene.Generate( GPGeneType.Argument );
				}
				else
				{
					// go further to children
					foreach ( GPTreeNode n in node.Children )
					{
						Trim( n, level - 1 );
					}
				}
			}
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
