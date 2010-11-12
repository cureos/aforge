// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;
    using AForge;

    /// <summary>
    /// The class encapsulates 2D line and provides some tool methods related to lines.
    /// </summary>
    /// 
    /// <remarks><para>The class provides some methods which are related to lines:
    /// angle between lines, distance to point, finding intersection point, etc.
    /// </para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create a line
    /// Line line = new Line( new DoublePoint( 0, 0 ), new DoublePoint( 3, 4 ) );
    /// // get line's length
    /// double length = line.Length;
    /// // check if it is vertical or horizontal
    /// if ( line.IsVertical || line.IsHorizontal )
    /// {
    ///     // ...
    /// }
    /// 
    /// // get intersection point with another line
    /// DoublePoint intersection = line.GetIntersectionWith(
    ///     new Line( new DoublePoint( 3, 0 ), new DoublePoint( 0, 4 ) ) );
    /// </code>
    /// </remarks>
    /// 
    public class Line
    {
        // line's start/end point
        private readonly DoublePoint start;
        private readonly DoublePoint end;

        // line's parameters from its equation: y = k * x + b;
        private readonly double k; // line's slope
        private readonly double b; // Y-coordinate where line intersects Y-axis

        /// <summary>
        /// Start point of the line.
        /// </summary>
        public DoublePoint Start
        {
            get { return start; }
        }

        /// <summary>
        /// End point of the line.
        /// </summary>
        public DoublePoint End
        {
            get { return end; }
        }

        /// <summary>
        /// Get line's length - Euclidean distance between its <see cref="Start"/> and <see cref="End"/> points.
        /// </summary>
        public double Length
        {
            get { return start.DistanceTo( end ); }
        }

        /// <summary>
        /// Checks if the line vertical or not.
        /// </summary>
        ///
        public bool IsVertical
        {
            get { return double.IsInfinity( k ); }
        }

        /// <summary>
        /// Checks if the line horizontal or not.
        /// </summary>
        public bool IsHorizontal
        {
            get { return ( k == 0 ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> class.
        /// </summary>
        /// 
        /// <param name="start">Line's start point.</param>
        /// <param name="end">Line's end point.</param>
        /// 
        /// <exception cref="ArgumentException">Thrown if start point equals to end point.</exception>
        /// 
        public Line( DoublePoint start, DoublePoint end )
        {
            if ( start == end )
            {
                throw new ArgumentException( "Start point of the line cannot be the same as its end point." );
            }

            this.start = start;
            this.end = end;

            k = (double) ( end.Y - start.Y ) / ( end.X - start.X );
            b = start.Y - k * start.X;
        }

        /// <summary>
        /// Calculate minimum angle between this line and the specified line measured in [0, 90] degrees range.
        /// </summary>
        /// 
        /// <param name="secondLine">The line to find angle between.</param>
        /// 
        /// <returns>Returns minimum angle between lines.</returns>
        /// 
        public double GetAngleBetweenLines( Line secondLine )
        {
            double k2 = secondLine.k;

            bool isVertical1 = IsVertical;
            bool isVertical2 = secondLine.IsVertical;

            // check if lines are parallel
            if ( ( k == k2 ) || ( isVertical1 && isVertical2 ) )
                return 0;

            double angle = 0;

            if ( ( !isVertical1 ) && ( !isVertical2 ) )
            {
                double tanPhi = ( ( k2 > k ) ? ( k2 - k ) : ( k - k2 ) ) / ( 1 + k * k2 );
                angle = Math.Atan( tanPhi );
            }
            else
            {
                // one of the lines is parallel to Y axis

                if ( isVertical1 )
                {
                    angle = Math.PI / 2 - Math.Atan( k2 ) * Math.Sign( k2 );
                }
                else
                {
                    angle = Math.PI / 2 - Math.Atan( k ) * Math.Sign( k );
                }
            }

            // convert radians to degrees
            angle *= ( 180.0 / Math.PI );

            if ( angle < 0 )
            {
                angle = -angle;
            }

            return angle;
        }

        /// <summary>
        /// Finds intersection point with the specified line.
        /// </summary>
        /// 
        /// <param name="secondLine">Line to find intersection with.</param>
        /// 
        /// <returns>Returns intersection point with the specified line.</returns>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if the specified line is parallel to this line.</exception>
        /// 
        public DoublePoint GetIntersectionWith( Line secondLine )
        {
            double k2 = secondLine.k;
            double b2 = secondLine.b;

            bool isVertical1 = IsVertical;
            bool isVertical2 = secondLine.IsVertical;

            if ( ( k == k2 ) || ( isVertical1 && isVertical2 ) )
            {
                throw new InvalidOperationException( "Parallel lines do not have an intersection point." );
            }

            DoublePoint intersection;

            if ( isVertical1 )
            {
                intersection = new DoublePoint( start.X, k2 * start.X + b2 );
            }
            else if ( isVertical2 )
            {
                intersection = new DoublePoint( secondLine.start.X, k * secondLine.start.X + b );
            }
            else
            {
                // the intersection is at x=(b2-b1)/(k1-k2), and y=k1*x+b1
                double x = ( b2 - b ) / ( k - k2 );
                intersection = new DoublePoint( x, k * x + b );
            }

            return intersection;
        }

        /// <summary>
        /// Calculate Euclidean distance between a point and a line.
        /// </summary>
        /// 
        /// <param name="point">The point to calculate distance to.</param>
        /// 
        /// <returns>Returns the Euclidean distance between this line and the specified point.</returns>
        /// 
        public double DistanceToPoint( DoublePoint point )
        {
            double distance;

            if ( !IsVertical )
            {
                double div = Math.Sqrt( k * k + 1 );
                distance = Math.Abs( ( k * point.X + b - point.Y ) / div );
            }
            else
            {
                distance = Math.Abs( start.X - point.X );
            }

            return distance;
        }
    }
}
