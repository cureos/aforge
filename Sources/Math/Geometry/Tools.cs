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

    internal static class Tools
    {
        // Calculate angle between two vectors
        public static float GetAngleBetweenVectors( IntPoint startPoint, IntPoint vector1end, IntPoint vector2end )
        {
            int x1 = vector1end.X - startPoint.X;
            int y1 = vector1end.Y - startPoint.Y;

            int x2 = vector2end.X - startPoint.X;
            int y2 = vector2end.Y - startPoint.Y;

            return (float) ( Math.Acos( ( x1 * x2 + y1 * y2 ) / ( Math.Sqrt( x1 * x1 + y1 * y1 ) * Math.Sqrt( x2 * x2 + y2 * y2 ) ) ) * 180.0 / Math.PI );
        }

        // Get angle between two lines
        public static float GetAngleBetweenLines( IntPoint line1start, IntPoint line1end, IntPoint line2start, IntPoint line2end )
        {
            float k1, k2;

            if ( line1start.X != line1end.X )
            {
                k1 = (float) ( line1end.Y - line1start.Y ) / ( line1end.X - line1start.X );
            }
            else
            {
                k1 = float.PositiveInfinity;
            }

            if ( line2start.X != line2end.X )
            {
                k2 = (float) ( line2end.Y - line2start.Y ) / ( line2end.X - line2start.X );
            }
            else
            {
                k2 = float.PositiveInfinity;
            }

            // check if lines are parallel
            if ( k1 == k2 )
                return 0;

            float angle = 0;

            if ( ( k1 != float.PositiveInfinity ) && ( k2 != float.PositiveInfinity ) )
            {
                float tanPhi = ( ( k2 > k1 ) ? ( k2 - k1 ) : ( k1 - k2 ) ) / ( 1 + k1 * k2 );
                angle = (float) Math.Atan( tanPhi );
            }
            else
            {
                // one of the lines is parallel to Y axis

                if ( k1 == float.PositiveInfinity )
                {
                    angle = (float) ( Math.PI / 2 - Math.Atan( k2 ) * Math.Sign( k2 ) );
                }
                else
                {
                    angle = (float) ( Math.PI / 2 - Math.Atan( k1 ) * Math.Sign( k1 ) );
                }
            }

            // convert radians to degrees
            angle *= (float) ( 180.0 / Math.PI );

            if ( angle < 0 )
            {
                angle = -angle;
            }

            return angle;
        }
    }
}
