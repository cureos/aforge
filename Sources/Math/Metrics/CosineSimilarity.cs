// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace AForge.Math.Metrics
{
    using System;

    /// <summary>
    /// Cosine similarity metric. 
    /// </summary>
    /// 
    /// <remarks><para>This class represents the 
    /// <a href="http://en.wikipedia.org/wiki/Cosine_similarity">Cosine Similarity metric</a>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate new similarity class
    /// CosineSimilarity sim = new CosineSimilarity( ); 
    /// // create two vectors for inputs
    /// double[] p = new double[] { 2.5, 3.5, 3.0, 3.5, 2.5, 3.0 };
    /// double[] q = new double[] { 3.0, 3.5, 1.5, 5.0, 3.5, 3.0 };
    /// // get simirarity between the two vectors
    /// double similarityScore = sim.GetSimilarityScore( p, q );
    /// </code>    
    /// </remarks>
    /// 
    public sealed class CosineSimilarity : ISimilarity
    {
        /// <summary>
        /// Returns similarity score for two N-dimensional double vectors. 
        /// </summary>
        /// 
        /// <param name="p">1st point vector.</param>
        /// <param name="q">2nd point vector.</param>
        /// 
        /// <returns>Returns Cosine similarity between two supplied vectors.</returns>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two vectors are of different dimensions (if specified
        /// array have different length).</exception>
        /// 
        public double GetSimilarityScore( double[] p, double[] q )
        {
            double similarity;

            CosineDistance dist = new CosineDistance( );
            similarity = (double) 1 - dist.GetDistance( p, q );

            return similarity;
        }
    }
}
