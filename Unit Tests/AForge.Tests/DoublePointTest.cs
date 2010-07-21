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
    }
}
