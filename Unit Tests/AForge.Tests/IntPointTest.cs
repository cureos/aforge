using System;
using System.Collections.Generic;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class IntPointTest
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
        public void EuclideanNormTest( int x, int y, double expectedNorm )
        {
            IntPoint point = new IntPoint( x, y );

            Assert.AreEqual( point.EuclideanNorm( ), expectedNorm );
        }
    }
}
