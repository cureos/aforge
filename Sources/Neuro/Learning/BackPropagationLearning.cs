// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Back propagation learning algorithm
	/// </summary>
	/// 
	/// <remarks>The class implements back propagation learning algorithm,
	/// which is widely used for training multi-layer neural networks with
	/// continuous activation functions.</remarks>
	/// 
	public class BackPropagationLearning : ISupervisedLearning
	{
		// network to teach
		private ActivationNetwork network;
		// learning rate
		private double learningRate = 0.1;
		// momentum
		private double momentum = 0.0;

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
		/// Momentum
		/// </summary>
		/// 
		/// <remarks>The value determines the portion of previous weight's update
		/// to use on current iteration. Weight's update values are calculated on
		/// each iteration depending on neuron's error. The momentum specifies the amount
		/// of update to use from previous iteration and the amount of update
		/// to use from current iteration. If the value is equal to 0.1, for example,
		/// then 0.1 portion of previous update and 0.9 portion of current update are used
		/// to update weight's value.<br /><br />
		///	Default value is 0.0.</remarks>
		/// 
		private double Momentum
		{
			get { return momentum; }
			set
			{
				momentum = Math.Max( 0.0, Math.Min( 1.0, value ) );
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BackPropagationLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Network to teach</param>
		/// 
		public BackPropagationLearning( ActivationNetwork network )
		{
		}


		public double Run( double[] input, double[] output )
		{
			return 0;
		}

		public double RunEpoch( double[][] input, double[][] output )
		{
			return 0;
		}

	}
}
