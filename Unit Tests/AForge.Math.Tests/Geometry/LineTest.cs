using System;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
    public class LineTest
    {
        [Test]
        [Row( 1, 1, 45, 1.41421356, -1, 2 )]
        [Row( -2, 2, 135, 2 * 1.41421356, 1, 4 )]
        [Row( -0.5, -1.73205081 / 2, 240, 1, -1 / 1.73205081, -2 / 1.73205081 )]
        [Row( 1, 0, 0, 1, Double.NegativeInfinity, 1 )]
        [Row( 0, -1, 270, 1, 0, -1 )]
        public void RThetaTest( double x, double y, double theta, double expectedRadius, double expectedSlope, double expectedIntercept )
        {
            DoublePoint pt = new DoublePoint( x, y );

            // test Point-Theta factory
            Line line = Line.FromPointTheta( pt, theta );
            Assert.AreApproximatelyEqual( expectedSlope, line.Slope, 0.000001 );
            Assert.AreApproximatelyEqual( expectedIntercept, line.Intercept, 0.000001 );

            // calculate radius
            double radius = pt.EuclideanNorm( );
            Assert.AreApproximatelyEqual( expectedRadius, radius, 0.000001 );

            // test R-Theta factory
            line = Line.FromRTheta( radius, theta );
            Assert.AreApproximatelyEqual( expectedSlope, line.Slope, 0.000001 );
            Assert.AreApproximatelyEqual( expectedIntercept, line.Intercept, 0.000001 );
        }

        [Test]
        [Row( 0, 0, 0, 10, true, Double.PositiveInfinity, 0 )]
        [Row( 0, 0, 0, -10, true, Double.NegativeInfinity, 0 )]
        [Row( 0, 0, 10, 10, false, 1, 0 )]
        [Row( 0, 0, 10, 0, false, 0, 0 )]
        public void IsVerticalTest( double sx, double sy, double ex, double ey, bool expectedResult, double expectedSlope, double expectedIntercept )
        {
            Line line = Line.FromPoints( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual( expectedResult, line.IsVertical );
            Assert.AreEqual( expectedSlope, line.Slope );
            Assert.AreEqual( expectedIntercept, line.Intercept );
        }

        [Test]
        [Row( 0, 0, 10, 0, true, 0, 0 )]
        [Row( 0, 0, -10, 0, true, 0, 0 )]
        [Row( 0, 0, 10, 10, false, 1, 0 )]
        [Row( 0, 0, 0, 10, false, Double.PositiveInfinity, 0 )]
        public void IsHorizontalTest( double sx, double sy, double ex, double ey, bool expectedResult, double expectedSlope, double expectedIntercept )
        {
            Line line = Line.FromPoints( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual( expectedResult, line.IsHorizontal );
            Assert.AreEqual( expectedSlope, line.Slope );
            Assert.AreEqual( expectedIntercept, line.Intercept );
        }

        [Test]
        [Row( 0, 0, 10, 0, 0, 10, 10, 10, 0 )]
        [Row( 0, 0, 10, 0, 0, 10, 0, 20, 90 )]
        [Row( 0, 0, 10, 0, 1, 1, 10, 10, 45 )]
        [Row( 0, 0, 10, 0, 1, 1, -8, 10, 45 )]
        [Row( 0, 0, 10, 10, 0, 0, -100, 100, 90 )]
        public void GetAngleBetweenLinesTest( double sx1, double sy1, double ex1, double ey1, double sx2, double sy2, double ex2, double ey2, double expectedAngle )
        {
            Line line1 = Line.FromPoints( new DoublePoint( sx1, sy1 ), new DoublePoint( ex1, ey1 ) );
            Line line2 = Line.FromPoints( new DoublePoint( sx2, sy2 ), new DoublePoint( ex2, ey2 ) );

            double angle = line1.GetAngleBetweenLines( line2 );

            Assert.AreApproximatelyEqual<double, double>( expectedAngle, angle, 0.000001 );
        }

        [Test]
        [Row( 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, ExpectedException = typeof( InvalidOperationException ) )]
        [Row( 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, ExpectedException = typeof( InvalidOperationException ) )]
        [Row( 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, ExpectedException = typeof( InvalidOperationException ) )]
        [Row( 0, 0, 1, 1, 0, 1, 1, 2, 0, 0, ExpectedException = typeof( InvalidOperationException ) )]
        [Row( 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 )]
        [Row( 0, 0, 1, 0, 0, 1, 1, 2, -1, 0 )]
        [Row( 0, 0, 1, 0, 1, 1, 1, 2, 1, 0 )]
        [Row( 0, 0, 0, 1, 0, 1, -1, 1, 0, 1 )]
        [Row( -1, -1, 1, 1, 1, -1, -1, 1, 0, 0 )]
        public void GetIntersectionPointTest( double sx1, double sy1, double ex1, double ey1,
            double sx2, double sy2, double ex2, double ey2, double xRet, double yRet )
        {
            Line line1 = Line.FromPoints( new DoublePoint( sx1, sy1 ), new DoublePoint( ex1, ey1 ) );
            Line line2 = Line.FromPoints( new DoublePoint( sx2, sy2 ), new DoublePoint( ex2, ey2 ) );

            DoublePoint result = line1.GetIntersectionWith( line2 );

            Assert.IsTrue( result == new DoublePoint( xRet, yRet ) );
        }

        [Test]
        [Row( 0, 0, 5, 0, 8, 0, 0 )]
        [Row( 6, 2, 5, 0, 8, 0, 2 )]
        [Row( 2, 6, 0, 5, 0, 8, 2 )]
        [Row( 9, 0, 5, 0, 8, 0, 0 )]
        [Row( 3, 0, 0, 0, 3, 4, 2.4 )]
        public void DistanceToPointTest( double x, double y, double x1, double y1, double x2, double y2, double expectedDistance )
        {
            DoublePoint pt = new DoublePoint( x, y );
            DoublePoint pt1 = new DoublePoint( x1, y1 );
            DoublePoint pt2 = new DoublePoint( x2, y2 );
            Line line = Line.FromPoints( pt1, pt2 );

            Assert.AreEqual( expectedDistance, line.DistanceToPoint( pt ) );
        }
    }
}
