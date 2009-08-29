// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//
// Copyright © Fabio L. Caversan, 2008
// fabio.caversan@gmail.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Class for representing a pair of coordinates of double type.
    /// </summary>
    /// 
    /// <remarks><para>This is a very simple class used to store a pair of double coordinates, 
    /// an alternative for the System.Drawing.Point class that stores only integers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // assigning coordinates on the constructor
    /// DoublePoint p1 = new DoublePoint( 10,20 );
    /// // creating a point and assigning coordinates later
    /// DoublePoint p2 = new DoublePoint( );
    /// p2.X = 30;
    /// p2.Y = 40;
    /// </code>
    /// </remarks>
    /// 
    public class DoublePoint
    {
        // (x,y) coordinates pair
        private double x, y;

        /// <summary> 
        /// X coordinate.
        /// </summary> 
        /// 
        /// <remarks>
        /// <para>Default value is set to <b>0.0</b>.</para> 
        /// </remarks> 
        /// 
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary> 
        /// Y coordinate.
        /// </summary> 
        /// 
        /// <remarks>
        /// <para>Default value is set to <b>0.0</b>.</para> 
        /// </remarks> 
        /// 
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoublePoint"/> class.
        /// </summary>
        /// 
        public DoublePoint( )
        {
            this.x = 0;
            this.y = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoublePoint"/> class.
        /// </summary>
        /// 
        /// <param name="x">X axis coordinate.</param>
        /// <param name="y">Y axis coordinate.</param>
        /// 
        public DoublePoint( double x, double y )
        {
            this.x = x;
            this.y = y;
        }
    }
}
