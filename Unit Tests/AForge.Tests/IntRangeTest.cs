using System;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class IntRangeTest
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
        public void IsOverlappingTest( int min1, int max1, int min2, int max2, bool expectedResult )
        {
            IntRange range1 = new IntRange( min1, max1 );
            IntRange range2 = new IntRange( min2, max2 );

            Assert.AreEqual<bool>( expectedResult, range1.IsOverlapping( range2 ) );
        }
    }
}
