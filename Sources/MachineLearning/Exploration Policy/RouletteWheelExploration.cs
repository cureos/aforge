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
    /// Roulette wheel exploration policy.
    /// </summary>
    /// 
    /// <remarks><para>The class implements roulette whell exploration policy. Acording to the policy,
    /// action <i>a</i> at state <i>s</i> is selected with the next probability:</para>
    /// <code>
    ///                   Q( s, a )
    /// p( s, a ) = ------------------
    ///              SUM( Q( s, b ) )
    ///               b
    /// </code>
    /// <para><note>The exploration policy may be applied only in cases, when action estimates (usefulness)
    /// are represented with positive value greater then 0.</note></para>
    /// </remarks>
    /// 
    public class RouletteWheelExploration : IExplorationPolicy
    {
        // random number generator
        private Random rand = new Random( (int) DateTime.Now.Ticks );

        /// <summary>
        /// Initializes a new instance of the <see cref="RouletteWheelExploration"/> class.
        /// </summary>
        /// 
        public RouletteWheelExploration( ) { }

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
            // actions sum
            double sum = 0, estimateSum = 0;

            for ( int i = 0; i < actionsCount; i++ )
            {
                estimateSum += actionEstimates[i];
            }

            // get random number, which determines which action to choose
            double actionRandomNumber = rand.NextDouble( );

            for ( int i = 0; i < actionsCount; i++ )
            {
                sum += actionEstimates[i] / estimateSum;
                if ( actionRandomNumber <= sum )
                    return i;
            }

            return actionsCount - 1;
        }
    }
}
