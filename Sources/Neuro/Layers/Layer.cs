// AForge Neural Net Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
	using System;

	/// <summary>
	/// Base neural layer class
	/// </summary>
	public abstract class Layer
	{
		/// <summary>
		/// Layer's inputs count
		/// </summary>
		protected int		inputsCount = 0;

		/// <summary>
		/// Layer's neurons count
		/// </summary>
		protected int		neuronsCount = 0;

		/// <summary>
		/// Layer's neurons
		/// </summary>
		protected Neuron[]	neurons;

		/// <summary>
		/// Layer's output vector
		/// </summary>
		protected double[]	output;

		/// <summary>
		/// Layer's inputs count
		/// </summary>
		public int InputsCount
		{
			get { return inputsCount; }
		}

		/// <summary>
		/// Layer's neurons count
		/// </summary>
		public int NeuronsCount
		{
			get { return neuronsCount; }
		}

		/// <summary>
		/// Layer's output vector
		/// </summary>
		/// 
		/// <remarks>The calculation way of layer's output vector is determined by inherited class.</remarks>
		/// 
		public double[] Output
		{
			get { return output; }
		}

		/// <summary>
		/// Layer's neurons accessor
		/// </summary>
		/// 
		/// <param name="index">Neuron index</param>
		/// 
		/// <remarks>Allows to access layer's neurons.</remarks>
		/// 
		public Neuron this[int index]
		{
			get { return neurons[index]; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Layer"/> class
		/// </summary>
		/// 
		/// <param name="neuronsCount">Layer's neurons count</param>
		/// <param name="inputsCount">Layer's inputs count</param>
		/// 
		public Layer( int neuronsCount, int inputsCount )
		{
			this.inputsCount	= Math.Max( 1, inputsCount );
			this.neuronsCount	= Math.Max( 1, neuronsCount );
			// create collection of neurons
			neurons = new Neuron[neuronsCount];
			// allocate output array
			output = new double[neuronsCount];
		}


		/// <summary>
		/// Compute output value of the layer 
		/// </summary>
		/// 
		/// <param name="input">Input vector</param>
		/// 
		/// <returns>Returns layer's output vector</returns>
		/// 
		/// <remarks>The actual layer's output value is determined by inherited class and it
		/// consists of output values of layer's neurons. The output value is also stored in
		/// <see cref="Output"/> property.</remarks>
		/// 
		public virtual double[] Compute( double[] input )
		{
			// compute each neuron
			for ( int i = 0; i < neuronsCount; i++ )
				output[i] = neurons[i].Compute( input );

			return output;
		}


		/// <summary>
		/// Randomize neurons of the layer
		/// </summary>
		/// 
		/// <remarks>Randomizes layer's neurons by calling <see cref="Neuron.Randomize"/> method
		/// of each neuron.</remarks>
		/// 
		public virtual void Randomize( )
		{
			foreach ( Neuron neuron in neurons )
				neuron.Randomize( );
		}
	}
}
