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

namespace Approximation
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListView dataList;
		private System.Windows.Forms.ColumnHeader xColumnHeader;
		private System.Windows.Forms.ColumnHeader yColumnHeader;
		private System.Windows.Forms.Button loadDataButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private Approximation.Chart chart;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox populationSizeBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox selectionBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label4;

		private double[,] data = null;

		private int populationSize = 40;
		private int iterations = 100;
		private int selectionMethod = 0;

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

			selectionBox.SelectedIndex = selectionMethod;
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
			this.xColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.yColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.loadDataButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.chart = new Approximation.Chart();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.selectionBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.populationSizeBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
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
																					   this.xColumnHeader,
																					   this.yColumnHeader});
			this.dataList.GridLines = true;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(160, 255);
			this.dataList.TabIndex = 1;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// xColumnHeader
			// 
			this.xColumnHeader.Text = "X";
			// 
			// yColumnHeader
			// 
			this.yColumnHeader.Text = "Y";
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
																					this.label4,
																					this.iterationsBox,
																					this.label3,
																					this.selectionBox,
																					this.label2,
																					this.populationSizeBox,
																					this.label1});
			this.groupBox3.Location = new System.Drawing.Point(510, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(185, 238);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Settings";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.label4.Location = new System.Drawing.Point(125, 210);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 15);
			this.label4.TabIndex = 6;
			this.label4.Text = "( 0 - inifinity )";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 190);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 5;
			this.iterationsBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 192);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "Iteration:";
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
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Selection method:";
			// 
			// populationSizeBox
			// 
			this.populationSizeBox.Location = new System.Drawing.Point(125, 20);
			this.populationSizeBox.Name = "populationSizeBox";
			this.populationSizeBox.Size = new System.Drawing.Size(50, 20);
			this.populationSizeBox.TabIndex = 1;
			this.populationSizeBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Population size:";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(704, 330);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox3,
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Approximation";
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
		static void Main() 
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
			iterationsBox.Text			= iterations.ToString( );
		}

		// Load data
		private void loadDataButton_Click(object sender, System.EventArgs e)
		{
			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				// read maximum 100 points
				double[,] tempData = new double[100, 2];
				double minX = double.MaxValue;
				double maxX = double.MinValue;

				try
				{
					// open selected file
					StreamReader reader = File.OpenText( openFileDialog.FileName );
					string	str = null;
					int		i = 0;

					// read the data
					while ( ( i < 100 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						string[] strs = str.Split( ';' );
						// parse X
						tempData[i, 0] = double.Parse( strs[0] );
						tempData[i, 1] = double.Parse( strs[1] );

						// search for min value
						if ( tempData[i, 0 ] < minX )
							minX = tempData[i, 0];
						// search for max value
						if ( tempData[i, 0 ] > maxX )
							maxX = tempData[i, 0];

						i++;
					}

					// allocate and set data
					data = new Double[i, 2];
					Array.Copy( tempData, 0, data, 0, i * 2 );
				}
				catch (Exception)
				{
					MessageBox.Show( "Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				// update list and chart
				UpdateDataListView( );
				chart.RangeX = new DoubleRange( minX, maxX );
				chart.UpdateDataSeries( "data", data );
				chart.UpdateDataSeries( "solution", null );
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
				dataList.Items.Add( data[i, 0].ToString( ) );
				dataList.Items[i].SubItems.Add( data[i, 1].ToString( ) );
			}
		}
	}
}
