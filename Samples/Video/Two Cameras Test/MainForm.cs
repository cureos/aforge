// Two Cameras Test sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.DirectShow;

namespace TwoCamerasTest
{
    public partial class MainForm : Form
    {
        // list of video devices
        FilterInfoCollection videoDevices;

        // statistics length
        private const int statLength = 15;
        // current statistics index
        private int statIndex = 0;
        // ready statistics values
        private int statReady = 0;
        // statistics array
        private int[] statCount1 = new int[statLength];
        private int[] statCount2 = new int[statLength];


        public MainForm( )
        {
            InitializeComponent( );

            camera1FpsLabel.Text = string.Empty;
            camera2FpsLabel.Text = string.Empty;

            // show device list
			try
			{
                // enumerate video devices
                videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );

                if ( videoDevices.Count == 0 )
                {
                    throw new Exception( );
                }

                for ( int i = 1, n = videoDevices.Count; i <= n; i++ )
                {
                    string cameraName = i + " : " + videoDevices[i - 1].Name;

                    camera1Combo.Items.Add( cameraName );
                    camera2Combo.Items.Add( cameraName );
                }

                // check cameras count
                if ( videoDevices.Count == 1 )
                {
                    camera2Combo.Items.Clear( );

                    camera2Combo.Items.Add( "Only one camera found" );
                    camera2Combo.SelectedIndex = 0;
                    camera2Combo.Enabled = false;
                }
                else
                {
                    camera2Combo.SelectedIndex = 1;
                }
                camera1Combo.SelectedIndex = 0;
            }
            catch
            {
                startButton.Enabled = false;

                camera1Combo.Items.Add( "No cameras found" );
                camera2Combo.Items.Add( "No cameras found" );

                camera1Combo.SelectedIndex = 0;
                camera2Combo.SelectedIndex = 0;

                camera1Combo.Enabled = false;
                camera2Combo.Enabled = false;
            }
        }

        // On form closing
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCameras( );
        }

        // On "Start" button click
        private void startButton_Click( object sender, EventArgs e )
        {
            StartCameras( );

            startButton.Enabled = false;
            stopButton.Enabled = true;
        }

        // On "Stop" button click
        private void stopButton_Click( object sender, EventArgs e )
        {
            StopCameras( );

            startButton.Enabled = true;
            stopButton.Enabled = false;

            camera1FpsLabel.Text = string.Empty;
            camera2FpsLabel.Text = string.Empty;
        }

        // Start cameras
        private void StartCameras( )
        {
            // create first video source
            VideoCaptureDevice videoSource1 = new VideoCaptureDevice( videoDevices[camera1Combo.SelectedIndex].MonikerString );
            videoSource1.DesiredFrameRate = 10;

            videoSourcePlayer1.VideoSource = videoSource1;
            videoSourcePlayer1.Start( );

            // create second video source
            if ( camera2Combo.Enabled == true )
            {
                System.Threading.Thread.Sleep( 500 );

                VideoCaptureDevice videoSource2 = new VideoCaptureDevice( videoDevices[camera2Combo.SelectedIndex].MonikerString );
                videoSource2.DesiredFrameRate = 10;

                videoSourcePlayer2.VideoSource = videoSource2;
                videoSourcePlayer2.Start( );
            }

            // start timer
            timer.Start( );
        }

        // Stop cameras
        private void StopCameras( )
        {
            timer.Stop( );

            videoSourcePlayer1.SignalToStop( );
            videoSourcePlayer2.SignalToStop( );

            videoSourcePlayer1.WaitForStop( );
            videoSourcePlayer2.WaitForStop( );
        }

        // On times tick - collect statistics
        private void timer_Tick( object sender, EventArgs e )
        {
            IVideoSource videoSource1 = videoSourcePlayer1.VideoSource;
            IVideoSource videoSource2 = videoSourcePlayer2.VideoSource;

            // get number of frames for the last second
            if ( videoSource1 != null )
            {
                statCount1[statIndex] = videoSource1.FramesReceived;
            }

            if ( videoSource2 != null )
            {
                statCount2[statIndex] = videoSource2.FramesReceived;
            }

            // increment indexes
            if ( ++statIndex >= statLength )
                statIndex = 0;
            if ( statReady < statLength )
                statReady++;

            float fps1 = 0;
            float fps2 = 0;

            // calculate average value
            for ( int i = 0; i < statReady; i++ )
            {
                fps1 += statCount1[i];
                fps2 += statCount2[i];
            }
            fps1 /= statReady;
            fps2 /= statReady;

            statCount1[statIndex] = 0;
            statCount2[statIndex] = 0;

            camera1FpsLabel.Text = fps1.ToString( "F2" ) + " fps";
            camera2FpsLabel.Text = fps2.ToString( "F2" ) + " fps";
        }
    }
}
