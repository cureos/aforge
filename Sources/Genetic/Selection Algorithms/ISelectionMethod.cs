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
	/// Selection method interface
	/// </summary>
	public interface ISelectionMethod
	{
		/// <summary>
		/// Apply selection to the population
		/// </summary>
		void ApplySelection( ArrayList chromosomes, int size );
	}
}