// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;
	using AForge;

	/// <summary>
	/// Core image relatad methods.
	/// </summary>
	/// 
	/// <remarks>All methods of this class are static and represent general routines
	/// used by different image processing classes.</remarks>
	/// 
	public sealed class Image
	{
		// Avoid class instantiation
		private Image( ) { }

		/// <summary>
		/// Check if the image is grayscale.
		/// </summary>
		/// 
		/// <param name="image">Image to check.</param>
		/// 
		/// <returns>Returns <b>true</b> if the image is grayscale or <b>false</b> otherwise.</returns>
		/// 
		/// <remarks>The methods check if the image is a grayscale image of 256 gradients.
		/// The method first examines if the image's pixel format is
		/// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
		/// and then it examines its palette to check if the image is grayscale or not.</remarks>
		/// 
		public static bool IsGrayscale( Bitmap image )
		{
			bool ret = false;

			// check pixel format
			if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
			{
				ret = true;
				// check palette
				ColorPalette cp = image.Palette;
				Color c;
				// init palette
				for ( int i = 0; i < 256; i++ )
				{
					c = cp.Entries[i];
					if ( ( c.R != i ) || ( c.G != i ) || ( c.B != i ) )
					{
						ret = false;
						break;
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Create and initialize new grayscale image.
		/// </summary>
		/// 
		/// <param name="width">Image width.</param>
		/// <param name="height">Image height.</param>
		/// 
		/// <returns>Returns the created grayscale image.</returns>
		/// 
		/// <remarks>The methods create new grayscale image and initializes its palette.
		/// Grayscale image is represented as
		/// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
		/// image with palette initialized to 256 gradients of gray color.</remarks>
		/// 
		public static Bitmap CreateGrayscaleImage( int width, int height )
		{
			// create new image
			Bitmap image = new Bitmap( width, height, PixelFormat.Format8bppIndexed );
			// set palette to grayscale
			SetGrayscalePalette( image );
			// return new image
			return image;
		}

		/// <summary>
		/// Set pallete of the image to grayscale.
		/// </summary>
		/// 
		/// <param name="image">Image to initialize.</param>
		/// 
		/// <remarks>The method initializes palette of
		/// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
		/// image with 256 gradients of gray color.</remarks>
		/// 
		public static void SetGrayscalePalette( Bitmap image )
		{
			// check pixel format
			if ( image.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get palette
			ColorPalette cp = image.Palette;
			// init palette
			for ( int i = 0; i < 256; i++ )
			{
				cp.Entries[i] = Color.FromArgb( i, i, i );
			}
			// set palette back
			image.Palette = cp;
		}

		/// <summary>
		/// Clone image.
		/// </summary>
		/// 
        /// <param name="source">Source image.</param>
		/// <param name="format">Pixel format of result image.</param>
		/// 
		/// <returns>Returns clone of the source image with specified pixel format.</returns>
		///
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create a clone with specified pixel format.
		/// More of it, the original method does not create an actual clone - it does not create a copy
		/// of the image. That is why this method was implemented to provide the functionality.</remarks> 
		///
		public static Bitmap Clone( Bitmap source, PixelFormat format )
		{
			// copy image if pixel format is the same
            if ( source.PixelFormat == format )
                return Clone( source );

			int width = source.Width;
			int height = source.Height;

			// create new image with desired pixel format
			Bitmap bitmap = new Bitmap( width, height, format );

			// draw source image on the new one using Graphics
			Graphics g = Graphics.FromImage( bitmap );
            g.DrawImage( source, 0, 0, width, height );
			g.Dispose( );

			return bitmap;
		}

		/// <summary>
		/// Clone image.
		/// </summary>
		/// 
		/// <param name="source">Source image.</param>
		/// 
		/// <returns>Return clone of the source image.</returns>
		/// 
        /// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Rectangle, System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
        /// does not produce the desired result - it does not create an actual clone (it does not create a copy
		/// of the image). That is why this method was implemented to provide the functionality.</remarks> 
		/// 
		public static Bitmap Clone( Bitmap source )
		{
			// lock source bitmap data
            BitmapData sourceData = source.LockBits(
                new Rectangle( 0, 0, source.Width, source.Height ),
                ImageLockMode.ReadOnly, source.PixelFormat );

			// create new image
            Bitmap destination = Clone( sourceData );

            // unlock source image
            source.UnlockBits( sourceData );

			//
			if (
                ( source.PixelFormat == PixelFormat.Format1bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format4bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Format8bppIndexed ) ||
                ( source.PixelFormat == PixelFormat.Indexed ) )
			{
                ColorPalette srcPalette = source.Palette;
				ColorPalette dstPalette = destination.Palette;

				int n = srcPalette.Entries.Length;

				// copy pallete
				for ( int i = 0; i < n; i++ )
				{
					dstPalette.Entries[i] = srcPalette.Entries[i];
				}

				destination.Palette = dstPalette;
			}
			
			return destination;
		}

        /// <summary>
        /// Clone image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        ///
        /// <returns>Clones image from source image data. The message does not clone pallete in the
        /// case if the source image has indexed pixel format.</returns>
        /// 
        public static Bitmap Clone( BitmapData sourceData )
        {
            // get source image size
            int width = sourceData.Width;
            int height = sourceData.Height;

            // create new image
            Bitmap destination = new Bitmap( width, height, sourceData.PixelFormat );

            // lock destination bitmap data
            BitmapData destinationData = destination.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, destination.PixelFormat );

            Win32.memcpy( destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride );

            // unlock destination image
            destination.UnlockBits( destinationData );

            return destination;
        }

		/// <summary>
		/// Format an image.
		/// </summary>
		/// 
		/// <param name="image">Source image to format.</param>
		/// 
		/// <remarks>Formats the image to one of the formats, which are supported
		/// by the <b>AForge.Imaging</b> library. The image is left untouched in the
		/// case if it already of
		/// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>
		/// format or it is grayscale (<see cref="IsGrayscale"/>), otherwise the image converted to
		/// <see cref="System.Drawing.Imaging.PixelFormat">Format24bppRgb</see>
		/// format.</remarks>
		///
		public static void FormatImage( ref Bitmap image )
		{
			if (
				( image.PixelFormat != PixelFormat.Format24bppRgb ) &&
				( IsGrayscale( image ) == false )
				)
			{
				Bitmap tmp = image;
				// convert to 24 bits per pixel
				image = Clone( tmp, PixelFormat.Format24bppRgb );
				// delete old image
				tmp.Dispose( );
			}
		}
	}
}
