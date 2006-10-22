// AForge Framework
// Kohonen SOM 2D Organizing
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace SOMOrganizing
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.Panel pointsPanel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel mapPanel;
		private System.Windows.Forms.CheckBox showConnectionsCheck;
		private System.Windows.Forms.CheckBox showInactiveCheck;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox sizeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox radiusBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox rateBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		private const int	groupRadius = 20;
		private const int	pointsCount = 100;
		private int[,]		points = new int[pointsCount, 2];	// x, y
		private double[][]	trainingSet = new double[pointsCount][];

		private int			networkSize		= 15;
		private int			iterations		= 500;
		private double		learningRate	= 0.5;
		private int			learningRadius	= 7;

		private Random		rand = new Random( );
		private Thread		workerThread = null;
		private bool		needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			//
			GeneratePoints( );
			UpdateSettings( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if ( components != null ) 
				{
					components.Dispose( );
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.generateButton = new System.Windows.Forms.Button();
			this.pointsPanel = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.mapPanel = new System.Windows.Forms.Panel();
			this.showConnectionsCheck = new System.Windows.Forms.CheckBox();
			this.showInactiveCheck = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.sizeBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.radiusBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.rateBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.generateButton,
																					this.pointsPanel});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 295);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Points";
			// 
			// generateButton
			// 
			this.generateButton.Location = new System.Drawing.Point(10, 260);
			this.generateButton.Name = "generateButton";
			this.generateButton.TabIndex = 1;
			this.generateButton.Text = "&Generate";
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// pointsPanel
			// 
			this.pointsPanel.BackColor = System.Drawing.Color.White;
			this.pointsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pointsPanel.Location = new System.Drawing.Point(10, 20);
			this.pointsPanel.Name = "pointsPanel";
			this.pointsPanel.Size = new System.Drawing.Size(200, 200);
			this.pointsPanel.TabIndex = 0;
			this.pointsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.pointsPanel_Paint);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.showInactiveCheck,
																					this.showConnectionsCheck,
																					this.mapPanel});
			this.groupBox2.Location = new System.Drawing.Point(240, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(220, 295);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Map";
			// 
			// mapPanel
			// 
			this.mapPanel.BackColor = System.Drawing.Color.White;
			this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.mapPanel.Location = new System.Drawing.Point(10, 20);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(200, 200);
			this.mapPanel.TabIndex = 0;
			// 
			// showConnectionsCheck
			// 
			this.showConnectionsCheck.Location = new System.Drawing.Point(10, 240);
			this.showConnectionsCheck.Name = "showConnectionsCheck";
			this.showConnectionsCheck.Size = new System.Drawing.Size(150, 16);
			this.showConnectionsCheck.TabIndex = 1;
			this.showConnectionsCheck.Text = "Show Connections";
			// 
			// showInactiveCheck
			// 
			this.showInactiveCheck.Location = new System.Drawing.Point(10, 265);
			this.showInactiveCheck.Name = "showInactiveCheck";
			this.showInactiveCheck.Size = new System.Drawing.Size(160, 16);
			this.showInactiveCheck.TabIndex = 2;
			this.showInactiveCheck.Text = "Show Inactive Neurons";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.stopButton,
																					this.startButton,
																					this.currentIterationBox,
																					this.label8,
																					this.label7,
																					this.radiusBox,
																					this.label4,
																					this.rateBox,
																					this.label5,
																					this.iterationsBox,
																					this.label6,
																					this.label3,
																					this.label2,
																					this.sizeBox,
																					this.label1});
			this.groupBox3.Location = new System.Drawing.Point(470, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(180, 295);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Neural Network";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Size:";
			// 
			// sizeBox
			// 
			this.sizeBox.Location = new System.Drawing.Point(110, 20);
			this.sizeBox.Name = "sizeBox";
			this.sizeBox.Size = new System.Drawing.Size(60, 20);
			this.sizeBox.TabIndex = 1;
			this.sizeBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(150, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "(neurons count = size * size)";
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Location = new System.Drawing.Point(10, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(160, 2);
			this.label3.TabIndex = 3;
			// 
			// radiusBox
			// 
			this.radiusBox.Location = new System.Drawing.Point(110, 120);
			this.radiusBox.Name = "radiusBox";
			this.radiusBox.Size = new System.Drawing.Size(60, 20);
			this.radiusBox.TabIndex = 11;
			this.radiusBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 122);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 10;
			this.label4.Text = "Initial radius:";
			// 
			// rateBox
			// 
			this.rateBox.Location = new System.Drawing.Point(110, 95);
			this.rateBox.Name = "rateBox";
			this.rateBox.Size = new System.Drawing.Size(60, 20);
			this.rateBox.TabIndex = 9;
			this.rateBox.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 97);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 8;
			this.label5.Text = "Initial learning rate:";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(110, 70);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(60, 20);
			this.iterationsBox.TabIndex = 7;
			this.iterationsBox.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 72);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 16);
			this.label6.TabIndex = 6;
			this.label6.Text = "Iteraions:";
			// 
			// label7
			// 
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.Location = new System.Drawing.Point(10, 148);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(160, 2);
			this.label7.TabIndex = 12;
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(110, 160);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.ReadOnly = true;
			this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
			this.currentIterationBox.TabIndex = 14;
			this.currentIterationBox.Text = "";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(10, 162);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 16);
			this.label8.TabIndex = 13;
			this.label8.Text = "Curren iteration:";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(95, 260);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 16;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(10, 260);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 15;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(659, 315);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Kohonen SOM 2D Organizing";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
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

		// On main form closing
		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// check if worker thread is running
			if ( ( workerThread != null ) && ( workerThread.IsAlive ) )
			{
				needToStop = true;
				workerThread.Join( );
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			sizeBox.Text		= networkSize.ToString( );
			iterationsBox.Text	= iterations.ToString( );
			rateBox.Text		= learningRate.ToString( );
			radiusBox.Text		= learningRadius.ToString( );
		}

		// On "Generate" button click
		private void generateButton_Click(object sender, System.EventArgs e)
		{
			GeneratePoints( );
		}

		// Generate point
		private void GeneratePoints( )
		{
			int width	= pointsPanel.ClientRectangle.Width;
			int height	= pointsPanel.ClientRectangle.Height;
			int diameter = groupRadius * 2;

			// generate groups of ten points
			for ( int i = 0; i < pointsCount; )
			{
				int cx = rand.Next( width );
				int cy = rand.Next( height );

				// generate group
				for ( int j = 0 ; ( i < pointsCount ) && ( j < 10 ); )
				{
					int x = cx + rand.Next( diameter ) - groupRadius;
					int y = cy + rand.Next( diameter ) - groupRadius;

					// check if wee are not out
					if ( ( x < 0 ) || ( y < 0 ) || ( x >= width ) || ( y >= height ) )
					{
						continue;
					}

					// add point
					points[i, 0] = x;
					points[i, 1] = y;

					j++;
					i++;
				}
			}

//			map = null;
			pointsPanel.Invalidate( );
//			mapPanel.Invalidate( );
		}

		// Paint points
		private void pointsPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			using ( Brush brush = new SolidBrush( Color.Blue ) )
			{
				// draw all points
				for ( int i = 0, n = points.GetLength( 0 ); i < n; i++ )
				{
					g.FillEllipse( brush, points[i, 0] - 2, points[i, 1] - 2, 5, 5 );
				}
			}
		}

		// On "Start" button click
		private void startButton_Click(object sender, System.EventArgs e)
		{
		
		}

		// On "Stop" button click
		private void stopButton_Click(object sender, System.EventArgs e)
		{
			// stop worker thread
			needToStop = true;
			workerThread.Join( );
			workerThread = null;
		}
	}
}
