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
    /// <para>Sample usage:</para>
    /// <code>
    /// // create a segment
    /// LineSegment segment = new LineSegment( new DoublePoint( 0, 0 ), new DoublePoint( 3, 4 ) );
    /// // get segment's length
    /// double length = segment.Length;
    /// 
    /// // get intersection point with a line
    /// DoublePoint? intersection = segment.GetIntersectionWith(
    ///     new Line( new DoublePoint( -3, 8 ), new DoublePoint( 0, 4 ) ) );
    /// </code>
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

        /// <summary>
        /// Calculate Euclidean distance between a point and a finite line segment.
        /// </summary>
        /// 
        /// <param name="point">The point to calculate the distance to.</param>
        /// 
        /// <returns>Returns the Euclidean distance between this line segment and the specified point. Unlike
        /// <see cref="Line.DistanceToPoint"/>, this returns the distance from the finite segment. (0,0) is 5 units
        /// from the segment (0,5)-(0,8), but is 0 units from the line through those points.</returns>
        /// 
        public double DistanceToPoint( DoublePoint point )
        {
            double segmentDistance;

            switch ( LocateProjection( point ) )
            {
                case ProjectionLocation.RayA:
                    segmentDistance = point.DistanceTo( start );
                    break;
                case ProjectionLocation.RayB:
                    segmentDistance = point.DistanceTo( end );
                    break;
                default:
                    segmentDistance = line.DistanceToPoint( point );
                    break;
            };

            return segmentDistance;
        }

        /// <summary>
        /// Finds, provided it exists, the intersection point with the specified <see cref="LineSegment"/>.
        /// </summary>
        /// 
        /// <param name="other"><see cref="LineSegment"/> to find intersection with.</param>
        /// 
        /// <returns>Returns intersection point with the specified <see cref="LineSegment"/>, or <see langword="null"/>, if
        /// the two segments do not intersect.</returns>
        /// 
        /// <remarks><para>If the two segments do not intersect, the method returns <see langword="null"/>. If the two
        /// segments share multiple points, this throws an <see cref="InvalidOperationException"/>.
        /// </para></remarks>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if the segments overlap - if they have
        /// multiple points in common.</exception>
        /// 
        public DoublePoint? GetIntersectionWith( LineSegment other )
        {
            DoublePoint? result = null;

            if ( ( line.Slope == other.line.Slope ) || ( line.IsVertical && other.line.IsVertical ) )
            {
                if ( line.Intercept == other.line.Intercept )
                {
                    // Collinear segments. Inspect and handle.
                    // Consider this segment AB and other as CD. (start/end in both cases)
                    // There are three cases:
                    // 0 shared points: C and D both project onto the same ray of AB
                    // 1 shared point: One of A or B equals one of C or D, and the other of C/D 
                    //      projects on the correct ray.
                    // Many shared points.

                    ProjectionLocation projC = LocateProjection( other.start ), projD = LocateProjection( other.end );

                    if ( ( projC != ProjectionLocation.SegmentAB ) && ( projC == projD ) )
                    {
                        // no shared points
                        result = null;
                    }
                    else if ( ( ( start == other.start ) && ( projD == ProjectionLocation.RayA ) ) ||
                              ( ( start == other.end ) && ( projC == ProjectionLocation.RayA ) ) )
                    {
                        // shared start point
                        result = start;
                    }
                    else if ( ( ( end == other.start ) && ( projD == ProjectionLocation.RayB ) ) ||
                              ( ( end == other.end ) && ( projC == ProjectionLocation.RayB ) ) )
                    {
                        // shared end point
                        result = end;
                    }
                    else
                    {
                        // overlapping
                        throw new InvalidOperationException( "Overlapping segments do not have a single intersection point." );
                    }
                }
            }
            else
            {
                result = GetIntersectionWith( other.line );

                if ( ( result.HasValue ) && ( other.LocateProjection( result.Value ) != ProjectionLocation.SegmentAB ) )
                {
                    // the intersection is on the extended line of this segment
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Finds, provided it exists, the intersection point with the specified <see cref="Line"/>.
        /// </summary>
        /// 
        /// <param name="other"><see cref="Line"/> to find intersection with.</param>
        /// 
        /// <returns>Returns intersection point with the specified <see cref="Line"/>, or <see langword="null"/>, if
        /// the line does not intersect with this segment.</returns>
        /// 
        /// <remarks><para>If the line and the segment do not intersect, the method returns <see langword="null"/>. If the line
        /// and the segment share multiple points, the method throws an <see cref="InvalidOperationException"/>.
        /// </para></remarks>
        /// 
        /// <exception cref="InvalidOperationException">Thrown if this segment is a portion of
        /// <paramref name="other"/> line.</exception>
        /// 
        public DoublePoint? GetIntersectionWith( Line other )
        {
            DoublePoint? result;

            if ( ( line.Slope == other.Slope ) || ( line.IsVertical && other.IsVertical ) )
            {
                if ( line.Intercept == other.Intercept ) throw new InvalidOperationException( "Segment is a portion of the specified line." );

                // unlike Line.GetIntersectionWith(Line), this does not throw on parallel distinct lines
                result = null;
            }
            else
            {
                result = line.GetIntersectionWith( other );
            }

            if ( ( result.HasValue ) && ( LocateProjection( result.Value ) != ProjectionLocation.SegmentAB ) )
            {
                // the intersection is on this segment's extended line, but not on the segment itself
                result = null;
            }

            return result;
        }

        // Represents the location of a projection of a point on the line that contains this segment.
        // If the point projects to A,B, or anything between them, it is SegmentAB.
        // If it projects beyond A, it's RayA; if it projects beyond B, it's RayB.
        private enum ProjectionLocation { RayA, SegmentAB, RayB }

        // Get type of point's projections to this line segment
        private ProjectionLocation LocateProjection( DoublePoint point )
        {
            // Modified from http://www.codeguru.com/forum/showthread.php?t=194400

            /*  How do I find the distance from a point to a line segment?

                Let the point be C (Cx,Cy) and the line be AB (Ax,Ay) to (Bx,By).
                Let P be the point of perpendicular projection of C on AB.  The parameter
                r, which indicates P's position along AB, is computed by the dot product 
                of AC and AB divided by the square of the length of AB:
                
                (1)     AC dot AB
                    r = ---------  
                        ||AB||^2
                
                r has the following meaning:
                
                    r=0      P = A
                    r=1      P = B
                    r<0      P is on the backward extension of AB (and distance C-AB is distance C-A)
                    r>1      P is on the forward extension of AB (and distance C-AB is distance C-B)
                    0<r<1    P is interior to AB (and distance C-AB(segment) is distance C-AB(line))
                
                The length of the line segment AB is computed by:
                
                    L = sqrt( (Bx-Ax)^2 + (By-Ay)^2 )
                
                and the dot product of two 2D vectors, U dot V, is computed:
                
                    D = (Ux * Vx) + (Uy * Vy) 
                
                So (1) expands to:
                
                        (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay)
                    r = -------------------------------
                             (Bx-Ax)^2 + (By-Ay)^2
            */

            // the above is modified here to compare the numerator and denominator, rather than doing the division
            DoublePoint abDelta = end - start;
            DoublePoint acDelta = point - start;

            double numerator   = acDelta.X * abDelta.X + acDelta.Y * abDelta.Y;
            double denomenator = abDelta.X * abDelta.X + abDelta.Y * abDelta.Y;

            ProjectionLocation result = ( numerator < 0 ) ? ProjectionLocation.RayA : ( numerator > denomenator ) ? ProjectionLocation.RayB : ProjectionLocation.SegmentAB;

            return result;
        }
    }
}
