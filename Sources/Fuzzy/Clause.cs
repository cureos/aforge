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
    /// This class represents a fuzzy clause, a linguistic expression of the type "Variable IS Value".
    /// </summary>
    /// 
    /// <remarks><para>A Fuzzy Clause is used to verify if a linguistic variable can be considered
    /// as a specific value at a specific moment. Linguistic variables can only assume value of
    /// their linugistic labels. Because of the nature of the Fuzzy Logic, a Variable can be 
    /// several of its labels at the same time, with different membership values.</para>
    /// 
    /// <para>For example, a linguistic variable "temperature" can be "hot" with a membership 0.3
    /// and "warm" with a membership 0.7 at the same time. To obtain those memberships, Fuzzy Clauses
    /// "temperature is hot" and "temperature is war" can be built.</para>
    /// 
    /// <para>Typically Fuzzy Clauses are used to build Fuzzy Rules (<see cref="Rule"/>).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // lets consider the existence of a <see cref="LinguisticVariable"/> lvTemperature
    /// // and a <see cref="FuzzySet"/> hot as its label.
    /// Clause fuzzyClause = new Clause( lvTemperature, lvTemperature.GetLabel( "Hot" ) );
    /// // setting the numerical input of the variable to evaluate the Clause
    /// lvTemperature.NumericInput = 35;
    /// double result = fuzzyClause.Evaluate( );
    /// </code>    
    /// </remarks>
    /// 
    public class Clause
    {
        // the linguistic var of the clause
        private LinguisticVariable variable;
        // the label of the clause
        private FuzzySet label;

        /// <summary>
        /// Gets the <see cref="LinguisticVariable"/> of the <see cref="Clause"/>.
        /// </summary>
        public LinguisticVariable Variable
        {
            get { return variable; }
        }

        /// <summary>
        /// Gets the <see cref="FuzzySet"/> of the <see cref="Clause"/>.
        /// </summary>
        public FuzzySet Label
        {
            get { return label; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Clause"/> class.
        /// </summary>
        /// 
        /// <param name="variable">Linguistic variable of the clause. </param>
        /// 
        /// <param name="label">Label of the linguistic variable, a fuzzy set used as label into the linguistic variable.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The label indicated was not found in the linguistic variable.</exception>
        /// 
        public Clause( LinguisticVariable variable, FuzzySet label )
        {
            // check if label belongs to var.
            variable.GetLabel( label.Name );
            
            // initializing attributes
            this.label    = label;
            this.variable = variable;
        }

        /// <summary>
        /// Evaluates the fuzzy clause.
        /// </summary>
        /// 
        /// <param name="x">Value which membership needs to be calculated.</param>
        /// 
        /// <returns>Degree of membership [0..1] of the clause.</returns>
        /// 
        public double Evaluate( double x )
        {
            return label.GetMembership( x );
        }

        /// <summary>
        /// Evaluates the fuzzy clause using the linguistic variable's numeric input.
        /// </summary>
        /// 
        /// <returns>Degree of membership [0..1] of the clause.</returns>
        /// 
        public double Evaluate( )
        {
            return Evaluate( variable.NumericInput );
        }

        /// <summary>
        /// Returns the fuzzy clause in its linguistic representation.
        /// </summary>
        /// 
        /// <returns>A string representing the fuzzy clause.</returns>
        /// 
        public override string ToString( )
        {
            return this.variable.Name + " IS " + this.label.Name;
        }
    }
}
