// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Drawing;
	using System.Drawing.Imaging;

	/// <summary>
	/// Blob counter - counts objects in binrary image
	/// </summary>
	/// 
	/// <remarks>The class counts and extracts stand alone objects in
	/// binary images.</remarks>
	/// 
	public class BlobCounter
	{
		private int		objectsCount;
		private int[]	objectLabels;

		/// <summary>
		/// Objects count
		/// </summary>
		public int ObjectsCount
		{
			get { return objectsCount; }
		}

		/// <summary>
		/// Object labels
		/// </summary>
		/// 
		/// <remarks>The array of <b>width</b> * <b>height</b> size, which holds
		/// labels for all objects. The background is represented with <b>0</b> value,
		/// but objects are represented with labels starting from <b>1</b>.</remarks>
		/// 
		public int[] ObjectLabels
		{
			get { return objectLabels; }
		}

		/// <summary>
		/// Builds objects map
		/// </summary>
		/// 
		/// <param name="image">Source binary image</param>
		/// 
		/// <remarks>Processes the image and builds objects map, which used later to extracts blobs.</remarks>
		/// 
		public void ProcessImage( Bitmap image )
		{
			// lock source bitmap data
			BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			// process image
			ProcessImage( imageData );

			// unlock source images
			image.UnlockBits( imageData );
		}

		/// <summary>
		/// Builds objects map
		/// </summary>
		/// 
		/// <param name="imageData">Source image data</param>
		/// 
		/// <remarks>Processes the image and builds objects map, which used later to extracts blobs.</remarks>
		/// 
		public void ProcessImage( BitmapData imageData )
		{
			// check for grayscale image
			// actually we need binary image, but binary images are
			// represented as grayscale
			if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
				throw new ArgumentException( );

			// get source image size
			int width = imageData.Width;
			int height = imageData.Height;
			int stride = imageData.Stride;
			int offset = stride - width;

			// we don't want one pixel width images
			if ( width == 1 )
				throw new ArgumentException( "Too small image" );

			// allocate labels array
			objectLabels = new int[width * height];
			// initial labels count
			int labelsCount = 0;

			// create map
			int		maxObjects = ( ( width / 2 ) + 1 ) * ( ( height / 2 ) + 1 ) + 1;
			int[]	map = new int[maxObjects];

			// initially map all labels to themself
			for ( int i = 0; i < maxObjects; i++ )
			{
				map[i] = i;
			}

			// do the job
			unsafe
			{
				byte*	src = (byte *) imageData.Scan0.ToPointer( );
				int		p = 0;

				// 1 - for pixels of the first row
				if ( *src != 0 )
				{
					objectLabels[p] = ++labelsCount;
				}
				++src;
				++p;

				// process the rest of the first row
				for ( int x = 1; x < width; x++, src++, p++ )
				{
					// check if we need to label current pixel
					if ( *src != 0 )
					{
						// check if the previous pixel already was labeled
						if ( src[-1] != 0 )
						{
							// label current pixel, as the previous
							objectLabels[p] = objectLabels[p - 1];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
				}
				src += offset;

				// 2 - for other rows
				// for each row
				for ( int y = 1; y < height; y++ )
				{
					// for the first pixel of the row, we need to check
					// only upper and upper-right pixels
					if ( *src != 0 )
					{
						// check surrounding pixels
						if ( src[-stride] != 0 )
						{
							// label current pixel, as the above
							objectLabels[p] = objectLabels[p - width];
						}
						else if ( src[1 - stride] != 0 )
						{
							// label current pixel, as the above right
							objectLabels[p] = objectLabels[p + 1 - width];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
					++src;
					++p;

					// check left pixel and three upper pixels for the rest of pixels
					for ( int x = 1; x < width - 1; x++, src++, p++ )
					{
						if ( *src != 0 )
						{
							// check surrounding pixels
							if ( src[-1] != 0 )
							{
								// label current pixel, as the left
								objectLabels[p] = objectLabels[p - 1];
							}
							else if ( src[-1 - stride] != 0 )
							{
								// label current pixel, as the above left
								objectLabels[p] = objectLabels[p - 1 - width];
							}
							else if ( src[-stride] != 0 )
							{
								// label current pixel, as the above
								objectLabels[p] = objectLabels[p - width];
							}

							if ( src[1 - stride] != 0 )
							{
								if ( objectLabels[p] == 0 )
								{
									// label current pixel, as the above right
									objectLabels[p] = objectLabels[p + 1 - width];
								}
								else
								{
									int	l1 = objectLabels[p];
									int l2 = objectLabels[p + 1 - width];

									if ( ( l1 != l2 ) && ( map[l1] != map[l2] ) )
									{
										// merge
										if ( map[l1] == l1 )
										{
											// map left value to the right
											map[l1] = map[l2];
										}
										else if ( map[l2] == l2 )
										{
											// map right value to the left
											map[l2] = map[l1];
										}
										else
										{
											// both values already mapped
											map[ map[l1] ] = map[l2];
											map[l1] = map[l2];
										}

										// reindex
										for ( int i = 1; i <= labelsCount; i++ )
										{
											if ( map[i] != i )
											{
												// reindex
												int j = map[i];
												while ( j != map[j] )
												{
													j = map[j];
												}
												map[i] = j;
											}
										}
									}
								}
							}

							// label the object if it is not yet
							if ( objectLabels[p] == 0 )
							{
								// create new label
								objectLabels[p] = ++labelsCount;
							}
						}
					}

					// for the last pixel of the row, we need to check
					// only upper and upper-left pixels
					if ( *src != 0 )
					{
						// check surrounding pixels
						if ( src[-1] != 0 )
						{
							// label current pixel, as the left
							objectLabels[p] = objectLabels[p - 1];
						}
						else if ( src[-1 - stride] != 0 )
						{
							// label current pixel, as the above left
							objectLabels[p] = objectLabels[p - 1 - width];
						}
						else if ( src[-stride] != 0 )
						{
							// label current pixel, as the above
							objectLabels[p] = objectLabels[p - width];
						}
						else
						{
							// create new label
							objectLabels[p] = ++labelsCount;
						}
					}
					++src;
					++p;

					src += offset;
				}
			}

			// allocate remapping array
			int[] reMap = new int[map.Length];

			// count objects and prepare remapping array
			objectsCount = 0;
			for ( int i = 1; i <= labelsCount; i++ )
			{
				if ( map[i] == i )
				{
					// increase objects count
					reMap[i] = ++objectsCount;
				}
			}
			// second pass to compete remapping
			for ( int i = 1; i <= labelsCount; i++ )
			{
				if ( map[i] != i )
				{
					reMap[i] = reMap[map[i]];
				}
			}

			// repair object labels
			for ( int i = 0, n = objectLabels.Length; i < n; i++ )
			{
				objectLabels[i] = reMap[objectLabels[i]];
			}
		}

		/// <summary>
		/// Gets objects rectangles
		/// </summary>
		/// 
		/// <param name="image">Source image</param>
		/// 
		/// <returns>Returns array of objects rectangles</returns>
		/// 
		/// <remarks>The method returns array of objects rectangles. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be calls, which will build objects map.</remarks>
		/// 
		public Rectangle[] GetObjectRectangles( Bitmap image )
		{
			// lock source bitmap data
			BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			// process image
			Rectangle[] rects = GetObjectRectangles( imageData );

			// unlock source images
			image.UnlockBits( imageData );

			return rects;
		}

		/// <summary>
		/// Gets objects rectangles
		/// </summary>
		/// 
		/// <param name="imageData">Source image data</param>
		/// 
		/// <returns>Returns array of objects rectangles</returns>
		/// 
		/// <remarks>The method returns array of objects rectangles. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be calls, which will build objects map.</remarks>
        /// 
		public Rectangle[] GetObjectRectangles( BitmapData imageData )
		{
			// process the image
			ProcessImage( imageData );

			// image size
			int width = imageData.Width;
			int height = imageData.Height;
			int i = 0, label;

			// create object coordinates arrays
			int[] x1 = new int[objectsCount + 1];
			int[] y1 = new int[objectsCount + 1];
			int[] x2 = new int[objectsCount + 1];
			int[] y2 = new int[objectsCount + 1];

			for ( int j = 1; j <= objectsCount; j++ )
			{
				x1[j] = width;
				y1[j] = height;
			}

			// walk through labels array
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++, i++ )
				{
					// get current label
					label = objectLabels[i];

					// skip unlabeled pixels
					if ( label == 0 )
						continue;

					// check and update all coordinates

					if ( x < x1[label] )
					{
						x1[label] = x;
					}
					if ( x > x2[label] )
					{
						x2[label] = x;
					}
					if ( y < y1[label] )
					{
						y1[label] = y;
					}
					if ( y > y2[label] )
					{
						y2[label] = y;
					}
				}
			}

			// create rectangles
			Rectangle[] rects = new Rectangle[objectsCount];

			for ( int j = 1; j <= objectsCount; j++ )
			{
				rects[j - 1] = new Rectangle( x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1 );
			}

			return rects;
		}

		/// <summary>
		/// Gets blobs
		/// </summary>
		/// 
		/// <param name="image">Source image</param>
		/// 
		/// <returns>Returns array of blobs</returns>
		/// 
		/// <remarks>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be calls, which will build objects map.</remarks>
        /// 
		public Blob[] GetObjects( Bitmap image )
		{
			// lock source bitmap data
			BitmapData imageData = image.LockBits(
				new Rectangle( 0, 0, image.Width, image.Height ),
				ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

			// process image
			Blob[] blobs= GetObjects( imageData );

			// unlock source images
			image.UnlockBits( imageData );

			return blobs;
		}

		/// <summary>
		/// Gets blobs
		/// </summary>
		/// 
		/// <param name="imageData">Source image data</param>
		/// 
		/// <returns>Returns array of blobs</returns>
		/// 
		/// <remarks>The method returns array of blobs. Before calling the
        /// method, the <see cref="ProcessImage(Bitmap)"/> or <see cref="ProcessImage(BitmapData)"/>
        /// method should be calls, which will build objects map.</remarks>
        /// 
		public Blob[] GetObjects( BitmapData imageData )
		{
			// process the image
			ProcessImage( imageData );

			// image size
			int width = imageData.Width;
			int height = imageData.Height;
			int i = 0, label;

			// --- STEP 1 - find each objects coordinates

			// create object coordinates arrays
			int[] x1 = new int[objectsCount + 1];
			int[] y1 = new int[objectsCount + 1];
			int[] x2 = new int[objectsCount + 1];
			int[] y2 = new int[objectsCount + 1];

			for ( int k = 1; k <= objectsCount; k++ )
			{
				x1[k] = width;
				y1[k] = height;
			}

			// walk through labels array
			for ( int y = 0; y < height; y++ )
			{
				for ( int x = 0; x < width; x++, i++ )
				{
					// get current label
					label = objectLabels[i];

					// skip unlabeled pixels
					if ( label == 0 )
						continue;

					// check and update all coordinates

					if ( x < x1[label] )
					{
						x1[label] = x;
					}
					if ( x > x2[label] )
					{
						x2[label] = x;
					}
					if ( y < y1[label] )
					{
						y1[label] = y;
					}
					if ( y > y2[label] )
					{
						y2[label] = y;
					}
				}
			}

			// --- STEP 2 - get each object
			Blob[] objects = new Blob[objectsCount];

			int srcStride = imageData.Stride;

			// create each image
			for ( int k = 1; k <= objectsCount; k++ )
			{
				int xmin = x1[k];
				int xmax = x2[k];
				int ymin = y1[k];
				int ymax = y2[k];
				int objectWidth = xmax - xmin + 1;
				int objectHeight = ymax - ymin + 1;

				// create new image
				Bitmap dstImg = AForge.Imaging.Image.CreateGrayscaleImage( objectWidth, objectHeight );

				// lock destination bitmap data
				BitmapData dstData = dstImg.LockBits(
					new Rectangle( 0, 0, objectWidth, objectHeight ),
					ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

				// copy image
				unsafe
				{
					byte * src = (byte *) imageData.Scan0.ToPointer( ) + ymin * srcStride + xmin;
					byte * dst = (byte *) dstData.Scan0.ToPointer( );
					int p = ymin * width + xmin;

					int srcOffset = srcStride - objectWidth;
					int dstOffset = dstData.Stride - objectWidth;
					int labelsOffset = width - objectWidth;

					// for each line
					for ( int y = ymin; y <= ymax; y++ )
					{
						// copy each pixel
						for ( int x = xmin; x <= xmax; x++, src++, dst++, p++ )
						{
							if ( objectLabels[p] == k )
								*dst = *src;
						}
						src += srcOffset;
						dst += dstOffset;
						p += labelsOffset;
					}
				}
				// unlock destination image
				dstImg.UnlockBits( dstData );

				objects[k - 1] = new Blob( dstImg, new Point( xmin, ymin ) );
			}

			return objects;
		}
	}
}
