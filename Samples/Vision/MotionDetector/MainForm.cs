// Motion Detection sample application
// AForge.NET Framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AForge.Video;
using AForge.Video.VFW;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;

namespace MotionDetectorSample
{
    public partial class MainForm : Form
    {
        // opened video source
        private IVideoSource videoSource = null;
        // motion detector
        MotionDetector detector = new MotionDetector(
            new TwoFramesDifferenceDetector( ),
            new MotionAreaHighlighting( ) );
        // motion detection and processing algorithm
        private int motionDetectionType = 1;
        private int motionProcessingType = 1;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount = new int[statLength];


        // Constructor
        public MainForm( )
        {
            InitializeComponent( );
        }

        // Application's main form is closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            CloseVideoSource( );
        }

        // "Exit" menu item clicked
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        // "About" menu item clicked
        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );
            form.ShowDialog( );
        }

        // "Open" menu item clieck - open AVI file
        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                // create video source
                AVIFileVideoSource fileSource = new AVIFileVideoSource( openFileDialog.FileName );

                OpenVideoSource( fileSource );
            }
        }

        // Open JPEG URL
        private void openJPEGURLToolStripMenuItem_Click( object sender, EventArgs e )
        {
            URLForm form = new URLForm( );

            form.Description = "Enter URL of an updating JPEG from a web camera:";
            form.URLs = new string[]
				{
					"http://61.220.38.10/axis-cgi/jpg/image.cgi?camera=1",
					"http://212.98.46.120/axis-cgi/jpg/image.cgi?resolution=352x240",
					"http://webcam.mmhk.cz/axis-cgi/jpg/image.cgi?resolution=320x240",
					"http://195.243.185.195/axis-cgi/jpg/image.cgi?camera=1"
				};

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                JPEGStream jpegSource = new JPEGStream( form.URL );

                // open it
                OpenVideoSource( jpegSource );
            }
        }

        // Open MJPEG URL
        private void openMJPEGURLToolStripMenuItem_Click( object sender, EventArgs e )
        {
            URLForm form = new URLForm( );

            form.Description = "Enter URL of an MJPEG video stream:";
            form.URLs = new string[]
				{
					"http://129.186.47.239/axis-cgi/mjpg/video.cgi?resolution=352x240",
					"http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=3",
					"http://195.243.185.195/axis-cgi/mjpg/video.cgi?camera=4",
                    "http://chipmunk.uvm.edu/cgi-bin/webcam/nph-update.cgi?dummy=garb"
				};

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                MJPEGStream mjpegSource = new MJPEGStream( form.URL );

                // open it
                OpenVideoSource( mjpegSource );
            }
        }

        // Open local video capture device
        private void localVideoCaptureDeviceToolStripMenuItem_Click( object sender, EventArgs e )
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm( );

            if ( form.ShowDialog( this ) == DialogResult.OK )
            {
                // create video source
                VideoCaptureDevice videoSource = new VideoCaptureDevice( form.VideoDevice );

                // open it
                OpenVideoSource( videoSource );
            }
        }

        // Open video file using DirectShow
        private void openVideoFileusingDirectShowToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                // create video source
                FileVideoSource fileSource = new FileVideoSource( openFileDialog.FileName );

                // open it
                OpenVideoSource( fileSource );
            }

        }

        // Open video source
        private void OpenVideoSource( IVideoSource source )
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // close previous video source
            CloseVideoSource( );

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start( );

            // reset statistics
            statIndex = statReady = 0;

            // start timer
            timer.Start( );

            videoSource = source;

            this.Cursor = Cursors.Default;
        }

        // Close current video source
        private void CloseVideoSource( )
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            videoSourcePlayer.SignalToStop( );

            // wait 2 seconds until camera stops
            for ( int i = 0; ( i < 50 ) && ( videoSourcePlayer.IsRunning ); i++ )
            {
                Thread.Sleep( 100 );
            }
            if ( videoSourcePlayer.IsRunning )
                videoSourcePlayer.Stop( );

            // stop timer
            timer.Stop( );

            // reset motion detector
            if ( detector != null )
                detector.Reset( );

            this.Cursor = Cursors.Default;
        }

        // New frame received by the player
        private void videoSourcePlayer_NewFrame( object sender, ref Bitmap image )
        {
            lock ( this )
            {
                if ( detector != null )
                {
                    double motionLevel = detector.ProcessFrame( image );
                }
            }
        }

        // On timer event - gather statistics
        private void timer_Tick( object sender, EventArgs e )
        {
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if ( videoSource != null )
            {
                // get number of frames for the last second
                statCount[statIndex] = videoSource.FramesReceived;

                // increment indexes
                if ( ++statIndex >= statLength )
                    statIndex = 0;
                if ( statReady < statLength )
                    statReady++;

                float fps = 0;

                // calculate average value
                for ( int i = 0; i < statReady; i++ )
                {
                    fps += statCount[i];
                }
                fps /= statReady;

                statCount[statIndex] = 0;

                fpsLabel.Text = fps.ToString( "F2" ) + " fps";
            }
        }

        // Turn off motion detection
        private void noneToolStripMenuItem1_Click( object sender, EventArgs e )
        {
            motionDetectionType = 0;
            SetMotionDetectionAlgorithm( null );
        }

        // Set Two Frames Difference motion detection algorithm
        private void twoFramesDifferenceToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionDetectionType = 1;
            SetMotionDetectionAlgorithm( new TwoFramesDifferenceDetector( ) );
        }

        // Set Simple Background Modeling motion detection algorithm
        private void simpleBackgroundModelingToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionDetectionType = 2;
            SetMotionDetectionAlgorithm( new SimpleBackgroundModelingDetector( true, true ) );
        }

        // Turn off motion prcessing
        private void noneToolStripMenuItem2_Click( object sender, EventArgs e )
        {
            motionProcessingType = 0;
            SetMotionProcessingAlgorithm( null );
        }

        // Set motion area highlighting
        private void motionAreaHighlightingToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionProcessingType = 1;
            SetMotionProcessingAlgorithm( new MotionAreaHighlighting( ) );
        }

        // Set motion borders highlighting
        private void motionBorderHighlightingToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionProcessingType = 2;
            SetMotionProcessingAlgorithm( new MotionBorderHighlighting( ) );
        }

        // Set objects' counter
        private void blobCountingToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionProcessingType = 3;
            SetMotionProcessingAlgorithm( new BlobCountingObjectsProcessing( ) );
        }

        // Set grid motion processing
        private void gridMotionAreaProcessingToolStripMenuItem_Click( object sender, EventArgs e )
        {
            motionProcessingType = 4;
            SetMotionProcessingAlgorithm( new GridMotionAreaProcessing( 32, 32 ) );
        }

        // Set new motion detection algorithm
        private void SetMotionDetectionAlgorithm( IMotionDetector detectionAlgorithm )
        {
            lock ( this )
            {
                detector.MotionDetectionAlgorthm = detectionAlgorithm;
            }
        }

        // Set new motion processing algorithm
        private void SetMotionProcessingAlgorithm( IMotionProcessing processingAlgorithm )
        {
            lock ( this )
            {
                detector.MotionProcessingAlgorithm = processingAlgorithm;
            }
        }

        // Motion menu is opening
        private void motionToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            ToolStripMenuItem[] motionDetectionItems = new ToolStripMenuItem[]
            {
                noneToolStripMenuItem1, twoFramesDifferenceToolStripMenuItem,
                simpleBackgroundModelingToolStripMenuItem
            };
            ToolStripMenuItem[] motionProcessingItems = new ToolStripMenuItem[]
            {
                noneToolStripMenuItem2, motionAreaHighlightingToolStripMenuItem,
                motionBorderHighlightingToolStripMenuItem, blobCountingToolStripMenuItem,
                gridMotionAreaProcessingToolStripMenuItem
            };

            for ( int i = 0; i < motionDetectionItems.Length; i++ )
            {
                motionDetectionItems[i].Checked = ( i == motionDetectionType );
            }
            for ( int i = 0; i < motionProcessingItems.Length; i++ )
            {
                motionProcessingItems[i].Checked = ( i == motionProcessingType );
            }

            // enable/disable defining motion zones
//            defineMotionregionsToolStripMenuItem.Enabled =
//                ( ( cameraWindow.Camera != null ) && ( cameraWindow.Camera.LastFrame != null ) && ( detector is IZonesMotionDetector ) );
        }

        // On "Define motion regions" menu item selected
        private void defineMotionregionsToolStripMenuItem_Click( object sender, EventArgs e )
        {
/*            if ( ( cameraWindow.Camera != null ) && ( cameraWindow.Camera.LastFrame != null ) && ( detector is IZonesMotionDetector ) )
            {
                MotionRegionsForm form = new MotionRegionsForm( );
                IZonesMotionDetector zonesDetector = (IZonesMotionDetector) detector;

                // get last frame from camera
                cameraWindow.Camera.Lock( );
                form.VideoFrame = AForge.Imaging.Image.Clone( cameraWindow.Camera.LastFrame );
                cameraWindow.Camera.Unlock( );

                form.MotionRectangles = zonesDetector.MotionZones;

                // show the dialog
                if ( form.ShowDialog( this ) == DialogResult.OK )
                {
                    Rectangle[] rects = form.MotionRectangles;

                    if ( rects.Length == 0 )
                        rects = null;

                    zonesDetector.MotionZones = rects;
                }
            }*/
        }

        // On opening of Tools menu
        private void toolsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            localVideoCaptureSettingsToolStripMenuItem.Enabled =
                ( ( videoSource != null ) && ( videoSource is VideoCaptureDevice ) );
        }

        // Display properties of local capture device
        private void localVideoCaptureSettingsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( ( videoSource != null ) && ( videoSource is VideoCaptureDevice ) )
            {
                ( (VideoCaptureDevice) videoSource ).DisplayPropertyPage( this.Handle );
            }
        }
    }
}