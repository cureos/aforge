// AForge.NET Framework
// Hough line and circle transformation demo
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace HoughTransform
{
    public partial class MainForm : Form
    {
        // binarization filtering sequence
        private FiltersSequence filter = new FiltersSequence(
            new GrayscaleBT709( ),
            new Threshold( )
        );

        HoughLineTransformation lineTransform = new HoughLineTransformation( );
        HoughCircleTransformation circleTransform = new HoughCircleTransformation( 35 );

        // Construct MainForm
        public MainForm( )
        {
            InitializeComponent( );
        }

        // Exit from application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Application.Exit( );
        }

        // Open image file
        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            try
            {
                // show file open dialog
                if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
                {
                    // load image
                    Bitmap image = (Bitmap) Bitmap.FromFile( openFileDialog.FileName );
                    // format image
                    AForge.Imaging.Image.FormatImage( ref image );
                    // lock the source image
                    BitmapData sourceData = image.LockBits(
                        new Rectangle( 0, 0, image.Width, image.Height ),
                        ImageLockMode.ReadOnly, image.PixelFormat );
                    // binarize the image
                    UnmanagedImage binarySource = filter.Apply( new UnmanagedImage( sourceData ) );
                    
                    // apply Hough line transofrm
                    lineTransform.ProcessImage( binarySource );
                    // get lines using relative intensity
                    HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );

                    foreach ( HoughLine line in lines )
                    {
                        string s = string.Format( "Theta = {0}, R = {1}, I = {2} ({3})", line.Theta, line.Radius, line.Intensity, line.RelativeIntensity );
                        System.Diagnostics.Debug.WriteLine( s );
                    }

                    System.Diagnostics.Debug.WriteLine( "Found lines: " + lineTransform.LinesCount );
                    System.Diagnostics.Debug.WriteLine( "Max intensity: " + lineTransform.MaxIntensity );

                    // apply Hough circle transform
                    circleTransform.ProcessImage( binarySource );
                    // get circles using relative intensity
                    HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity( 0.5 );

                    foreach ( HoughCircle circle in circles )
                    {
                        string s = string.Format( "X = {0}, Y = {1}, I = {2} ({3})", circle.X, circle.Y, circle.Intensity, circle.RelativeIntensity );
                        System.Diagnostics.Debug.WriteLine( s );
                    }

                    System.Diagnostics.Debug.WriteLine( "Found circles: " + circleTransform.CirclesCount );
                    System.Diagnostics.Debug.WriteLine( "Max intensity: " + circleTransform.MaxIntensity );

                    // unlock source image
                    image.UnlockBits( sourceData );
                    // dispose temporary binary source image
                    binarySource.Dispose( );

                    // show images
                    sourcePictureBox.Image = image;
                    houghLinePictureBox.Image = lineTransform.ToBitmap( );
                    houghCirclePictureBox.Image = circleTransform.ToBitmap( );
                }
            }
            catch
            {
                MessageBox.Show( "Failed loading the image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }
    }
}