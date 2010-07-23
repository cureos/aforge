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
    }
}
