using System;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
    [TestFixture]
    public class GeometryToolsTest
    {
        [Test]
        [Row( 0, 0, 10, 0, 100, 0, 0 )]
        [Row( 0, 0, 10, 10, 100, 100, 0 )]
        [Row( 0, 0, 10, 0, 0, 100, 90 )]
        [Row( 0, 0, 10, 0, 100, 100, 45 )]
        [Row( 0, 0, 10, 10, -100, 100, 90 )]
        [Row( 0, 0, 10, 0, -100, 100, 135 )]
        [Row( 0, 0, 10, 0, -100, 0, 180 )]
        [Row( 0, 0, 10, 0, -100, -100, 135 )]
        public void GetAngleBetweenVectorsTest( int sx, int sy, int ex1, int ey1, int ex2, int ey2, double expectedAngle )
        {
            IntPoint startPoint = new IntPoint( sx, sy );
            IntPoint vector1end = new IntPoint( ex1, ey1 );
            IntPoint vector2end = new IntPoint( ex2, ey2 );

            double angle = GeometryTools.GetAngleBetweenVectors( startPoint, vector1end, vector2end );

            Assert.AreApproximatelyEqual<double, double>( expectedAngle,  angle, 0.000001 );
        }

        [Test]
        [Row( 0, 0, 10, 0, 0, 10, 10, 10, 0 )]
        [Row( 0, 0, 10, 0, 0, 10, 0, 20, 90 )]
        [Row( 0, 0, 10, 0, 1, 1, 10, 10, 45 )]
        [Row( 0, 0, 10, 0, 1, 1, -8, 10, 45 )]
        [Row( 0, 0, 10, 10, 0, 0, -100, 100, 90 )]
        public void GetAngleBetweenLinesTest( int sx1, int sy1, int ex1, int ey1, int sx2, int sy2, int ex2, int ey2, double expectedAngle )
        {
            IntPoint line1start = new IntPoint( sx1, sy1 );
            IntPoint line1end   = new IntPoint( ex1, ey1 );
            IntPoint line2start = new IntPoint( sx2, sy2 );
            IntPoint line2end   = new IntPoint( ex2, ey2 );

            double angle = GeometryTools.GetAngleBetweenLines( line1start, line1end, line2start, line2end );

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
        public void GetIntersectionPointTest( double ax1, double ay1, double ax2, double ay2,
            double bx1, double by1, double bx2, double by2, double xRet, double yRet )
        {
            DoublePoint a1 = new DoublePoint( ax1, ay1 );
            DoublePoint a2 = new DoublePoint( ax2, ay2 );
            DoublePoint b1 = new DoublePoint( bx1, by1 );
            DoublePoint b2 = new DoublePoint( bx2, by2 );

            DoublePoint result = GeometryTools.GetIntersectionPoint( a1, a2, b1, b2 );
            Assert.IsTrue( result == new DoublePoint( xRet, yRet ) );
        }
    }
}
