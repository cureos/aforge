// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Elastic network learning algorithm
	/// </summary>
	///
	/// <remarks></remarks> 
	///
	public class ElasticNetworkLearning : IUnsupervisedLearning
	{
		// neural network to train
		private DistanceNetwork	network;

		// array of distances between neurons
		private double[] distance;

		// learning rate
		private double	learningRate = 0.1;
		
		/// <summary>
		/// Learning rate
		/// </summary>
		/// 
		/// <remarks>Determines speed of learning. Value range is [0, 1].
		/// Default value equals to 0.1.</remarks>
		/// 
		public double LearningRate
		{
			get { return learningRate; }
			set
			{
				learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) );
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ElasticNetworkLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Neural network to train</param>
		/// 
		public ElasticNetworkLearning( DistanceNetwork network )
		{
			this.network = network;

			// precalculate distances array
			int		neurons = network[0].NeuronsCount;
			double	deltaAlpha = Math.PI * 2.0 / neurons;
			double	alpha = deltaAlpha;

			distance = new double[neurons];
			distance[0] = 0.0;

			// calculate all distance values
			for ( int i = 1; i < neurons; i++ )
			{
				double dx = 0.5 * Math.Cos( alpha ) - 0.5;
				double dy = 0.5 * Math.Sin( alpha );

				distance[i] = dx * dx + dy * dy;

				alpha += deltaAlpha;
			}
		}


		/// <summary>
		/// Runs learning iteration
		/// </summary>
		/// 
		/// <param name="input">input vector</param>
		/// 
		public void Run( double[] input )
		{
			// compute the network
			network.Compute( input );
			int winner = network.GetWinner( );

			// get layer of the network
			Layer layer = network[0];

			// walk through all neurons of the layer
			for ( int j = 0, m = layer.NeuronsCount; j < m; j++ )
			{
				Neuron neuron = layer[j];

				// update factor
				double factor = Math.Exp( - distance[Math.Abs( j - winner )] / ( 2 * learningRate * learningRate ) );

				// update weight of the neuron
				for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
				{
					neuron[i] += ( input[i] - neuron[i] ) * learningRate * factor;
				}

			}
		}

		/// <summary>
		/// Runs learning epoch
		/// </summary>
		/// 
		/// <param name="input">array of input vectors</param>
		/// 
		public void RunEpoch( double[][] input )
		{
			// walk through all training samples
			foreach ( double[] sample in input )
			{
				Run( sample );
			}
		}
	}
}
