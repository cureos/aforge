// AForge Framework
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using AForge;

namespace TSP
{
	/// <summary>
	/// Summary description for MapControl.
	/// </summary>
	public class MapControl : System.Windows.Forms.Control
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// pens and brushes
		private Pen			blackPen = new Pen( Color.Black );
		private Brush		whiteBrush = new SolidBrush( Color.White );
		// map ranges
		private IntRange	rangeX = new IntRange( 0, 1000 );
		private IntRange	rangeY = new IntRange( 0, 1000 );
		// map
		private int[,]		map = null;
		// path
		private ushort[]	path = null;

		/// <summary>
		/// X range
		/// </summary>
		public IntRange RangeX
		{
			get { return rangeX; }
			set
			{
				if ( value != null )
				{
					rangeX = value;
					Invalidate( );
				}
			}
		}

		/// <summary>
		/// Y range
		/// </summary>
		public IntRange RangeY
		{
			get { return rangeY; }
			set
			{
				if ( value != null )
				{
					rangeY = value;
					Invalidate( );
				}
			}
		}

		/// <summary>
		/// Map
		/// </summary>
		public int[,] Map
		{
			get { return map; }
			set
			{
				map = value;
				Invalidate( );
			}
		}

		/// <summary>
		/// Path
		/// </summary>
		public ushort[] Path
		{
			get { return path; }
			set
			{
				path = value;
				Invalidate( );
			}
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public MapControl( )
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Update control style
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();

				// free graphics resources
				blackPen.Dispose( );
				whiteBrush.Dispose( );
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		// Paint the control
		protected override void OnPaint( PaintEventArgs pe )
		{
			Graphics	g = pe.Graphics;
			int			clientWidth = ClientRectangle.Width;
			int			clientHeight = ClientRectangle.Height;
			double		xFactor = (double)( clientWidth - 10 ) / ( rangeX.Length );
			double		yFactor = (double)( clientHeight - 10 ) / ( rangeY.Length );

			// fill with white background
			g.FillRectangle( whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1 );

			// draw a black rectangle
			g.DrawRectangle( blackPen, 0, 0, clientWidth - 1, clientHeight - 1 );

			// draw map
			if ( map != null )
			{
				Brush brush = new SolidBrush( Color.Red );

				// draw all points
				for ( int i = 0, n = map.GetLength( 0 ); i < n; i++ )
				{
					int x = (int) ( ( map[i, 0] - rangeX.Min ) * xFactor );
					int y = (int) ( ( map[i, 1] - rangeY.Min ) * yFactor );

					x += 5;
					y = clientHeight - 6 - y;

					g.FillRectangle( brush, x - 2, y - 2, 5, 5 );
				}

				brush.Dispose( );
			}
			// draw path
			if ( path != null )
			{
				Pen pen = new Pen( Color.Blue, 1 );
				int prev = path[path.Length - 1];
				int x1 = (int) ( ( map[prev, 0] - rangeX.Min ) * xFactor );
				int y1 = (int) ( ( map[prev, 1] - rangeY.Min ) * yFactor );

				x1 += 5;
				y1 = clientHeight - 6 - y1;

				// connect all cities
				for ( int i = 0, n = path.Length; i < n; i++ )
				{
					int curr = path[ i ];

					// calculate coordinates of the current city
					int x2 = (int) ( ( map[curr, 0] - rangeX.Min ) * xFactor );
					int y2 = (int) ( ( map[curr, 1] - rangeY.Min ) * yFactor );

					x2 += 5;
					y2 = clientHeight - 6 - y2;

					// connect previous city with the current one
					g.DrawLine( pen, x1, y1, x2, y2 );

					x1 = x2;
					y1 = y2;
				}
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
	}
}
