using System;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class RangeTest
    {
        [Test]
        [Row( 0, 1, 1, 2, true )]
        [Row( 0, 1, 2, 3, false )]
        [Row( 0, 10, 2, 4, true )]
        [Row( 0, 10, 5, 15, true )]
        [Row( 0, 10, -5, 5, true )]
        [Row( 2, 4, 0, 10, true )]
        [Row( 5, 15, 0, 10, true )]
        [Row( -5, 5, 0, 10, true )]
        public void IsOverlappingTest( float min1, float max1, float min2, float max2, bool expectedResult )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.AreEqual<bool>( expectedResult, range1.IsOverlapping( range2 ) );
        }
    }
}
