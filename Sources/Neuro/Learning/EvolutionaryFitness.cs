// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2008-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Cezary Wagner, 2008
// Initial implementation of evolutionary learning algorithm
// Cezary.Wagner@gmail.com
//

namespace AForge.Neuro.Learning
{
    using System;
    using System.Diagnostics;
    using AForge.Genetic;
    
    /// <summary>
    /// Fitness function used for chromosomes representing collection of neural network's weights.
    /// </summary>
    /// 
    internal class EvolutionaryFitness : IFitnessFunction
    {
        // neural network for which fitness will be calculated
        private ActivationNetwork network;

        // input data samples for neural network
        private double[][] input;

        // output data samples for neural network (desired output)
        private double[][] output;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvolutionaryFitness"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network for which fitness will be calculated.</param>
        /// <param name="input">Input data samples for neural network.</param>
        /// <param name="output">Output data sampels for neural network (desired output).</param>
        /// 
        /// <exception cref="ArgumentException">Length of inputs and outputs arrays must be equal and greater than 0.</exception>
        /// <exception cref="ArgumentException">Length of each input vector must be equal to neural network's inputs count.</exception>
        /// 
        public EvolutionaryFitness( ActivationNetwork network, double[][] input, double[][] output )
        {
            if ( ( input.Length == 0 ) || ( input.Length != output.Length ) )
            {
                throw new ArgumentException( "Length of inputs and outputs arrays must be equal and greater than 0." );
            }

            if ( network.InputsCount != input[0].Length )
            {
                throw new ArgumentException( "Length of each input vector must be equal to neural network's inputs count." );
            }

            this.network = network;
            this.input   = input;
            this.output  = output;
        }

        /// <summary>
        /// Evaluates chromosome.
        /// </summary>
        /// 
        /// <param name="chromosome">Chromosome to evaluate.</param>
        /// 
        /// <returns>Returns chromosome's fitness value.</returns>
        ///
        /// <remarks>The method calculates fitness value of the specified
        /// chromosome.</remarks>
        ///
        public double Evaluate( IChromosome chromosome )
        {
            DoubleArrayChromosome daChromosome = (DoubleArrayChromosome) chromosome;
            double[] chromosomeGenes = daChromosome.Value;
            // total number of weight in neural network
            int totalNumberOfWeights = 0;

            // asign new weights and thresholds to network from the given chromosome
            for ( int i = 0, layersCount = network.LayersCount; i < layersCount; i++ )
            {
                ActivationLayer layer = network[i];

                for ( int j = 0, neuronsCount = layer.NeuronsCount; j < neuronsCount; j++ )
                {
                    ActivationNeuron neuron = layer[j];

                    for ( int k = 0, weightsCount = neuron.InputsCount; k < weightsCount; k++ )
                    {
                        neuron[k] = chromosomeGenes[totalNumberOfWeights++];
                    }
                    neuron.Threshold = chromosomeGenes[totalNumberOfWeights++];
                }
            }

            // post check if all values are processed and lenght of chromosome
            // is equal to network size
            Debug.Assert( totalNumberOfWeights == daChromosome.Length );

            double totalError = 0;

            for ( int i = 0, inputVectorsAmount = input.Length; i < inputVectorsAmount; i++ )
            {
                double[] computedOutput = network.Compute( input[i] );

                for ( int j = 0, outputLength = output[0].Length; j < outputLength; j++ )
                {
                    double error = output[i][j] - computedOutput[j];
                    totalError += error * error;
                }
            }

            if ( totalError > 0 )
                return 1.0 / totalError;

            // zero error means the best fitness
            return double.MaxValue;
        }
    }
}
