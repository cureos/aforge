// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Fabio L. Caversan, 2008
// fabio.caversan@gmail.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Structure for representing a pair of coordinates of double type.
    /// </summary>
    /// 
    /// <remarks><para>The structure is used to store a pair of floating point
    /// coordinates with double precision.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // assigning coordinates in the constructor
    /// DoublePoint p1 = new DoublePoint( 10, 20 );
    /// // creating a point and assigning coordinates later
    /// DoublePoint p2;
    /// p2.X = 30;
    /// p2.Y = 40;
    /// // calculating distance between two points
    /// double distance = p1.DistanceTo( p2 );
    /// </code>
    /// </remarks>
    /// 
    public struct DoublePoint
    {
        /// <summary> 
        /// X coordinate.
        /// </summary> 
        /// 
        public double X;

        /// <summary> 
        /// Y coordinate.
        /// </summary> 
        /// 
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoublePoint"/> structure.
        /// </summary>
        /// 
        /// <param name="x">X axis coordinate.</param>
        /// <param name="y">Y axis coordinate.</param>
        /// 
        public DoublePoint( double x, double y )
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Calculate Euclidean distance between two points.
        /// </summary>
        /// 
        /// <param name="anotherPoint">Point to calculate distance to.</param>
        /// 
        /// <returns>Returns Euclidean distance between this point and
        /// <paramref name="anotherPoint"/> points.</returns>
        /// 
        public double DistanceTo( DoublePoint anotherPoint )
        {
            double dx = X - anotherPoint.X;
            double dy = Y - anotherPoint.Y;

            return System.Math.Sqrt( dx * dx + dy * dy );
        }

        /// <summary>
        /// Addition operator - adds values of two points.
        /// </summary>
        /// 
        /// <param name="p1">First point for addition.</param>
        /// <param name="p2">Second point for addition.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to sum of corresponding
        /// coordinates of specified points.</returns>
        /// 
        public static DoublePoint operator +( DoublePoint p1, DoublePoint p2 )
        {
            return new DoublePoint( p1.X + p2.X, p1.Y + p2.Y );
        }

        /// <summary>
        /// Subtraction operator - subtracts values of two points.
        /// </summary>
        /// 
        /// <param name="p1">Point to subtract from.</param>
        /// <param name="p2">Point to subtract.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to difference of corresponding
        /// coordinates of specified points.</returns>
        ///
        public static DoublePoint operator -( DoublePoint p1, DoublePoint p2 )
        {
            return new DoublePoint( p1.X + p2.X, p1.Y + p2.Y );
        }

        /// <summary>
        /// Get string representation of the class.
        /// </summary>
        /// 
        /// <returns>Returns string, which contains values of the point in readable form.</returns>
        ///
        public override string ToString( )
        {
            return string.Format( "{0}, {1}", X, Y );
        }
    }
}
