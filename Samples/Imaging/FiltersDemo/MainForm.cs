// AForge Framework
// Image Processing filters demo
//
// Copyright © Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FiltersDemo
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MenuItem fileItem;
		private System.Windows.Forms.MenuItem openFileItem;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem exitFilrItem;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem sizeItem;
		private System.Windows.Forms.MenuItem normalSizeItem;
		private System.Windows.Forms.MenuItem stretchedSizeItem;
		private System.Windows.Forms.MenuItem centeredSizeItem;
		private System.Windows.Forms.MenuItem filtersItem;
		private System.Windows.Forms.MenuItem noneFiltersItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem sepiaFiltersItem;
		private System.Windows.Forms.MenuItem invertFiltersItem;
		private System.Windows.Forms.MenuItem rotateChannelFiltersItem;

		private System.Drawing.Bitmap	sourceImage;
		private System.Drawing.Bitmap	filteredImage;
		private System.Windows.Forms.MenuItem grayscaleFiltersItem;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// set default size mode of picture box
			pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
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
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.fileItem = new System.Windows.Forms.MenuItem();
			this.openFileItem = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.exitFilrItem = new System.Windows.Forms.MenuItem();
			this.sizeItem = new System.Windows.Forms.MenuItem();
			this.normalSizeItem = new System.Windows.Forms.MenuItem();
			this.stretchedSizeItem = new System.Windows.Forms.MenuItem();
			this.centeredSizeItem = new System.Windows.Forms.MenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.filtersItem = new System.Windows.Forms.MenuItem();
			this.noneFiltersItem = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.sepiaFiltersItem = new System.Windows.Forms.MenuItem();
			this.invertFiltersItem = new System.Windows.Forms.MenuItem();
			this.rotateChannelFiltersItem = new System.Windows.Forms.MenuItem();
			this.grayscaleFiltersItem = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.fileItem,
																					 this.filtersItem,
																					 this.sizeItem});
			// 
			// fileItem
			// 
			this.fileItem.Index = 0;
			this.fileItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.openFileItem,
																					 this.menuItem3,
																					 this.exitFilrItem});
			this.fileItem.Text = "&File";
			// 
			// openFileItem
			// 
			this.openFileItem.Index = 0;
			this.openFileItem.Text = "&Open";
			this.openFileItem.Click += new System.EventHandler(this.openFileItem_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// exitFilrItem
			// 
			this.exitFilrItem.Index = 2;
			this.exitFilrItem.Text = "E&xit";
			this.exitFilrItem.Click += new System.EventHandler(this.exitFilrItem_Click);
			// 
			// sizeItem
			// 
			this.sizeItem.Index = 2;
			this.sizeItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.normalSizeItem,
																					 this.stretchedSizeItem,
																					 this.centeredSizeItem});
			this.sizeItem.Text = "&Size mode";
			this.sizeItem.Popup += new System.EventHandler(this.sizeItem_Popup);
			// 
			// normalSizeItem
			// 
			this.normalSizeItem.Index = 0;
			this.normalSizeItem.Text = "&Normal";
			this.normalSizeItem.Click += new System.EventHandler(this.normalSizeItem_Click);
			// 
			// stretchedSizeItem
			// 
			this.stretchedSizeItem.Index = 1;
			this.stretchedSizeItem.Text = "&Stretched";
			this.stretchedSizeItem.Click += new System.EventHandler(this.stretchedSizeItem_Click);
			// 
			// centeredSizeItem
			// 
			this.centeredSizeItem.Index = 2;
			this.centeredSizeItem.Text = "&Centered";
			this.centeredSizeItem.Click += new System.EventHandler(this.centeredSizeItem_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Image files (*.jpg,*.png,*.tif,*.bmp,*.gif)|*.jpg;*.png;*.tif;*.bmp;*.gif|JPG fil" +
				"es (*.jpg)|*.jpg|PNG files (*.png)|*.png|TIF files (*.tif)|*.tif|BMP files (*.bm" +
				"p)|*.bmp|GIF files (*.gif)|*.gif";
			this.openFileDialog.Title = "Open image";
			// 
			// pictureBox
			// 
			this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(6, 5);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(530, 315);
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// filtersItem
			// 
			this.filtersItem.Enabled = false;
			this.filtersItem.Index = 1;
			this.filtersItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.noneFiltersItem,
																						this.menuItem1,
																						this.grayscaleFiltersItem,
																						this.sepiaFiltersItem,
																						this.invertFiltersItem,
																						this.rotateChannelFiltersItem});
			this.filtersItem.Text = "Fi&lters";
			// 
			// noneFiltersItem
			// 
			this.noneFiltersItem.Index = 0;
			this.noneFiltersItem.Text = "&None";
			this.noneFiltersItem.Click += new System.EventHandler(this.noneFiltersItem_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.Text = "-";
			// 
			// sepiaFiltersItem
			// 
			this.sepiaFiltersItem.Index = 3;
			this.sepiaFiltersItem.Text = "&Sepia";
			this.sepiaFiltersItem.Click += new System.EventHandler(this.sepiaFiltersItem_Click);
			// 
			// invertFiltersItem
			// 
			this.invertFiltersItem.Index = 4;
			this.invertFiltersItem.Text = "&Invert";
			this.invertFiltersItem.Click += new System.EventHandler(this.invertFiltersItem_Click);
			// 
			// rotateChannelFiltersItem
			// 
			this.rotateChannelFiltersItem.Index = 5;
			this.rotateChannelFiltersItem.Text = "&Rotate channel";
			this.rotateChannelFiltersItem.Click += new System.EventHandler(this.rotateChannelFiltersItem_Click);
			// 
			// grayscaleFiltersItem
			// 
			this.grayscaleFiltersItem.Index = 2;
			this.grayscaleFiltersItem.Text = "&Grayscale";
			this.grayscaleFiltersItem.Click += new System.EventHandler(this.grayscaleFiltersItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(542, 325);
			this.Controls.Add(this.pictureBox);
			this.Menu = this.mainMenu;
			this.MinimumSize = new System.Drawing.Size(320, 240);
			this.Name = "MainForm";
			this.Text = "Image Processing filters demo";
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

		// On File->Exit menu item
		private void exitFilrItem_Click( object sender, System.EventArgs e )
		{
			Application.Exit( );
		}

		// On File->Open menu item
		private void openFileItem_Click( object sender, System.EventArgs e )
		{
			try
			{
				// show file open dialog
				if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
				{
					// load image
					sourceImage = (Bitmap) Bitmap.FromFile( openFileDialog.FileName );
					// format image
					AForge.Imaging.Image.FormatImage( ref sourceImage );

					// image type
					if ( sourceImage.PixelFormat != PixelFormat.Format24bppRgb )
					{
						MessageBox.Show( "The demo application support only color images.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						// free image
						sourceImage.Dispose( );
						sourceImage = null;
					}

					// display image
					pictureBox.Image = sourceImage;
					noneFiltersItem.Checked = true;

					// enable filters menu
					filtersItem.Enabled = ( sourceImage != null );
				}
			}
			catch
			{
				MessageBox.Show( "Failed loading the image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		// On Size mode->Normal menu item
		private void normalSizeItem_Click(object sender, System.EventArgs e)
		{
			pictureBox.SizeMode = PictureBoxSizeMode.Normal;
		}

		// On Size mode->Stretched menu item
		private void stretchedSizeItem_Click( object sender, System.EventArgs e )
		{
			pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
		}

		// On Size mode->Centered size menu item
		private void centeredSizeItem_Click(object sender, System.EventArgs e)
		{
			pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
		}

		// On Size menu item popup
		private void sizeItem_Popup(object sender, System.EventArgs e)
		{
			normalSizeItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.Normal );
			stretchedSizeItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.StretchImage );
			centeredSizeItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.CenterImage );
		}

		// Clear current image in picture box
		private void ClearCurrentImage( )
		{
			// clear current image from picture box
			pictureBox.Image = null;
			// free current image
			if ( ( noneFiltersItem.Checked == false ) && ( filteredImage != null ) )
			{
				filteredImage.Dispose( );
				filteredImage = null;
			}
			// uncheck all menu items
			foreach ( MenuItem item in filtersItem.MenuItems )
				item.Checked = false;
		}

		// Apply filter to the source image and show the filtered image
		private void ApplyFilter( IFilter filter )
		{
			ClearCurrentImage( );
			// apply filter
			filteredImage = filter.Apply( sourceImage );
			// display filtered image
			pictureBox.Image = filteredImage;
		}

		// On Filters->None item
		private void noneFiltersItem_Click( object sender, System.EventArgs e )
		{
			ClearCurrentImage( );
			// display source image
			pictureBox.Image = sourceImage;
			noneFiltersItem.Checked = true;
		}

		// On Filters->Grayscale item
		private void grayscaleFiltersItem_Click(object sender, System.EventArgs e)
		{
			ApplyFilter( new GrayscaleBT709( ) );
			grayscaleFiltersItem.Checked = true;
		}

		// On Filters->Sepia item
		private void sepiaFiltersItem_Click(object sender, System.EventArgs e)
		{
			ApplyFilter( new Sepia( ) );
			sepiaFiltersItem.Checked = true;
		}

		// On Filters->Invert item
		private void invertFiltersItem_Click(object sender, System.EventArgs e)
		{
			ApplyFilter( new Invert( ) );
			invertFiltersItem.Checked = true;
		}

		// On Filters->Rotate Channels item
		private void rotateChannelFiltersItem_Click(object sender, System.EventArgs e)
		{
			ApplyFilter( new RotateChannels( ) );
			rotateChannelFiltersItem.Checked = true;
		}
	}
}
