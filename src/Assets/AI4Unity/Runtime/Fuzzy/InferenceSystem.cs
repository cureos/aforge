// AForge Fuzzy Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AForge.Fuzzy;

namespace AI4Unity.Fuzzy{
    /// <summary>
    /// This class represents a Fuzzy Inference System. 
    /// </summary>
    /// 
    /// <remarks><para>A Fuzzy Inference System is a model capable of executing fuzzy computing.
    /// It is mainly composed by a <see cref="Database"/> with the linguistic variables
    /// (see <see cref="LinguisticVariable"/>) and a <see cref="Rulebase"/>
    /// with the fuzzy rules (see <see cref="Rule"/>) that represent the behavior of the system.
    /// The typical operation of a Fuzzy Inference System is:
    /// <list type="bullet">
    /// <item>Get the numeric inputs;</item>
    /// <item>Use the <see cref="Database"/> with the linguistic variables
    /// (see <see cref="LinguisticVariable"/>) to obtain linguistic meaning for each
    /// numerical input;</item>
    /// <item>Verify which rules (see <see cref="Rule"/>) of the <see cref="Rulebase"/> are
    /// activated by the input;</item>
    /// <item>Combine the consequent of the activated rules to obtain a <see cref="FuzzyOutput"/>;</item>
    /// <item>Use some defuzzifier (see <see cref="IDefuzzifier"/>) to obtain a numerical output. </item>
    /// </list>
    /// </para>
    /// 
    /// <para>The following sample usage is a Fuzzy Inference System that controls an
    /// auto guided vehicle avoing frontal collisions:</para>
    /// <code>
    /// // linguistic labels (fuzzy sets) that compose the distances
    /// FuzzySet fsNear = new FuzzySet( "Near",
    ///     new TrapezoidalFunction( 15, 50, TrapezoidalFunction.EdgeType.Right ) );
    /// FuzzySet fsMedium = new FuzzySet( "Medium",
    ///     new TrapezoidalFunction( 15, 50, 60, 100 ) );
    /// FuzzySet fsFar = new FuzzySet( "Far",
    ///     new TrapezoidalFunction( 60, 100, TrapezoidalFunction.EdgeType.Left ) );
    ///             
    /// // front distance (input)
    /// LinguisticVariable lvFront = new LinguisticVariable( "FrontalDistance", 0, 120 );
    /// lvFront.AddLabel( fsNear );
    /// lvFront.AddLabel( fsMedium );
    /// lvFront.AddLabel( fsFar );
    /// 
    /// // linguistic labels (fuzzy sets) that compose the angle
    /// FuzzySet fsZero = new FuzzySet( "Zero",
    ///     new TrapezoidalFunction( -10, 5, 5, 10 ) );
    /// FuzzySet fsLP = new FuzzySet( "LittlePositive",
    ///     new TrapezoidalFunction( 5, 10, 20, 25 ) );
    /// FuzzySet fsP = new FuzzySet( "Positive",
    ///     new TrapezoidalFunction( 20, 25, 35, 40 ) );
    /// FuzzySet fsVP = new FuzzySet( "VeryPositive",
    ///     new TrapezoidalFunction( 35, 40, TrapezoidalFunction.EdgeType.Left ) );
    /// 
    /// // angle
    /// LinguisticVariable lvAngle = new LinguisticVariable( "Angle", -10, 50 );
    /// lvAngle.AddLabel( fsZero );
    /// lvAngle.AddLabel( fsLP );
    /// lvAngle.AddLabel( fsP );
    /// lvAngle.AddLabel( fsVP );
    /// 
    /// // the database
    /// Database fuzzyDB = new Database( );
    /// fuzzyDB.AddVariable( lvFront );
    /// fuzzyDB.AddVariable( lvAngle );
    /// 
    /// // creating the inference system
    /// InferenceSystem IS = new InferenceSystem( fuzzyDB, new CentroidDefuzzifier( 1000 ) );
    /// 
    /// // going Straight
    /// IS.NewRule( "Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero" );
    /// // Turning Left
    /// IS.NewRule( "Rule 2", "IF FrontalDistance IS Near THEN Angle IS Positive" );
    /// 
    /// ...
    /// // inference section
    /// 
    /// // setting inputs
    /// IS.SetInput( "FrontalDistance", 20 );
    /// 
    /// // getting outputs
    /// try
    /// {
    ///     float newAngle = IS.Evaluate( "Angle" );
    /// }
    /// catch ( Exception )
    /// {
    /// ...
    /// }
    /// </code>    
    /// </remarks>
    /// 
    public class InferenceSystem{
		#region protected instance fields
        // The linguistic variables of this system
        protected Database database;

        // The fuzzy rules of this system
		protected Rulebase rulebase;

        // The defuzzifier method choosen 
		protected DefuzzificationMethod defuzzificationMethod;

        // Norm operator used in rules and deffuzification
		protected INorm normOperator;

        // CoNorm operator used in rules
		protected ICoNorm conormOperator;

		// Input linguistic variables
		protected HashSet<string> inputVariables;

		// Output linguistic variables
		protected HashSet<string> outputVariables;
		#endregion

		#region public instance constructors
		/// <summary>
        /// Initializes a new Fuzzy <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="defuzzifier">A defuzzyfier method used to evaluate the numeric uotput of the system.</param>
        /// 
		public InferenceSystem(DefuzzificationMethod defuzzificationMethod ) 
		: this(defuzzificationMethod, new ProductNorm(), new MaximumCoNorm()){}

        /// <summary>
        /// Initializes a new Fuzzy <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="defuzzifier">A defuzzyfier method used to evaluate the numeric otput
        /// of the system.</param>
        /// <param name="normOperator">A <see cref="INorm"/> operator used to evaluate the norms
        /// in the <see cref="InferenceSystem"/>. For more information of the norm evaluation see <see cref="Rule"/>.</param>
        /// <param name="conormOperator">A <see cref="ICoNorm"/> operator used to evaluate the
        /// conorms in the <see cref="InferenceSystem"/>. For more information of the conorm evaluation see <see cref="Rule"/>.</param>
        /// 
		public InferenceSystem(DefuzzificationMethod defuzzificationMethod, INorm normOperator, ICoNorm conormOperator)
        {
			this.defuzzificationMethod = defuzzificationMethod;
            this.normOperator = normOperator;
            this.conormOperator = conormOperator;

			this.database = new Database();
			this.rulebase = new Rulebase();
			this.inputVariables = new HashSet<string>();
			this.outputVariables = new HashSet<string>();
        }
		#endregion

		#region public instance methods: Rules
		/// <summary>
		/// Removes all the fuzzy rules of the <see cref="Rulebase"/>. 
		/// </summary>
		/// 
		public void ClearRules()
		{
			this.rulebase.ClearRules();
		}

		/// <summary>
		/// Gets one of the Rules of the <see cref="Rulebase"/>. 
		/// </summary>
		/// 
		/// <param name="ruleName">Name of the <see cref="Rule"/> to get.</param>
		/// 
		/// <exception cref="KeyNotFoundException">The rule indicated in <paramref name="ruleName"/>
		/// was not found in the rulebase.</exception>
		/// 
		public Rule GetRule( string ruleName )
		{
			return this.rulebase.GetRule ( ruleName );
		}

		/// <summary>
		/// Gets all the Rules of the <see cref="Rulebase"/>. 
		/// </summary>
		/// 
		public ReadOnlyCollection<Rule> GetRules()
		{
			return new ReadOnlyCollection<Rule>(this.rulebase.GetRules());
		}

        /// <summary>
        /// Creates a new <see cref="Rule"/> and add it to the <see cref="Rulebase"/> of the 
        /// <see cref="InferenceSystem"/>.
        /// </summary>
        /// 
        /// <param name="name">Name of the <see cref="Rule"/> to create.</param>
        /// <param name="rule">A string representing the fuzzy rule.</param>
        /// 
        /// <returns>The new <see cref="Rule"/> reference. </returns>
        /// 
        public Rule NewRule( string name, string rule )
        {
            Rule r = new Rule(this.database, name, rule, this.normOperator, this.conormOperator );
            this.rulebase.AddRule( r );
            return r;
        }
		#endregion

		#region public instance methods: LinguisticVariables
		public void AddInputVariable(LinguisticVariable linguisticVariable){
			this.database.AddVariable(linguisticVariable);
			this.inputVariables.Add(linguisticVariable.Name);
		}

		public void AddOutputVariable(LinguisticVariable linguisticVariable){
			this.database.AddVariable(linguisticVariable);
			this.outputVariables.Add(linguisticVariable.Name);
		}

		public void ClearAll(){
			this.ClearRules();
			this.database.ClearVariables();
		}

		public LinguisticVariable GetInputVariable(string variableName){
			if (this.inputVariables.Contains(variableName)){
				return this.GetLinguisticVariable(variableName);
			}
			return null;
		}

		public ReadOnlyCollection<LinguisticVariable> GetInputVariables(){
			return this.GetLinguisticVariables(this.inputVariables);
		}

		public LinguisticVariable GetOutputVariable(string variableName){
			if (this.outputVariables.Contains(variableName)){
				return this.GetLinguisticVariable(variableName);
			}
			return null;
		}

		public ReadOnlyCollection<LinguisticVariable> GetOutputVariables(){
			return this.GetLinguisticVariables(this.outputVariables);
		}
		#endregion
		
		#region public instance methods: Inputs
		/// <summary>
        /// Sets a numerical input for one of the linguistic variables of the <see cref="Database"/>. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/>.</param>
        /// <param name="value">Numeric value to be used as input.</param>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated in <paramref name="variableName"/>
        /// was not found in the database.</exception>
        /// 
        public void SetInput(string variableName, float value){
			if (this.inputVariables.Contains(variableName)){
            	this.database.GetVariable( variableName ).NumericInput = value;
			}else{
				throw new KeyNotFoundException(variableName + " is not an input variable.");
			}
        }

		public void SetInputs(IEnumerable<KeyValuePair<string, float>> inputs){
			if (inputs != null){
				// First check if every variable is an input variable...
				foreach (KeyValuePair<string, float> input in inputs){
					if (!this.inputVariables.Contains(input.Key)){
						throw new KeyNotFoundException(input.Key + " is not an input variable.");
					}
				}

				// Then update the value of all variables
				foreach (KeyValuePair<string, float> input in inputs){
					this.SetInput(input.Key, input.Value);
				}
			}
		}
		#endregion

		#region public instance methods: Outputs
		/// <summary>
        /// Executes the fuzzy inference, obtaining a numerical output for a choosen output
        /// linguistic variable. 
        /// </summary>
        /// 
        /// <param name="variableName">Name of the <see cref="LinguisticVariable"/> to evaluate.</param>
        /// 
        /// <returns>The numerical output of the Fuzzy Inference System for the choosen variable.</returns>
        /// 
        /// <exception cref="KeyNotFoundException">The variable indicated was not found in the database.</exception>
        /// 
		public float Evaluate(string variableName, float defaultValue = 0f){
			HashSet<string> variableNames = new HashSet<string>();
			variableNames.Add(variableName);
			return this.DoEvaluate(variableNames, defaultValue)[variableName];
        }

		public Dictionary<string, float> Evaluate(HashSet<string> variableNames, float defaultValue = 0f){
			return this.DoEvaluate(variableNames, defaultValue);
		}

		public Dictionary<string, float> Evaluate(IEnumerable<string> variableNames, float defaultValue = 0f){
			if (variableNames != null){
				HashSet<string> hs = new HashSet<string>();
				foreach(string variableName in variableNames){
					hs.Add(variableName);
				}

				return this.DoEvaluate(hs, defaultValue);
			}else{
				throw new ArgumentNullException("variableNames");
			}
		}

		public Dictionary<string, float> EvaluateAll(float defaultValue = 0f){
			return this.DoEvaluate(this.outputVariables, defaultValue);
		}
		#endregion

		#region protected instance methods
		/// <summary>
		/// Gets one of the <see cref="LinguisticVariable"/> of the <see cref="Database"/>. 
		/// </summary>
		/// 
		/// <param name="variableName">Name of the <see cref="LinguisticVariable"/> to get.</param>
		/// 
		/// <exception cref="KeyNotFoundException">The variable indicated in <paramref name="variableName"/>
		/// was not found in the database.</exception>
		/// 
		protected LinguisticVariable GetLinguisticVariable( string variableName ){
			return this.database.GetVariable( variableName );
		}

		protected ReadOnlyCollection<LinguisticVariable> GetLinguisticVariables(IEnumerable<string> variableNames){
			List<LinguisticVariable> variables = new List<LinguisticVariable>();

			if (variableNames != null){
				foreach (string variableName in variableNames){
					if (!string.IsNullOrEmpty(variableName)){
						LinguisticVariable linguisticVariable = this.GetLinguisticVariable(variableName);
						if (linguisticVariable != null){
							variables.Add(linguisticVariable);
						}
					}
				}
			}
			
			return new ReadOnlyCollection<LinguisticVariable>(variables);
		}

		protected Dictionary<string, float> DoEvaluate(HashSet<string> variableNames, float defaultValue){
			if (variableNames != null){
				Dictionary<string, float> sums = new Dictionary<string, float>();
				Dictionary<string, int> counts = new Dictionary<string, int>();

				// First, check if the variables passed as parameter are output variables...
				foreach (string variableName in variableNames){
					if (!string.IsNullOrEmpty(variableName) && this.outputVariables.Contains(variableName)){
						counts[variableName] = 0;
						sums[variableName] = 0f;
					}else{
						throw new KeyNotFoundException(variableName + " is not an output variable.");
					}
				}

				// Then iterate over all rules...
				foreach (Rule r in this.rulebase.GetRules()){
					// Check if the output of the current rule is one of the variables we want to evaluate...
					if (variableNames.Contains(r.Output.Variable.Name)){
						// In that case calculate the effect of the current rule in the value of the variable...
						float firingStrength = r.EvaluateFiringStrength();
						string variableName = r.Output.Variable.Name;
						
						if (firingStrength > 0){
							if (!sums.ContainsKey(variableName)){
								counts[variableName] = 0;
								sums[variableName] = 0f;
							}

							sums[variableName] += 
								0.5f * (r.Output.Label.LeftLimit + r.Output.Label.RightLimit) * firingStrength;

							counts[variableName] += 1;
						}
					}
				}

				// Finally, calculate the final value of each variable
				Dictionary<string, float> values = new Dictionary<string, float>();
				foreach (string variableName in variableNames){
					if (!sums.ContainsKey(variableName)){
						values[variableName] = defaultValue;
					}else{
						values[variableName] = this.CalculateOutput(
							variableName, 
							sums[variableName], 
							counts[variableName], 
							defaultValue
						);
					}
				}
				return values;
			}else{
				throw new ArgumentNullException("variableNames");
			}
		}

		protected float CalculateOutput(string variableName, float sum, int count, float defaultValue){
			// returns the fuzzy output obtained
			if (count > 0){
				switch (this.defuzzificationMethod){
				case DefuzzificationMethod.Average:
					return sum / (float)(count);
				case DefuzzificationMethod.Sum:
					return sum;
				case DefuzzificationMethod.ClampedSum:	
					LinguisticVariable variable = this.GetLinguisticVariable(variableName);
					return UnityEngine.Mathf.Clamp(sum, variable.Start, variable.End);
				}
			}
			return defaultValue;
		}
		#endregion
    }
}
