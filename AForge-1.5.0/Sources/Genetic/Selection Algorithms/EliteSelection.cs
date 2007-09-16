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
	/// Elite selection method
	/// </summary>
	/// 
	/// <remarks>Elite selection method selects specified amount of
	/// best chromosomes to the next generation</remarks> 
	/// 
	public class EliteSelection : ISelectionMethod
	{
		private bool shuffle = true;

		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Determines if population should be shuffled after applying selection
		/// </summary>
		///
		/// <remarks>If the value is <b>false</b>, then the result new generation
		/// is sorted in the descending order. If the value is <b>true</b>, then
		/// the new generation is shuffled.</remarks> 
		///
		public bool Shuffle
		{
			get { return shuffle; }
			set { shuffle = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EliteSelection"/> class
		/// </summary>
		public EliteSelection( ) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="EliteSelection"/> class
		/// </summary>
		/// 
		/// <param name="shuffle">Specifies if new generation should be shuffled after
		/// applying selection</param>
		/// 
		public EliteSelection( bool shuffle )
		{
			this.shuffle = shuffle;
		}


		/// <summary>
		/// Apply selection to the population
		/// </summary>
		/// 
		/// <param name="chromosomes">Population, which should be filtered</param>
		/// <param name="size">The amount of chromosomes to keep</param>
		/// 
		/// <remarks>Filters specified population according to the implemented
		/// algorithm</remarks>
		/// 
		public void ApplySelection( ArrayList chromosomes, int size )
		{
			// sort chromosomes
			chromosomes.Sort( );

			// remove bad chromosomes
			chromosomes.RemoveRange( size, chromosomes.Count - size );

			// shuffle chromosomes
			if ( shuffle )
			{
				for ( int i = 0, n = size / 2; i < n; i++ )
				{
					int c1 = rand.Next( size );
					int c2 = rand.Next( size );

					// swap two chromosomes
					object temp = chromosomes[c1];
					chromosomes[c1] = chromosomes[c2];
					chromosomes[c2] = temp;
				}
			}
		}
	}
}
