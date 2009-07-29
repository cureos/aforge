// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Image in unmanaged memory.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>The class represents wrapper of an image in unmanaged memory. Using this class
    /// it is possible as to allocate new image in unmanaged memory, as to just wrap provided
    /// pointer to unmanaged memory, where an image is stored.</para>
    /// </remarks>
    /// 
    public class UnmanagedImage : IDisposable
    {
        // pointer to image data in unmanaged memory
        private IntPtr imageData;
        // image size
        private int width, height;
        // image stride (line size)
        private int stride;
        // image pixel format
        private PixelFormat pixelFormat;
        // flag which indicates if the image should be disposed or not
        private bool mustBeDisposed = false;

        /// <summary>
        /// Pointer to image data in unmanaged memory.
        /// </summary>
        public IntPtr ImageData
        {
            get { return imageData; }
        }

        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Image stride (line size in bytes).
        /// </summary>
        public int Stride
        {
            get { return stride; }
        }

        /// <summary>
        /// Image pixel format.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get { return pixelFormat; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        /// <param name="imageData">Pointer to image data in unmanaged memory.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="stride">Image stride (line size in bytes).</param>
        /// <param name="pixelFormat">Image pixel format.</param>
        /// 
        /// <remarks><para><note>Using this constructor, make sure all specified image attributes are correct
        /// and correspond to unmanaged memory buffer. If some attributes are specified incorrectly,
        /// this may lead to exceptions working with the unmanaged memory.</note></para></remarks>
        /// 
        public UnmanagedImage( IntPtr imageData, int width, int height, int stride, PixelFormat pixelFormat )
        {
            this.imageData   = imageData;
            this.width       = width;
            this.height      = height;
            this.stride      = stride;
            this.pixelFormat = pixelFormat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        /// <param name="bitmapData">Locked bitmap data.</param>
        /// 
        /// <remarks><note>Unlike <see cref="FromManagedImage(BitmapData)"/> method, this constructor does not make
        /// copy of managed image. This means that managed image must stay locked for the time of using the instance
        /// of unamanged image.</note></remarks>
        /// 
        public UnmanagedImage( BitmapData bitmapData )
        {
            this.imageData   = bitmapData.Scan0;
            this.width       = bitmapData.Width;
            this.height      = bitmapData.Height;
            this.stride      = bitmapData.Stride;
            this.pixelFormat = bitmapData.PixelFormat;
        }

        /// <summary>
        /// Destroys the instance of the <see cref="UnmanagedImage"/> class.
        /// </summary>
        /// 
        ~UnmanagedImage( )
        {
            Dispose( false );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <remarks><para>Frees unmanaged resources used by the object. The object becomes unusable
        /// after that.</para>
        /// 
        /// <par><note>The method needs to be called only in the case if unmanaged image was allocated
        /// using <see cref="Create"/> method. In the case if the class instance was created using constructor,
        /// this method does not free unmanaged memory.</note></par>
        /// </remarks>
        /// 
        public void Dispose( )
        {
            Dispose( true );
            // remove me from the Finalization queue 
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Dispose the object.
        /// </summary>
        /// 
        /// <param name="disposing">Indicates if disposing was initiated manually.</param>
        /// 
        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // dispose managed resources
            }
            // free image memory if the image was allocated using this class
            if ( ( mustBeDisposed ) && ( imageData != IntPtr.Zero ) )
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal( imageData );
                imageData = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Clone the unmanaged images.
        /// </summary>
        /// 
        /// <returns>Returns clone of the unmanaged image.</returns>
        /// 
        /// <remarks><para>The method does complete cloning of the object.</para></remarks>
        /// 
        public UnmanagedImage Clone( )
        {
            // allocate memory for the image
            IntPtr newImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( stride * height );

            UnmanagedImage newImage = new UnmanagedImage( newImageData, width, height, stride, pixelFormat );
            newImage.mustBeDisposed = true;

            AForge.SystemTools.CopyUnmanagedMemory( newImageData, imageData, stride * height );

            return newImage;
        }

        /// <summary>
        /// Copy unmanaged image.
        /// </summary>
        /// 
        /// <param name="destImage">Destination image to copy this image to.</param>
        /// 
        /// <remarks><para>The method copies current unmanaged image to the specified image.
        /// Size and pixel format of the destination image must be exactly the same.</para></remarks>
        /// 
        /// <exception cref="InvalidImagePropertiesException">Destination image has different size or pixel format.</exception>
        /// 
        public void Copy( UnmanagedImage destImage )
        {
            if (
                ( width != destImage.width ) || ( height != destImage.height ) ||
                ( pixelFormat != destImage.pixelFormat ) )
            {
                throw new InvalidImagePropertiesException( "Destination image has different size or pixel format." );
            }

            if ( stride == destImage.stride )
            {
                // copy entire image
                AForge.SystemTools.CopyUnmanagedMemory( destImage.imageData, imageData, stride * height );
            }
            else
            {
                unsafe
                {
                    int dstStride = destImage.stride;
                    int copyLength = ( stride < dstStride ) ? stride : dstStride;

                    byte* src = (byte*) imageData.ToPointer( );
                    byte* dst = (byte*) destImage.imageData.ToPointer( );

                    // copy line by line
                    for ( int i = 0; i < height; i++ )
                    {
                        AForge.SystemTools.CopyUnmanagedMemory( dst, src, copyLength );

                        dst += dstStride;
                        src += stride;
                    }
                }
            }
        }

        /// <summary>
        /// Allocate new image in unmanaged memory.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// <param name="pixelFormat">Image pixel format.</param>
        /// 
        /// <returns>Return image allocated in unmanaged memory.</returns>
        /// 
        /// <remarks><para>Allocate new image with specified attributes in unmanaged memory.</para>
        /// 
        /// <para><note>The method supports only
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format16bppGrayScale</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppRgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format32bppArgb</see>,
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format48bppRgb</see> and
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format64bppArgb</see> pixel formats.
        /// In the case if <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// format is specified, pallete is not not created for the image (supposed that it is
        /// 8 bpp grayscale image).
        /// </note></para>
        /// </remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format was specified.</exception>
        /// <exception cref="InvalidImagePropertiesException">Invalid image size was specified.</exception>
        /// 
        public static UnmanagedImage Create( int width, int height, PixelFormat pixelFormat )
        {
            int bytesPerPixel = 0 ;

            // calculate bytes per pixel
            switch ( pixelFormat )
            {
                case PixelFormat.Format8bppIndexed:
                    bytesPerPixel = 1;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    bytesPerPixel = 2;
                    break;
                case PixelFormat.Format24bppRgb:
                    bytesPerPixel = 3;
                    break;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    bytesPerPixel = 4;
                    break;
                case PixelFormat.Format48bppRgb:
                    bytesPerPixel = 6;
                    break;
                case PixelFormat.Format64bppArgb:
                    bytesPerPixel = 8;
                    break;
                default:
                    throw new UnsupportedImageFormatException( "Can not create image with specified pixel format." );
            }

            // check image size
            if ( ( width <= 0 ) || ( height <= 0 ) )
            {
                throw new InvalidImagePropertiesException( "Invalid image size specified." );
            }

            // calculate stride
            int stride = width * bytesPerPixel;

            if ( stride % 4 != 0 )
            {
                stride += ( 4 - ( stride % 4 ) );
            }

            // allocate memory for the image
            IntPtr imageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( stride * height );

            UnmanagedImage image = new UnmanagedImage( imageData, width, height, stride, pixelFormat );
            image.mustBeDisposed = true;

            return image;
        }

        /// <summary>
        /// Create managed image from the unmanaged.
        /// </summary>
        /// 
        /// <returns>Returns managed copy of the unmanaged image.</returns>
        /// 
        /// <remarks><para>The method creates a managed copy of the unmanaged image with the
        /// same size and pixel format.</para></remarks>
        /// 
        public Bitmap ToManagedImage( )
        {
            // create new image of required format
            Bitmap dstImage = ( pixelFormat == PixelFormat.Format8bppIndexed ) ?
                AForge.Imaging.Image.CreateGrayscaleImage( width, height ) :
                new Bitmap( width, height, pixelFormat );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, pixelFormat );

            int dstStride = dstData.Stride;
            int lineSize  = Math.Min( stride, dstStride );

            unsafe
            {
                byte* dst = (byte*) dstData.Scan0.ToPointer( );
                byte* src = (byte*) imageData.ToPointer( );

                // copy image
                for ( int y = 0; y < height; y++ )
                {
                    AForge.SystemTools.CopyUnmanagedMemory( dst, src, lineSize );
                    dst += dstStride;
                    src += stride;
                }
            }

            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Create unmanaged image from the specified managed image.
        /// </summary>
        /// 
        /// <param name="image">Source managed image.</param>
        /// 
        /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
        /// 
        /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
        /// in unmanaged memory.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
        /// 
        public static UnmanagedImage FromManagedImage( Bitmap image )
        {
            UnmanagedImage dstImage = null;

            BitmapData sourceData = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, image.PixelFormat );

            try
            {
                dstImage = FromManagedImage( sourceData );
            }
            finally
            {
                image.UnlockBits( sourceData );
            }

            return dstImage;
        }

        /// <summary>
        /// Create unmanaged image from the specified managed image.
        /// </summary>
        /// 
        /// <param name="imageData">Source locked image data.</param>
        /// 
        /// <returns>Returns new unmanaged image, which is a copy of source managed image.</returns>
        /// 
        /// <remarks><para>The method creates an exact copy of specified managed image, but allocated
        /// in unmanaged memory. This means that managed image may be unlocked right after call to this
        /// method.</para></remarks>
        /// 
        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of source image.</exception>
        /// 
        public static UnmanagedImage FromManagedImage( BitmapData imageData )
        {
            PixelFormat pixelFormat = imageData.PixelFormat;

            // check source pixel format
            if (
                ( pixelFormat != PixelFormat.Format8bppIndexed ) &&
                ( pixelFormat != PixelFormat.Format16bppGrayScale ) &&
                ( pixelFormat != PixelFormat.Format24bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppRgb ) &&
                ( pixelFormat != PixelFormat.Format32bppArgb ) &&
                ( pixelFormat != PixelFormat.Format48bppRgb ) &&
                ( pixelFormat != PixelFormat.Format64bppArgb ) )
            {
                throw new UnsupportedImageFormatException( "Unsupported pixel format of the source image." );
            }

            // allocate memory for the image
            IntPtr dstImageData = System.Runtime.InteropServices.Marshal.AllocHGlobal( imageData.Stride * imageData.Height );

            UnmanagedImage image = new UnmanagedImage( dstImageData, imageData.Width, imageData.Height, imageData.Stride, pixelFormat );
            AForge.SystemTools.CopyUnmanagedMemory( dstImageData, imageData.Scan0, imageData.Stride * imageData.Height );
            image.mustBeDisposed = true;

            return image;
        }
    }
}
