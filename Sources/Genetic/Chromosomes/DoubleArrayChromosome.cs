// AForge Genetic Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Genetic
{
    using System;
    using System.Text;
    using AForge.Math.Random;

    /// <summary>
    /// Double array chromosome
    /// </summary>
    /// 
    public class DoubleArrayChromosome : IChromosome
    {
        /// <summary>
        /// Chromosome generator 
        /// </summary>
        protected IRandomNumberGenerator chromosomeGenerator;

        /// <summary>
        /// Mutation generator 
        /// </summary>
        protected IRandomNumberGenerator mutationGenerator;

        /// <summary>
        /// Chromosome's length
        /// </summary>
        private int length;

        /// <summary>
        /// Chromosome's values
        /// </summary>
        protected double[] values = null;

        /// <summary>
        /// Chromosome's fitness 
        /// </summary>
        protected double fitness = 0;

        /// <summary>
        /// Random number generator for mutation point selection
        /// </summary>
        protected static Random rand = new Random( (int) DateTime.Now.Ticks );

        /// <summary>
        /// Chromosome's length
        /// </summary>
        /// 
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Chromosome's values
        /// </summary>
        /// 
        public double[] Value
        {
            get { return values; }
        }

        /// <summary>
        /// Chromosome's fintess value
        /// </summary>
        /// 
        public double Fitness
        {
            get { return fitness; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class
        /// </summary>
        /// 
        /// <param name="chromosomeGenerator">Chromosome generator</param>
        /// <param name="mutationGenerator">Mutation generator</param>
        /// <param name="length">Chromosome's length</param>
        /// 
        public DoubleArrayChromosome(
            IRandomNumberGenerator chromosomeGenerator,
            IRandomNumberGenerator mutationGenerator,
            int length )
        {
            // save parameters
            this.chromosomeGenerator = chromosomeGenerator;
            this.mutationGenerator = mutationGenerator;
            this.length = length;

            // allocate array
            values = new double[length];

            // generate random chromosome
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleArrayChromosome"/> class
        /// </summary>
        /// 
        /// <param name="source">Source chromosome</param>
        /// 
        /// <remarks>This constructor is a copy constructor, which creates an exact
        /// copy of source chromosome.</remarks>
        /// 
        public DoubleArrayChromosome( DoubleArrayChromosome source )
        {
            this.chromosomeGenerator    = source.chromosomeGenerator;
            this.mutationGenerator      = source.mutationGenerator;
            this.length                 = source.length;
            this.fitness                = source.fitness;

            // copy genes
            values = (double[]) source.values.Clone( );
        }

        /// <summary>
        /// Get string representation of the chromosome
        /// </summary>
        /// 
        /// <returns>Returns string representation of the chromosome</returns>
        /// 
        public override string ToString( )
        {
            StringBuilder sb = new StringBuilder( );

            // append first gene
            sb.Append( values[0] );
            // append all other genes
            for ( int i = 1; i < length; i++ )
            {
                sb.Append( ' ' );
                sb.Append( values[i] );
            }

            return sb.ToString( );
        }

        /// <summary>
        /// Compare two chromosomes
        /// </summary>
        /// 
        /// <param name="o">Second chromosome to compare with</param>
        /// 
        /// <returns>Returns comparison result: 0 - if fitness values of both
        /// chromosomes are equal; 1 - if fitness value of this chromosome's is
        /// smaller; -1 - if fitness value of second chromosome's is smaller.</returns>
        /// 
        public int CompareTo( object o )
        {
            double f = ( (DoubleArrayChromosome) o ).fitness;

            return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
        }

        /// <summary>
        /// Generate random chromosome values
        /// </summary>
        /// 
        public virtual void Generate( )
        {
            for ( int i = 0; i < length; i++ )
            {
                // generate next value
                values[i] = chromosomeGenerator.Next( );
            }
        }

        /// <summary>
        /// Create new random chromosome (factory method)
        /// </summary>
        /// 
        /// <returns>Returns new chromosome</returns>
        /// 
        public virtual IChromosome CreateOffspring( )
        {
            return new DoubleArrayChromosome( chromosomeGenerator, mutationGenerator, length );
        }

        /// <summary>
        /// Clone the chromosome
        /// </summary>
        /// 
        /// <returns>Returns chromosome's clone</returns>
        /// 
        public virtual IChromosome Clone( )
        {
            return new DoubleArrayChromosome( this );
        }

        /// <summary>
        /// Mutation operator
        /// </summary>
        /// 
        /// <remarks>Adds a random number to each chromosome's gene.
        /// The random numbers are generated with help of mutation generator.
        /// </remarks>
        /// 
        public virtual void Mutate( )
        {
            for ( int i = 0; i < length; i++ )
            {
                // generate next value
                values[i] += mutationGenerator.Next( );
            }
        }

        /// <summary>
        /// Crossover operator
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome for crossover</param>
        /// 
        public virtual void Crossover( IChromosome pair )
        {
            DoubleArrayChromosome p = (DoubleArrayChromosome) pair;

            // check for correct pair
            if ( ( p != null ) && ( p.length == length ) )
            {
                // crossover point
                int crossOverPoint = rand.Next( length - 1 ) + 1;
                // length of chromosome to be crossed
                int crossOverLength = length - crossOverPoint;
                // temporary array
                double[] temp = new double[crossOverLength];

                // copy part of first (this) chromosome to temp
                Array.Copy( values, crossOverPoint, temp, 0, crossOverLength );
                // copy part of second (pair) chromosome to the first
                Array.Copy( p.values, crossOverPoint, values, crossOverPoint, crossOverLength );
                // copy temp to the second
                Array.Copy( temp, 0, p.values, crossOverPoint, crossOverLength );
            }
        }

        /// <summary>
        /// Evaluate chromosome with specified fitness function
        /// </summary>
        /// 
        /// <param name="function">Fitness function to use for
        /// evaluation of this chromosome.</param>
        /// 
        public void Evaluate( IFitnessFunction function )
        {
            fitness = function.Evaluate( this );
        }
    }
}
