// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Cezary Wagner, 2008
// changes optimizing algorithm performance
// Cezary.Wagner@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Back propagation learning algorithm.
	/// </summary>
	/// 
	/// <remarks><para>The class implements back propagation learning algorithm,
	/// which is widely used for training multi-layer neural networks with
    /// continuous activation functions.</para>
    /// 
    /// <para>Sample usage (training network to calculate XOR function):</para>
    /// <code>
    /// // initialize input and output values
    /// double[][] input = new double[4][] {
    ///     new double[] {0, 0}, new double[] {0, 1},
    ///     new double[] {1, 0}, new double[] {1, 1}
    /// };
    /// double[][] output = new double[4][] {
    ///     new double[] {0}, new double[] {1},
    ///     new double[] {1}, new double[] {0}
    /// };
    /// // create neural network
    /// ActivationNetwork   network = new ActivationNetwork(
    ///     SigmoidFunction( 2 ),
    ///     2, // two inputs in the network
    ///     2, // two neurons in the first layer
    ///     1 ); // one neuron in the second layer
    /// // create teacher
    /// BackPropagationLearning teacher = new BackPropagationLearning( network );
    /// // loop
    /// while ( !needToStop )
    /// {
    ///     // run epoch of learning procedure
    ///     double error = teacher.RunEpoch( input, output );
    ///     // check error value to see if we need to stop
    ///     // ...
    /// }
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="EvolutionaryLearning"/>
	/// 
	public class BackPropagationLearning : ISupervisedLearning
	{
		// network to teach
		private ActivationNetwork network;
		// learning rate
		private double learningRate = 0.1;
		// momentum
		private double momentum = 0.0;

		// neuron's errors
		private double[][]		neuronErrors = null;
		// weight's updates
		private double[][][]	weightsUpdates = null;
		// threshold's updates
		private double[][]		thresholdsUpdates = null;

		/// <summary>
        /// Learning rate, [0, 1].
		/// </summary>
		/// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.1</b>.</para>
        /// </remarks>
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
        /// Momentum, [0, 1].
		/// </summary>
		/// 
		/// <remarks><para>The value determines the portion of previous weight's update
		/// to use on current iteration. Weight's update values are calculated on
		/// each iteration depending on neuron's error. The momentum specifies the amount
		/// of update to use from previous iteration and the amount of update
		/// to use from current iteration. If the value is equal to 0.1, for example,
		/// then 0.1 portion of previous update and 0.9 portion of current update are used
        /// to update weight's value.</para>
        /// 
        /// <para>Default value equals to <b>0.0</b>.</para>
		///	</remarks>
		/// 
		public double Momentum
		{
			get { return momentum; }
			set
			{
				momentum = Math.Max( 0.0, Math.Min( 1.0, value ) );
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BackPropagationLearning"/> class.
		/// </summary>
		/// 
		/// <param name="network">Network to teach.</param>
		/// 
		public BackPropagationLearning( ActivationNetwork network )
		{
			this.network = network;

			// create error and deltas arrays
			neuronErrors      = new double[network.LayersCount][];
			weightsUpdates    = new double[network.LayersCount][][];
			thresholdsUpdates = new double[network.LayersCount][];

			// initialize errors and deltas arrays for each layer
			for ( int i = 0, n = network.LayersCount; i < n; i++ )
			{
				Layer layer = network[i];

				neuronErrors[i]      = new double[layer.NeuronsCount];
				weightsUpdates[i]    = new double[layer.NeuronsCount][];
				thresholdsUpdates[i] = new double[layer.NeuronsCount];

				// for each neuron
				for ( int j = 0; j < layer.NeuronsCount; j++ )
				{
					weightsUpdates[i][j] = new double[layer.InputsCount];
				}
			}
		}

		/// <summary>
		/// Runs learning iteration.
		/// </summary>
		/// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns squared error (difference between current network's output and
        /// desired output) divided by 2.</returns>
        /// 
        /// <remarks><para>Runs one learning iteration and updates neuron's
        /// weights.</para></remarks>
        ///
		public double Run( double[] input, double[] output )
		{
			// compute the network's output
			network.Compute( input );

			// calculate network error
			double error = CalculateError( output );

			// calculate weights updates
			CalculateUpdates( input );

			// update the network
			UpdateNetwork( );

			return error;
		}

		/// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
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


		/// <summary>
		/// Calculates error values for all neurons of the network.
		/// </summary>
		/// 
		/// <param name="desiredOutput">Desired output vector.</param>
		/// 
		/// <returns>Returns summary squared error of the last layer divided by 2.</returns>
		/// 
		private double CalculateError( double[] desiredOutput )
		{
			// current and the next layers
			ActivationLayer	layer, layerNext;
			// current and the next errors arrays
			double[] errors, errorsNext;
			// error values
			double error = 0, e, sum;
			// neuron's output value
			double output;
			// layers count
			int layersCount = network.LayersCount;

			// assume, that all neurons of the network have the same activation function
			IActivationFunction	function = network[0][0].ActivationFunction;

			// calculate error values for the last layer first
			layer	= network[layersCount - 1];
			errors	= neuronErrors[layersCount - 1];

			for ( int i = 0, n = layer.NeuronsCount; i < n; i++ )
			{
				output = layer[i].Output;
				// error of the neuron
				e = desiredOutput[i] - output;
				// error multiplied with activation function's derivative
				errors[i] = e * function.Derivative2( output );
				// squre the error and sum it
				error += ( e * e );
			}

			// calculate error values for other layers
			for ( int j = layersCount - 2; j >= 0; j-- )
			{
				layer		= network[j];
				layerNext	= network[j + 1];
				errors		= neuronErrors[j];
				errorsNext	= neuronErrors[j + 1];

				// for all neurons of the layer
				for ( int i = 0, n = layer.NeuronsCount; i < n; i++ )
				{
					sum = 0.0;
					// for all neurons of the next layer
					for ( int k = 0, m = layerNext.NeuronsCount; k < m; k++ )
					{
						sum += errorsNext[k] * layerNext[k][i];
					}
					errors[i] = sum * function.Derivative2( layer[i].Output );
				}
			}

			// return squared error of the last layer divided by 2
			return error / 2.0;
		}

		/// <summary>
		/// Calculate weights updates.
		/// </summary>
		/// 
		/// <param name="input">Network's input vector.</param>
		/// 
		private void CalculateUpdates( double[] input )
		{
			// current neuron
			ActivationNeuron	neuron;
			// current and previous layers
			ActivationLayer		layer, layerPrev;
			// layer's weights updates
			double[][]	layerWeightsUpdates;
			// layer's thresholds updates
			double[]	layerThresholdUpdates;
			// layer's error
			double[]	errors;
			// neuron's weights updates
			double[]	neuronWeightUpdates;
			// error value
			// double		error;

			// 1 - calculate updates for the last layer fisrt
			layer = network[0];
			errors = neuronErrors[0];
			layerWeightsUpdates = weightsUpdates[0];
			layerThresholdUpdates = thresholdsUpdates[0];

            // cache for frequently used values
            double cachedMomentum   = learningRate * momentum;
            double cached1mMomentum = learningRate * (1 - momentum);
            double cachedError;

			// for each neuron of the layer
			for ( int i = 0, n = layer.NeuronsCount; i < n; i++ )
			{
				neuron = layer[i];
				cachedError	= errors[i] * cached1mMomentum;
				neuronWeightUpdates	= layerWeightsUpdates[i];

				// for each weight of the neuron
				for ( int j = 0, m = neuron.InputsCount; j < m; j++ )
				{
					// calculate weight update
					neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * input[j];
				}

				// calculate treshold update
				layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
			}

			// 2 - for all other layers
			for ( int k = 1, l = network.LayersCount; k < l; k++ )
			{
				layerPrev			= network[k - 1];
				layer				= network[k];
				errors				= neuronErrors[k];
				layerWeightsUpdates	= weightsUpdates[k];
				layerThresholdUpdates = thresholdsUpdates[k];

				// for each neuron of the layer
				for ( int i = 0, n = layer.NeuronsCount; i < n; i++ )
				{
					neuron	= layer[i];
					cachedError	= errors[i] * cached1mMomentum;
					neuronWeightUpdates	= layerWeightsUpdates[i];

					// for each synapse of the neuron
					for ( int j = 0, m = neuron.InputsCount; j < m; j++ )
					{
						// calculate weight update
						neuronWeightUpdates[j] = cachedMomentum * neuronWeightUpdates[j] + cachedError * layerPrev[j].Output;
					}

					// calculate treshold update
					layerThresholdUpdates[i] = cachedMomentum * layerThresholdUpdates[i] + cachedError;
				}
			}
		}

		/// <summary>
		/// Update network'sweights.
		/// </summary>
		/// 
		private void UpdateNetwork( )
		{
			// current neuron
			ActivationNeuron	neuron;
			// current layer
			ActivationLayer		layer;
			// layer's weights updates
			double[][]	layerWeightsUpdates;
			// layer's thresholds updates
			double[]	layerThresholdUpdates;
			// neuron's weights updates
			double[]	neuronWeightUpdates;

			// for each layer of the network
			for ( int i = 0, n = network.LayersCount; i < n; i++ )
			{
				layer = network[i];
				layerWeightsUpdates = weightsUpdates[i];
				layerThresholdUpdates = thresholdsUpdates[i];

				// for each neuron of the layer
				for ( int j = 0, m = layer.NeuronsCount; j < m; j++ )
				{
					neuron = layer[j];
					neuronWeightUpdates = layerWeightsUpdates[j];

					// for each weight of the neuron
					for ( int k = 0, s = neuron.InputsCount; k < s; k++ )
					{
						// update weight
						neuron[k] += neuronWeightUpdates[k];
					}
					// update treshold
					neuron.Threshold += layerThresholdUpdates[j];
				}
			}
		}
	}
}
