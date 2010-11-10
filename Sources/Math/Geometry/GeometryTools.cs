// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;

    /// <summary>
    /// Collection of some gemetry tool methods.
    /// </summary>
    /// 
    public static class GeometryTools
    {
        /// <summary>
        /// Calculate angle between to vectors measured in [0, 180] degrees range.
        /// </summary>
        /// 
        /// <param name="startPoint">Starting point of both vectors.</param>
        /// <param name="vector1end">Ending point of the first vector.</param>
        /// <param name="vector2end">Ending point of the second vector.</param>
        /// 
        /// <returns>Returns angle between specified vectors measured in degrees.</returns>
        /// 
        public static double GetAngleBetweenVectors( IntPoint startPoint, IntPoint vector1end, IntPoint vector2end )
        {
            int x1 = vector1end.X - startPoint.X;
            int y1 = vector1end.Y - startPoint.Y;

            int x2 = vector2end.X - startPoint.X;
            int y2 = vector2end.Y - startPoint.Y;

            return Math.Acos( ( x1 * x2 + y1 * y2 ) / ( Math.Sqrt( x1 * x1 + y1 * y1 ) * Math.Sqrt( x2 * x2 + y2 * y2 ) ) ) * 180.0 / Math.PI;
        }

        /// <summary>
        /// Calculate minimum angle between two lines measured in [0, 90] degrees range.
        /// </summary>
        /// 
        /// <param name="line1start">Starting point of the first line.</param>
        /// <param name="line1end">Ending point of the first line.</param>
        /// <param name="line2start">Starting point of the second line.</param>
        /// <param name="line2end">Ending point of the second line.</param>
        /// 
        /// <returns>Returns minimum angle between two lines.</returns>
        /// 
        public static double GetAngleBetweenLines( IntPoint line1start, IntPoint line1end, IntPoint line2start, IntPoint line2end )
        {
            double k1, k2;

            if ( line1start.X != line1end.X )
            {
                k1 = (double) ( line1end.Y - line1start.Y ) / ( line1end.X - line1start.X );
            }
            else
            {
                k1 = double.PositiveInfinity;
            }

            if ( line2start.X != line2end.X )
            {
                k2 = (double) ( line2end.Y - line2start.Y ) / ( line2end.X - line2start.X );
            }
            else
            {
                k2 = double.PositiveInfinity;
            }

            // check if lines are parallel
            if ( k1 == k2 )
                return 0;

            double angle = 0;

            if ( ( k1 != double.PositiveInfinity ) && ( k2 != double.PositiveInfinity ) )
            {
                double tanPhi = ( ( k2 > k1 ) ? ( k2 - k1 ) : ( k1 - k2 ) ) / ( 1 + k1 * k2 );
                angle = Math.Atan( tanPhi );
            }
            else
            {
                // one of the lines is parallel to Y axis

                if ( k1 == double.PositiveInfinity )
                {
                    angle = Math.PI / 2 - Math.Atan( k2 ) * Math.Sign( k2 );
                }
                else
                {
                    angle = Math.PI / 2 - Math.Atan( k1 ) * Math.Sign( k1 );
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
        /// Finds intersection point of the two specified lines A and B.
        /// </summary>
        /// 
        /// <param name="a1">The first point defining line A.</param>
        /// <param name="a2">The second point defining line A.</param>
        /// <param name="b1">The first point defining line B.</param>
        /// <param name="b2">The second point defining line B.</param>
        /// 
        /// <returns>Returns intersection point of the two lines.</returns>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if the two lines are parallel.</exception>
        /// 
        public static DoublePoint GetIntersectionPoint( DoublePoint a1, DoublePoint a2, DoublePoint b1, DoublePoint b2 )
        {
            // calculate each line in slope-intercept form (m and b, resp.)
            DoublePoint a = a1 - a2;
            DoublePoint b = b1 - b2;

            double mA = a.Y / a.X;
            double mB = b.Y / b.X;

            if ( ( Double.IsInfinity( mA ) && Double.IsInfinity( mB ) ) || ( mA == mB ) )
            {
                throw new InvalidOperationException( "Parallel lines do not have an intersection point." );
            }

            // y=mx+b => y-mx=b
            double bA = a1.Y - mA * a1.X;
            double bB = b1.Y - mB * b1.X;

            DoublePoint intersection;

            if ( Double.IsInfinity( mA ) )
            {
                intersection = new DoublePoint( a1.X, mB * a1.X + bB );
            }
            else if ( Double.IsInfinity( mB ) )
            {
                intersection = new DoublePoint( b1.X, mA * b1.X + bA );
            }
            else
            {
                // the intersection is at x=(b2-b1)/(m1-m2), and y=m1*x+b1
                double x = ( bB - bA ) / ( mA - mB );
                intersection = new DoublePoint( x, mA * x + bA );
            }

            return intersection;
        }
    }
}
