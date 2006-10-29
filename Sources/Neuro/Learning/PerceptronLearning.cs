// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Perceptron learning algorithm
	/// </summary>
	/// 
	/// <remarks>This learning algorithm is used to train one layer neural
	/// network of <see cref="AForge.Neuro.ActivationNeuron">Activation Neurons</see>
	/// with the <see cref="AForge.Neuro.ThresholdFunction">Threshold</see>
	/// activation function.</remarks>
	/// 
	public class PerceptronLearning : ISupervisedLearning
	{
		// network to teach
		private ActivationNetwork network;
		// learning rate
		private double learningRate = 0.1;

		/// <summary>
		/// Learning rate
		/// </summary>
		/// 
		/// <remarks>The value determines speed of learning. Default value is 0.1.</remarks>
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
		/// Initializes a new instance of the <see cref="PerceptronLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Network to teach</param>
		/// 
		public PerceptronLearning( ActivationNetwork network )
		{
			// check layers count
			if ( network.LayersCount != 1 )
			{
				throw new ArgumentException( "Invalid nuaral network. It should have one layer only." );
			}

			this.network = network;
		}

		/// <summary>
		/// Runs learning iteration
		/// </summary>
		/// 
		/// <param name="input">input vector</param>
		/// <param name="output">desired output vector</param>
		/// 
		/// <returns>Returns absolute error</returns>
		/// 
		/// <remarks>Runs one learning iteration and updates neuron's
		/// weights in case if neuron's output does not equal to the
		/// desired output.</remarks>
		/// 
		public double Run( double[] input, double[] output )
		{
			// compute output of network
			double[] networkOutput = network.Compute( input );

			// get the only layer of the network
			ActivationLayer layer = network[0];

			// summary network absolute error
			double error = 0.0;

			// check output of each neuron and update weights
			for ( int j = 0, k = layer.NeuronsCount; j < k; j++ )
			{
				double e = output[j] - networkOutput[j];

				if ( e != 0 )
				{
					ActivationNeuron perceptron = layer[j];

					// update weights
					for ( int i = 0, n = perceptron.InputsCount; i < n; i++ )
					{
						perceptron[i] += learningRate * e * input[i];
					}

					// update threshold value
					perceptron.Threshold += learningRate * e;

					// make error to be absolute
					error += Math.Abs( e );
				}
			}

			return error;
		}

		/// <summary>
		/// Runs learning epoch
		/// </summary>
		/// 
		/// <param name="input">array of input vectors</param>
		/// <param name="output">array of output vectors</param>
		/// 
		/// <returns>Returns sum of absolute errors</returns>
		/// 
		/// <remarks>Runs series of learning iterations - one iteration
		/// for each input sample. Updates neuron's weights each time,
		/// when neuron's output does not equal to the desired output.</remarks>
		/// 
		public double RunEpoch( double[][] input, double[][] output )
		{
			double error = 0.0;

			// run learning procedure for all samples
			for ( int i = 0, n = input.Length; i < n; i++ )
			{
				error += Run( input[i], output[i] );
			}

			// return summary error
			return error;
		}
	}
}
