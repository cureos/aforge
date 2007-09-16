// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Genetic programming gene, which represents simple arithmetic function
	/// </summary>
	public class SimpleGeneFunction : IGPGene
	{
		// supported functions list
		protected enum Functions
		{
			Add,
			Subtract,
			Multiply,
			Divide,
		}

		protected const int FunctionsCount = 4;

		// gene type
		private GPGeneType	type;
		// total amount of variables in the task which is supposed to be solved
		private int			variablesCount;
		//
		private int			val;
		
		// random number generator for chromosoms generation
		protected static Random	rand = new Random( (int) DateTime.Now.Ticks );

		
		/// <summary>
		/// Gene type
		/// </summary>
		public GPGeneType GeneType
		{
			get { return type; }
		}

		/// <summary>
		/// Arguments count
		/// </summary>
		public int ArgumentsCount
		{
			get { return ( type == GPGeneType.Argument ) ? 0 : 2; }
		}

		/// <summary>
		/// Maximum arguments count
		/// </summary>
		public int MaxArgumentsCount
		{
			get { return 2; }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public SimpleGeneFunction( int variablesCount ) : this( variablesCount, true ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public SimpleGeneFunction( int variablesCount, GPGeneType type )
		{
			this.variablesCount = variablesCount;
			// generate the gene value
			Generate( type );
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected SimpleGeneFunction( int variablesCount, bool random )
		{
			this.variablesCount = variablesCount;
			// generate the gene value
			if ( random )
				Generate( );
		}

		/// <summary>
		/// Get string representation of the chromosome
		/// </summary>
		public override string ToString( )
		{
			if ( type == GPGeneType.Function )
			{
				// get function string representation
				switch ( (Functions) val )
				{
					case Functions.Add:			// addition
						return "+";

					case Functions.Subtract:	// subtraction
						return "-";

					case Functions.Multiply:	// multiplication
						return "*";

					case Functions.Divide:		// division
						return "/";
				}
			}

			// get argument string representation
			return string.Format( "${0}", val );
		}

		/// <summary>
		/// Clone the gene
		/// </summary>
		public IGPGene Clone( )
		{
			// create new gene ...
			SimpleGeneFunction clone = new SimpleGeneFunction( variablesCount, false );
			// ... with the same type and value
			clone.type	= type;
			clone.val	= val;

			return clone;
		}

		/// <summary>
		/// Generate random gene with random type
		/// </summary>
		public void Generate( )
		{
			// give more chance to function
			Generate( ( rand.Next( 4 ) == 3 ) ? GPGeneType.Argument : GPGeneType.Function );
		}

		/// <summary>
		/// Generate random gene with certain type
		/// </summary>
		public void Generate( GPGeneType type )
		{
			// gene type
			this.type = type;
			// gene value
			val = rand.Next( ( type == GPGeneType.Function ) ? FunctionsCount : variablesCount );
				
		}

		/// <summary>
		/// Creates new gene with random type
		/// </summary>
		public IGPGene CreateNew( )
		{
			return new SimpleGeneFunction( variablesCount );
		}

		/// <summary>
		/// Creates new gene with certain type
		/// </summary>
		public IGPGene CreateNew( GPGeneType type )
		{
			return new SimpleGeneFunction( variablesCount, type );
		}
	}
}
