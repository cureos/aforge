// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Replace channel of YCbCr color space.
    /// </summary>
    /// 
    /// <remarks>Replaces specified YCbCr channel of color image with
    /// specified grayscale imge.</remarks>
    /// 
    public class YCbCrReplaceChannel : FilterColorToColorPartial
    {
        private short channel = YCbCr.YIndex;
        private Bitmap channelImage;

        /// <summary>
        /// YCbCr channel to replace.
        /// </summary>
        public short Channel
        {
            get { return channel; }
            set
            {
                if (
                    ( value != YCbCr.YIndex ) &&
                    ( value != YCbCr.CbIndex ) &&
                    ( value != YCbCr.CrIndex )
                    )
                {
                    throw new ArgumentException( "Invalid YCbCr channel was specified" );
                }
                channel = value;
            }
        }

        /// <summary>
        /// Grayscale image to use for channel replacement.
        /// </summary>
        public Bitmap ChannelImage
        {
            get { return channelImage; }
            set
            {
                // check for not null
                if ( value == null )
                    throw new NullReferenceException( "Channel image was not specified" );
                // check for valid format
                if ( value.PixelFormat != PixelFormat.Format8bppIndexed )
                    throw new ArgumentException( "Channel image should be 8bpp indexed image (grayscale)" );

                channelImage = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YCbCrReplaceChannel"/> class.
        /// </summary>
        /// 
        /// <param name="channel">YCbCr channel to replace.</param>
        /// <param name="channelImage">Channel image to use for replacement.</param>
        /// 
        public YCbCrReplaceChannel( short channel, Bitmap channelImage )
        {
            this.Channel = channel;
            this.ChannelImage = channelImage;
        }


        /// <summary>
        /// Process the filter on the specified image
        /// </summary>
        /// 
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            int width   = imageData.Width;
            int height  = imageData.Height;
            int startX  = rect.Left;
            int startY  = rect.Top;
            int stopX   = startX + rect.Width;
            int stopY   = startY + rect.Height;
            int offset  = imageData.Stride - rect.Width * 3;

            // check channel's image dimension
            if ( ( width != channelImage.Width ) || ( height != channelImage.Height ) )
                throw new ArgumentException( "Channel image size does not match source image size" );

            // lock channel image
            BitmapData chData = channelImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            int     offsetCh = chData.Stride - rect.Width;
            RGB     rgb = new RGB( );
            YCbCr   ycbcr = new YCbCr( );

            // do the job
            byte* dst = (byte*) imageData.Scan0.ToPointer( );
            byte* ch = (byte*) chData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            dst += ( startY * imageData.Stride + startX * 3 );
            ch  += ( startY * chData.Stride + startX );

            // for each line
            for ( int y = startY; y < stopY; y++ )
            {
                // for each pixel
                for ( int x = startX; x < stopX; x++, dst += 3, ch++ )
                {
                    rgb.Red     = dst[RGB.R];
                    rgb.Green   = dst[RGB.G];
                    rgb.Blue    = dst[RGB.B];

                    // convert to YCbCr
                    AForge.Imaging.ColorConverter.RGB2YCbCr( rgb, ycbcr );

                    switch ( channel )
                    {
                        case YCbCr.YIndex:
                            ycbcr.Y = (double) *ch / 255;
                            break;

                        case YCbCr.CbIndex:
                            ycbcr.Cb = (double) *ch / 255 - 0.5;
                            break;

                        case YCbCr.CrIndex:
                            ycbcr.Cr = (double) *ch / 255 - 0.5;
                            break;
                    }

                    // convert back to RGB
                    AForge.Imaging.ColorConverter.YCbCr2RGB( ycbcr, rgb );

                    dst[RGB.R] = rgb.Red;
                    dst[RGB.G] = rgb.Green;
                    dst[RGB.B] = rgb.Blue;
                }
                dst += offset;
                ch += offsetCh;
            }
            // unlock
            channelImage.UnlockBits( chData );
        }
    }
}
