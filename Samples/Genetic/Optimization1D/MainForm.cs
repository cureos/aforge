using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using AForge;
using AForge.Genetic;

namespace Optimization1D
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private Optimization1D.Chart chart;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox minXBox;
		private System.Windows.Forms.TextBox maxXBox;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;

		private UserFunction userFunction = new UserFunction( );

		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// add data series to chart
			chart.AddDataSeries( "function", Color.Red, Chart.SeriesType.Line, 1 );
			UpdateChart( );

			// update controls
			minXBox.Text = userFunction.Range.Min.ToString( );
			maxXBox.Text = userFunction.Range.Max.ToString( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chart = new Optimization1D.Chart();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.minXBox = new System.Windows.Forms.TextBox();
			this.maxXBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(280, 270);
			this.chart.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.chart,
																					this.label1,
																					this.minXBox,
																					this.maxXBox,
																					this.label2});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(300, 330);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Function";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 297);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Range:";
			// 
			// minXBox
			// 
			this.minXBox.Location = new System.Drawing.Point(60, 295);
			this.minXBox.Name = "minXBox";
			this.minXBox.Size = new System.Drawing.Size(50, 20);
			this.minXBox.TabIndex = 3;
			this.minXBox.Text = "";
			this.minXBox.TextChanged += new System.EventHandler(this.minXBox_TextChanged);
			// 
			// maxXBox
			// 
			this.maxXBox.Location = new System.Drawing.Point(130, 295);
			this.maxXBox.Name = "maxXBox";
			this.maxXBox.Size = new System.Drawing.Size(50, 20);
			this.maxXBox.TabIndex = 4;
			this.maxXBox.Text = "";
			this.maxXBox.TextChanged += new System.EventHandler(this.maxXBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(115, 297);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "-";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label3});
			this.groupBox2.Location = new System.Drawing.Point(320, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(160, 254);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Settings";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(85, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Population size:";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(506, 350);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "1D Optimization";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( ) 
		{
			Application.Run( new MainForm( ) );
		}

		// Update chart
		private void UpdateChart( )
		{
			// update chart range
			chart.RangeX = userFunction.Range;

			double[,] data = null;

			if ( chart.RangeX.Length > 0 )
			{
				// prepare data
				data = new double[501, 2];

				double minX = userFunction.Range.Min;
				double length = userFunction.Range.Length;

				for ( int i = 0; i <= 500; i++ )
				{
					data[i, 0] = minX + length * i / 500;
					data[i, 1] = userFunction.OptimizationFunction( data[i, 0] );
				}
			}

			// update chart series
			chart.UpdateDataSeries( "function", data );
		}

		// Update min value
		private void minXBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				userFunction.Range = new DoubleRange( double.Parse( minXBox.Text ), userFunction.Range.Max );
				UpdateChart( );
			}
			catch
			{
			}
		}

		// Update max value
		private void maxXBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				userFunction.Range = new DoubleRange( userFunction.Range.Min, double.Parse( maxXBox.Text ) );
				UpdateChart( );
			}
			catch
			{
			}
		}

	}

	// Function to optimize
	public class UserFunction : OptimizationFunction1D
	{
		public UserFunction( ) : base( new DoubleRange( 0, 255 ) ) { }

		public override double OptimizationFunction( double x )
		{
			return Math.Cos( x / 23 ) * Math.Sin( x / 50 ) + 2;
		}
	}
}
