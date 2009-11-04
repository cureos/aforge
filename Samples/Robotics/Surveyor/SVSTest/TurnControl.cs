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
    // Turn Control
    public partial class TurnControl : Control
    {
        // pens and brushes for drawing
        private Pen borderPen = new Pen( Color.Black );
        private Brush rightTurnAreaBrush = new SolidBrush( Color.White );
        private Brush leftTurnAreaBrush = new SolidBrush( Color.LightGray );

        private Brush manipulatorBrush = new SolidBrush( Color.BlueViolet );

        private Brush disabledBrash = new SolidBrush( Color.FromArgb( 240, 240, 240 ) );
        
        // manipulator's size
        private const int manipulatorWidth = 11;
        private const int manipulatorHeight = 20;

        // manipulator's position
        private int manipulatatorPosition = 0;

        // tracking information
        private bool tracking = false;
        private int trackingStartX = 0;

        // number of timer ticks before notifying user (-1 means no notification)
        private int ticksBeforeNotificiation = -1;

        // delegate and event which are used for client notifications
        public delegate void PositionChangedHandler( float x );
        public event PositionChangedHandler PositionChanged;

        // Constructor
        public TurnControl( )
        {
            InitializeComponent( );

            // update control style
            SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
        }

        // Paint the control
        private void TurnControl_Paint( object sender, PaintEventArgs e )
        {
            Graphics g = e.Graphics;

            int clientWidth = ClientRectangle.Width;

            // calculate area margins
            int leftMargin = manipulatorWidth / 2;
            int topMargin = manipulatorHeight / 4;

            // draw area
            g.FillRectangle( ( this.Enabled ) ? leftTurnAreaBrush : disabledBrash, leftMargin, topMargin,
               clientWidth / 2 - leftMargin, manipulatorHeight / 2 );
            g.FillRectangle( ( this.Enabled ) ? rightTurnAreaBrush : disabledBrash, clientWidth / 2, topMargin,
               clientWidth / 2 - leftMargin, manipulatorHeight / 2 );
            g.DrawRectangle( borderPen, leftMargin, topMargin,
               clientWidth - 1 - leftMargin * 2, manipulatorHeight / 2 );

            // calculate manipulator's center point
            int ctrlManipulatorX = manipulatatorPosition + clientWidth / 2;

            // draw manipulator
            g.FillRectangle( ( this.Enabled ) ? manipulatorBrush : disabledBrash, ctrlManipulatorX - manipulatorWidth / 2, 0,
                manipulatorWidth, manipulatorHeight );
            g.DrawRectangle( borderPen, ctrlManipulatorX - manipulatorWidth / 2, 0,
                manipulatorWidth, manipulatorHeight );
        }

        // On mouse down event
        private void TurnControl_MouseDown( object sender, MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                // calculate area margins
                int leftMargin = manipulatorWidth / 2;
                int topMargin = manipulatorHeight / 4;

                if (
                    ( e.X >= leftMargin ) &&
                    ( e.X < ClientRectangle.Width - leftMargin ) &&
                    ( e.Y >= topMargin ) &&
                    ( e.Y < ClientRectangle.Height - topMargin ) )
                {
                    manipulatatorPosition = e.X - ClientRectangle.Width / 2;

                    tracking = true;
                    trackingStartX = ClientRectangle.Width / 2;

                    this.Capture = true;

                    // start time, which is used to notify
                    // about manipulator's position change
                    ticksBeforeNotificiation = -1;
                    timer.Start( );
                }
            }
        }

        // On mouse up event
        private void TurnControl_MouseUp( object sender, MouseEventArgs e )
        {
            if ( ( e.Button == MouseButtons.Left ) && ( tracking ) )
            {
                tracking = false;
                this.Capture = false;

                manipulatatorPosition = 0;

                Invalidate( );
                timer.Stop( );

                if ( PositionChanged != null )
                {
                    PositionChanged( 0 );
                }
            }
        }

        // On mouse move event
        private void TurnControl_MouseMove( object sender, MouseEventArgs e )
        {
            if ( tracking )
            {
                manipulatatorPosition = e.X - trackingStartX;

                int maxPosition = ( ClientRectangle.Width - manipulatorWidth ) / 2;

                manipulatatorPosition = Math.Max( Math.Min( manipulatatorPosition, maxPosition ), -maxPosition );

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
                        PositionChanged( Math.Min( 1.0f, (float) manipulatatorPosition / ( ( ClientRectangle.Width - manipulatorWidth ) / 2 ) ) );
                    }
                }

                ticksBeforeNotificiation--;
            }
        }
    }
}
