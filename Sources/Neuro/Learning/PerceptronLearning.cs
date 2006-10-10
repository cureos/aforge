// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Summary description for PerceptronLearning.
	/// </summary>
	public class PerceptronLearning : ISupervisedLearning
	{
		// perceptron to teach
		private ActivationNeuron perceptron;

		/// <summary>
		/// Initializes a new instance of the <see cref="PerceptronLearning"/> class
		/// </summary>
		/// 
		/// <param name="perceptron">Perceptron to teach</param>
		/// 
		public PerceptronLearning( ActivationNeuron perceptron )
		{
			this.perceptron = perceptron;
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
		/// 
		/// 
		double Learn( double[] input, double[] output )
		{
			return 0;
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
		/// 
		/// 
		double LearnEpoch( double[][] input, double[][] output )
		{
			return 0;
		}
	}
}
