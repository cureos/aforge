// AForge Machine Learning Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.MachineLearning
{
    using System;

    /// <summary>
    /// Boltzmann distribution exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements exploration policy base on Boltzmann distribution.
    /// Acording to the policy, action <i>a</i> at state <i>s</i> is selected with the next probability:
    /// <code>
    ///                   exp( Q( s, a ) / t )
    /// p( s, a ) = -----------------------------
    ///              SUM( exp( Q( s, b ) / t ) )
    ///               b
    /// </code>
    /// where <i>t</i> is <see cref="Temperature"/>.</para>
    /// </remarks>
    /// 
    public class BoltzmannExploration : IExplorationPolicy
    {
        // termperature parameter of Boltzmann distribution
        private double temperature;

        // random number generator
        private Random rand = new Random( (int) DateTime.Now.Ticks );

        /// <summary>
        /// Termperature parameter of Boltzmann distribution.
        /// </summary>
        /// 
        public double Temperature
        {
            get { return temperature; }
            set { temperature = Math.Max( 0, value ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoltzmannExploration"/> class.
        /// </summary>
        /// 
        /// <param name="temperature">Termperature parameter of Boltzmann distribution.</param>
        /// 
        public BoltzmannExploration( double temperature )
        {
            Temperature = temperature;
        }

        /// <summary>
        /// Choose an action.
        /// </summary>
        /// 
        /// <param name="actionEstimates">Action estimates.</param>
        /// 
        /// <returns>Returns the next action.</returns>
        /// 
        /// <remarks>The method chooses an action depending on the provided estimates. The
        /// estimates can be any sort of estimate, which values usefulness of the action
        /// (expected summary reward, discounted reward, etc).</remarks>
        /// 
        public int ChooseAction( double[] actionEstimates )
        {
            // actions count
            int actionsCount = actionEstimates.Length;
            // action probabilities
            double[] actionProbabilities = new double[actionsCount];
            // actions sum
            double sum = 0, probabilitiesSum = 0;

            for ( int i = 0; i < actionsCount; i++ )
            {
                double actionProbability = Math.Exp( actionEstimates[i] / temperature );

                actionProbabilities[i] = actionProbability;
                probabilitiesSum += actionProbability;
            }

            if ( ( double.IsInfinity( probabilitiesSum ) ) || ( probabilitiesSum == 0 ) )
            {
                // do greedy selection in the case of infinity or zero
                double maxReward = actionEstimates[0];
                int greedyAction = 0;

                for ( int i = 1; i < actionsCount; i++ )
                {
                    if ( actionEstimates[i] > maxReward )
                    {
                        maxReward = actionEstimates[i];
                        greedyAction = i;
                    }
                }
                return greedyAction;
            }

            // get random number, which determines which action to choose
            double actionRandomNumber = rand.NextDouble( );

            for ( int i = 0; i < actionsCount; i++ )
            {
                sum += actionProbabilities[i] / probabilitiesSum;
                if ( actionRandomNumber <= sum )
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
