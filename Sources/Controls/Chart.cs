// AForge Controls Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Controls
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Windows.Forms;
	using AForge;

	/// <summary>
	/// Chart control
	/// </summary>
	/// 
	/// <remarks></remarks>
	/// 
	public class Chart : System.Windows.Forms.Control
	{
		/// <summary>
		/// Chart series type enumeration
		/// </summary>
		public enum SeriesType
		{
			/// <summary>
			/// Line style
			/// </summary>
			Line,
			/// <summary>
			/// Dots style
			/// </summary>
			Dots,
			/// <summary>
			/// Connected dots style
			/// </summary>
			ConnectedDots
		}

		// series data
		private class DataSeries
		{
			public double[,]	data = null;
			public Color		color = Color.Blue;
			public SeriesType	type = SeriesType.Line;
			public int			width = 1;
			public bool			updateYRange = true;
		}

		// data series table
		Hashtable		seriesTable = new Hashtable( );

		private Pen		blackPen = new Pen( Color.Black );
		private Brush	whiteBrush = new SolidBrush( Color.White );

		private DoubleRange	rangeX = new DoubleRange( 0, 1 );
		private DoubleRange	rangeY = new DoubleRange( 0, 1 );

		/// <summary>
		/// Chart's X range
		/// </summary>
		/// 
		/// <remarks>The value sets the X's range of data to be displayed on the chart</remarks>
		/// 
		public DoubleRange RangeX
		{
			get { return rangeX; }
			set
			{
				rangeX = value;
				Invalidate( );
			}
		}

		/// <summary>
		/// Chart's Y range
		/// </summary>
		/// 
		/// <remarks>The value sets the Y's range of data to be displayed on the chart</remarks>
		///
		public DoubleRange RangeY
		{
			get { return rangeY; }
			set
			{
				rangeY = value;
				Invalidate( );
			}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="Chart"/> class
		/// </summary>
		public Chart( )
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent( );

			// Update control style
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
				ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if ( components != null )
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

		/// <summary>
		/// Paint the control with all available data series
		/// </summary>
		/// 
		/// <param name="pe">Data for Paint event</param>
		/// 
		protected override void OnPaint( PaintEventArgs pe )
		{
			Graphics	g = pe.Graphics;
			int			clientWidth = ClientRectangle.Width;
			int			clientHeight = ClientRectangle.Height;

			// fill with white background
			g.FillRectangle( whiteBrush, 0, 0, clientWidth - 1, clientHeight - 1 );

			// draw a black rectangle
			g.DrawRectangle( blackPen, 0, 0, clientWidth - 1, clientHeight - 1 );

			// check if there are any series
			if ( rangeY != null )
			{
				double xFactor = (double)( clientWidth - 10 ) / ( rangeX.Length );
				double yFactor = (double)( clientHeight - 10 ) / ( rangeY.Length );

				// walk through all data series
				IDictionaryEnumerator en = seriesTable.GetEnumerator( );
				while ( en.MoveNext( ) )
				{
					DataSeries series = (DataSeries) en.Value;
					// get data of the series
					double[,] data = series.data;

					// check for available data
					if ( data == null )
						continue;

					// check series type
					if ( series.type == SeriesType.Dots )
					{
						// draw dots
						Brush	brush = new SolidBrush( series.color );
						int		width = series.width;
						int		r = width >> 1;

						// draw all points
						for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
						{
							int x = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
							int y = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

							x += 5;
							y = clientHeight - 6 - y;

							g.FillRectangle( brush, x - r, y - r, width, width );
						}
						brush.Dispose( );
					}
					else if ( series.type == SeriesType.ConnectedDots )
					{
						// draw dots connected with 1-pixel width line
						Brush	brush = new SolidBrush( series.color );
						Pen		pen = new Pen( series.color, 1 );
						int		width = series.width;
						int		r = width >> 1;

						int x1 = (int) ( ( data[0, 0] - rangeX.Min ) * xFactor );
						int y1 = (int) ( ( data[0, 1] - rangeY.Min ) * yFactor );

						x1 += 5;
						y1 = clientHeight - 6 - y1;
						g.FillRectangle( brush, x1 - r, y1 - r, width, width );

						// draw all lines
						for ( int i = 1, n = data.GetLength( 0 ); i < n; i++ )
						{
							int x2 = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
							int y2 = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

							x2 += 5;
							y2 = clientHeight - 6 - y2;
							g.FillRectangle( brush, x2 - r, y2 - r, width, width );

							g.DrawLine( pen, x1, y1, x2, y2 );

							x1 = x2;
							y1 = y2;
						}

						pen.Dispose( );
						brush.Dispose( );
					}
					else if ( series.type == SeriesType.Line )
					{
						// draw line
						Pen pen = new Pen( series.color, series.width );

						int x1 = (int) ( ( data[0, 0] - rangeX.Min ) * xFactor );
						int y1 = (int) ( ( data[0, 1] - rangeY.Min ) * yFactor );

						x1 += 5;
						y1 = clientHeight - 6 - y1;

						// draw all lines
						for ( int i = 1, n = data.GetLength( 0 ); i < n; i++ )
						{
							int x2 = (int) ( ( data[i, 0] - rangeX.Min ) * xFactor );
							int y2 = (int) ( ( data[i, 1] - rangeY.Min ) * yFactor );

							x2 += 5;
							y2 = clientHeight - 6 - y2;

							g.DrawLine( pen, x1, y1, x2, y2 );

							x1 = x2;
							y1 = y2;
						}
						pen.Dispose( );
					}
				}
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		/// <summary>
		/// Add data series to the chart
		/// </summary>
		/// 
		/// <param name="name">Data series name</param>
		/// <param name="color">Data series color</param>
		/// <param name="type">Data series type</param>
		/// <param name="width">Width (depends on the data series type, see remarks)</param>
		/// 
		/// <remarks>Adds new empty data series to the collection of data series. To update this
		/// series the <see cref="UpdateDataSeries"/> method should be used.<br /><br />
		/// The meaning of the width parameter depends on the data series type:
		/// <list type="bullet">
		/// <item><b>Line</b> - width of the line;</item>
		/// <item><b>Dots</b> - size of dots (rectangular dots with specified width and the same height);</item>
		/// <item><b>Connected dots</b> - size of dots (dots are connected with one pixel width line).</item>
		/// </list>
		/// </remarks>
		/// 
		public void AddDataSeries( string name, Color color, SeriesType type, int width )
		{
			AddDataSeries( name, color, type, width, true );
		}

		/// <summary>
		/// Add data series to the chart
		/// </summary>
		/// 
		/// <param name="name">Data series name</param>
		/// <param name="color">Data series color</param>
		/// <param name="type">Data series type</param>
		/// <param name="width">Width (depends on the data series type, see remarks)</param>
		/// <param name="updateYRange">Specifies if <see cref="RangeY"/> should be updated</param>
		/// 
		/// <remarks></remarks>
		/// 
		public void AddDataSeries( string name, Color color, SeriesType type, int width, bool updateYRange  )
		{
			// create new series definition ...
			DataSeries	series = new DataSeries( );
			// ... add fill it
			series.color	= color;
			series.type		= type;
			series.width	= width;
			series.updateYRange = updateYRange;
			// add to series table
			seriesTable.Add( name, series );
		}

		/// <summary>
		/// Update data series on the chart
		/// </summary>
		public void UpdateDataSeries( string name, double[,] data )
		{
			// get data series
			DataSeries	series = (DataSeries) seriesTable[name];
			// update data
			series.data = data;

			// update Y range
			if ( series.updateYRange )
				UpdateYRange( );
			// invalidate the control
			Invalidate( );
		}

		// Recalculate Y range
		private void UpdateYRange( )
		{
			double	minY = double.MaxValue;
			double	maxY = double.MinValue;

			// walk through all data series
			IDictionaryEnumerator en = seriesTable.GetEnumerator( );
			while ( en.MoveNext( ) )
			{
				DataSeries series = (DataSeries) en.Value;
				// get data of the series
				double[,] data = series.data;

				if ( ( series.updateYRange ) && ( data != null ) )
				{
					for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
					{
						double v = data[i, 1];
						// check for max
						if ( v > maxY )
							maxY = v;
						// check for min
						if ( v < minY )
							minY = v;
					}
				}
			}

			if ( ( minY != double.MaxValue) || ( maxY != double.MinValue ) )
			{
				rangeY = new DoubleRange( minY, maxY );
			}
		}
	}
}
