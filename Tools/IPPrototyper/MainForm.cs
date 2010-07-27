// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using AForge;
using AForge.Imaging;
using AForge.Imaging.IPPrototyper;

namespace IPPrototyper
{
    internal partial class MainForm : Form
    {
        // selected folder containing images to process
        private string selectedFolder;
        // list of found image processing routines
        private Dictionary<string, IImageProcessingRoutine> processingRoutines = new Dictionary<string,IImageProcessingRoutine>( );
        // currently active image processing routine to use
        private IImageProcessingRoutine ipRoutineToUse = null;
        // image processing log
        private ImageProcessingLog processingLog = new ImageProcessingLog( );

        private HistogramForm histogramForm = null;

        public MainForm( )
        {
            InitializeComponent( );

        }

        // On form loading
        private void MainForm_Load( object sender, EventArgs e )
        {
            // collect available modules in application's directory
            CollectModules( Path.GetDirectoryName( Application.ExecutablePath ) );
            // add modules' name to application's menu
            foreach ( string routineName in processingRoutines.Keys )
            {
                ToolStripItem item = modulesToolStripMenuItem.DropDownItems.Add( routineName );

                item.Click += new System.EventHandler( this.module_Click );

                if ( ipRoutineToUse == null )
                {
                    ipRoutineToUse = processingRoutines[routineName];
                }
            }

            // load configuratio
            Configuration config = Configuration.Instance;

            if ( config.Load( ) )
            {
                RebuildRecentFoldersList( );
            }
        }

        // Rebuild menu with the list of recently used folders
        private void RebuildRecentFoldersList( )
        {
            // unsubscribe from events
            foreach ( ToolStripItem item in recentFoldersToolStripMenuItem.DropDownItems )
            {
                item.Click -= new EventHandler( recentFolder_Click );
            }

            // remove all current items
            recentFoldersToolStripMenuItem.DropDownItems.Clear( );

            // add new items
            foreach ( string folderName in Configuration.Instance.RecentFolders )
            {
                ToolStripItem item = recentFoldersToolStripMenuItem.DropDownItems.Add( folderName );

                item.Click += new EventHandler( recentFolder_Click );
            }
        }

        // On form closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Configuration.Instance.Save( );
        }

        // Update check style of modules' items
        private void modulesToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            foreach ( ToolStripMenuItem item in modulesToolStripMenuItem.DropDownItems )
            {
                item.Checked = ( ( ipRoutineToUse != null ) && ( ipRoutineToUse.Name == item.Text ) );
            }
        }

        // Item is clicked in modules' menu
        private void module_Click( object sender, EventArgs e )
        {
            ipRoutineToUse = processingRoutines[( (ToolStripMenuItem) sender ).Text];
            ProcessSelectedImage( );
        }

        // Exit from application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        // Collect information about available modules
        private void CollectModules( string path )
        {
            // create directory info
            DirectoryInfo dir = new DirectoryInfo( path );

            // get all dll files from the directory
            FileInfo[] files = dir.GetFiles( "*.dll" );

            // walk through all files
            foreach ( FileInfo f in files )
            {
                CollectImageProcessingRoutines( Path.Combine( path, f.Name ) );
            }
        }

        // Collect available image processing routines in the specified assembly
        private void CollectImageProcessingRoutines( string fname )
        {
            Type typeIImageProcessingRoutine = typeof( IImageProcessingRoutine );
            Assembly assembly = null;

            try
            {
                // try to load assembly
                assembly = Assembly.LoadFrom( fname );

                // get types of the assembly
                Type[] types = assembly.GetTypes( );

                // check all types
                foreach ( Type type in types )
                {
                    // get interfaces ot the type
                    Type[] interfaces = type.GetInterfaces( );

                    // check, if the type is inherited from IImageProcessingRoutine
                    if ( Array.IndexOf( interfaces, typeIImageProcessingRoutine ) != -1 )
                    {
                        IImageProcessingRoutine	ipRoutine = null;

                        try
                        {
                            // create an instance of the type
                            ipRoutine = (IImageProcessingRoutine) Activator.CreateInstance( type );
                            // add routine to collection
                            if ( !processingRoutines.ContainsKey( ipRoutine.Name ) )
                                processingRoutines.Add( ipRoutine.Name, ipRoutine );
                        }
                        catch ( Exception )
                        {
                            // something failed during instance creatinion
                        }
                    }
                }
            }
            catch ( Exception )
            {
            }
        }

        // Open folder
        private void openFolderToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( folderBrowserDialog.ShowDialog( ) == DialogResult.OK )
            {
                if ( OpenFolder( folderBrowserDialog.SelectedPath ) )
                {
                    // remember this folder
                    Configuration.Instance.AddRecentFolder( selectedFolder );
                    RebuildRecentFoldersList( );
                }
            }
        }

        // Item is clicked in recent folders list
        private void recentFolder_Click( object sender, EventArgs e )
        {
            string folderName = ( (ToolStripMenuItem) sender ).Text;

            if ( OpenFolder( folderName ) )
            {
                // move the folder up in the list
                Configuration.Instance.AddRecentFolder( folderName );
            }
            else
            {
                // remove failing folder
                Configuration.Instance.RemoveRecentFolder( folderName );
            }
            RebuildRecentFoldersList( );
        }

        // Open specified folder
        private bool OpenFolder( string folderName )
        {
            bool success = false;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo( folderName );
                FileInfo[] fileInfos = dirInfo.GetFiles( );

                filesListView.Items.Clear( );

                // collect all image files
                foreach ( FileInfo fi in fileInfos )
                {
                    string ext = fi.Extension.ToLower( );

                    // check for supported extension
                    if (
                        ( ext == ".jpg" ) || ( ext == ".jpeg" ) ||
                        ( ext == ".bmp" ) || ( ext == ".png" ) )
                    {
                        filesListView.Items.Add( fi.Name );
                    }
                }

                logListView.Items.Clear( );
                filesListView.Focus( );
                ProcessSelectedImage( );

                selectedFolder = folderName;
                success = true;
            }
            catch
            {
                MessageBox.Show( "Failed opening the folder:\n" + folderName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            return success;
        }

        // Selection has changed in files list view control
        private void filesListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            ProcessSelectedImage( );
        }

        // Process currently selected image
        private void ProcessSelectedImage( )
        {
            if ( filesListView.SelectedItems.Count == 1 )
            {
                Bitmap image = null;

                try
                {
                    image = (Bitmap) Bitmap.FromFile( Path.Combine( selectedFolder, filesListView.SelectedItems[0].Text ) );
                }
                catch
                {
                }

                ProcessImage( image );
                ShowLogMessages( );
                UpdateLogView( );
            }
            else
            {
                pictureBox.Image = null;
                logBox.Text = string.Empty;
            }
        }

        // Process specified image
        private void ProcessImage( Bitmap image )
        {
            processingLog.Clear( );
            processingLog.AddImage( "Source", image );

            if ( ipRoutineToUse != null )
            {
                ipRoutineToUse.Process( image, processingLog );
            }
        }

        // Update log view
        private void UpdateLogView( )
        {
            string currentSelection = string.Empty;
            int newSelectionIndex = 0;
            int i = 0;

            if ( logListView.SelectedIndices.Count > 0 )
            {
                currentSelection = logListView.Items[logListView.SelectedIndices[0]].Text;
            }

            logListView.Items.Clear( );

            foreach ( KeyValuePair<string, Bitmap> kvp in processingLog.Images )
            {
                logListView.Items.Add( kvp.Key );

                if ( kvp.Key == currentSelection )
                    newSelectionIndex = i;

                i++;
            }

            logListView.SelectedIndices.Add( newSelectionIndex );
            logListView.EnsureVisible( newSelectionIndex );
        }

        // Display log messages
        private void ShowLogMessages( )
        {
            StringBuilder sb = new StringBuilder( );

            foreach ( string message in processingLog.Messages )
            {
                sb.Append( message );
                sb.Append( "\r\n" );
            }

            logBox.Text = sb.ToString( );
        }

        // Selection has changed in log list view - update image
        private void logListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( logListView.SelectedIndices.Count == 1 )
            {
                string stepName = logListView.SelectedItems[0].Text;

                Bitmap oldImage = (Bitmap) pictureBox.Image;
                pictureBox.Image = (Bitmap) processingLog.Images[stepName].Clone( );

                if ( oldImage != null )
                {
                    oldImage.Dispose( );
                }

                ShowCurrentImageHistogram( );
            }
        }

        // Update status of menu items in Settings->Image view
        private void imageviewToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            normalToolStripMenuItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.Normal );
            centerToolStripMenuItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.CenterImage );
            stretchToolStripMenuItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.StretchImage );
        }

        // Set Normal view for images
        private void normalToolStripMenuItem_Click( object sender, EventArgs e )
        {
            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
        }

        // Set Centred view for images
        private void centerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        // Set Stretched view for images
        private void stretchToolStripMenuItem_Click( object sender, EventArgs e )
        {
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        // Show about form
        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }

        // Copy current image to clipboard
        private void copyImageClipboardToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                Clipboard.SetImage( pictureBox.Image );
            }
        }

        // Update status of "copy to clipboard" menu item
        private void toolsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            copyImageClipboardToolStripMenuItem.Enabled = ( pictureBox.Image != null );
        }

        // Show histogram window
        private void showhistogramToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( histogramForm == null )
            {
                histogramForm = new HistogramForm( );
                histogramForm.FormClosing += delegate( object eventSender, FormClosingEventArgs eventArgs )
                {
                    histogramForm = null;
                };
            }

            histogramForm.Show( );
            ShowCurrentImageHistogram( );
        }

        // Show histomgram for 
        private void ShowCurrentImageHistogram( )
        {
            if ( ( pictureBox.Image != null ) && ( histogramForm != null ) )
            {
                histogramForm.SetImageStatistics( new ImageStatistics( (Bitmap) pictureBox.Image ) );
            }
        }
    }
}
