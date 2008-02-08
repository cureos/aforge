using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NXTTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( )
        {
            sbyte a = -50;
            sbyte b = 50;

            System.Diagnostics.Debug.WriteLine( (byte) a );
            System.Diagnostics.Debug.WriteLine( (byte) b );


            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new MainForm( ) );
        }
    }
}