using NUnit.Framework;

namespace AForge.Tests
{
    [TestFixture]
    public class RangeTest
    {
        [TestCase( 0f, 1f, 1f, 2f, true )]
        [TestCase( 0f, 1f, 2f, 3f, false )]
        [TestCase( 0f, 10f, 2f, 4f, true )]
        [TestCase( 0f, 10f, 5f, 15f, true )]
        [TestCase( 0f, 10f, -5f, 5f, true )]
        [TestCase( 2f, 4f, 0f, 10f, true )]
        [TestCase( 5f, 15f, 0f, 10f, true )]
        [TestCase( -5f, 5f, 0f, 10f, true )]
        public void IsOverlappingTest( float min1, float max1, float min2, float max2, bool expectedResult )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.AreEqual( expectedResult, range1.IsOverlapping( range2 ) );
        }

        [TestCase( 0.4f, 7.3f, 1, 7, true )]
        [TestCase( -6.6f, -0.1f, -6, -1, true )]
        [TestCase( 0.4f, 7.3f, 0, 8, false )]
        [TestCase( -6.6f, -0.1f, -7, 0, false )]
        public void ToRangeTest( float fMin, float fMax, int iMin, int iMax, bool innerRange )
        {
            Range range = new Range( fMin, fMax );
            IntRange iRange = range.ToIntRange( innerRange );

            Assert.AreEqual( iMin, iRange.Min );
            Assert.AreEqual( iMax, iRange.Max );
        }

        [TestCase( 1.1f, 2.2f, 1.1f, 2.2f, true )]
        [TestCase( -2.2f, -1.1f, -2.2f, -1.1f, true )]
        [TestCase( 1.1f, 2.2f, 2.2f, 3.3f, false )]
        [TestCase( 1.1f, 2.2f, 1.1f, 4.4f, false )]
        [TestCase( 1.1f, 2.2f, 3.3f, 4.4f, false )]
        public void EqualityOperatorTest( float min1, float max1, float min2, float max2, bool areEqual )
        {
            Range range1 = new Range( min1, max1 );
            Range range2 = new Range( min2, max2 );

            Assert.AreEqual( range1.Equals( range2 ), areEqual );
            Assert.AreEqual( range1 == range2, areEqual );
            Assert.AreEqual( range1 != range2, !areEqual );
        }
    }
}
