// AForge Framework
// One-Layer Perceptron Classifier
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
using System.IO;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace Classifier
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private AForge.Controls.Chart chart;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			samples = 0;
		private double[,]	data = null;
		private int[]		classes = null;
		private int			classesCount = 0;
		private int[]		samplesPerClass = null;

		private Thread	workerThread = null;
		private bool	needToStop = false;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;

		// color for data series
		private static Color[]	dataSereisColors = new Color[10] {
																	 Color.Red,		Color.Blue,
																	 Color.Green,	Color.DarkOrange,
																	 Color.Violet,	Color.Brown,
																	 Color.Black,	Color.Pink,
																	 Color.Olive,	Color.Navy };

		// Constructor
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.loadButton = new System.Windows.Forms.Button();
			this.chart = new AForge.Controls.Chart();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.loadButton);
			this.groupBox1.Controls.Add(this.chart);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 286);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 240);
			this.loadButton.Name = "loadButton";
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(200, 200);
			this.chart.TabIndex = 0;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.stopButton);
			this.groupBox2.Controls.Add(this.startButton);
			this.groupBox2.Controls.Add(this.iterationsBox);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.learningRateBox);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(240, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 155);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Training";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Learning rate:";
			// 
			// learningRateBox
			// 
			this.learningRateBox.Location = new System.Drawing.Point(90, 20);
			this.learningRateBox.Name = "learningRateBox";
			this.learningRateBox.Size = new System.Drawing.Size(50, 20);
			this.learningRateBox.TabIndex = 1;
			this.learningRateBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Iterations:";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(90, 45);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.ReadOnly = true;
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 3;
			this.iterationsBox.Text = "";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(155, 49);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 10;
			this.stopButton.Text = "S&top";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(155, 19);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 9;
			this.startButton.Text = "&Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(519, 319);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "One-Layer Perceptron Classifier";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
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

		// Load input data
		private void loadButton_Click(object sender, System.EventArgs e)
		{
			// data file format:
			// X1, X2, class

			// load maximum 10 classes !

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 200 samples only)
				double[,]	tempData = new double[200, 2];
				int[]		tempClasses = new int[200];

				// min and max X values
				double minX = double.MaxValue;
				double maxX = double.MinValue;

				// samples count
				samples = 0;
				// classes count
				classesCount = 0;
				samplesPerClass = new int[10];

				try
				{
					string	str = null;

					// open selected file
					reader = File.OpenText( openFileDialog.FileName );

					// read the data
					while ( ( samples < 200 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// split the string
						string[] strs = str.Split( ';' );
						if ( strs.Length == 1 )
							strs = str.Split( ',' );

						// check tokens count
						if ( strs.Length != 3 )
							throw new ApplicationException( "Invalid file format" );

						// parse tokens
						tempData[samples, 0] = double.Parse( strs[0] );
						tempData[samples, 1] = double.Parse( strs[1] );
						tempClasses[samples] = int.Parse( strs[2] );

						// skip classes over 10, except only first 10 classes
						if ( tempClasses[samples] >= 10 )
							continue;

						// count the amount of different classes
						if ( tempClasses[samples] >= classesCount )
							classesCount = tempClasses[samples] + 1;
						// count samples per class
						samplesPerClass[tempClasses[samples]]++;

						// search for min value
						if ( tempData[samples, 0] < minX )
							minX = tempData[samples, 0];
						// search for max value
						if ( tempData[samples, 0] > maxX )
							maxX = tempData[samples, 0];

						samples++;
					}

					// allocate and set data
					data = new double[samples, 2];
					Array.Copy( tempData, 0, data, 0, samples * 2 );
					classes = new int[samples];
					Array.Copy( tempClasses, 0, classes, 0, samples );

					// clear current result
//					ClearCurrentSolution( );
				}
				catch ( Exception )
				{
					MessageBox.Show( "Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				finally
				{
					// close file
					if ( reader != null )
						reader.Close( );
				}

				// update chart
				chart.RangeX = new DoubleRange( minX, maxX );
				ShowTrainingData( );
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
		}

		// Show training data on chart
		private void ShowTrainingData( )
		{
			double[][,]	dataSeries = new double[classesCount][,];
			int[]		indexes = new int[classesCount];

			// allocate data arrays
			for ( int i = 0; i < classesCount; i++ )
			{
				dataSeries[i] = new double[samplesPerClass[i], 2];
			}

			// fill data arrays
			for ( int i = 0; i < samples; i++ )
			{
				// get sample's class
				int dataClass = classes[i];
				// copy data into appropriate array
				dataSeries[dataClass][indexes[dataClass], 0] = data[i, 0];
				dataSeries[dataClass][indexes[dataClass], 1] = data[i, 1];
				indexes[dataClass]++;
			}

			// remove all previous data series from chart control
			chart.RemoveAllDataSeries( );

			// add new data series
			for ( int i = 0; i < classesCount; i++ )
			{
				string className = string.Format( "class" + i );

				chart.AddDataSeries( className, dataSereisColors[i], Chart.SeriesType.Dots, 5 );
				chart.UpdateDataSeries( className, dataSeries[i] );
			}
		}

		private void startButton_Click(object sender, System.EventArgs e)
		{
		
		}

		private void stopButton_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
