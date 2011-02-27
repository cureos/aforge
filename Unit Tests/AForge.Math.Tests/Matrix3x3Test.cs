using System;
using AForge;
using AForge.Math;
using MbUnit.Framework;

namespace AForge.Math.Tests
{
    [TestFixture]
    public class Matrix3x3Test
    {
        [Test]
        public void ToArrayTest( )
        {
            Matrix3x3 matrix = new Matrix3x3( );

            matrix.V00 = 1;
            matrix.V01 = 2;
            matrix.V02 = 3;

            matrix.V10 = 4;
            matrix.V11 = 5;
            matrix.V12 = 6;

            matrix.V20 = 7;
            matrix.V21 = 8;
            matrix.V22 = 9;

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }
        }

        [Test]
        public void FromRowsTest( )
        {
            Vector3 row0 = new Vector3( 1, 2, 3 );
            Vector3 row1 = new Vector3( 4, 5, 6 );
            Vector3 row2 = new Vector3( 7, 8, 9 );
            Matrix3x3 matrix = Matrix3x3.FromRows( row0, row1, row2 );

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }
        }

        [Test]
        public void FromColumnsTest( )
        {
            Vector3 column0 = new Vector3( 1, 4, 7 );
            Vector3 column1 = new Vector3( 2, 5, 8 );
            Vector3 column2 = new Vector3( 3, 6, 9 );
            Matrix3x3 matrix = Matrix3x3.FromColumns( column0, column1, column2 );

            float[] array = matrix.ToArray( );

            for ( int i = 0; i < 9; i++ )
            {
                Assert.AreEqual<float>( array[i], (float) ( i + 1 ) );
            }
        }
    }
}
