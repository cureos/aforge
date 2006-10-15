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
	public class EliteSelection : ISelectionMethod
	{
		private bool shuffle = true;

		// random number generator
		private static Random rand = new Random( (int) DateTime.Now.Ticks );

		/// <summary>
		/// Determines if population should be shuffled after applying selection
		/// </summary>
		public bool Shuffle
		{
			get { return shuffle; }
			set { shuffle = value; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public EliteSelection( ) { }

		/// <summary>
		/// Constructor
		/// </summary>
		public EliteSelection( bool shuffle )
		{
			this.shuffle = shuffle;
		}


		/// <summary>
		/// Apply selection to the population
		/// </summary>
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
