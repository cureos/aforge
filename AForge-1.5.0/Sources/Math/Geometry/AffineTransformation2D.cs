// AForge Math Library
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Math.Geometry
{
    using System;

    /// <summary>
    /// 2D affine transformation
    /// </summary>
    /// 
    /// <remarks>The class performs 2D affine transformation.</remarks>
    /// 
    ///
    public class AffineTransformation2D
    {
        // transformation matrix values
        private double a, b, c, d;
        // move vector values
        private double e, f;

        /// <summary>
        /// Initializes a new instance of the <see cref="AffineTransformation2D"/> class
        /// </summary>
        /// 
        /// <param name="transformationMatrix">Transformation matrix</param>
        /// <param name="moveVector">Move vector</param>
        /// 
        public AffineTransformation2D( double[,] transformationMatrix, double[] moveVector )
        {
            // save transformation matrix
            a = transformationMatrix[0, 0];
            b = transformationMatrix[0, 1];
            c = transformationMatrix[1, 0];
            d = transformationMatrix[1, 1];
            // save move vector
            e = moveVector[0];
            f = moveVector[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AffineTransformation2D"/> class
        /// </summary>
        /// 
        /// <param name="a">[0, 0] element of transformation matrix</param>
        /// <param name="b">[0, 1] element of transformation matrix</param>
        /// <param name="c">[1, 0] element of transformation matrix</param>
        /// <param name="d">[1, 1] element of transformation matrix</param>
        /// <param name="e">[0] element of move vector</param>
        /// <param name="f">[1] element of move vector</param>
        /// 
        public AffineTransformation2D( double a, double b, double c, double d, double e, double f )
        {
            // save transformation matrix
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            // save move vector
            this.e = e;
            this.f = f;
        }

        /// <summary>
        /// Transform point
        /// </summary>
        /// 
        /// <param name="x">X coordinate of the point</param>
        /// <param name="y">Y coordinate of the point</param>
        /// 
        public void Transform( ref double x, ref double y )
        {
            // do transformation to temporary variables
            double tx = a * x + b * y + e;
            double ty = c * x + d * y + f;
            // copy temporary values to output values
            x = tx;
            y = ty;
        }
    }
}
