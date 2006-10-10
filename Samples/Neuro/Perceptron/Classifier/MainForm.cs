using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using AForge;

namespace Classifier
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private Classifier.Chart chart;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			variables = 0;
		private double[,]	data = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox learningRateBox;
		private double[]	classes = null;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			//
			chart.AddDataSeries( "class1", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "class2", Color.Blue, Chart.SeriesType.Dots, 5 );
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
			this.dataList = new System.Windows.Forms.ListView();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.chart = new Classifier.Chart();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.chart,
																					this.loadButton,
																					this.dataList});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(190, 420);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 390);
			this.loadButton.Name = "loadButton";
			this.loadButton.TabIndex = 1;
			this.loadButton.Text = "&Load";
			this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
			// 
			// dataList
			// 
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(170, 190);
			this.dataList.TabIndex = 0;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 215);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(170, 170);
			this.chart.TabIndex = 2;
			this.chart.Text = "chart1";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.learningRateBox,
																					this.label1});
			this.groupBox2.Location = new System.Drawing.Point(210, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 200);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Training";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Learning rate:";
			// 
			// learningRateBox
			// 
			this.learningRateBox.Location = new System.Drawing.Point(85, 20);
			this.learningRateBox.Name = "learningRateBox";
			this.learningRateBox.Size = new System.Drawing.Size(50, 20);
			this.learningRateBox.TabIndex = 1;
			this.learningRateBox.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(732, 440);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Perceptron Classifier";
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

		// On "Load" button click - load data
		private void loadButton_Click( object sender, System.EventArgs e )
		{
			// data file format:
			// X1, X2, ... Xn, class (0|1)

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 50 samples only)
				double[,]	tempData = null;
				double[]	tempClasses = new double[50];

				// min and max X values
				double minX = double.MaxValue;
				double maxX = double.MinValue;

				try
				{
					string	str = null;
					int		i = 0;

					// open selected file
					reader = File.OpenText( openFileDialog.FileName );

					// read the data
					while ( ( i < 50 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// split the string
						string[] strs = str.Split( ';' );
						if ( strs.Length == 1 )
							strs = str.Split( ',' );

						// allocate data array
						if ( i == 0 )
						{
							variables = strs.Length - 1;
							tempData = new double[50, variables];
						}

						// parse data
						for ( int j = 0; j < variables; j++ )
						{
							tempData[i, j] = double.Parse( strs[j] );
						}
						tempClasses[i] = double.Parse( strs[variables] );

						// search for min value
						if ( tempData[i, 0] < minX )
							minX = tempData[i, 0];
						// search for max value
						if ( tempData[i, 0] > maxX )
							maxX = tempData[i, 0];

						i++;
					}

					// allocate and set data
					data = new double[i, variables];
					Array.Copy( tempData, 0, data, 0, i * variables );
					classes = new double[i];
					Array.Copy( tempClasses, 0, classes, 0, i );
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

				// update list and chart
				UpdateDataListView( );

				if ( variables == 2 )
				{
					chart.RangeX = new DoubleRange( minX, maxX );
					ShowTrainingData( );
				}
			}
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all curent data and columns
			dataList.Items.Clear( );
			dataList.Columns.Clear( );

			// add columns
			for ( int i = 0, n = variables; i < n; i++ )
			{
				dataList.Columns.Add( string.Format( "X{0}", i + 1 ),
					50, HorizontalAlignment.Left );
			}
			dataList.Columns.Add( "Class", 50, HorizontalAlignment.Left );

			// add items
			for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				dataList.Items.Add( data[i, 0].ToString( ) );

				for ( int j = 1, k = variables; j < k; j++ )
				{
					dataList.Items[i].SubItems.Add( data[i, j].ToString( ) );
				}
				dataList.Items[i].SubItems.Add( classes[i].ToString( ) );
			}
		}

		// Show training data on chart
		private void ShowTrainingData( )
		{
			int class1Size = 0;
			int class2Size = 0;

			// calculate number of samples in each class
			for ( int i = 0, n = classes.Length; i < n; i++ )
			{
				if ( classes[i] == 0 )
					class1Size++;
				else
					class2Size++;
			}

			// allocate classes arrays
			double[,] class1 = new double[class1Size, 2];
			double[,] class2 = new double[class2Size, 2];

			// fill classes arrays
			for ( int i = 0, c1 = 0, c2 = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				if ( classes[i] == 0 )
				{
					// class 1
					class1[c1, 0] = data[i, 0];
					class1[c1, 1] = data[i, 1];
					c1++;
				}
				else
				{
					// class 2
					class2[c2, 0] = data[i, 0];
					class2[c2, 1] = data[i, 1];
					c2++;
				}
			}

			// updata chart control
			chart.UpdateDataSeries( "class1", class1 );
			chart.UpdateDataSeries( "class2", class2 );
		}
	}
}
