using System;
using AForge;
using MbUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class DoubleRangeTest
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
        public void IsOverlappingTest( double min1, double max1, double min2, double max2, bool expectedResult )
        {
            DoubleRange range1 = new DoubleRange( min1, max1 );
            DoubleRange range2 = new DoubleRange( min2, max2 );

            Assert.AreEqual<bool>( expectedResult, range1.IsOverlapping( range2 ) );
        }
    }
}
