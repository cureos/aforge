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
	/// Represents tree node of genetic programming tree
	/// </summary>
	public class GPTreeNode : ICloneable
	{
		// node's gene
		public IGPGene		Gene;
		//
		public ArrayList	Children;

		/// <summary>
		/// Constructor
		/// </summary>
		public GPTreeNode( ) { }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public GPTreeNode( IGPGene gene )
		{
			Gene = gene;
		}
		
		/// <summary>
		/// Get string representation of the node
		/// </summary>
		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );

			if ( Children != null )
			{
				// walk through all nodes
				foreach ( GPTreeNode node in Children )
				{
					sb.Append( node.ToString( ) );
				}
			}

			// add gene value
			sb.Append( Gene.ToString( ) );
			sb.Append( " " );

			return sb.ToString( );
		}

		/// <summary>
		/// Clone the tree node
		/// </summary>
		public object Clone( )
		{
			GPTreeNode clone = new GPTreeNode( );

			// clone gene
			clone.Gene = this.Gene.Clone( );
			// clone its children
			if ( this.Children != null )
			{
				clone.Children = new ArrayList( );
				// clone each child gene
				foreach ( GPTreeNode node in Children )
				{
					clone.Children.Add( node.Clone( ) );
				}
			}
			return clone;
		}
	}
}
