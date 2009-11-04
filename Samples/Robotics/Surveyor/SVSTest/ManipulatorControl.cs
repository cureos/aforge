// Surveyor SVS test application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace QwerkRobotCar
{
    // Manipulator Control
    public partial class ManipulatorControl : Control
    {
        // pens and brushes for drawing
        private Pen borderPen = new Pen( Color.Black );
        private Brush forwardAreaBrush = new SolidBrush( Color.White );
        private Brush backwardAreaBrush = new SolidBrush( Color.LightGray );

        private Brush manipulatorBrush = new SolidBrush( Color.BlueViolet );
        private Brush manipulatorNoseBrush = new SolidBrush( Color.LightGreen );

        private Brush disabledBrash = new SolidBrush( Color.FromArgb( 240, 240, 240 ) );

        // manipulator's size
        private const int manipulatorSize = 21;
        private const int manipulatorRadius = manipulatorSize / 2;

        // manipulator's position
        private int manipulatatorX = 0;
        private int manipulatatorY = 0;

        // size (width and height) of manipulator's area
        private int areaSize = 0;
        private int areaRadius = 0;
        private int areaMargin = manipulatorSize / 2;

        // tracking information
        private bool tracking = false;
        private int trackingStartX = 0;
        private int trackingStartY = 0;

        // number of timer ticks before notifying user (-1 means no notification)
        private int ticksBeforeNotificiation = -1;

        // delegate and event which are used for client notifications
        public delegate void PositionChangedHandler( float x, float y );
        public event PositionChangedHandler PositionChanged;

        // Constructor
        public ManipulatorControl( )
        {
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

        // Paint the control
        private void ManipulatorControl_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            // calculate size of the manipulator's ares
            areaSize   = Math.Min( ClientRectangle.Width, ClientRectangle.Height ) - areaMargin * 2;
            areaRadius = areaSize / 2;

            // draw area
            g.FillPie( ( this.Enabled ) ? forwardAreaBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, -180 );
            g.FillPie( ( this.Enabled ) ? backwardAreaBrush : disabledBrash, areaMargin, areaMargin, areaSize - 1, areaSize - 1, 0, 180 );
            g.DrawEllipse( borderPen, areaMargin, areaMargin, areaSize - 1, areaSize - 1 );
            g.DrawLine( borderPen, areaMargin, areaMargin + areaRadius, areaMargin + areaSize - 1, areaMargin + areaRadius );

            // calculate manipulator's center point
            int ctrlManipulatorX = manipulatatorX + areaMargin + areaRadius;
            int ctrlManipulatorY = manipulatatorY + areaMargin + areaRadius;

            // draw manipulator
            g.FillEllipse( ( this.Enabled ) ? manipulatorBrush : disabledBrash,
                ctrlManipulatorX - manipulatorRadius, ctrlManipulatorY - manipulatorRadius,
                manipulatorSize, manipulatorSize );
            g.FillPie( ( this.Enabled ) ? manipulatorNoseBrush : disabledBrash,
                ctrlManipulatorX - manipulatorRadius, ctrlManipulatorY - manipulatorRadius,
                manipulatorSize, manipulatorSize, -45, -90 );
            g.DrawEllipse( borderPen,
                ctrlManipulatorX - manipulatorRadius, ctrlManipulatorY - manipulatorRadius,
                manipulatorSize, manipulatorSize );
        }

        // On mouse down event
        private void ManipulatorControl_MouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                // get click point relatively to manipulation area's center
                int cX = e.X - areaMargin - areaRadius;
                int cY = e.Y - areaMargin - areaRadius;

                // check if the point is inside of manipulator
                if ( Math.Sqrt( cX * cX + cY * cY ) <= areaRadius )
                {
                    manipulatatorX = cX;
                    manipulatatorY = cY;

                    tracking = true;
                    trackingStartX = 0;
                    trackingStartY = 0;

                    this.Capture = true;

                    // start timer, which is used to notify
                    // about manipulator's position change
                    ticksBeforeNotificiation = -1;
                    timer.Start( );
                }
            }
        }

        // On mouse up event
        private void ManipulatorControl_MouseUp( object sender, MouseEventArgs e )
        {
            if ( ( e.Button == MouseButtons.Left ) && ( tracking ) )
            {
                tracking = false;
                this.Capture = false;

                manipulatatorX = 0;
                manipulatatorY = 0;

                Invalidate( );
                timer.Stop( );

                if ( PositionChanged != null )
                {
                    PositionChanged( 0, 0 );
                }
            }
        }

        // On mouse move event
        private void ManipulatorControl_MouseMove( object sender, MouseEventArgs e )
        {
            if ( tracking )
            {
                // get mouse point relatively to manipulation area's center
                int cX = e.X - areaMargin - areaRadius;
                int cY = e.Y - areaMargin - areaRadius;

                manipulatatorX = cX - trackingStartX;
                manipulatatorY = cY - trackingStartY;

                // get distance from center
                int cR = (int) Math.Sqrt( manipulatatorX * manipulatatorX + manipulatatorY * manipulatatorY );

                // correct point if it is too far away
                if ( cR > areaRadius )
                {
                    double coef = (double) areaRadius / cR;
                    manipulatatorX = (int) ( coef * manipulatatorX );
                    manipulatatorY = (int) ( coef * manipulatatorY );
                }
                
                Invalidate( );

                // notify user after 10 timer ticks
                ticksBeforeNotificiation = 10;
            }
        }

        // Timer handler, which is used to notify clients about manipulator's changes
        private void timer_Tick( object sender, EventArgs e )
        {
            if ( ticksBeforeNotificiation != -1 )
            {
                // time to notify
                if ( ticksBeforeNotificiation == 0 )
                {
                    // notify users
                    if ( PositionChanged != null )
                    {
                        PositionChanged( (float) manipulatatorX / areaRadius, (float) -manipulatatorY / areaRadius );
                    }
                }

                ticksBeforeNotificiation--;
            }
        }
    }
}
