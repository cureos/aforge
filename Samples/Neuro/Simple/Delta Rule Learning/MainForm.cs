// AForge Framework
// Classifier using Delta Rule Learning 
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

namespace Classifier
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button loadButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ListView dataList;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int			samples = 0;
		private int			variables = 0;
		private double[,]	data = null;
		private int[]		classes = null;
		private int			classesCount = 0;
		private int[]		samplesPerClass = null;

		private double		learningRate = 0.1;
		private bool		saveStatisticsToFiles = false;

		private Thread	workerThread = null;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox learningRateBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox alphaBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox errorLimitBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox iterationsBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox neuronsBox;
		private System.Windows.Forms.CheckBox oneNeuronForTwoCheck;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox currentIterationBox;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Label label9;
		private bool	needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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
			this.dataList = new System.Windows.Forms.ListView();
			this.loadButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.learningRateBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.alphaBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.errorLimitBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.iterationsBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.neuronsBox = new System.Windows.Forms.TextBox();
			this.oneNeuronForTwoCheck = new System.Windows.Forms.CheckBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.currentIterationBox = new System.Windows.Forms.TextBox();
			this.stopButton = new System.Windows.Forms.Button();
			this.startButton = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.dataList,
																					this.loadButton});
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(230, 300);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// dataList
			// 
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(210, 240);
			this.dataList.TabIndex = 0;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 265);
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
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.label9,
																					this.currentIterationBox,
																					this.label8,
																					this.label7,
																					this.checkBox2,
																					this.oneNeuronForTwoCheck,
																					this.neuronsBox,
																					this.label6,
																					this.label5,
																					this.iterationsBox,
																					this.label4,
																					this.errorLimitBox,
																					this.label3,
																					this.alphaBox,
																					this.label2,
																					this.label1,
																					this.learningRateBox,
																					this.stopButton,
																					this.startButton});
			this.groupBox2.Location = new System.Drawing.Point(250, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(185, 300);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Training";
			// 
			// learningRateBox
			// 
			this.learningRateBox.Location = new System.Drawing.Point(125, 20);
			this.learningRateBox.Name = "learningRateBox";
			this.learningRateBox.Size = new System.Drawing.Size(50, 20);
			this.learningRateBox.TabIndex = 3;
			this.learningRateBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Learning rate:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 15);
			this.label2.TabIndex = 4;
			this.label2.Text = "Sigmoid\'s alpha value:";
			// 
			// alphaBox
			// 
			this.alphaBox.Location = new System.Drawing.Point(125, 45);
			this.alphaBox.Name = "alphaBox";
			this.alphaBox.Size = new System.Drawing.Size(50, 20);
			this.alphaBox.TabIndex = 5;
			this.alphaBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(10, 75);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110, 15);
			this.label3.TabIndex = 6;
			this.label3.Text = "Learning error limit:";
			// 
			// errorLimitBox
			// 
			this.errorLimitBox.Location = new System.Drawing.Point(125, 70);
			this.errorLimitBox.Name = "errorLimitBox";
			this.errorLimitBox.Size = new System.Drawing.Size(50, 20);
			this.errorLimitBox.TabIndex = 7;
			this.errorLimitBox.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Iterations limit:";
			// 
			// iterationsBox
			// 
			this.iterationsBox.Location = new System.Drawing.Point(125, 95);
			this.iterationsBox.Name = "iterationsBox";
			this.iterationsBox.Size = new System.Drawing.Size(50, 20);
			this.iterationsBox.TabIndex = 9;
			this.iterationsBox.Text = "";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.label5.Location = new System.Drawing.Point(125, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "( 0 - inifinity )";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 137);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 12);
			this.label6.TabIndex = 11;
			this.label6.Text = "Neurons:";
			// 
			// neuronsBox
			// 
			this.neuronsBox.Location = new System.Drawing.Point(125, 135);
			this.neuronsBox.Name = "neuronsBox";
			this.neuronsBox.ReadOnly = true;
			this.neuronsBox.Size = new System.Drawing.Size(50, 20);
			this.neuronsBox.TabIndex = 12;
			this.neuronsBox.Text = "";
			// 
			// oneNeuronForTwoCheck
			// 
			this.oneNeuronForTwoCheck.Location = new System.Drawing.Point(10, 165);
			this.oneNeuronForTwoCheck.Name = "oneNeuronForTwoCheck";
			this.oneNeuronForTwoCheck.Size = new System.Drawing.Size(168, 15);
			this.oneNeuronForTwoCheck.TabIndex = 13;
			this.oneNeuronForTwoCheck.Text = "Use 1 neuron for 2 classes";
			// 
			// checkBox2
			// 
			this.checkBox2.Location = new System.Drawing.Point(10, 190);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(157, 25);
			this.checkBox2.TabIndex = 14;
			this.checkBox2.Text = "Use error limit (checked) or iterations limit";
			// 
			// label7
			// 
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.Location = new System.Drawing.Point(10, 220);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(165, 2);
			this.label7.TabIndex = 15;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(10, 232);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(98, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "Current iteration:";
			// 
			// currentIterationBox
			// 
			this.currentIterationBox.Location = new System.Drawing.Point(125, 230);
			this.currentIterationBox.Name = "currentIterationBox";
			this.currentIterationBox.Size = new System.Drawing.Size(50, 20);
			this.currentIterationBox.TabIndex = 17;
			this.currentIterationBox.Text = "";
			// 
			// stopButton
			// 
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(100, 268);
			this.stopButton.Name = "stopButton";
			this.stopButton.TabIndex = 6;
			this.stopButton.Text = "S&top";
			// 
			// startButton
			// 
			this.startButton.Enabled = false;
			this.startButton.Location = new System.Drawing.Point(10, 268);
			this.startButton.Name = "startButton";
			this.startButton.TabIndex = 5;
			this.startButton.Text = "&Start";
			// 
			// label9
			// 
			this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label9.Location = new System.Drawing.Point(10, 258);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(165, 2);
			this.label9.TabIndex = 18;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(496, 320);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox2,
																		  this.groupBox1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Classifier using Delta Rule Learning";
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
			// X1, X2, ..., Xn, class

			// load maximum 10 classes !

			// show file selection dialog
			if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;

				// temp buffers (for 200 samples only)
				double[,]	tempData = null;
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

						// allocate data array
						if ( samples == 0 )
						{
							variables = strs.Length - 1;
							tempData = new double[200, variables];
						}

						// parse data
						for ( int j = 0; j < variables; j++ )
						{
							tempData[samples, j] = double.Parse( strs[j] );
						}
						tempClasses[samples] = int.Parse( strs[variables] );

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
					data = new double[samples, variables];
					Array.Copy( tempData, 0, data, 0, samples * variables );
					classes = new int[samples];
					Array.Copy( tempClasses, 0, classes, 0, samples );
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

				startButton.Enabled = true;
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
			for ( int i = 0; i < samples; i++ )
			{
				dataList.Items.Add( data[i, 0].ToString( ) );

				for ( int j = 1; j < variables; j++ )
				{
					dataList.Items[i].SubItems.Add( data[i, j].ToString( ) );
				}
				dataList.Items[i].SubItems.Add( classes[i].ToString( ) );
			}
		}
	}
}
