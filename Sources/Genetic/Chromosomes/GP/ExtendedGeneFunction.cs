// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Genetic programming gene, which represents arithmetic functions and
	/// common mathematical function
	/// </summary>
	public class ExtendedGeneFunction : IGPGene
	{
		// supported functions list
		protected enum Functions
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			Sin,
			Cos,
			Ln,
			Exp,
			Sqrt
		}

		protected const int FunctionsCount = 9;

		// gene type
		private GPGeneType	type;
		// total amount of variables in the task which is supposed to be solved
		private int			variablesCount;
		//
		private int			val;
		// arguments count
		private int			argumentsCount = 0;
		
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
		/// Arguments count in case the gene is of function type
		/// </summary>
		public int ArgumentsCount
		{
			get { return argumentsCount; }
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
		public ExtendedGeneFunction( int variablesCount ) : this( variablesCount, true ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public ExtendedGeneFunction( int variablesCount, GPGeneType type )
		{
			this.variablesCount = variablesCount;
			// generate the gene value
			Generate( type );
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected ExtendedGeneFunction( int variablesCount, bool random )
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

					case Functions.Sin:			// sine
						return "sin";

					case Functions.Cos:			// cosine
						return "cos";

					case Functions.Ln:			// natural logarithm
						return "ln";

					case Functions.Exp:			// exponent
						return "exp";

					case Functions.Sqrt:		// square root
						return "sqrt";
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
			ExtendedGeneFunction clone = new ExtendedGeneFunction( variablesCount, false );
			// ... with the same type and value
			clone.type	= type;
			clone.val	= val;
			clone.argumentsCount = argumentsCount;

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
			// arguments count
			argumentsCount = ( type == GPGeneType.Argument ) ? 0 :
				( val <= (int) Functions.Divide ) ? 2 : 1;
		}

		/// <summary>
		/// Creates new gene with random type
		/// </summary>
		public IGPGene CreateNew( )
		{
			return new ExtendedGeneFunction( variablesCount );
		}

		/// <summary>
		/// Creates new gene with certain type
		/// </summary>
		public IGPGene CreateNew( GPGeneType type )
		{
			return new ExtendedGeneFunction( variablesCount, type );
		}
	}
}
