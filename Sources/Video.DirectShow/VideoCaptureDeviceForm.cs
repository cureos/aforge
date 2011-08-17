// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AForge.Video.DirectShow;

namespace AForge.Video.DirectShow
{
    /// <summary>
    /// Local video device selection form.
    /// </summary>
    /// 
    /// <remarks><para>The form provides a standard way of selecting local video
    /// device (USB web camera, capture board, etc. - anything supporting DirectShow
    /// interface), which can be reused across applications. It allows selecting video
    /// device, video size and snapshots size (if device supports snapshots and
    /// <see cref="ConfigureSnapshots">user needs them</see>).</para>
    /// </remarks>
    /// 
    public partial class VideoCaptureDeviceForm : Form
    {
        // collection of available video devices
        private FilterInfoCollection videoDevices;
        // selected video device
        private VideoCaptureDevice videoDevice;

        // supported capabilities of video and snapshots
        private VideoCapabilities[] videoCapabilities;
        private VideoCapabilities[] snapshotCapabilities;

        // flag telling if user wants to configure snapshots as well
        private bool configureSnapshots = false;

        /// <summary>
        /// Specifies if snapshot configuration should be done or not.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies if the dialog form should
        /// allow configuration of snapshot sizes (if selected video source supports
        /// snapshots). If the property is set to <see langword="true"/>, then
        /// the form will provide additional combo box enumerating supported
        /// snapshot sizes. Otherwise the combo boxes will be hidden.
        /// </para>
        /// 
        /// <para>If the property is set to <see langword="true"/> and selected
        /// device supports snapshots, then <see cref="VideoCaptureDevice.ProvideSnapshots"/>
        /// property of the <see cref="VideoDevice">configured device</see> is set to
        /// <see langword="true"/>.</para>
        /// 
        /// <para>Default value of the property is set to <see langword="false"/>.</para>
        /// </remarks>
        /// 
        public bool ConfigureSnapshots
        {
            get { return configureSnapshots; }
            set
            {
                configureSnapshots = value;
                snapshotsLabel.Visible = value;
                snapshotResolutionsCombo.Visible = value;
            }
        }

        /// <summary>
        /// Provides configured video device.
        /// </summary>
        /// 
        /// <remarks><para>The property provides configured video device if user confirmed
        /// the dialog using "OK" button. If user canceled the dialog, the property is
        /// set to <see langword="null"/>.</para></remarks>
        /// 
        public VideoCaptureDevice VideoDevice
        {
            get { return videoDevice; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoCaptureDeviceForm"/> class.
        /// </summary>
        /// 
        public VideoCaptureDeviceForm( )
        {
            InitializeComponent( );
            ConfigureSnapshots = false;

            // show device list
			try
			{
                // enumerate video devices
                videoDevices = new FilterInfoCollection( FilterCategory.VideoInputDevice );

                if ( videoDevices.Count == 0 )
                    throw new ApplicationException( );

                // add all devices to combo
                foreach ( FilterInfo device in videoDevices )
                {
                    devicesCombo.Items.Add( device.Name );
                }
            }
            catch ( ApplicationException )
            {
                devicesCombo.Items.Add( "No local capture devices" );
                devicesCombo.Enabled = false;
                okButton.Enabled = false;
            }
        }

        // On form loaded
        private void VideoCaptureDeviceForm_Load( object sender, EventArgs e )
        {
            devicesCombo.SelectedIndex = 0;
            videoDevice = null;
        }

        // Ok button clicked
        private void okButton_Click( object sender, EventArgs e )
        {
            // set video size
            if ( videoCapabilities.Length != 0 )
            {
                int selectedSize = videoResolutionsCombo.SelectedIndex;

                videoDevice.DesiredFrameSize = videoCapabilities[selectedSize].FrameSize;
                videoDevice.DesiredFrameRate = videoCapabilities[selectedSize].FrameRate;
            }

            if ( configureSnapshots )
            {
                // set snapshots size
                if ( snapshotCapabilities.Length != 0 )
                {
                    int selectedSize = snapshotResolutionsCombo.SelectedIndex;

                    videoDevice.ProvideSnapshots = true;
                    videoDevice.DesiredSnapshotSize = snapshotCapabilities[selectedSize].FrameSize;
                }
            }
        }

        // New video device is selected
        private void devicesCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( videoDevices.Count != 0 )
            {
                videoDevice = new VideoCaptureDevice( videoDevices[devicesCombo.SelectedIndex].MonikerString );
                EnumeratedSupportedFrameSizes( videoDevice );
            }
        }

        // Collect supported video and snapshot sizes
        private void EnumeratedSupportedFrameSizes( VideoCaptureDevice videoDevice )
        {
            this.Cursor = Cursors.WaitCursor;

            videoResolutionsCombo.Items.Clear( );
            snapshotResolutionsCombo.Items.Clear( );

            try
            {
                // collect video capabilities
                videoCapabilities = videoDevice.VideoCapabilities;

                foreach ( VideoCapabilities capabilty in videoCapabilities )
                {
                    string item = string.Format(
                        "{0} x {1}", capabilty.FrameSize.Width, capabilty.FrameSize.Height );

                    if ( !videoResolutionsCombo.Items.Contains( item ) )
                    {
                        videoResolutionsCombo.Items.Add( item );
                    }
                }

                if ( videoCapabilities.Length == 0 )
                {
                    videoResolutionsCombo.Items.Add( "Not supported" );
                }

                videoResolutionsCombo.SelectedIndex = 0;

                if ( configureSnapshots )
                {
                    // collect snapshot capabilities
                    snapshotCapabilities = videoDevice.SnapshotCapabilities;

                    foreach ( VideoCapabilities capabilty in snapshotCapabilities )
                    {
                        string item = string.Format(
                            "{0} x {1}", capabilty.FrameSize.Width, capabilty.FrameSize.Height );

                        if ( !snapshotResolutionsCombo.Items.Contains( item ) )
                        {
                            snapshotResolutionsCombo.Items.Add( item );
                        }
                    }

                    if ( snapshotCapabilities.Length == 0 )
                    {
                        snapshotResolutionsCombo.Items.Add( "Not supported" );
                    }

                    snapshotResolutionsCombo.SelectedIndex = 0;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
