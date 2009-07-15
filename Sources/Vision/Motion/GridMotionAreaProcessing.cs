// AForge Vision Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge.Vision.Motion
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    using AForge.Imaging;

    public class GridMotionAreaProcessing : IMotionProcessing
    {
        private Color highlightColor = Color.Red;

        private int gridWidth  = 16;
        private int gridHeight = 16;

        /// <summary>
        /// Color used to highlight motion regions.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>Default value is set to <b>red</b> color.</para>
        /// </remarks>
        /// 
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        public GridMotionAreaProcessing( )
        {
        }

        public GridMotionAreaProcessing( int gridWidth, int gridHeight )
        {
            this.gridWidth  = gridWidth;
            this.gridHeight = gridHeight;
        }


        public unsafe void ProcessFrame( UnmanagedImage videoFrame, UnmanagedImage motionFrame )
        {
            int width  = videoFrame.Width;
            int height = videoFrame.Height;

            if ( ( motionFrame.Width != width ) || ( motionFrame.Height != height ) )
                return;

            int cellWidth  = width  / gridWidth;
            int cellHeight = height / gridHeight;

            byte* motion = (byte*) motionFrame.ImageData.ToPointer( );

            int motionOffset = motionFrame.Stride - width;

            double[,] motionAmout = new double[cellHeight, cellWidth];

            for ( int y = 0; y < height; y++ )
            {
                int yCell = y / cellHeight;
                if ( yCell == gridHeight )
                    yCell--;

                for ( int x = 0; x < width; x++, motion++ )
                {
                    int xCell = x / cellWidth;
                    if ( xCell == gridWidth )
                        xCell--;

                    if ( *motion != 0 )
                    {
                        motionAmout[yCell, xCell]++;
                    }
                }
                motion += motionOffset;
            }

            for ( int y = 0; y < gridHeight; y++ )
            {
                int ch = ( y != gridHeight - 1 ) ? cellHeight : ( height - cellHeight * y );

                for ( int x = 0; x < gridWidth; x++ )
                {
                    int cw = ( x != gridWidth - 1 ) ? cellWidth : ( width - cellWidth * x );

                    motionAmout[y, x] /= ( cw * ch );
                }
            }

            byte* src = (byte*) videoFrame.ImageData.ToPointer( );
            int srcOffset = videoFrame.Stride - width * 3;

            byte fillR = highlightColor.R;
            byte fillG = highlightColor.G;
            byte fillB = highlightColor.B;

            for ( int y = 0; y < height; y++ )
            {
                int yCell = y / cellHeight;
                if ( yCell == gridHeight )
                    yCell--;

                for ( int x = 0; x < width; x++, motion++, src += 3 )
                {
                    int xCell = x / cellWidth;
                    if ( xCell == gridWidth )
                        xCell--;


                    if ( ( motionAmout[yCell, xCell] > 0.15 ) && ( ( ( x + y ) & 1 ) == 0 ) )
                    {
                        src[RGB.R] = fillR;
                        src[RGB.G] = fillG;
                        src[RGB.B] = fillB;
                    }
                }
                src += srcOffset;
                motion += motionOffset;
            }

        }

        /// <summary>
        /// Reset internal state of motion processing algorithm.
        /// </summary>
        /// 
        /// <remarks><para>The method allows to reset internal state of motion processing
        /// algorithm and prepare it for processing of next video stream or to restart
        /// the algorithm.</para></remarks>
        ///
        public void Reset( )
        {
        }
    }
}
