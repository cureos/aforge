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

        [Test]
        public void CollectActivePixelsTest( )
        {
            // create grayscale image
            UnmanagedImage image24 = UnmanagedImage.Create( 7, 7, PixelFormat.Format24bppRgb );
            UnmanagedImage image8  = UnmanagedImage.Create( 7, 7, PixelFormat.Format8bppIndexed );

            Drawing.FillRectangle( image24, new Rectangle( 1, 1, 5, 5 ), Color.FromArgb( 255, 255, 255 ) );
            Drawing.FillRectangle( image24, new Rectangle( 2, 2, 3, 3 ), Color.FromArgb(   1,   1,   1 ) );
            Drawing.FillRectangle( image24, new Rectangle( 3, 3, 1, 1 ), Color.FromArgb(   0,   0,   0 ) );

            Drawing.FillRectangle( image8, new Rectangle( 1, 1, 5, 5 ), Color.FromArgb( 255, 255, 255 ) );
            Drawing.FillRectangle( image8, new Rectangle( 2, 2, 3, 3 ), Color.FromArgb(   1,   1,   1 ) );
            Drawing.FillRectangle( image8, new Rectangle( 3, 3, 1, 1 ), Color.FromArgb(   0,   0,   0 ) );

            List<IntPoint> pixels24 = image24.CollectActivePixels( );
            List<IntPoint> pixels8  = image8.CollectActivePixels( );

            Assert.AreEqual<int>( pixels24.Count, 24 );
            Assert.AreEqual<int>( pixels8.Count, 24 );

            for ( int i = 1; i < 6; i++ )
            {
                for ( int j = 1; j < 6; j++ )
                {
                    if ( ( i == 3 ) && ( j == 3 ) )
                        continue;

                    Assert.IsTrue( pixels24.Contains( new IntPoint( j, i ) ) );
                    Assert.IsTrue( pixels8.Contains( new IntPoint( j, i ) ) );
                }
            }

            pixels24 = image24.CollectActivePixels( new Rectangle( 1, 0, 5, 4 ) );
            pixels8 = image8.CollectActivePixels( new Rectangle( 1, 0, 5, 4 ) );

            Assert.AreEqual<int>( pixels24.Count, 14 );
            Assert.AreEqual<int>( pixels8.Count, 14 );

            for ( int i = 1; i < 4; i++ )
            {
                for ( int j = 1; j < 6; j++ )
                {
                    if ( ( i == 3 ) && ( j == 3 ) )
                        continue;

                    Assert.IsTrue( pixels24.Contains( new IntPoint( j, i ) ) );
                    Assert.IsTrue( pixels8.Contains( new IntPoint( j, i ) ) );
                }
            }
        }

        [Test]
        [Row( PixelFormat.Format8bppIndexed )]
        [Row( PixelFormat.Format24bppRgb )]
        [Row( PixelFormat.Format32bppArgb)]
        [Row( PixelFormat.Format32bppRgb )]
        [Row( PixelFormat.Format16bppGrayScale )]
        [Row( PixelFormat.Format48bppRgb )]
        [Row( PixelFormat.Format64bppArgb )]
        [Row( PixelFormat.Format32bppPArgb, ExpectedException = typeof( UnsupportedImageFormatException ) )]
        public void SetPixelTest( PixelFormat pixelFormat )
        {
            UnmanagedImage image = UnmanagedImage.Create( 320, 240, pixelFormat );
            Color color = Color.White;

            image.SetPixel( 0, 0, color );
            image.SetPixel( 319, 0, color );
            image.SetPixel( 0, 239, color );
            image.SetPixel( 319, 239, color );
            image.SetPixel( 160, 120, color );

            image.SetPixel( -1, -1, color );
            image.SetPixel( 320, 0, color );
            image.SetPixel( 0, 240, color );
            image.SetPixel( 320, 240, color );

            List<IntPoint> pixels = image.CollectActivePixels( );

            Assert.AreEqual<int>( 5, pixels.Count );

            Assert.IsTrue( pixels.Contains( new IntPoint( 0, 0 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 319, 0 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 0, 239 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 319, 239 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 160, 120 ) ) );
        }

        [Test]
        [Row( PixelFormat.Format8bppIndexed )]
        [Row( PixelFormat.Format24bppRgb )]
        [Row( PixelFormat.Format32bppArgb )]
        [Row( PixelFormat.Format32bppRgb )]
        [Row( PixelFormat.Format16bppGrayScale )]
        [Row( PixelFormat.Format48bppRgb )]
        [Row( PixelFormat.Format64bppArgb )]
        [Row( PixelFormat.Format32bppPArgb, ExpectedException = typeof( UnsupportedImageFormatException ) )]
        public void SetPixelsTest( PixelFormat pixelFormat )
        {
            UnmanagedImage image = UnmanagedImage.Create( 320, 240, pixelFormat );
            Color color = Color.White;
            List<IntPoint> points = new List<IntPoint>( );

            points.Add( new IntPoint( 0, 0 ) );
            points.Add( new IntPoint( 319, 0 ) );
            points.Add( new IntPoint( 0, 239 ) );
            points.Add( new IntPoint( 319, 239 ) );
            points.Add( new IntPoint( 160, 120 ) );

            points.Add( new IntPoint( -1, -1 ) );
            points.Add( new IntPoint( 320, 0 ) );
            points.Add( new IntPoint( 0, 240 ) );
            points.Add( new IntPoint( 320, 240 ) );

            image.SetPixels( points, color );

            List<IntPoint> pixels = image.CollectActivePixels( );

            Assert.AreEqual<int>( 5, pixels.Count );

            Assert.IsTrue( pixels.Contains( new IntPoint( 0, 0 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 319, 0 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 0, 239 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 319, 239 ) ) );
            Assert.IsTrue( pixels.Contains( new IntPoint( 160, 120 ) ) );
        }
    }
}
