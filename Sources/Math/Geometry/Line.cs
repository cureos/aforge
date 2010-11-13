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
    /// <para>Generally, the equation of the line is y = <see cref="Slope"/> * x + 
    /// <see cref="Intercept"/>. However, when <see cref="Slope"/> is an Infinity,
    /// <paramref name="Intercept"/> would normally be meaningless, and it would be
    /// impossible to distinguish the line x = 5 from the line x = -5. Therefore,
    /// if <see cref="Slope"/> is <see cref="Double.PositiveInfinity"/> or
    /// <see cref="Double.NegativeInfinity"/>, the line's equation is instead 
    /// x = <see cref="Intercept"/>.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create a line
    /// Line line = Line.FromPoints( new DoublePoint( 0, 0 ), new DoublePoint( 3, 4 ) );
    /// // check if it is vertical or horizontal
    /// if ( line.IsVertical || line.IsHorizontal )
    /// {
    ///     // ...
    /// }
    /// 
    /// // get intersection point with another line
    /// DoublePoint intersection = line.GetIntersectionWith(
    ///     Line.FromPoints( new DoublePoint( 3, 0 ), new DoublePoint( 0, 4 ) ) );
    /// </code>
    /// </remarks>
    /// 
    public sealed class Line
    {
        // line's parameters from its equation: y = k * x + b;
        // If k is an Infinity, the equation is x = b.
        private readonly double k; // line's slope
        private readonly double b; // Y-coordinate where line intersects Y-axis

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
        /// Gets the slope of the line.
        /// </summary>
        public double Slope { get { return k; } }

        /// <summary>
        /// If not <see cref="IsVertical"/>, gets the Line's Y-intercept.
        /// If <see cref="IsVertical"/> gets the line's X-intercept.
        /// </summary>
        public double Intercept { get { return b; } }

        /// <summary>
        /// Creates a <see cref="Line"/>  that goes through the two specified points.
        /// </summary>
        /// 
        /// <param name="point1">One point on the line.</param>
        /// <param name="point2">Another point on the line.</param>
        /// 
        /// <returns>Returns a <see cref="Line"/> representing the line between <paramref name="point1"/>
        /// and <paramref name="point2"/>.</returns>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two points are the same.</exception>
        /// 
        public static Line FromPoints( DoublePoint point1, DoublePoint point2 )
        {
            return new Line( point1, point2 );
        }

        /// <summary>
        /// Creates a <see cref="Line"/> with the specified slope and intercept.
        /// </summary>
        /// 
        /// <param name="slope">The slope of the line</param>
        /// <param name="intercept">The Y-intercept of the line, unless the slope is an
        /// infinity, in which case the line's equation is "x = intercept" instead.</param>
        /// 
        /// <returns>Returns <see cref="Line"/> representing the specified line.</returns>
        /// 
        /// <remarks><para>The construction here follows the same rules as for the rest of this class.
        /// Most lines are expressed as y = slope * x + intercept. Vertical lines, however, are 
        /// x = intercept. This is indicated by <see cref="IsVertical"/> being true or by 
        /// <see cref="Slope"/> returning <see cref="Double.PositiveInfinity"/> or 
        /// <see cref="Double.NegativeInfinity"/>.</para></remarks>
        /// 
        public static Line FromSlopeIntercept( double slope, double intercept )
        {
            return new Line( slope, intercept );
        }

        /// <summary>
        /// Constructs a <see cref="Line"/> from a radius and an angle (in degrees).
        /// </summary>
        /// 
        /// <param name="radius">The minimum distance from the line to the origin.</param>
        /// <param name="theta">The angle of the vector from the origin to the line.</param>
        /// 
        /// <returns>Returns a <see cref="Line"/> representing the specified line.</returns>
        /// 
        /// <remarks><para><paramref name="radius"/> is the minimum distance from the origin
        /// to the line, and <paramref name="theta"/> is the counterclockwise rotation from
        /// the positive X axis to the vector through the origin and normal to the line.</para>
        /// <para>This means that if <paramref name="theta"/> is in [0,180), the point on the line
        /// closest to the origin is on the positive X or Y axes, or in quadrants I or II. Likewise,
        /// if <paramref name="theta"/> is in [180,360), the point on the line closest to the
        /// origin is on the negative X or Y axes, or in quadrants III or IV.</para></remarks>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">Thrown if radius is negative.</exception>
        /// 
        public static Line FromRTheta( double radius, double theta )
        {
            return new Line( radius, theta, false );
        }

        /// <summary>
        /// Constructs a <see cref="Line"/> from a point and an angle (in degrees).
        /// </summary>
        /// 
        /// <param name="point">The minimum distance from the line to the origin.</param>
        /// <param name="theta">The angle of the normal vector from the origin to the line.</param>
        /// 
        /// <remarks><para><paramref name="theta"/> is the counterclockwise rotation from
        /// the positive X axis to the vector through the origin and normal to the line.</para>
        /// <para>This means that if <paramref name="theta"/> is in [0,180), the point on the line
        /// closest to the origin is on the positive X or Y axes, or in quadrants I or II. Likewise,
        /// if <paramref name="theta"/> is in [180,360), the point on the line closest to the
        /// origin is on the negative X or Y axes, or in quadrants III or IV.</para></remarks>
        /// 
        /// <returns>Returns a <see cref="Line"/> representing the specified line.</returns>
        /// 
        public static Line FromPointTheta( DoublePoint point, double theta )
        {
            return new Line( point, theta );
        }

        #region Private Constructors
        private Line( DoublePoint start, DoublePoint end )
        {
            if ( start == end )
            {
                throw new ArgumentException( "Start point of the line cannot be the same as its end point." );
            }

            k = ( end.Y - start.Y ) / ( end.X - start.X );
            b = Double.IsInfinity( k ) ? start.X : start.Y - k * start.X;
        }

        private Line( double slope, double intercept )
        {
            k = slope;
            b = intercept;
        }

        private Line( double radius, double theta, bool unused )
        {
            if ( radius < 0 )
            {
                throw new ArgumentOutOfRangeException( "radius", radius, "Must be non-negative" );
            }

            theta *= Math.PI / 180;

            double sine = Math.Sin( theta ), cosine = Math.Cos( theta );
            DoublePoint pt1 = new DoublePoint( radius * cosine, radius * sine );

            // -1/tan, to get the slope of the line, and not the slope of the normal
            k = -cosine / sine;

            if ( !Double.IsInfinity( k ) )
            {
                b = pt1.Y - k * pt1.X;
            }
            else
            {
                b = Math.Abs( radius );
            }
        }

        private Line( DoublePoint point, double theta )
        {
            theta *= Math.PI / 180;

            k = -1 / Math.Tan( theta );

            if ( !Double.IsInfinity( k ) )
            {
                b = point.Y - k * point.X;
            }
            else
            {
                b = point.X;
            }
        }
        #endregion

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
                intersection = new DoublePoint( b, k2 * b + b2 );
            }
            else if ( isVertical2 )
            {
                intersection = new DoublePoint( b2, k * b2 + b );
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
                distance = Math.Abs( b - point.X );
            }

            return distance;
        }
    }
}
