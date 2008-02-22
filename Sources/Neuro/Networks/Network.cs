// AForge Neural Net Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Base neural network class.
    /// </summary>
    /// 
    /// <remarks>This is a base neural netwok class, which represents
    /// collection of neuron's layers.</remarks>
    /// 
    [Serializable]
    public abstract class Network
    {
        /// <summary>
        /// Network's inputs count.
        /// </summary>
        protected int inputsCount;

        /// <summary>
        /// Network's layers count.
        /// </summary>
        protected int layersCount;

        /// <summary>
        /// Network's layers.
        /// </summary>
        protected Layer[] layers;

        /// <summary>
        /// Network's output vector.
        /// </summary>
        protected double[] output;

        /// <summary>
        /// Network's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Network's layers count.
        /// </summary>
        public int LayersCount
        {
            get { return layersCount; }
        }

        /// <summary>
        /// Network's output vector.
        /// </summary>
        /// 
        /// <remarks>The calculation way of network's output vector is determined by
        /// inherited class.</remarks>
        /// 
        public double[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// Network's layers accessor.
        /// </summary>
        /// 
        /// <param name="index">Layer index.</param>
        /// 
        /// <remarks>Allows to access network's layer.</remarks>
        /// 
        public Layer this[int index]
        {
            get { return layers[index]; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Network"/> class.
        /// </summary>
        /// 
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="layersCount">Network's layers count.</param>
        /// 
        /// <remarks>Protected constructor, which initializes <see cref="inputsCount"/>,
        /// <see cref="layersCount"/> and <see cref="layers"/> members.</remarks>
        /// 
        protected Network( int inputsCount, int layersCount )
        {
            this.inputsCount = Math.Max( 1, inputsCount );
            this.layersCount = Math.Max( 1, layersCount );
            // create collection of layers
            layers = new Layer[this.layersCount];
        }

        /// <summary>
        /// Compute output vector of the network.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns network's output vector.</returns>
        /// 
        /// <remarks>The actual network's output vecor is determined by inherited class and it
        /// represents an output vector of the last layer of the network. The output vector is
        /// also stored in <see cref="Output"/> property.</remarks>
        /// 
        public virtual double[] Compute( double[] input )
        {
            output = input;

            // compute each layer
            foreach ( Layer layer in layers )
            {
                output = layer.Compute( output );
            }

            return output;
        }

        /// <summary>
        /// Randomize layers of the network.
        /// </summary>
        /// 
        /// <remarks>Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
        /// of each layer.</remarks>
        /// 
        public virtual void Randomize( )
        {
            foreach ( Layer layer in layers )
            {
                layer.Randomize( );
            }
        }

        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to save network into.</param>
        /// 
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        /// 
        public void Save( string fileName )
        {
            FileStream stream = new FileStream( fileName, FileMode.Create, FileAccess.Write, FileShare.None );
            Save( stream );
            stream.Close( );
        }

        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// 
        /// <param name="stream">Stream to save network into.</param>
        /// 
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        /// 
        public void Save( Stream stream )
        {
            IFormatter formatter = new BinaryFormatter( );
            formatter.Serialize( stream, this );
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to load network from.</param>
        /// 
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// 
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        /// 
        public static Network Load( string fileName )
        {
            FileStream stream = new FileStream( fileName, FileMode.Open, FileAccess.Read, FileShare.Read );
            Network network = Load( stream );
            stream.Close( );

            return network;
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// 
        /// <param name="stream">Stream to load network from.</param>
        /// 
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// 
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        /// 
        public static Network Load( Stream stream )
        {
            IFormatter formatter = new BinaryFormatter( );
            Network network = (Network) formatter.Deserialize( stream );
            return network;
        }
    }
}
