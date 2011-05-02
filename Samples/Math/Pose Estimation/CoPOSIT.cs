// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;
    using AForge;
    using AForge.Math;

    public class CoplanarPosit
    {
        // camera's focal length
        private float focalLength;

        // points of the model to estimate position for
        private Vector3[] modelPoints;
        // 3 vectors of the model kept as a matrix
        private Matrix3x3 modelVectors;
        // pseudoinverse of the model vectors matrix
        private Matrix3x3 modelPseudoInverse;
        // unit normal to the model
        private Vector3 modelNormal;

        public CoplanarPosit( Vector3[] model, float focalLength )
        {
            if ( model.Length != 4 )
            {
                throw new ArgumentException( "The model must have 4 points." );
            }

            this.focalLength = focalLength;
            modelPoints = (Vector3[]) model.Clone( );

            // compute model vectors
            modelVectors = Matrix3x3.CreateFromRows(
                model[1] - model[0],
                model[2] - model[0],
                model[3] - model[0] );

            // compute pseudo inverse of the model matrix
            Matrix3x3 u, v;
            Vector3 e;

            modelVectors.SVD( out u, out e, out v );
            modelPseudoInverse = v * Matrix3x3.CreateDiagonal( e.Inverse( ) ) * u.Transpose( );

            // computer unit vector normal to the model
            modelNormal = v.GetColumn( e.MinIndex );
        }

        public Matrix3x3 Rotation2 = new Matrix3x3( );
        public Vector3 Translation2 = new Vector3( );

        public float Error1 = 0, Error2 = 0;

        public void EstimatePose( Point[] points, out Matrix3x3 rotation, out Vector3 translation )
        {
            if ( points.Length != 4 )
            {
                throw new ArgumentException( "4 points must be be given for pose estimation." );
            }

            Matrix3x3 rotation1, rotation2;
            Vector3 translation1, translation2;

            // find initial rotation
            POS( points, new Vector3( 1 ), out rotation1, out rotation2, out translation1, out translation2 );

            // iterate further and fine tune the solution
            float error1 = Iterate( points, ref rotation1, ref translation1 );
            float error2 = Iterate( points, ref rotation2, ref translation2 );

            // take the best found pose
            if ( error1 < error2 )
            {
                rotation = rotation1;
                translation = translation1;

                Rotation2 = rotation2;
                Translation2 = translation2;

                Error1 = error1;
                Error2 = error2;
            }
            else
            {
                rotation = rotation2;
                translation = translation2;

                Rotation2 = rotation1;
                Translation2 = translation1;

                Error1 = error2;
                Error2 = error1;
            }
        }

        private const float ErrorLimit = 2;

        // Iterate POS algorithm starting from the specified rotation and translation and finu tune it
        private float Iterate( Point[] points, ref Matrix3x3 rotation, ref Vector3 translation )
        {
            float prevError = float.MaxValue;
            float error = 0;

            // run maximum 100 iterations (seems to be overkill, since typicaly it requires around 1-2 iterations)
            for ( int count = 0; count < 100; count++ )
            {
                Matrix3x3 rotation1, rotation2;
                Vector3 translation1, translation2;

                // calculates new epsilon values
                Vector3 eps = ( modelVectors * rotation.GetRow( 2 ) ) / translation.Z + 1;
                // and new pose
                POS( points, eps, out rotation1, out rotation2, out translation1, out translation2 );

                // calculate error for both new poses
                float error1 = GetError( points, rotation1, translation1 );
                float error2 = GetError( points, rotation2, translation2 );

                // select the pose which gives smaller error
                if ( error1 < error2 )
                {
                    rotation = rotation1;
                    translation = translation1;
                    error = error1;
                }
                else
                {
                    rotation = rotation2;
                    translation = translation2;
                    error = error2;
                }

                // stop if error is small enough or started to grow
                if ( ( error <= ErrorLimit ) || ( error > prevError ) )
                    break;

                prevError = error;
            }

            return error;
        }

        // Perform single iteration of POS (pos estimations) algorithm to find possible rotations and translation vector
        private void POS( Point[] imagePoints, Vector3 eps, out Matrix3x3 rotation1, out Matrix3x3 rotation2, out Vector3 translation1, out Vector3 translation2 )
        {
            // create vectors keeping all X and Y coordinates for the 1st, 2nd and 3rd points
            Vector3 XI = new Vector3( imagePoints[1].X, imagePoints[2].X, imagePoints[3].X );
            Vector3 YI = new Vector3( imagePoints[1].Y, imagePoints[2].Y, imagePoints[3].Y );

            // calculate scale orthographic projection (SOP)
            Vector3 imageXs = XI * eps - imagePoints[0].X;
            Vector3 imageYs = YI * eps - imagePoints[0].Y;

            // calculate I0 and J0 vectors
            Vector3 I0Vector = modelPseudoInverse * imageXs;
            Vector3 J0Vector = modelPseudoInverse * imageYs;

            Vector3 iVector = new Vector3( );
            Vector3 jVector = new Vector3( );
            Vector3 kVector = new Vector3( );

            // find roots of complex number C^2
            float j2i2dif = J0Vector.Square - I0Vector.Square;
            float ij = Vector3.Dot( I0Vector, J0Vector );

            float r = 0, theta = 0;

            if ( j2i2dif == 0 )
            {
                theta = (float) ( ( -System.Math.PI / 2 ) * System.Math.Sign( ij ) );
                r = (float) System.Math.Sqrt( System.Math.Abs( 2 * ij ) );
            }
            else
            {
                r = (float) System.Math.Sqrt( System.Math.Sqrt( j2i2dif * j2i2dif + 4 * ij * ij ) );
                theta = (float) System.Math.Atan( -2 * ij / j2i2dif );

                if ( j2i2dif < 0 )
                    theta += (float) System.Math.PI;

                theta /= 2;
            }

            float lambda = (float) ( r * System.Math.Cos( theta ) );
            float mu = (float) ( r * System.Math.Sin( theta ) );

            // first possible rotation
            iVector = I0Vector + ( modelNormal * lambda );
            jVector = J0Vector + ( modelNormal * mu );

            float iNorm = iVector.Normalize( );
            float jNorm = jVector.Normalize( );
            kVector = Vector3.Cross( iVector, jVector );

            rotation1 = Matrix3x3.CreateFromRows( iVector, jVector, kVector );

            // calculate translation vector
            float scale = ( iNorm + jNorm ) / 2;
            // ----
            Vector3 temp = rotation1 * modelPoints[0];
            translation1 = new Vector3( imagePoints[0].X / scale - temp.X, imagePoints[0].Y / scale - temp.Y, focalLength / scale );
            // ----
            //translation = new Vector3( imagePoints[0].X / scale, imagePoints[0].Y / scale, focalLength / scale );

            float minZ = ( modelVectors * kVector + translation1.Z ).Min;

            // second possible rotation
            iVector = I0Vector - ( modelNormal * lambda );
            jVector = J0Vector - ( modelNormal * mu );

            iNorm = iVector.Normalize( );
            jNorm = jVector.Normalize( );
            kVector = Vector3.Cross( iVector, jVector );

            rotation2 = Matrix3x3.CreateFromRows( iVector, jVector, kVector );

            scale = ( iNorm + jNorm ) / 2;
            // ----
            temp = rotation2 * modelPoints[0];
            translation2 = new Vector3( imagePoints[0].X / scale - temp.X, imagePoints[0].Y / scale - temp.Y, focalLength / scale );

            minZ = ( modelVectors * kVector + translation2.Z ).Min;
        }

        // Calculate mean squared error between real image points and projection calculate with found rotation/translation
        private float GetError( Point[] imagePoints, Matrix3x3 rotation, Vector3 translation )
        {
            float error = 0;


            Vector3 v1 = rotation * modelPoints[0] + translation;
            v1.X = v1.X * focalLength / v1.Z;
            v1.Y = v1.Y * focalLength / v1.Z;

            Vector3 v2 = rotation * modelPoints[1] + translation;
            v2.X = v2.X * focalLength / v2.Z;
            v2.Y = v2.Y * focalLength / v2.Z;

            Vector3 v3 = rotation * modelPoints[2] + translation;
            v3.X = v3.X * focalLength / v3.Z;
            v3.Y = v3.Y * focalLength / v3.Z;

            Vector3 v4 = rotation * modelPoints[3] + translation;
            v4.X = v4.X * focalLength / v4.Z;
            v4.Y = v4.Y * focalLength / v4.Z;

            if ( ( v1.Z < 0 ) || ( v2.Z < 0 ) || ( v3.Z < 0 ) || ( v4.Z < 0 ) )
            {
                System.Diagnostics.Debug.WriteLine( "===========================" );
            }

            Point[] modeledPoints = new Point[4]
            {
                new Point( v1.X, v1.Y ),
                new Point( v2.X, v2.Y ),
                new Point( v3.X, v3.Y ),
                new Point( v4.X, v4.Y ),
            };

            float ia1 = GeometryTools.GetAngleBetweenVectors( imagePoints[0], imagePoints[1], imagePoints[3] );
            float ia2 = GeometryTools.GetAngleBetweenVectors( imagePoints[1], imagePoints[2], imagePoints[0] );
            float ia3 = GeometryTools.GetAngleBetweenVectors( imagePoints[2], imagePoints[3], imagePoints[1] );
            float ia4 = GeometryTools.GetAngleBetweenVectors( imagePoints[3], imagePoints[0], imagePoints[2] );

            float ma1 = GeometryTools.GetAngleBetweenVectors( modeledPoints[0], modeledPoints[1], modeledPoints[3] );
            float ma2 = GeometryTools.GetAngleBetweenVectors( modeledPoints[1], modeledPoints[2], modeledPoints[0] );
            float ma3 = GeometryTools.GetAngleBetweenVectors( modeledPoints[2], modeledPoints[3], modeledPoints[1] );
            float ma4 = GeometryTools.GetAngleBetweenVectors( modeledPoints[3], modeledPoints[0], modeledPoints[2] );


            return (
                System.Math.Abs( ia1 - ma1 ) +
                System.Math.Abs( ia2 - ma2 ) +
                System.Math.Abs( ia3 - ma3 ) +
                System.Math.Abs( ia4 - ma4 )
                ) / 4;


            float il1 = (float) System.Math.Sqrt(
                    System.Math.Pow( imagePoints[1].X - imagePoints[0].X, 2 ) +
                    System.Math.Pow( imagePoints[1].Y - imagePoints[0].Y, 2 ) );
            float il2 = (float) System.Math.Sqrt(
                    System.Math.Pow( imagePoints[2].X - imagePoints[1].X, 2 ) +
                    System.Math.Pow( imagePoints[2].Y - imagePoints[1].Y, 2 ) );
            float il3 = (float) System.Math.Sqrt(
                    System.Math.Pow( imagePoints[3].X - imagePoints[2].X, 2 ) +
                    System.Math.Pow( imagePoints[3].Y - imagePoints[2].Y, 2 ) );
            float il4 = (float) System.Math.Sqrt(
                    System.Math.Pow( imagePoints[0].X - imagePoints[3].X, 2 ) +
                    System.Math.Pow( imagePoints[0].Y - imagePoints[3].Y, 2 ) );

            float ml1 = (float) System.Math.Sqrt(
                    System.Math.Pow( v2.X - v1.X, 2 ) +
                    System.Math.Pow( v2.Y - v1.Y, 2 ) );
            float ml2 = (float) System.Math.Sqrt(
                    System.Math.Pow( v3.X - v2.X, 2 ) +
                    System.Math.Pow( v3.Y - v2.Y, 2 ) );
            float ml3 = (float) System.Math.Sqrt(
                    System.Math.Pow( v4.X - v3.X, 2 ) +
                    System.Math.Pow( v4.Y - v3.Y, 2 ) );
            float ml4 = (float) System.Math.Sqrt(
                    System.Math.Pow( v1.X - v4.X, 2 ) +
                    System.Math.Pow( v1.Y - v4.Y, 2 ) );

            float dl1 = (float) System.Math.Abs( il1 - ml1 );
            float dl2 = (float) System.Math.Abs( il2 - ml2 );
            float dl3 = (float) System.Math.Abs( il3 - ml3 );
            float dl4 = (float) System.Math.Abs( il4 - ml4 );



            return System.Math.Abs( il1 / il3 - ml1 / ml3 ) +
                   System.Math.Abs( il2 / il4 - ml2 / ml4 );


            return ( dl1 + dl2 + dl3 + dl4 );

            for ( int i = 0; i < modelPoints.Length; i++ )
            {
                Vector3 v = rotation * modelPoints[i] + translation;

                v.X = v.X * focalLength / v.Z;
                v.Y = v.Y * focalLength / v.Z;

                float dx = imagePoints[i].X - v.X;
                float dy = imagePoints[i].Y - v.Y;

                error += ( dx * dx + dy * dy );
            }

            return error / modelPoints.Length;
        }
    }
}


/*
            float i0i0 = I0Vector.Sqr;
            float j0j0 = J0Vector.Sqr;
            float i0j0 = Vector3.Dot( I0Vector, J0Vector );

            float q = 0;

            float delta = ( j0j0 - i0i0 ) * ( j0j0 - i0i0 ) + 4 * ( i0j0 * i0j0 );

            if ( ( i0i0 - j0j0 ) >= 0 )
            {
                q = -(float) ( i0i0 - j0j0 + Math.Sqrt( delta ) ) / 2;
            }
            else
            {
                q = -(float) ( i0i0 - j0j0 - Math.Sqrt( delta ) ) / 2;
            }

            float lambda = 0;
            float mu = 0;

            if (q>=0)
            {
                lambda=(float)Math.Sqrt(q);
                if ( lambda == 0.0 )
                {
                    mu = 0.0f;
                }
                else
                {
                    mu = -i0j0 / (float) Math.Sqrt( q );
                }
            }
            else
            {
                lambda= (float) Math.Sqrt(-(i0j0*i0j0)/q);

                if ( lambda == 0.0 )
                {
                    mu = (float) Math.Sqrt( i0i0 - j0j0 );
                }
                else
                {
                    mu = (float) ( -i0j0 / Math.Sqrt( -( i0j0 * i0j0 ) / q ) );
                }
            }
*/