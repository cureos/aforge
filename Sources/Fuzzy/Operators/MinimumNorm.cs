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
    using AForge;

    /// <summary>
    /// Minimum Norm, used to calculate the linguistic value of a AND operation. 
    /// </summary>
    /// 
    /// <remarks><para>The minimum Norm uses a minimum operator to compute the AND
    /// among two fuzzy memberships. </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // creating 2 fuzzy sets to represent Cool (Temperatura) and Near (Distance)
    /// TrapezoidalFunction function1 = new TrapezoidalFunction( 13, 18, 23, 28 );
    /// FuzzySet fsCool = new FuzzySet( "Cool", function1 );
    /// TrapezoidalFunction function2 = new TrapezoidalFunction( 23, 28, 33, 38 );
    /// FuzzySet fsNear = new FuzzySet( "Near", function2 );
    /// 
    /// // getting memberships
    /// double m1 = fsCool.GetMembership( 15 );
    /// double m2 = fsNear.GetMembership( 35 );
    /// 
    /// // computing the membership of "Cool AND Near"
    /// MinimumNorm AND = new MinimumNorm( );
    /// double result = AND.Evaluate( m1, m2 );
    ///              
    /// // show result
    /// Console.WriteLine( result );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="ProductNorm"/>
    /// 
    public class MinimumNorm : INorm
    {
        /// <summary>
        /// Calculates the numerical result of the AND operation applied to
        /// two fuzzy membership values using the minimum rule.
        /// </summary>
        /// 
        /// <param name="membershipA">A fuzzy membership value, [0..1].</param>
        /// 
        /// <param name="membershipB">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result of the AND operation applied to <paramref name="membershipA"/>
        /// and <paramref name="membershipB"/>.</returns>
        /// 
        public double Evaluate( double membershipA, double membershipB )
        {
            return Math.Min( membershipA, membershipB );
        }
    }
}

