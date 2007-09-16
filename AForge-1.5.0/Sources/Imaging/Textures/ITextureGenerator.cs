// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Textures
{
	using System;

	/// <summary>
	/// Texture generator interface
	/// </summary>
    /// 
    /// <remarks>Each texture generator generates a texture of the specified size and returns
	/// a two dimensional array of intensities in the range of [0; 1].</remarks>
    /// 
	public interface ITextureGenerator
	{
		/// <summary>
		/// Generate texture
		/// </summary>
        /// 
        /// <param name="width">Texture's width</param>
        /// <param name="height">Texture's height</param>
        /// 
        /// <returns>Two dimensional array of intensities</returns>
        /// 
        /// <remarks>Generates new texture with specified dimension.</remarks>
        /// 
		float[,] Generate( int width, int height );

		/// <summary>
		/// Reset generator
		/// </summary>
        /// 
        /// <remarks>Resets the generator - resets interl variables, regenerates
        /// internal random numbers, etc.</remarks>
        /// 
		void Reset( );
	}
}
