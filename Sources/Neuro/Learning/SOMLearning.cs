// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Kohonen Self Organizing Map (SOM) learning algorithm
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class SOMLearning : IUnsupervisedLearning
	{
		// neural network to train
		private DistanceNetwork	network;
		// network's dimension
		private int		width;
		private int		height;

		// learning rate
		private double	learningRate = 0.1;
		// learning radius
		private int		learningRadius = 7;
		
		// squared learning radius multiplied by 2 (precalculated value to speed up computations)
		private int		squaredRadius2 = 2 * 7 * 7;

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
		/// Learning radius
		/// </summary>
		/// 
		/// <remarks>Determines the amount of neurons to be updated around
		/// winner neuron. Neurons, which are in the circle of specified radius,
		/// are updated during the learning procedure. Neurons, which are closer
		/// to the winner neuron, get more update.</remarks>
		/// 
		public int LearningRadius
		{
			get { return learningRadius; }
			set
			{
				learningRadius = Math.Max( 0, value );
				squaredRadius2 = 2 * learningRadius * learningRadius;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SOMLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Neural network to train</param>
		/// 
		/// <remarks>This constructor supposes that a square network will be passed for training -
		/// it should be possible to get square root of network's neurons amount.</remarks>
		/// 
		public SOMLearning( DistanceNetwork network )
		{
			// network's dimension was not specified, let's try to guess
			int neuronsCount = network[0].NeuronsCount;
			width = (int) Math.Sqrt( neuronsCount );

			if ( width * width != neuronsCount )
			{
				throw new ArgumentException( "Invalid network size" );
			}

			// ok, we got it
			this.network	= network;
			this.width		= width;
			this.height		= height;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="SOMLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Neural network to train</param>
		/// <param name="width">Neural network's width</param>
		/// <param name="height">Neural network's height</param>
		///
		/// <remarks>The constructor allows to pass network of arbitrary rectangular shape.
		/// The amount of neurons in the network should be equal to <b>width</b> * <b>height</b>.
		/// </remarks> 
		///
		public SOMLearning( DistanceNetwork network, int width, int height )
		{
			// check network size
			if ( network[0].NeuronsCount != width * height )
			{
				throw new ArgumentException( "Invalid network size" );
			}

			this.network	= network;
			this.width		= width;
			this.height		= height;
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

			// check learning radius
			if ( learningRadius == 0 )
			{
				Neuron neuron = layer[winner];

				// update weight of the winner only
				for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
				{
					neuron[i] += ( input[i] - neuron[i] ) * learningRate;
				}
			}
			else
			{
				// winner's X and Y
				int wx = winner % width;
				int wy = winner / width;

				// walk through all neurons of the layer
				for ( int j = 0, m = layer.NeuronsCount; j < m; j++ )
				{
					Neuron neuron = layer[j];

					int dx = ( j % width ) - wx;
					int dy = ( j / width ) - wy;

					// update factor ( Gaussian based )
					double factor = Math.Exp( - (double) ( dx * dx + dy * dy ) / squaredRadius2 );

					// update weight of the neuron
					for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
					{
						neuron[i] += ( input[i] - neuron[i] ) * learningRate * factor;
					}
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
