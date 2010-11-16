using System;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
    [TestFixture]
    public class LineSegmentTest
    {
        [Test]
        [Row( 0, 0, 10, 0, 10 )]
        [Row( 0, 0, 0, 10, 10 )]
        [Row( 0, 0, 3, 4, 5 )]
        [Row( 0, 0, -3, 4, 5 )]
        [Row( 0, 0, -3, -4, 5 )]
        public void LengthTest( double sx, double sy, double ex, double ey, double expectedResult )
        {
            LineSegment segment = new LineSegment( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual( expectedResult, segment.Length );
        }

        [Test]
        [Row( 0, 0, 5, 0, 8, 0, 5 )]
        [Row( 6, 2.5, 5, 0, 8, 0, 2.5 )]
        [Row( 2.5, 6, 0, 5, 0, 8, 2.5 )]
        [Row( 9, 0, 5, 0, 8, 0, 1 )]
        [Row( 3, 4, 0, 0, -10, 0, 5 )]
        public void DistanceToPointTest( double x, double y, double x1, double y1, double x2, double y2, double expectedDistance )
        {
            DoublePoint pt = new DoublePoint( x, y );
            DoublePoint pt1 = new DoublePoint( x1, y1 );
            DoublePoint pt2 = new DoublePoint( x2, y2 );
            LineSegment segment = new LineSegment( pt1, pt2 );

            Assert.AreEqual( expectedDistance, segment.DistanceToPoint( pt ) );
        }

        // Denotes which versions of the test are supposed to return non-null values:
        // SegmentA means that the segment A1-A2 intersects with the line B1-B2, but not
        // with the segment B1-B2.
        public enum IntersectionType { LinesOnly, SegmentA, SegmentB, AllFour };

        [Test]
        [Row( 0, 0, 4, 4, 0, 4, 4, 0, 2, 2, IntersectionType.AllFour )]
        [Row( 0, 0, 4, 0, 0, 0, 0, 4, 0, 0, IntersectionType.AllFour )]
        [Row( 0, 0, 4, 4, 4, 8, 8, 4, 6, 6, IntersectionType.SegmentB )]
        [Row( -4, -4, 0, 0, 4, 0, 8, -4, 2, 2, IntersectionType.LinesOnly )]
        [Row( 0, 0, 6, 0, 5, 1, 5, 5, 5, 0, IntersectionType.SegmentA )]
        [Row( 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, IntersectionType.LinesOnly, ExpectedException = typeof( ArgumentException ), ExpectedExceptionMessage = "Start point of the line cannot be the same as its end point." )]
        [Row( 0, 0, 0, 5, 1, 0, 1, 5, 0, 0, IntersectionType.LinesOnly, ExpectedException = typeof( InvalidOperationException ), ExpectedExceptionMessage = "Parallel lines do not have an intersection point." )]
        public void IntersectionPointTest( double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2, double ix, double iy, IntersectionType type )
        {
            LineSegment segA = new LineSegment( new DoublePoint( ax1, ay1 ), new DoublePoint( ax2, ay2 ) );
            LineSegment segB = new LineSegment( new DoublePoint( bx1, by1 ), new DoublePoint( bx2, by2 ) );
            DoublePoint expectedIntersection = new DoublePoint( ix, iy );

            Assert.AreEqual( expectedIntersection, ( (Line) segA ).GetIntersectionWith( (Line) segB ) );

            Assert.DoesNotThrow( ( ) =>
            {
                DoublePoint? segSeg = segA.GetIntersectionWith( segB );
                DoublePoint? segLine = segA.GetIntersectionWith( (Line) segB );
                DoublePoint? lineSeg = ( (Line) segA ).GetIntersectionWith( segB );

                if ( type == IntersectionType.AllFour )
                {
                    Assert.AreEqual( expectedIntersection, segSeg );
                }
                else
                {
                    Assert.AreEqual( null, segSeg );
                }

                if ( ( type == IntersectionType.AllFour ) || ( type == IntersectionType.SegmentA ) )
                {
                    Assert.AreEqual( expectedIntersection, segLine );
                }
                else
                {
                    Assert.AreEqual( null, segLine );
                }

                if ( ( type == IntersectionType.AllFour ) || ( type == IntersectionType.SegmentB ) )
                {
                    Assert.AreEqual( expectedIntersection, lineSeg );
                }
                else
                {
                    Assert.AreEqual( null, lineSeg );
                }
            } );
        }

        [Test]
        [Row( 0, 0, 0, 1, 1, 1, 1, 2 )]
        [Row( 0, 0, 4, 4, 3, -1, 7, 3 )]
        [Row( 0, 0, 1, 0, 1, 1, 2, 1 )]
        public void ParallelIntersectionPointTest( double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2 )
        {
            LineSegment segA = new LineSegment( new DoublePoint( ax1, ay1 ), new DoublePoint( ax2, ay2 ) );
            LineSegment segB = new LineSegment( new DoublePoint( bx1, by1 ), new DoublePoint( bx2, by2 ) );

            // are we really parallel?
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( (Line) segB ) );

            Assert.AreEqual( null, segA.GetIntersectionWith( (Line) segB ) );
            Assert.AreEqual( null, ( (Line) segA ).GetIntersectionWith( segB ) );
            Assert.AreEqual( null, segB.GetIntersectionWith( (Line) segA ) );
            Assert.AreEqual( null, ( (Line) segB ).GetIntersectionWith( segA ) );
            Assert.AreEqual( null, segB.GetIntersectionWith( segA ) );
            Assert.AreEqual( null, segA.GetIntersectionWith( segB ) );
        }

        [Test]
        [Row( 0, 0, 1, 1, 2, 2, 3, 3 )]
        [Row( 0, 1, 0, 2, 0, 3, 0, 4 )]
        [Row( 0, 0, -1, 1, -2, 2, -3, 3, -4, 4 )]
        [Row( 1, 0, 2, 0, 3, 0, 4, 0 )]
        public void CollinearIntersectionPointTest( double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2 )
        {
            LineSegment segA = new LineSegment( new DoublePoint( ax1, ay1 ), new DoublePoint( ax2, ay2 ) );
            LineSegment segB = new LineSegment( new DoublePoint( bx1, by1 ), new DoublePoint( bx2, by2 ) );

            // are we really parallel?
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( (Line) segB ) );

            Assert.Throws<InvalidOperationException>( ( ) => segA.GetIntersectionWith( (Line) segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => segB.GetIntersectionWith( (Line) segA ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segB ).GetIntersectionWith( segA ) );
            Assert.AreEqual( null, segB.GetIntersectionWith( segA ) );
            Assert.AreEqual( null, segA.GetIntersectionWith( segB ) );
        }

        [Test]
        [Row( 0, 0, 1, 1, 1, 1, 3, 3, 1, 1 )]
        [Row( 0, 0, 1, 1, 3, 3, 1, 1, 1, 1 )]
        [Row( 0, 0, 1, 1, 0, 0, -3, -3, 0, 0 )]
        [Row( 0, 0, 1, 1, -1, -1, 0, 0, 0, 0 )]
        [Row( 0, 1, 0, 2, 0, 1, 0, 0, 0, 1 )]
        [Row( 0, 1, 0, 2, 0, 2, 0, 4, 0, 2 )]
        [Row( 0, 1, 0, 2, 0, 0, 0, 1, 0, 1 )]
        [Row( 0, 1, 0, 2, 0, 3, 0, 2, 0, 2 )]
        public void CommonIntersectionPointTest( double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2, double ix, double iy )
        {
            LineSegment segA = new LineSegment( new DoublePoint( ax1, ay1 ), new DoublePoint( ax2, ay2 ) );
            LineSegment segB = new LineSegment( new DoublePoint( bx1, by1 ), new DoublePoint( bx2, by2 ) );
            DoublePoint expectedIntersection = new DoublePoint( ix, iy );

            // are we really parallel?
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( (Line) segB ) );

            Assert.Throws<InvalidOperationException>( ( ) => segA.GetIntersectionWith( (Line) segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => segB.GetIntersectionWith( (Line) segA ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segB ).GetIntersectionWith( segA ) );
            Assert.AreEqual( expectedIntersection, segB.GetIntersectionWith( segA ) );
            Assert.AreEqual( expectedIntersection, segA.GetIntersectionWith( segB ) );
        }

        [Test]
        [Row( 0, 0, 0, 2, 0, 1, 0, 3 )]
        [Row( 1, 2, 3, 4, 2, 3, 4, 5 )]
        [Row( 0, 0, 2, 0, 3, 0, 1, 0 )]
        public void OverlappingSegmentIntersectionPointTest( double ax1, double ay1, double ax2, double ay2, double bx1, double by1, double bx2, double by2 )
        {
            LineSegment segA = new LineSegment( new DoublePoint( ax1, ay1 ), new DoublePoint( ax2, ay2 ) );
            LineSegment segB = new LineSegment( new DoublePoint( bx1, by1 ), new DoublePoint( bx2, by2 ) );

            // are we really parallel?
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( (Line) segB ) );

            Assert.Throws<InvalidOperationException>( ( ) => segA.GetIntersectionWith( (Line) segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segA ).GetIntersectionWith( segB ) );
            Assert.Throws<InvalidOperationException>( ( ) => segB.GetIntersectionWith( (Line) segA ) );
            Assert.Throws<InvalidOperationException>( ( ) => ( (Line) segB ).GetIntersectionWith( segA ) );
            Assert.Throws<InvalidOperationException>( ( ) => segB.GetIntersectionWith( segA ) );
            Assert.Throws<InvalidOperationException>( ( ) => segA.GetIntersectionWith( segB ) );
        }
    }
}
