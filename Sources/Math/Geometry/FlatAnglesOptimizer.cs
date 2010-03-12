// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2010
// andrew.kirillov@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;
    using System.Collections.Generic;
    using AForge;

    /// <summary>
    /// Shape optimizer, which removes obtuse angles (close to flat) from a shape.
    /// </summary>
    /// 
    /// <remarks><para>This shape optimizing algorithm checks all adjacent edges of a shape
    /// and substitutes any 2 edges with a single edge if angle between them is greater than
    /// <see cref="MaxAngleToKeep"/>. The algorithm makes sure there are not obtuse angles in
    /// a shape, which are very close to flat line.</para>
    /// 
    /// <para>
    /// For example, the below circle shape comprised of 65 points, can be optimized to 10 points
    /// by setting <see cref="MaxAngleToKeep"/> to 160.<br />
    /// <img src="img/math/flat_angles_optimizer.png" width="268" height="238" />
    /// </para>
    /// </remarks>
    /// 
    public class FlatAnglesOptimizer : IShapeOptimizer
    {
        private double maxAngleToKeep = 160;

        /// <summary>
        /// Maximum angle between adjacent edges to keep in a shape, [140, 180].
        /// </summary>
        /// 
        /// <remarks><para>The property sets maximum angle between adjacent edges, which is kept
        /// during optimization. All edges, which have a greater angle between them, are substituted
        /// by a single edge.</para>
        /// 
        /// <para>Default value is set to <b>160</b>.</para></remarks>
        /// 
        public double MaxAngleToKeep
        {
            get { return maxAngleToKeep; }
            set { maxAngleToKeep = Math.Min( 180, Math.Max( 140, value ) ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatAnglesOptimizer"/> class.
        /// </summary>
        /// 
        public FlatAnglesOptimizer( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatAnglesOptimizer"/> class.
        /// </summary>
        /// 
        /// <param name="maxAngleToKeep">Maximum acceptable angle between two edges of a shape (see <see cref="MaxAngleToKeep"/>).</param>
        /// 
        public FlatAnglesOptimizer( double maxAngleToKeep )
        {
            this.maxAngleToKeep = maxAngleToKeep;
        }

        /// <summary>
        /// Optimize specified shape.
        /// </summary>
        /// 
        /// <param name="shape">Shape to be optimized.</param>
        /// 
        /// <returns>Returns final optimized shape, which may have reduced amount of points.</returns>
        /// 
        public List<IntPoint> OptimizeShape( List<IntPoint> shape )
        {
            // max angle in radians
            double maxAngle = maxAngleToKeep * Math.PI / 180;
            // optimized shape
            List<IntPoint> optimizedShape = new List<IntPoint>( );

            if ( shape.Count <= 3 )
            {
                // do nothing if shape has 3 points or less
                optimizedShape.AddRange( shape );
            }
            else
            {
                double angle = 0;

                // add first 2 points to the new shape
                optimizedShape.Add( shape[0] );
                optimizedShape.Add( shape[1] );
                int pointsInOptimizedHull = 2;

                for ( int i = 2, n = shape.Count; i < n; i++ )
                {
                    // add new point
                    optimizedShape.Add( shape[i] );
                    pointsInOptimizedHull++;

                    // get angle between 2 vectors, which start from the next to last point
                    angle = GetAngleBetweenVectors( optimizedShape[pointsInOptimizedHull - 2],
                        optimizedShape[pointsInOptimizedHull - 3], optimizedShape[pointsInOptimizedHull - 1] );

                    if ( ( angle > maxAngle ) &&
                         ( ( pointsInOptimizedHull > 3 ) || ( i < n - 1 ) ) )
                    {
                        // remove the next to last point
                        optimizedShape.RemoveAt( pointsInOptimizedHull - 2 );
                        pointsInOptimizedHull--;
                    }
                }

                if ( pointsInOptimizedHull > 3 )
                {
                    // check the last point
                    angle = GetAngleBetweenVectors( optimizedShape[pointsInOptimizedHull - 1],
                        optimizedShape[pointsInOptimizedHull - 2], optimizedShape[0] );

                    if ( angle > maxAngle )
                    {
                        optimizedShape.RemoveAt( pointsInOptimizedHull - 1 );
                        pointsInOptimizedHull--;
                    }

                    if ( pointsInOptimizedHull > 3 )
                    {
                        // check the first point
                        angle = GetAngleBetweenVectors( optimizedShape[0],
                            optimizedShape[pointsInOptimizedHull - 1], optimizedShape[1] );

                        if ( angle > maxAngle )
                        {
                            optimizedShape.RemoveAt( 0 );
                        }
                    }
                }
            }

            return optimizedShape;
        }

        // Calculate angle between two vectors
        private double GetAngleBetweenVectors( IntPoint startPoint, IntPoint vector1end, IntPoint vector2end )
        {
            int x1 = vector1end.X - startPoint.X;
            int y1 = vector1end.Y - startPoint.Y;

            int x2 = vector2end.X - startPoint.X;
            int y2 = vector2end.Y - startPoint.Y;

            return Math.Acos( ( x1 * x2 + y1 * y2 ) / ( Math.Sqrt( x1 * x1 + y1 * y1 ) * Math.Sqrt( x2 * x2 + y2 * y2 ) ) );

        }
    }
}
