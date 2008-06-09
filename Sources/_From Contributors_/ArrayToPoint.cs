using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

// SUSAN corners detection
// Copyright © Frank Nagl, 2007
// admin@franknagl.de
//
namespace AForge.Imaging
{
    /// <summary>
    /// Static class with helper functions for the SUSAN corners detector
    /// </summary>
    /// <author>Frank Nagl, admin@franknagl.de</author>
    public static class ArrayToPoint
    {
        /// <summary>
        /// Transforms the index-position in the one-dimensional 
        /// image-Array into a twodimnesional point (x,y-coordinates).
        /// </summary>
        /// <param name="pPos">The index-position</param>
        /// <param name="pArraySize">The Arraysize</param>
        /// <param name="pStride">Bitmapwidth inclusive offset (=Image.Width+Offset)</param>
        /// <returns>The twodimensional point (x,y-coordinate)</returns>
        public static Point transformInXYCoord(int pPos, int pArraySize, int pStride)
        {
            Point tCoord = new Point();
            tCoord.Y = pPos / pStride;
            tCoord.X = pPos - tCoord.Y * pStride;
            return tCoord;
        }

        /// <summary>
        /// Transforms an two-dimensional Point into a one-dimensional array-index.
        /// </summary>
        /// <param name="pPoint">The original Point to transform</param>
        /// <param name="pStride">Bitmapwidth inclusive offset (=Image.Width+Offset)</param>
        /// <returns>The one-dimensional array-index</returns>
        public static int transformXYCoordInBytes(Point pPoint, int pStride)
        {
            int tPos = pStride * pPoint.Y + pPoint.X;
            return tPos;
        }
    }
}
