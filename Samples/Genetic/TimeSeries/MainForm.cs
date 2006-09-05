using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;

using AForge;
using AForge.Genetic;

namespace TimeSeries
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.ColumnHeader yColumnHeader;
		private System.Windows.Forms.Button loadDataButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private TimeSeries.Chart chart;

		private double[] data = null;
		private double[,] dataToShow = null;

		// Constructor
		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Line, 1 );
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
			this.dataList = new System.Windows.Forms.ListView();
			this.yColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.loadDataButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chart = new TimeSeries.Chart();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dataList,
																					this.loadDataButton});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(180, 310);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// dataList
			// 
			this.dataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.yColumnHeader});
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(120, 255);
			this.dataList.TabIndex = 1;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// yColumnHeader
			// 
			this.yColumnHeader.Text = "Y";
			this.yColumnHeader.Width = 100;
			// 
			// loadDataButton
			// 
			this.loadDataButton.Location = new System.Drawing.Point(10, 280);
			this.loadDataButton.Name = "loadDataButton";
			this.loadDataButton.TabIndex = 1;
			this.loadDataButton.Text = "Load";
			this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
			this.openFileDialog.Title = "Select data file";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.chart});
			this.groupBox2.Location = new System.Drawing.Point(200, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(300, 310);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Function";
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(10, 20);
			this.chart.Name = "chart";
			this.chart.Size = new System.Drawing.Size(280, 280);
			this.chart.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(650, 364);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Time Series Prediction";
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
		
		}

		// Load data
		private void loadDataButton_Click(object sender, System.EventArgs e)
		{
			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				// read maximum 50 points
				double[] tempData = new double[50];

				try
				{
					// open selected file
					StreamReader reader = File.OpenText( openFileDialog.FileName );
					string	str = null;
					int		i = 0;

					// read the data
					while ( ( i < 50 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// parse the value
						tempData[i] = double.Parse( str );

						i++;
					}

					// allocate and set data
					data = new double[i];
					dataToShow = new double[i, 2];
					Array.Copy( tempData, 0, data, 0, i );
					for ( int j = 0; j < i; j++ )
					{
						dataToShow[j, 0] = j;
						dataToShow[j, 1] = data[j];
					}
				}
				catch (Exception)
				{
					MessageBox.Show( "Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				// update list and chart
				UpdateDataListView( );
				chart.RangeX = new DoubleRange( 0, data.Length - 1 );
				chart.UpdateDataSeries( "data", dataToShow );
				chart.UpdateDataSeries( "solution", null );
				// enable "Start" button
//				startButton.Enabled = true;
			}
		
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all current records
			dataList.Items.Clear( );
			// add new records
			for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				dataList.Items.Add( data[i].ToString( ) );
			}
		}


	}
}
