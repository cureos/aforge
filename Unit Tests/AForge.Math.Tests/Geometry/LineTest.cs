using System;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Tests.Geometry
{
    public class LineTest
    {
        [Test]
        [Row( 0, 0, 10, 0, 10 )]
        [Row( 0, 0, 0, 10, 10 )]
        [Row( 0, 0, 3, 4, 5 )]
        [Row( 0, 0, -3, 4, 5 )]
        [Row( 0, 0, -3, -4, 5 )]
        public void LengthTest( double sx, double sy, double ex, double ey, double expectedResult )
        {
            Line line = new Line( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual<double>( line.Length, expectedResult );
        }

        [Test]
        [Row( 0, 0, 0, 10, true )]
        [Row( 0, 0, 0, -10, true )]
        [Row( 0, 0, 10, 10, false )]
        [Row( 0, 0, 10, 0, false )]
        public void IsVerticalTest( double sx, double sy, double ex, double ey, bool expectedResult )
        {
            Line line = new Line( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual<bool>( line.IsVertical, expectedResult );
        }

        [Test]
        [Row( 0, 0, 10, 0, true )]
        [Row( 0, 0, -10, 0, true )]
        [Row( 0, 0, 10, 10, false )]
        [Row( 0, 0, 0, 10, false )]
        public void IsHorizontalTest( double sx, double sy, double ex, double ey, bool expectedResult )
        {
            Line line = new Line( new DoublePoint( sx, sy ), new DoublePoint( ex, ey ) );

            Assert.AreEqual<bool>( line.IsHorizontal, expectedResult );
        }

        [Test]
        [Row( 0, 0, 10, 0, 0, 10, 10, 10, 0 )]
        [Row( 0, 0, 10, 0, 0, 10, 0, 20, 90 )]
        [Row( 0, 0, 10, 0, 1, 1, 10, 10, 45 )]
        [Row( 0, 0, 10, 0, 1, 1, -8, 10, 45 )]
        [Row( 0, 0, 10, 10, 0, 0, -100, 100, 90 )]
        public void GetAngleBetweenLinesTest( double sx1, double sy1, double ex1, double ey1, double sx2, double sy2, double ex2, double ey2, double expectedAngle )
        {
            Line line1 = new Line( new DoublePoint( sx1, sy1 ), new DoublePoint( ex1, ey1 ) );
            Line line2 = new Line( new DoublePoint( sx2, sy2 ), new DoublePoint( ex2, ey2 ) );

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
            Line line1 = new Line( new DoublePoint( sx1, sy1 ), new DoublePoint( ex1, ey1 ) );
            Line line2 = new Line( new DoublePoint( sx2, sy2 ), new DoublePoint( ex2, ey2 ) );

            DoublePoint result = line1.GetIntersectionWith( line2 );

            Assert.IsTrue( result == new DoublePoint( xRet, yRet ) );
        }
    }
}
