/*
 *  This file is adapted from Code Project article "Image Per Pixel Enumeration, Pixel Format Conversion and More"
 *  Copyright (c) 2010-2012 Smart K8
 *  http://www.codeproject.com/Articles/66550/Image-Per-Pixel-Enumeration-Pixel-Format-Conversio
 *  
 *  Released under Code Project Open License, CPOL, http://www.codeproject.com/info/cpol10.aspx
 */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImagePixelEnumerator.PathProviders
{
    internal class StandardPathProvider : IPathProvider
    {
        public IList<Point> GetPointPath(Int32 width, Int32 height)
        {
            List<Point> result = new List<Point>(width*height);

            for (Int32 y = 0; y < height; y++)
            for (Int32 x = 0; x < width; x++)
            {
                Point point = new Point(x, y);
                result.Add(point);
            }

            return result;
        }

        public static IList<Point> CreatePath(Int32 width, Int32 height)
        {
            StandardPathProvider result = new StandardPathProvider();
            return result.GetPointPath(width, height);
        }
    }
}
