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
    /// not support the format. Check documentation of image the image processing routine
    /// to discover which formats are supported by the routine.</para>
    /// </remarks>
    /// 
    public class UnsupportedImageFormatException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
        /// </summary>
        public UnsupportedImageFormatException( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public UnsupportedImageFormatException( string message ) :
            base( message ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedImageFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public UnsupportedImageFormatException( string message, string paramName ) :
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
    public class InvalidImagePropertiesException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        public InvalidImagePropertiesException( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public InvalidImagePropertiesException( string message ) :
            base( message ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidImagePropertiesException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public InvalidImagePropertiesException( string message, string paramName ) :
            base( message, paramName ) { }
    }
}