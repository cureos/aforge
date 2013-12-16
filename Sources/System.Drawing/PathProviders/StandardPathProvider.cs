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
