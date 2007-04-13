// AForge.NET Framework
// Motion Detection sample application
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MotionDetector
{
    public partial class CameraWindow : System.Windows.Forms.Control
    {
        private Camera camera = null;
        private bool autosize = false;
        private bool needSizeUpdate = false;
        private bool firstFrame = true;

        // timer used for flashing
        private int flash = 0;

        // rectangle's color
        private Color rectColor = Color.Black;

        // Auto size value
        [DefaultValue( false )]
        public bool AutoSize
        {
            get { return autosize; }
            set
            {
                autosize = value;
                UpdatePosition( );
            }
        }

        // Asociated camera
        [Browsable( false )]
        public Camera Camera
        {
            get { return camera; }
            set
            {
                // lock
                Monitor.Enter( this );

                // detach event
                if ( camera != null )
                {
                    camera.NewFrame -= new EventHandler( camera_NewFrame );
                    camera.Alarm -= new EventHandler( camera_Alarm );
                    camera.VideoSourceError -= new EventHandler( camera_VideoSourceError );
                    timer.Stop( );
                }

                camera = value;
                needSizeUpdate = true;
                firstFrame = true;
                flash = 0;

                // atach event
                if ( camera != null )
                {
                    camera.NewFrame += new EventHandler( camera_NewFrame );
                    camera.Alarm += new EventHandler( camera_Alarm );
                    camera.VideoSourceError += new EventHandler( camera_VideoSourceError );
                    timer.Start( );
                }

                // unlock
                Monitor.Exit( this );

                // repaint the control
                Invalidate( );
            }
        }

        // Constructir
        public CameraWindow( )
        {
            InitializeComponent( );

            // update style of the control
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true );
        }

		// Paint the control
        protected override void OnPaint( PaintEventArgs pe )
        {
            if ( ( needSizeUpdate ) || ( firstFrame ) )
            {
                UpdatePosition( );
                needSizeUpdate = false;
            }

            // lock
            Monitor.Enter( this );

            Graphics g = pe.Graphics;
            Rectangle rc = this.ClientRectangle;
            Pen pen = new Pen( rectColor, 1 );

            // draw rectangle
            g.DrawRectangle( pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1 );

            if ( camera != null )
            {
                try
                {
                    camera.Lock( );

                    // draw frame
                    if ( ( camera.LastFrame != null ) && ( camera.LastVideoSourceError == null ) )
                    {
                        g.DrawImage( camera.LastFrame, rc.X + 1, rc.Y + 1, rc.Width - 2, rc.Height - 2 );
                        firstFrame = false;
                    }
                    else
                    {
                        // Create font and brush
                        Font drawFont = new Font( "Arial", 12 );
                        SolidBrush drawBrush = new SolidBrush( Color.White );

                        g.DrawString( ( camera.LastVideoSourceError == null ) ?
                            "Connecting ..." : camera.LastVideoSourceError,
                            drawFont, drawBrush, new PointF( 5, 5 ) );

                        drawBrush.Dispose( );
                        drawFont.Dispose( );
                    }
                }
                catch ( Exception )
                {
                }
                finally
                {
                    camera.Unlock( );
                }
            }

            pen.Dispose( );

            // unlock
            Monitor.Exit( this );

            base.OnPaint( pe );
        }

        // Update position and size of the control
        public void UpdatePosition( )
        {
            // lock
            Monitor.Enter( this );

            if ( ( autosize ) && ( this.Parent != null ) )
            {
                Rectangle rc = this.Parent.ClientRectangle;
                int width = 320;
                int height = 240;

                if ( camera != null )
                {
                    camera.Lock( );

                    // get frame dimension
                    if ( camera.LastFrame != null )
                    {
                        width = camera.LastFrame.Width;
                        height = camera.LastFrame.Height;
                    }
                    camera.Unlock( );
                }

                //
                this.SuspendLayout( );
                this.Location = new Point( ( rc.Width - width - 2 ) / 2, ( rc.Height - height - 2 ) / 2 );
                this.Size = new Size( width + 2, height + 2 );
                this.ResumeLayout( );
            }
            // unlock
            Monitor.Exit( this );
        }

        // On new frame ready
        private void camera_NewFrame( object sender, System.EventArgs e )
        {
            Invalidate( );
        }

        // On alarm
        private void camera_Alarm( object sender, System.EventArgs e )
        {
            // flash for 2 seconds
            flash = (int) ( 2 * ( 1000 / timer.Interval ) );
        }

        // On video source error
        private void camera_VideoSourceError( object sender, System.EventArgs e )
        {
            Invalidate( );
        }

        // On timer
        private void timer_Tick( object sender, EventArgs e )
        {
            if ( flash > 0 )
            {
                // calculate color
                if ( --flash == 0 )
                {
                    rectColor = Color.Black;
                }
                else
                {
                    rectColor = ( rectColor == Color.Red ) ? Color.Black : Color.Red;
                }

                // draw rectangle
                if ( !needSizeUpdate )
                {
                    Graphics g = this.CreateGraphics( );
                    Rectangle rc = this.ClientRectangle;
                    Pen pen = new Pen( rectColor, 1 );

                    // draw rectangle
                    g.DrawRectangle( pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1 );

                    g.Dispose( );
                    pen.Dispose( );
                }
            }
        }
    }
}
