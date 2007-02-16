// AForge Framework
// Textures demo
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

using AForge.Imaging;
using AForge.Imaging.Textures;

namespace TexturesDemo
{
    public partial class MainForm : Form
    {
        ITextureGenerator textureGenerator = null;

        public MainForm( )
        {
            InitializeComponent( );

            // show first texture
            texturesCombo.SelectedIndex = 0;
        }

        // Texture changed
        private void texturesCombo_SelectedIndexChanged( object sender, EventArgs e )
        {
            // create texture generator
            switch ( texturesCombo.SelectedIndex )
            {
                case 0:     // clouds
                    textureGenerator = new CloudsTexture( );
                    break;
                case 1:     // marble
                    textureGenerator = new MarbleTexture( );
                    break;
                case 2:     // wood
                    textureGenerator = new WoodTexture( 7 );
                    break;
                case 3:     // labyrinth
                    textureGenerator = new LabyrinthTexture( );
                    break;
                case 4:     // textile
                    textureGenerator = new TextileTexture( );
                    break;
                default:
                    textureGenerator = null;
                    break;
            }

            // show texture
            ShowTexture( );
        }

        // Generate and show texture
        private void ShowTexture( )
        {
            // check generator
            if ( textureGenerator == null )
            {
                pictureBox.Image = null;
                return;
            }

            int width = pictureBox.ClientRectangle.Width;
            int height = pictureBox.ClientRectangle.Height;

            // generate texture
            float[,] texture = textureGenerator.Generate( width, height );

            // create grayscale image
            Bitmap image = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock image
            BitmapData imageData = image.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            unsafe
            {
                int offset = imageData.Stride - width;
                byte* dst = (byte*) imageData.Scan0.ToPointer( );

                // for each line
                for ( int y = 0; y < height; y++ )
                {
                    // for each pixel
                    for ( int x = 0; x < width; x++, dst++ )
                    {
                        *dst = (byte) ( texture[y, x] * 255.0f );
                    }
                    dst += offset;
                }
            }

            // unlock image
            image.UnlockBits( imageData );

            // show image
            pictureBox.Image = image;
        }

        // Regenerate texture
        private void regenerateButton_Click( object sender, EventArgs e )
        {
            if ( textureGenerator != null )
            {
                textureGenerator.Reset( );
                ShowTexture( );
            }
        }
    }
}