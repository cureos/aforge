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
    /// Image's blob.
    /// </summary>
    /// 
    /// <remarks><para>The class represents a blob - part of another images. The
    /// class encapsulates the blob itself and information about its position
    /// in parent image.</para>
    /// 
    /// <para><note>The class is not responsible for blob's image disposing, so it should be
    /// done manually when it is required.</note></para>
    /// </remarks>
    /// 
    public class Blob
    {
        // blob's image
        private Bitmap image;
        // blob's rectangle on the original image
        private Rectangle rect;
        // blob's ID in the original image
        private int id;

        /// <summary>
        /// Blob's image.
        /// </summary>
        ///
        /// <remarks><para>The property keeps blob's image. In the case if it equals to <b>null</b>,
        /// the image may be extracted using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( BitmapData, Blob )"/> method.</para></remarks>
        ///
        public Bitmap Image
        {
            get { return image; }
            internal set { image = value; }
        }

        /// <summary>
        /// Blob's rectangle on the original image.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return rect; }
        }

        /// <summary>
        /// Blob's ID on the original image.
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="id">Blob's ID in the original image.</param>
        /// <param name="rect">Blob's rectangle on the original image.</param>
        /// 
        /// <remarks><para>This constructor leaves <see cref="Image"/> property not initialized. The blob's
        /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( BitmapData, Blob )"/> method.</para></remarks>
        /// 
        public Blob( int id, Rectangle rect )
        {
            this.id = id;
            this.rect = rect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="id">Blob's ID in the original image.</param>
        /// <param name="rect">Blob's rectangle on the original image.</param>
        /// <param name="image">Blob's image.</param>
        /// 
        public Blob( int id, Rectangle rect, Bitmap image )
        {
            this.id = id;
            this.rect = rect;
            this.image = image;
        }
    }
}
