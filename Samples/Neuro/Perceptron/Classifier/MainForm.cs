using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

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
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private double[,]	data = null;
		private double[]	classes = null;

		// Constructor
		public MainForm( )
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
			this.dataList = new System.Windows.Forms.ListView();
			this.loadButton = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.loadButton);
			this.groupBox1.Controls.Add(this.dataList);
			this.groupBox1.Location = new System.Drawing.Point(10, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(190, 360);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Data";
			// 
			// dataList
			// 
			this.dataList.FullRowSelect = true;
			this.dataList.GridLines = true;
			this.dataList.Location = new System.Drawing.Point(10, 20);
			this.dataList.Name = "dataList";
			this.dataList.Size = new System.Drawing.Size(170, 300);
			this.dataList.TabIndex = 0;
			this.dataList.View = System.Windows.Forms.View.Details;
			// 
			// loadButton
			// 
			this.loadButton.Location = new System.Drawing.Point(10, 330);
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
			this.ClientSize = new System.Drawing.Size(732, 378);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Perceptron Classifier";
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
					int		i = 0, k = 0;

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
							k = strs.Length - 1;
							tempData = new double[50, k];
						}

						// parse data
						for ( int j = 0; j < k; j++ )
						{
							tempData[i, j] = double.Parse( strs[j] );
						}
						tempClasses[i] = double.Parse( strs[k] );

						// search for min value
						if ( tempData[i, 0] < minX )
							minX = tempData[i, 0];
						// search for max value
						if ( tempData[i, 0] > maxX )
							maxX = tempData[i, 0];

						i++;
					}

					// allocate and set data
					data = new double[i, k];
					Array.Copy( tempData, 0, data, 0, i * k );
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
			}
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all curent data and columns
			dataList.Items.Clear( );
			dataList.Columns.Clear( );

			// add columns
			for ( int i = 0, n = data.GetLength( 1 ); i < n; i++ )
			{
				dataList.Columns.Add( string.Format( "X{0}", i + 1 ),
					50, HorizontalAlignment.Left );
			}
			dataList.Columns.Add( "Class", 50, HorizontalAlignment.Left );

			// add items
			for ( int i = 0, n = data.GetLength( 0 ); i < n; i++ )
			{
				dataList.Items.Add( data[i, 0].ToString( ) );

				for ( int j = 1, k = data.GetLength( 1 ); j < k; j++ )
				{
					dataList.Items[i].SubItems.Add( data[i, j].ToString( ) );
				}
				dataList.Items[i].SubItems.Add( classes[i].ToString( ) );
			}
		}
	}
}
