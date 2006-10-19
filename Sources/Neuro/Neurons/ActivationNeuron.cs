// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Activation neuron
	/// </summary>
	/// 
	/// <remarks>Activation neuron computes weighted sum of its inputs, adds
	/// threshold value and then applies activation function. The neuron is
	/// usually used in multi layer networks.</remarks>
	/// 
	public class ActivationNeuron : Neuron
	{
		/// <summary>
		/// Threshold value
		/// </summary>
		/// 
		/// <remarks>The value is added to inputs weighted sum.</remarks>
		/// 
		protected double threshold = 0.0f;

		/// <summary>
		/// Activation function
		/// </summary>
		/// 
		/// <remarks>The function is applied to inputs weighted sum plus
		/// threshold value.</remarks>
		/// 
		protected IActivationFunction function = null;

		/// <summary>
		/// Threshold value
		/// </summary>
		/// 
		/// <remarks>The value is added to inputs weighted sum.</remarks>
		/// 
		public double Threshold
		{
			get { return threshold; }
			set { threshold = value; }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationNeuron"/> class
		/// </summary>
		/// 
		/// <param name="inputs">Neuron's inputs count</param>
		/// <param name="function">Neuron's activation function</param>
		/// 
		public ActivationNeuron( int inputs, IActivationFunction function ) : base( inputs )
		{
			this.function = function;
		}

		/// <summary>
		/// Randomize neuron 
		/// </summary>
		/// 
		/// <remarks>Calls base class <see cref="Neuron.Randomize">Randomize</see> method
		/// to randomize neuron's weights and then randomize threshold's value.</remarks>
		/// 
		public override void Randomize( )
		{
			// randomize weights
			base.Randomize( );
			// randomize threshold
			threshold = rand.NextDouble( ) * ( randRange.Length ) + randRange.Min;
		}

		/// <summary>
		/// Computes output value of neuron
		/// </summary>
		/// 
		/// <param name="input">Input vector</param>
		/// 
		/// <returns>Returns neuron's output value</returns>
		/// 
		/// <remarks>The output value of activation neuron is equal to value
		/// of nueron's activation function, which parameter is weighted sum
		/// of its inputs plus threshold value. The output value is also stored
		/// in <see cref="Neuron.Output">Output</see> property.</remarks>
		/// 
		public override double Compute( double[] input )
		{
			// check for corrent input vector
			if ( input.Length != inputsCount )
				throw new ArgumentException( );

			// initial sum value
			double sum = 0.0;

			// compute weighted sum of inputs
			for ( int i = 0; i < inputsCount; i++ )
			{
				sum += weights[i] * input[i];
			}
			sum += threshold;

			return ( output = function.Function( sum ) );
		}
	}
}
