// AForge Framework
// Textures demo
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TexturesDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( )
        {
            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new MainForm( ) );
        }
    }
}