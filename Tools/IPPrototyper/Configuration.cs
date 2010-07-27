// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;

namespace IPPrototyper
{
    // Manages/loads/saves application's configuration
    internal class Configuration
    {
        private static Configuration singleton = null;

        private const string baseConfigFileName = "ipprototyper.cfg";
        private string configFileName = null;
        bool isSuccessfullyLoaded = false;

        private List<string> recentFolders = new List<string>( );

        // Configuration load status
        public bool IsSuccessfullyLoaded
        {
            get { return isSuccessfullyLoaded; }
        }

        // List of recently opened folders
        public ReadOnlyCollection<string> RecentFolders
        {
            get { return recentFolders.AsReadOnly( ); }
        }

        // Disable making class instances
        private Configuration( )
        {
            configFileName = Path.Combine(
                Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                baseConfigFileName );
        }

        // Get instance of the configuration storage
        public static Configuration Instance
        {
            get
            {
                if ( singleton == null )
                {
                    singleton = new Configuration( );
                }
                return singleton;
            }
        }

        // Save application's configuration
        public void Save( )
        {
            lock ( baseConfigFileName )
            {
                try
                {
                    // open file
                    FileStream fs = new FileStream( configFileName, FileMode.Create );
                    // create XML writer
                    XmlTextWriter xmlOut = new XmlTextWriter( fs, Encoding.UTF8 );

                    // use indenting for readability
                    xmlOut.Formatting = Formatting.Indented;

                    // start document
                    xmlOut.WriteStartDocument( );
                    xmlOut.WriteComment( "IPPrototyper configuration file" );

                    // main node
                    xmlOut.WriteStartElement( "IPPrototyper" );

                    // recent folders
                    xmlOut.WriteStartElement( "Recent" );
                    xmlOut.WriteAttributeString( "count", recentFolders.Count.ToString( ) );

                    foreach ( string folderName in recentFolders )
                    {
                        xmlOut.WriteStartElement( "Folder" );
                        xmlOut.WriteString( folderName );
                        xmlOut.WriteEndElement( );
                    }

                    xmlOut.WriteEndElement( ); // end of recent folders


                    xmlOut.WriteEndElement( ); // end of main node
                    // close file
                    xmlOut.Close( );
                }
                catch
                {
                }
            }
        }

        // Load application's configration
        public bool Load( )
        {
            isSuccessfullyLoaded = false;

            lock ( baseConfigFileName )
            {
                // check file existance
                if ( File.Exists( configFileName ) )
                {
                    FileStream fs = null;
                    XmlTextReader xmlIn = null;

                    try
                    {
                        // open file
                        fs = new FileStream( configFileName, FileMode.Open );
                        // create XML reader
                        xmlIn = new XmlTextReader( fs );

                        xmlIn.WhitespaceHandling = WhitespaceHandling.None;
                        xmlIn.MoveToContent( );

                        // check main node
                        if ( xmlIn.Name != "IPPrototyper" )
                            throw new ApplicationException( );

                        // move to next node
                        xmlIn.Read( );

                        // check Recent node
                        if ( xmlIn.Name != "Recent" )
                            throw new ApplicationException( );

                        int recentCount = int.Parse( xmlIn.GetAttribute( "count" ) );

                        if ( recentCount > 0 )
                        {
                            for ( int i = 0; i < recentCount; i++ )
                            {
                                xmlIn.Read( );
                                xmlIn.Read( );
                                recentFolders.Add( xmlIn.ReadContentAsString( ) );
                            }

                            // read end element
                            xmlIn.Read( );
                        }

                        isSuccessfullyLoaded = true;
                        // ignore the rest
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if ( xmlIn != null )
                            xmlIn.Close( );
                    }
                }
            }

            return isSuccessfullyLoaded;
        }

        // Add folder to the list of recently used folders
        public void AddRecentFolder( string folderName )
        {
            lock ( baseConfigFileName )
            {
                int index = recentFolders.IndexOf( folderName );

                if ( index != 0 )
                {
                    if ( index != -1 )
                    {
                        // remove previous entry
                        recentFolders.RemoveAt( index );
                    }

                    // put this folder as the most recent
                    recentFolders.Insert( 0, folderName );

                    if ( recentFolders.Count > 7 )
                    {
                        recentFolders.RemoveAt( 7 );
                    }
                }
            }
        }

        // Remove specified folder from the list of recently used folders
        public void RemoveRecentFolder( string folderName )
        {
            lock ( baseConfigFileName )
            {
                recentFolders.Remove( folderName );
            }
        }
    }
}
