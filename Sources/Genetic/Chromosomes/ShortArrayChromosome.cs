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
	/// Short Array Chromosome is a chromosome, which is represented
	/// by array of unsigned short values. Array length is in the
	/// range of [2, 65536]
	/// </summary>
	public class ShortArrayChromosome : IChromosome
	{
		protected int		length;			// chromosome's length
		protected int		maxValue;		// max value of chromosome's gene
		protected ushort[]	val = null;		// chromosome's value 
		protected double	fitness = 0;	// chromosome's fitness

		// random number generator for chromosoms generation
		protected static Random	rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Chromosome's maximum length
		/// </summary>
		public const int MaxLength = 65536;

		/// <summary>
		/// Chromosome's length
		/// </summary>
		public int Length
		{
			get { return length; }
		}

		/// <summary>
		/// Chromosome's value
		/// </summary>
		public ushort[] Value
		{
			get { return val; }
		}

		/// <summary>
		/// Max possible value of single chromosomes element - gene 
		/// </summary>
		public int MaxValue
		{
			get { return maxValue; }
		}

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
		public ShortArrayChromosome( int length ) : this( length, ushort.MaxValue ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public ShortArrayChromosome( int length, int maxValue )
		{
			// save parameters
			this.length		= Math.Max( 2, Math.Min( MaxLength, length ) );
			this.maxValue	= Math.Max( 1, Math.Min( ushort.MaxValue, maxValue ) );

			// allocate array
			val = new ushort[this.length];

			// generate random chromosome
			Generate( );
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected ShortArrayChromosome( ShortArrayChromosome source )
		{
			// copy all properties
			length		= source.length;
			maxValue	= source.maxValue;
			val			= (ushort[]) source.val.Clone( );
			fitness		= source.fitness;
		}

		/// <summary>
		/// Get string representation of the chromosome
		/// </summary>
		public override string ToString( )
		{
			StringBuilder sb = new StringBuilder( );

			// append first gene
			sb.Append( val[0] );
			// append all other genes
			for ( int i = 1; i < length; i++ )
			{
				sb.Append( ' ' );
				sb.Append( val[i] );
			}

			return sb.ToString( );
		}

		/// <summary>
		/// Compare two chromosomes
		/// </summary>
		public int CompareTo( object o )
		{
			double f = ((ShortArrayChromosome) o).fitness;

			return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
		}

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		public virtual void Generate( )
		{
			int max = maxValue + 1;

			for ( int i = 0; i < length; i++ )
			{
				// generate next value
				val[i] = (ushort) rand.Next( max );
			}
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public virtual IChromosome CreateOffspring( )
		{
			return new ShortArrayChromosome( length, maxValue );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public virtual IChromosome Clone( )
		{
			return new ShortArrayChromosome( this );
		}

		/// <summary>
		/// Mutation operator
		/// </summary>
		public virtual void Mutate( )
		{
			// get random index
			int i = rand.Next( length );
			// randomize the gene
			val[i] = (ushort) rand.Next( maxValue + 1 );
		}

		/// <summary>
		/// Crossover operator
		/// </summary>
		public virtual void Crossover( IChromosome pair )
		{
			ShortArrayChromosome p = (ShortArrayChromosome) pair;

			// check for correct pair
			if ( ( p != null ) && ( p.length == length ) )
			{
				// crossover point
				int crossOverPoint = rand.Next( length - 1 ) + 1;
				// length of chromosome to be crossed
				int crossOverLength = length - crossOverPoint;
				// temporary array
				ushort[] temp = new ushort[crossOverLength];

				// copy part of first (this) chromosome to temp
				Array.Copy( val, crossOverPoint, temp, 0, crossOverLength );
				// copy part of second (pair) chromosome to the first
				Array.Copy( p.val, crossOverPoint, val, crossOverPoint, crossOverLength );
				// copy temp to the second
				Array.Copy( temp, 0, p.val, crossOverPoint, crossOverLength );
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
