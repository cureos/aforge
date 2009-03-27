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

    /// <summary>
    /// Interface with the common methods of a Binary Fuzzy Operator.
    /// </summary>
    /// 
    /// <remarks><para>All fuzzy binary operators must implement this interface.
    /// </para></remarks>
    /// 
    public interface IBinaryOperator : IOperator
    {
        /// <summary>
        /// Calculates the numerical result of the binary operation applied to
        /// two fuzzy membership values.
        /// </summary>
        /// 
        /// <param name="membershipA">A fuzzy membership value, [0..1].</param>
        /// <param name="membershipB">A fuzzy membership value, [0..1].</param>
        /// 
        /// <returns>The numerical result of the binary operation applied to <paramref name="membershipA"/>
        /// and <paramref name="membershipB"/>.</returns>
        /// 
        double Evaluate( double membershipA, double membershipB );
    }
}
