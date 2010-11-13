using System;
using AForge;
using AForge.Math.Geometry;
using MbUnit.Framework;

namespace AForge.Math.Geometry.Tests
{
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
    }
}
