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
			this.chart = new AForge.Controls.Chart();
			this.loadButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.loadButton,
																					this.chart});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(220, 286);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(200, 200);
			this.chart.TabIndex = 0;
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 240);
			this.loadButton.Name = "loadButton";
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(418, 319);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "One-Layer Perceptron Classifier";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.groupBox1.ResumeLayout(false);
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

				chart.RangeX = new DoubleRange( minX, maxX );
				chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
				chart.UpdateDataSeries( "data", data );

				System.Diagnostics.Debug.WriteLine( classesCount );
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
		}

		// Show training data on chart
		private void ShowTrainingData( )
		{
			// show maximum 10 classes
		}
	}
}
