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

	/// <summary>
	/// Image's blob.
	/// </summary>
	/// 
	/// <remarks>The class represents a blob - part of another images. The
	/// class encapsulates the blob itself and information about its position
	/// in parent image.</remarks>
	/// 
	public class Blob : IDisposable
	{
		private Bitmap	image;		// blobs image
		private Point	location;	// blobs location
		private Bitmap	owner;		// image containing the blob

		private bool	disposed = false;

		/// <summary>
		/// Blob's image.
		/// </summary>
		public Bitmap Image
		{
			get { return image; }
		}

		/// <summary>
		/// Blob's location in parent (owner) image (see <see cref="Owner"/> property).
		/// </summary>
		public Point Location
		{
			get { return location; }
		}

		/// <summary>
		/// Blob's owner image.
		/// </summary>
		public Bitmap Owner
		{
			get { return owner; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Blob"/> class.
		/// </summary>
		/// 
		/// <param name="image">Blob's image.</param>
		/// <param name="location">Blob's location in parent image.</param>
		/// 
		public Blob( Bitmap image, Point location )
		{
			this.image		= image;
			this.location	= location;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Blob"/> class.
		/// </summary>
		/// 
		/// <param name="image">Blob's image.</param>
		/// <param name="location">Blob's location in parent image.</param>
		/// <param name="owner">Blob's owner image.</param>
		/// 
		public Blob( Bitmap image, Point location, Bitmap owner )
		{
			this.image		= image;
			this.location	= location;
			this.owner		= owner;
		}

		/// <summary>
		/// Class destructor.
		/// </summary>
		~Blob( )
		{
			Dispose( false );
		}

		/// <summary>
		/// Dispose the object.
		/// </summary>
		public void Dispose( )
		{
			Dispose( true );
			// Take yourself off of the Finalization queue 
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Object disposing routine.
		/// </summary>
		/// 
		/// <param name="disposing"><b>True</b> if the method is called from
		/// <see cref="AForge.Imaging.Blob.Dispose()"/> method, <b>false</b> if the method is called
		/// from destructor.</param>
		/// 
		protected virtual void Dispose( bool disposing )
		{
			if ( !disposed )
			{
				if ( disposing )
				{
					// dispose managed resources
					image.Dispose( );
				}
			}
		}
	}
}
