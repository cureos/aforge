// Image Processing Prototyper
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using AForge.Imaging.IPPrototyper;

namespace IPPrototyper
{
    // The class, which keeps image processing log
    internal class ImageProcessingLog : IImageProcessingLog
    {
        private Dictionary<string, Bitmap> images = new Dictionary<string,Bitmap>( );
        private List<string> messages = new List<string>( );

        // Collection of images representing image processing steps
        public Dictionary<string, Bitmap> Images
        {
            get { return images; }
        }

        // List of log messages
        public List<string> Messages
        {
            get { return messages; }
        }

        // Clears current the log
        public void Clear( )
        {
            foreach ( KeyValuePair<string, Bitmap> kvp in images )
            {
                kvp.Value.Dispose( );
            }
            images.Clear( );
            messages.Clear( );
        }

        // Add new image to the log
        public void AddImage( string key, Bitmap image )
        {
            Bitmap imageToStore = (Bitmap) image.Clone( );

            if ( images.ContainsKey( key ) )
            {
                images[key].Dispose( );
                images[key] = imageToStore;
            }
            else
            {
                images.Add( key, imageToStore );
            }
        }

        // Add message to log
        public void AddMessage( string message )
        {
            messages.Add( message );
        }
    }
}
