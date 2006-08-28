// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
	using System;

	/// <summary>
	/// Binary chromosome, which supports length from 2 till 64
	/// </summary>
	public class BinaryChromosome : IChromosome
	{
		protected int		length;			// chromosome's length
		protected ulong		val = 0;		// chromosome's value 
		protected double	fitness = 0;	// chromosome's fitness

		// random number generator for chromosoms generation
		protected static Random	rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Chromosome's maximum length
		/// </summary>
		public const int MaxLength = 64;

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
		public ulong Value
		{
			get { return val & ( 0xFFFFFFFFFFFFFFFF >> ( 64 - length ) ); }
		}

		/// <summary>
		/// Max possible chromosome's value
		/// </summary>
		public ulong MaxValue
		{
			get { return 0xFFFFFFFFFFFFFFFF >> ( 64 - length ); }
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
		public BinaryChromosome( int length )
		{
			this.length = Math.Max( 2, Math.Min( MaxLength, length ) );
			// randomize the chromosome
				Generate( );
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected BinaryChromosome( BinaryChromosome source )
		{
			length	= source.length;
			val		= source.val;
			fitness	= source.fitness;
		}

		/// <summary>
		/// Get string representation of the chromosome
		/// </summary>
		public override string ToString( )
		{
			ulong	tval = val;
			char[]	chars = new char[length];

			for ( int i = length - 1; i >= 0; i-- )
			{
				chars[i] = (char) ( ( tval & 1 ) + '0' );
				tval >>= 1;
			}

			// return the result string
			return new string( chars );
		}

		/// <summary>
		/// Compare two chromosomes
		/// </summary>
		public int CompareTo( object o )
		{
			double f = ((BinaryChromosome) o).fitness;

			return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
		}

		/// <summary>
		/// Generate random chromosome value
		/// </summary>
		public virtual void Generate( )
		{
			byte[] bytes = new byte[8];

			// generate value
			rand.NextBytes( bytes );
			val = BitConverter.ToUInt64( bytes, 0 );
		}

		/// <summary>
		/// Create new random chromosome (factory method)
		/// </summary>
		public virtual IChromosome CreateOffspring( )
		{
			return new BinaryChromosome( length );
		}

		/// <summary>
		/// Clone the chromosome
		/// </summary>
		public virtual IChromosome Clone( )
		{
			return new BinaryChromosome( this );
		}

		/// <summary>
		/// Mutation operator
		/// </summary>
		public virtual void Mutate( )
		{
			val ^= ( (ulong) 1 << rand.Next( length ) );
		}

		/// <summary>
		/// Crossover operator
		/// </summary>
		public virtual void Crossover( IChromosome pair )
		{
			BinaryChromosome p = (BinaryChromosome) pair;

			// check for correct pair
			if ( ( p != null ) && ( p.length == length ) )
			{
				int		crossOverPoint = 63 - rand.Next( length - 1 );
				ulong	mask1 = 0xFFFFFFFFFFFFFFFF >> crossOverPoint;
				ulong	mask2 = ~mask1;

				ulong	v1 = val;
				ulong	v2 = p.val;

				// calculate new values
				val		= ( v1 & mask1 ) | ( v2 & mask2 );
				p.val	= ( v2 & mask1 ) | ( v1 & mask2 );
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
