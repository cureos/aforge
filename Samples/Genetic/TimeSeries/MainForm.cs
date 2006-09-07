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
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox populationSizeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox selectionBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox functionsSetBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox geneticMethodBox;

		private double[] data = null;
		private double[,] dataToShow = null;

		private int populationSize = 40;
		private int iterations = 100;
		private int selectionMethod = 0;
		private int functionsSet = 0;
		private int geneticMethod = 0;

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
			chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
			chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Line, 1 );

			selectionBox.SelectedIndex		= selectionMethod;
			functionsSetBox.SelectedIndex	= functionsSet;
			geneticMethodBox.SelectedIndex	= geneticMethod;
			UpdateSettings( );
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.populationSizeBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.selectionBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.functionsSetBox = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.geneticMethodBox = new System.Windows.Forms.ComboBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
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
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.geneticMethodBox,
																					this.label4,
																					this.functionsSetBox,
																					this.label3,
																					this.selectionBox,
																					this.label2,
																					this.populationSizeBox,
																					this.label1});
			this.groupBox3.Location = new System.Drawing.Point(510, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(185, 200);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Settings";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Population size:";
			// 
			// populationSizeBox
			// 
			this.populationSizeBox.Location = new System.Drawing.Point(125, 20);
			this.populationSizeBox.Name = "populationSizeBox";
			this.populationSizeBox.Size = new System.Drawing.Size(50, 20);
			this.populationSizeBox.TabIndex = 1;
			this.populationSizeBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Selection method:";
			// 
			// selectionBox
			// 
			this.selectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.selectionBox.Items.AddRange(new object[] {
															  "Elite",
															  "Rank",
															  "Roulette"});
			this.selectionBox.Location = new System.Drawing.Point(110, 45);
			this.selectionBox.Name = "selectionBox";
			this.selectionBox.Size = new System.Drawing.Size(65, 21);
			this.selectionBox.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Function set:";
			// 
			// functionsSetBox
			// 
			this.functionsSetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.functionsSetBox.Items.AddRange(new object[] {
																 "Simple",
																 "Extended"});
			this.functionsSetBox.Location = new System.Drawing.Point(110, 70);
			this.functionsSetBox.Name = "functionsSetBox";
			this.functionsSetBox.Size = new System.Drawing.Size(65, 21);
			this.functionsSetBox.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 6;
			this.label4.Text = "Genetic method:";
			// 
			// geneticMethodBox
			// 
			this.geneticMethodBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.geneticMethodBox.Items.AddRange(new object[] {
																  "GP",
																  "GEP"});
			this.geneticMethodBox.Location = new System.Drawing.Point(110, 95);
			this.geneticMethodBox.Name = "geneticMethodBox";
			this.geneticMethodBox.Size = new System.Drawing.Size(65, 21);
			this.geneticMethodBox.TabIndex = 7;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(705, 364);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Time Series Prediction";
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
		
		}

		// Update settings controls
		private void UpdateSettings( )
		{
			populationSizeBox.Text		= populationSize.ToString( );
//			iterationsBox.Text			= iterations.ToString( );
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
