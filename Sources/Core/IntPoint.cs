// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Structure for representing a pair of coordinates of integer type.
    /// </summary>
    /// 
    /// <remarks><para>The structure is used to store a pair of integer coordinates.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // assigning coordinates in the constructor
    /// IntPoint p1 = new IntPoint( 10, 20 );
    /// // creating a point and assigning coordinates later
    /// IntPoint p2;
    /// p2.X = 30;
    /// p2.Y = 40;
    /// // calculating distance between two points
    /// double distance = p1.DistanceTo( p2 );
    /// </code>
    /// </remarks>
    /// 
    public struct IntPoint
    {
        /// <summary> 
        /// X coordinate.
        /// </summary> 
        /// 
        public int X;

        /// <summary> 
        /// Y coordinate.
        /// </summary> 
        /// 
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntPoint"/> structure.
        /// </summary>
        /// 
        /// <param name="x">X axis coordinate.</param>
        /// <param name="y">Y axis coordinate.</param>
        /// 
        public IntPoint( int x, int y )
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
        public double DistanceTo( IntPoint anotherPoint )
        {
            int dx = X - anotherPoint.X;
            int dy = Y - anotherPoint.Y;

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
        public static IntPoint operator +( IntPoint p1, IntPoint p2 )
        {
            return new IntPoint( p1.X + p2.X, p1.Y + p2.Y );
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
        public static IntPoint operator -( IntPoint p1, IntPoint p2 )
        {
            return new IntPoint( p1.X - p2.X, p1.Y - p2.Y );
        }

        /// <summary>
        /// Addition operator - adds scalar to the specified point.
        /// </summary>
        /// 
        /// <param name="p">Point to increase coordinates of.</param>
        /// <param name="valueToAdd">Value to add to coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point increased by specified value.</returns>
        /// 
        public static IntPoint operator +( IntPoint p, int valueToAdd )
        {
            return new IntPoint( p.X + valueToAdd, p.Y + valueToAdd );
        }

        /// <summary>
        /// Subtraction operator - subtracts scalar from the specified point.
        /// </summary>
        /// 
        /// <param name="p">Point to decrease coordinates of.</param>
        /// <param name="valueToSubtract">Value to subtract from coordinates of the specified point.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point decreased by specified value.</returns>
        /// 
        public static IntPoint operator -( IntPoint p, int valueToSubtract )
        {
            return new IntPoint( p.X - valueToSubtract, p.Y - valueToSubtract );
        }

        /// <summary>
        /// Multiplication operator - multiplies coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="p">Point to multiply coordinates of.</param>
        /// <param name="factor">Multiplication factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point multiplied by specified value.</returns>
        ///
        public static IntPoint operator *( IntPoint p, int factor )
        {
            return new IntPoint( p.X * factor, p.Y * factor );
        }

        /// <summary>
        /// Division operator - divides coordinates of the specified point by scalar value.
        /// </summary>
        /// 
        /// <param name="p">Point to divide coordinates of.</param>
        /// <param name="factor">Division factor.</param>
        /// 
        /// <returns>Returns new point which coordinates equal to coordinates of
        /// the specified point divided by specified value.</returns>
        /// 
        public static IntPoint operator /( IntPoint p, int factor )
        {
            return new IntPoint( p.X / factor, p.Y / factor );
        }

        /// <summary>
        /// Implicit conversion to <see cref="DoublePoint"/>.
        /// </summary>
        /// 
        /// <param name="p">Integer point to convert to double precision point.</param>
        /// 
        /// <returns>Returns new double precision point which coordinates are implicitly converted
        /// to doubles from coordinates of the specified integer point.</returns>
        /// 
        public static implicit operator DoublePoint( IntPoint p )
        {
            return new DoublePoint( p.X, p.Y );
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
