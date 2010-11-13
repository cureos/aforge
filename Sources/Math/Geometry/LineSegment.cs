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

    /// <summary>
    /// The class encapsulates 2D line segment and provides some tool methods related to lines.
    /// </summary>
    /// 
    /// <remarks><para>The class provides some methods which are related to line segments:
    /// distance to point, finding intersection point, etc.
    /// </para>
    /// 
    /// <para>A line segment may be converted to a <see cref="Line"/>.</para>
    /// 
    /// </remarks>
    /// 
    public sealed class LineSegment
    {
        // segment's start/end point
        private readonly DoublePoint start;
        private readonly DoublePoint end;

        private readonly Line line;

        /// <summary>
        /// Start point of the line segment.
        /// </summary>
        public DoublePoint Start
        {
            get { return start; }
        }

        /// <summary>
        /// End point of the line segment.
        /// </summary>
        public DoublePoint End
        {
            get { return end; }
        }

        /// <summary>
        /// Get segment's length - Euclidean distance between its <see cref="Start"/> and <see cref="End"/> points.
        /// </summary>
        public double Length
        {
            get { return start.DistanceTo( end ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> class.
        /// </summary>
        /// 
        /// <param name="start">Segment's start point.</param>
        /// <param name="end">Segment's end point.</param>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two points are the same.</exception>
        /// 
        public LineSegment( DoublePoint start, DoublePoint end )
        {
            line = Line.FromPoints( start, end );
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Converts this <see cref="LineSegment"/> to a <see cref="Line"/> by discarding
        /// its endpoints and extending it infinitely in both directions.
        /// </summary>
        /// 
        /// <param name="segment">The segment to convert to a <see cref="Line"/>.</param>
        /// 
        /// <returns>Returns a <see cref="Line"/> that contains this <paramref name="segment"/>.</returns>
        /// 
        public static explicit operator Line( LineSegment segment )
        {
            return segment.line;
        }
    }
}
