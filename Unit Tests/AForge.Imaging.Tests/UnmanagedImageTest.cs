using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging;
using MbUnit.Framework;

namespace AForge.Imaging.Tests
{
    [TestFixture]
    public class UnmanagedImageTest
    {
        [Test]
        public void Collect8bppPixelValuesTest_Grayscale( )
        {
            // create grayscale image
            UnmanagedImage image = UnmanagedImage.Create( 320, 240, PixelFormat.Format8bppIndexed );

            // draw vertical and horizontal lines
            Drawing.Line( image, new IntPoint( 10, 10 ), new IntPoint( 20, 10 ), Color.FromArgb( 128, 128, 128 ) );
            Drawing.Line( image, new IntPoint( 20, 20 ), new IntPoint( 20, 30 ), Color.FromArgb( 64, 64, 64 ) );

            // prepare lists with coordinates
            List<IntPoint> horizontal  = new List<IntPoint>( );
            List<IntPoint> horizontalU = new List<IntPoint>( );
            List<IntPoint> horizontalD = new List<IntPoint>( );

            for ( int x = 10; x <= 20; x++ )
            {
                horizontal.Add( new IntPoint( x, 10 ) );  // on the line
                horizontalU.Add( new IntPoint( x, 9 ) );  // above
                horizontalD.Add( new IntPoint( x, 11 ) ); // below
            }

            List<IntPoint> vertical = new List<IntPoint>( );
            List<IntPoint> verticalL = new List<IntPoint>( );
            List<IntPoint> verticalR = new List<IntPoint>( );

            for ( int y = 20; y <= 30; y++ )
            {
                vertical.Add( new IntPoint( 20, y ) );    // on the line
                verticalL.Add( new IntPoint( 19, y ) );   // left
                verticalR.Add( new IntPoint( 21, y ) );   // right
            }

            // collect all pixel's values
            byte[] horizontalValues = image.Collect8bppPixelValues( horizontal );
            byte[] horizontalUValues = image.Collect8bppPixelValues( horizontalU );
            byte[] horizontalDValues = image.Collect8bppPixelValues( horizontalD );
            byte[] verticalValues = image.Collect8bppPixelValues( vertical );
            byte[] verticalLValues = image.Collect8bppPixelValues( verticalL );
            byte[] verticalRValues = image.Collect8bppPixelValues( verticalR );

            Assert.AreEqual( horizontal.Count, horizontalValues.Length );
            Assert.AreEqual( vertical.Count, verticalValues.Length );

            // check all pixel values
            for ( int i = 0, n = horizontalValues.Length; i < n; i++ )
            {
                Assert.AreEqual<byte>( 128, horizontalValues[i] );
                Assert.AreEqual<byte>( 0, horizontalUValues[i] );
                Assert.AreEqual<byte>( 0, horizontalDValues[i] );
            }

            for ( int i = 0, n = verticalValues.Length; i < n; i++ )
            {
                Assert.AreEqual<byte>( 64, verticalValues[i] );
                Assert.AreEqual<byte>( 0, verticalLValues[i] );
                Assert.AreEqual<byte>( 0, verticalRValues[i] );
            }
        }

        [Test]
        public void Collect8bppPixelValuesTest_RGB( )
        {
            // create grayscale image
            UnmanagedImage image = UnmanagedImage.Create( 320, 240, PixelFormat.Format24bppRgb );

            // draw vertical and horizontal lines
            Drawing.Line( image, new IntPoint( 10, 10 ), new IntPoint( 20, 10 ), Color.FromArgb( 128, 129, 130 ) );
            Drawing.Line( image, new IntPoint( 20, 20 ), new IntPoint( 20, 30 ), Color.FromArgb( 64, 65, 66 ) );

            // prepare lists with coordinates
            List<IntPoint> horizontal = new List<IntPoint>( );
            List<IntPoint> horizontalU = new List<IntPoint>( );
            List<IntPoint> horizontalD = new List<IntPoint>( );

            for ( int x = 10; x <= 20; x++ )
            {
                horizontal.Add( new IntPoint( x, 10 ) );  // on the line
                horizontalU.Add( new IntPoint( x, 9 ) );  // above
                horizontalD.Add( new IntPoint( x, 11 ) ); // below
            }

            List<IntPoint> vertical = new List<IntPoint>( );
            List<IntPoint> verticalL = new List<IntPoint>( );
            List<IntPoint> verticalR = new List<IntPoint>( );

            for ( int y = 20; y <= 30; y++ )
            {
                vertical.Add( new IntPoint( 20, y ) );    // on the line
                verticalL.Add( new IntPoint( 19, y ) );   // left
                verticalR.Add( new IntPoint( 21, y ) );   // right
            }

            // collect all pixel's values
            byte[] horizontalValues = image.Collect8bppPixelValues( horizontal );
            byte[] horizontalUValues = image.Collect8bppPixelValues( horizontalU );
            byte[] horizontalDValues = image.Collect8bppPixelValues( horizontalD );
            byte[] verticalValues = image.Collect8bppPixelValues( vertical );
            byte[] verticalLValues = image.Collect8bppPixelValues( verticalL );
            byte[] verticalRValues = image.Collect8bppPixelValues( verticalR );

            Assert.AreEqual( horizontal.Count * 3, horizontalValues.Length );
            Assert.AreEqual( vertical.Count * 3, verticalValues.Length );

            // check all pixel values
            for ( int i = 0, n = horizontalValues.Length; i < n; i += 3 )
            {
                Assert.AreEqual<byte>( 128, horizontalValues[i] );
                Assert.AreEqual<byte>( 129, horizontalValues[i + 1] );
                Assert.AreEqual<byte>( 130, horizontalValues[i + 2] );

                Assert.AreEqual<byte>( 0, horizontalUValues[i] );
                Assert.AreEqual<byte>( 0, horizontalUValues[i + 1] );
                Assert.AreEqual<byte>( 0, horizontalUValues[i + 2] );

                Assert.AreEqual<byte>( 0, horizontalDValues[i] );
                Assert.AreEqual<byte>( 0, horizontalDValues[i + 1] );
                Assert.AreEqual<byte>( 0, horizontalDValues[i + 2] );
            }

            for ( int i = 0, n = verticalValues.Length; i < n; i += 3 )
            {
                Assert.AreEqual<byte>( 64, verticalValues[i] );
                Assert.AreEqual<byte>( 65, verticalValues[i + 1] );
                Assert.AreEqual<byte>( 66, verticalValues[i + 2] );

                Assert.AreEqual<byte>( 0, verticalLValues[i] );
                Assert.AreEqual<byte>( 0, verticalLValues[i + 1] );
                Assert.AreEqual<byte>( 0, verticalLValues[i + 2] );

                Assert.AreEqual<byte>( 0, verticalRValues[i] );
                Assert.AreEqual<byte>( 0, verticalRValues[i + 1] );
                Assert.AreEqual<byte>( 0, verticalRValues[i + 2] );
            }
        }
    }
}
