// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;

    /// <summary>
    /// Unsupported image format exception.
    /// </summary>
    /// 
    /// <remarks><para>The unsupported image format exception is thrown in the case when
    /// user passes an image of certain format to an image processing routine, which does
    /// not support the format.</para>
    /// </remarks>
    /// 
    public class UnsupportedImageFormat : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormat"/> class.
        /// </summary>
        public UnsupportedImageFormat( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormat"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public UnsupportedImageFormat( string message ) :
            base( message ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormat"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public UnsupportedImageFormat( string message, string paramName ) :
            base( message, paramName ) { }
    }

    /// <summary>
    /// Invalid image properties exception.
    /// </summary>
    /// 
    /// <remarks><para>The invalid image properties exception is thrown in the case when
    /// user provides an image with certain properties, which are treated as invalid by
    /// particular image processing routine.</para>
    /// </remarks>
    /// 
    public class InvalidImageProperties : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImageProperties"/> class.
        /// </summary>
        public InvalidImageProperties( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImageProperties"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public InvalidImageProperties( string message ) :
            base( message ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImageProperties"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public InvalidImageProperties( string message, string paramName ) :
            base( message, paramName ) { }
    }
}