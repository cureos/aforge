// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2008-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Fabio L. Caversan, 2008-2009
// fabio.caversan@gmail.com
//

namespace AForge.Fuzzy
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The class represents the output of a Fuzzy Inference System. 
    /// </summary>
    /// 
    /// <remarks><para>The class keeps set of rule's output - pairs with the output fuzzy label
    /// and the rule's firing strength.
    /// </para></remarks>
    /// 
    public class FuzzyOutput
    {
        /// <summary>
        /// Inner class to store the pair fuzzy label / firing strength of 
        /// a fuzzy output.
        /// </summary>
        public class OutputConstraint
        {
            // The label of a fuzzy output
            private string label;
            // The firing strength of a fuzzy rule, to be applied to the label
            private double firingStrength;

            /// <summary>
            /// Initializes a new instance of the <see cref="OutputConstraint"/> class.
            /// </summary>
            /// 
            /// <param name="label">A string representing the output label of a <see cref="Rule"/>.</param>
            /// <param name="firingStrength">The firing strength of a <see cref="Rule"/>, to be applied to its output label.</param>
            /// 
            public OutputConstraint( string label, double firingStrength )
            {
                this.label          = label;
                this.firingStrength = firingStrength;
            }
            
            /// <summary>
            /// The <see cref="FuzzySet"/> representing the output label of a <see cref="Rule"/>.
            /// </summary>
            /// 
            public string Label
            {
                get { return label; }
            }
            
            /// <summary>
            /// The firing strength of a <see cref="Rule"/>, to be applied to its output label.
            /// </summary>
            /// 
            public double FiringStrength
            {
                get { return firingStrength; }
            }

        }

        // the linguistic variables repository 
        private List<OutputConstraint> outputList;

        // the output linguistic variable 
        private LinguisticVariable outputVar;

        /// <summary>
        /// A list with <see cref="OutputConstraint"/> of a Fuzzy Inference System's output.
        /// </summary>
        /// 
        public List<OutputConstraint> OutputList
        {
            get
            {
                return outputList;
            }
        }

        /// <summary>
        /// Gets the <see cref="LinguisticVariable"/> acting as a Fuzzy Inference System Output.
        /// </summary>
        /// 
        public LinguisticVariable OutputVariable
        {
            get { return outputVar; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyOutput"/> class.
        /// </summary>
        /// 
        /// <param name="outputVar">A <see cref="LinguisticVariable"/> representing a Fuzzy Inference System's output.</param>
        /// 
        public FuzzyOutput( LinguisticVariable outputVar )
        {
            // instance of the constraints list 
            this.outputList = new List<OutputConstraint>( 20 );

            // output linguistic variable
            this.outputVar  = outputVar; 
        }

        /// <summary>
        /// Adds an output to the Fuzzy Output. 
        /// </summary>
        /// 
        /// <param name="labelName">The name of a label representing a fuzzy rule's output.</param>
        /// <param name="firingStrength">The firing strength [0..1] of a fuzzy rule.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated was not found in the linguistic variable.</exception>
        /// 
        public void AddOutput( string labelName, double firingStrength )
        {
            // check if the label exists in the linguistic variable
            this.outputVar.GetLabel( labelName );

            // adding label
            this.outputList.Add( new OutputConstraint( labelName, firingStrength ) );
        }

        /// <summary>
        /// Removes all the linguistic variables of the database. 
        /// </summary>
        /// 
        public void ClearOutput( )
        {
            this.outputList.Clear( );
        }
    }
}
