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
        /// <remarks><para><note>It is preferred to use <see cref="Line.GetAngleBetweenLines"/> if it is required to calculate angle
        /// multiple times for one of the lines.</note></para></remarks>
        /// 
        public static double GetAngleBetweenLines( IntPoint line1start, IntPoint line1end, IntPoint line2start, IntPoint line2end )
        {
            Line line1 = new Line( line1start, line1end );
            return line1.GetAngleBetweenLines( new Line( line2start, line2end ) );
        }
    }
}
