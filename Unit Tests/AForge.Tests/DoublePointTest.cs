using System;
using System.Collections.Generic;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class DoublePointTest
    {
        [Test]
        [Row( 0, 0, 0 )]
        [Row( 0, 1, 1 )]
        [Row( 0, 10, 10 )]
        [Row( 10, 0, 10 )]
        [Row( 3, 4, 5 )]
        [Row( -3, 4, 5 )]
        [Row( 3, -4, 5 )]
        [Row( -3, -4, 5 )]
        [Row( 0.3, 0.4, 0.5 )]
        public void EuclideanNormTest( double x, double y, double expectedNorm )
        {
            DoublePoint point = new DoublePoint( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }

        [Test]
        [Row( 0, 0, 0, 0 )]
        [Row( 1, 2, 1, 2 )]
        [Row( -1, -2, -1, -2 )]
        [Row( 1.4, 3.3, 1, 3 )]
        [Row( 1.6, 3.7, 2, 4 )]
        [Row( -1.6, -3.3, -2, -3 )]
        [Row( -1.5, 1.5, -2, 2 )]
        [Row( -2.5, 2.5, -2, 2 )]
        public void RoundTest( double x, double y, int expectedX, int expectedY )
        {
            DoublePoint dPoint = new DoublePoint( x, y );
            IntPoint    iPoint = new IntPoint( expectedX, expectedY );

            Assert.AreEqual( iPoint, dPoint.Round( ) );
        }

        [Test]
        [Row( 1.1, 2.2, 1.1, 2.2, true )]
        [Row( 1.1, 2.2, 3.3, 2.2, false )]
        [Row( 1.1, 2.2, 1.1, 4.4, false )]
        [Row( 1.1, 2.2, 3.3, 4.4, false )]
        public void EqualityOperatorTest( double x1, double y1, double x2, double y2, bool areEqual )
        {
            DoublePoint point1 = new DoublePoint( x1, y1 );
            DoublePoint point2 = new DoublePoint( x2, y2 );

            Assert.AreEqual( point1 == point2, areEqual );
        }

        [Test]
        [Row( 1.1, 2.2, 1.1, 2.2, false )]
        [Row( 1.1, 2.2, 3.3, 2.2, true )]
        [Row( 1.1, 2.2, 1.1, 4.4, true )]
        [Row( 1.1, 2.2, 3.3, 4.4, true )]
        public void InequalityOperatorTest( double x1, double y1, double x2, double y2, bool areNotEqual )
        {
            DoublePoint point1 = new DoublePoint( x1, y1 );
            DoublePoint point2 = new DoublePoint( x2, y2 );

            Assert.AreEqual( point1 != point2, areNotEqual );
        }
    }
}
