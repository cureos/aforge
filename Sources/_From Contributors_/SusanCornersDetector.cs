using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

// SUSAN corners detection
// Copyright © Frank Nagl, 2007
// admin@franknagl.de
//
namespace AForge.Imaging
{
    /// <summary>
    /// The class implements Moravec corners detector. 
    /// For information about algorithm's details its description 
    /// (http://users.fmrib.ox.ac.uk/~steve/susan/susan.ps.gz) should be studied.
    /// </summary>
    /// <Implements>AForge.Imaging.ICornersDetector</Implements>
    /// <author>Frank Nagl, admin@franknagl.de</author>
    public class SusanDetector : ICornersDetector
    {
        /// <summary>
        /// Inner class of the SusanDetector class. 
        /// Represents all informations and values of an USAN.
        /// Implements the System.IComparable interface for sorting by the mR.
        /// <Implements>System.IComparable</Implements>
        /// <author>Frank Nagl, admin@franknagl.de</author>
        /// </summary>
        private class Usan : IComparable
        {
            /// <summary>
            /// Index in the one-dimensional Byte-Array of the image
            /// </summary>
            private int mIndex;
            /// <summary>
            /// Index in the one-dimensional Byte-Array of the image
            /// </summary>
            public int Index
            {
                get { return mIndex; }
                set { mIndex = value; }
            }

            /// <summary>
            /// Grayscale value of the nucleus
            /// </summary>
            public byte mNucleus;
            /// <summary>
            /// Count of all similarity-values of an USAN
            /// </summary>
            public float mN;
            public Point mSumOfMultOfCandR;
            public int mCentreOfGravity;
            public Point mCentreOfGravityCoords;
            /// <summary>
            /// Difference of geometrical Threshold and Count of all similarity-values
            /// </summary>
            public int mR;

            /// <summary>
            /// Creates a new USAN object.
            /// </summary>
            /// <param name="pIndex"></param>
            /// <param name="pNucleus"></param>
            /// <param name="pN"></param>
            public Usan(int pIndex, byte pNucleus, int pN)
            {
                mIndex = pIndex;
                mNucleus = pNucleus;
                mN = pN;
            }

            public int CompareTo(Object pObj)
            {
                if (pObj is Usan)
                {
                    if (this.mR - ((Usan)pObj).mR < 0.0f)
                        return 1;
                    else if (this.mR - ((Usan)pObj).mR > 0.0f)
                        return -1;
                }
                return 0;
            }
        }

        /// <summary>
        /// Measure for the distance between nucleus and centre of gravity.
        /// </summary>
        public enum DISTANCE
        {
            ZERO,
            SMALL,
            BIG
        }

        private List<Point> mCorners = new List<Point>();
        public List<Point> Corners
        {
            get { return mCorners; }
            set { mCorners = value; }
        }

        /// <summary>
        /// Geometrical Threshold g. Influenced the quality of detection. 
        /// Smaller value gets less corners and the error rate decreases,
        /// bigger value gets more corners and the error rate increases.
        /// Default value: 18
        /// </summary>
        private int mGeometricalThreshold = 18;
        /// <summary>
        /// Geometrical Threshold g. Influenced the quality of detection. 
        /// Smaller value gets less corners and the error rate decreases,
        /// bigger value gets more corners and the error rate increases.
        /// Default value: 18
        /// </summary>
        public int GeometricalThreshold
        {
            get { return mGeometricalThreshold; }
            set { mGeometricalThreshold = value; }
        }
        /// <summary>
        /// Threshold for allowed difference between two Pixel in the USAN.
        /// Influences the quantity of the detection.
        /// A higher value detects more potential corners, 
        /// a smaller value detects less potential corners.
        /// Default value: 27
        /// </summary>        
        private float mDifferenceThreshold = 27;
        /// <summary>
        /// Threshold for allowed difference between two Pixel in the USAN.
        /// Influences the quantity of the detection.
        /// A higher value detects more potential corners, 
        /// a smaller value detects less potential corners.
        /// Default value: 27
        /// </summary> 
        public float DifferenceThreshold
        {
            get { return mDifferenceThreshold; }
            set { mDifferenceThreshold = value; }
        }
        /// <summary>
        /// The radius for the 	minimum distance between nucleus and centre of gravity
        /// </summary>
        private DISTANCE mMinDistance = DISTANCE.SMALL;
        /// <summary>
        /// The radius for the minimum distance between nucleus and centre of gravity.
        /// </summary>
        public DISTANCE MinDistance
        {
            get { return mMinDistance; }
            set { mMinDistance = value; }
        }


        public SusanDetector()
        {            
        }

        public SusanDetector(float pDifferenceThreshold, int pGeometricalThreshold)
        {
            mDifferenceThreshold = pDifferenceThreshold;
            mGeometricalThreshold = pGeometricalThreshold;
        }

        public SusanDetector(float pDifferenceThreshold, int pGeometricalThreshold, DISTANCE pGravityRadius)
        {
            mDifferenceThreshold = pDifferenceThreshold;
            mGeometricalThreshold = pGeometricalThreshold;
            mMinDistance = pGravityRadius;
        }

        /// <summary>
        /// Process image looking for corners. 
        /// </summary>
        /// <param name="pImage">Source image to process.</param>
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        public Point[] ProcessImage(Bitmap pImage)
        {
            pImage = ConvertToGrayscale(pImage);//Convert to grayscale Image
            BitmapData tBitmapData = pImage.LockBits
                    (new Rectangle(0, 0, pImage.Width, pImage.Height),
                     ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            pImage.UnlockBits(tBitmapData);

            return ProcessImage(tBitmapData);
        }

        /// <summary>
        /// Process image looking for corners. 
        /// </summary>
        /// <param name="pBitmapData">Source image data to process.</param>
        /// <returns>Returns array of found corners (X-Y coordinates).</returns>
        public Point[] ProcessImage(BitmapData pBitmapData)
        {
            List<Usan> tAllCorners = new List<Usan>();
            Bitmap tImage = ConvertToGrayscale(pBitmapData);//Convert to grayscale Image
            //Lock the new grayscale image
            BitmapData tBitmapData = tImage.LockBits
                        (new Rectangle(0, 0, tImage.Width, tImage.Height),
                         ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            int tStride = tBitmapData.Stride;//Get Bitmapwidth inclusive offset (=Image.Width+Offset)
            IntPtr tScan0 = tBitmapData.Scan0;// Get the address of the first line.
            int tOffset = tStride - tBitmapData.Width;// *3;//bei RGB            
            int tArraySize = tStride * tBitmapData.Height;
            byte[] tValues = new byte[tArraySize];// Declare an array to hold the bytes of the bitmap.

            // Copy the grayscale values into the array.
            System.Runtime.InteropServices.Marshal.Copy(tScan0, tValues, 0, tArraySize);

            int tPos = 3 * tStride + 3; //startposition          

            for (int y = 3; y < tBitmapData.Height - 3; y++)
            {
                for (int x = 3; x < tBitmapData.Width - 3; x++)
                {
                    Usan tUsan = calcUSAN(tPos, tValues, tStride, mDifferenceThreshold);
                    if (tUsan.mN < GeometricalThreshold &&
                        controlCentreOfGravity(tUsan, tStride, mMinDistance) &&
                        counteractImageNoise(tUsan, tValues, tBitmapData, mDifferenceThreshold)
                                                                           )
                    {
                        tUsan.mR = GeometricalThreshold - (int)tUsan.mN;
                        tAllCorners.Add(tUsan);
                    }
                    tPos += 1;
                }
                tPos += (6 + tOffset);
            }

            tImage.UnlockBits(tBitmapData);


            //Checking 5x5 region
            tAllCorners.Sort();
            tAllCorners = checkRegion(tAllCorners, tStride, 4);

            for (int i = 0; i < tAllCorners.Count; i++)
            {
                mCorners.Add(
                    ArrayToPoint.transformInXYCoord(tAllCorners[i].Index, tArraySize, tStride));
            }

            return mCorners.ToArray();
        }

        /// <summary>
        /// Helperfunction, that calculates all parameters of an USAN.
        /// </summary>
        /// <param name="pArray">The one-dimensional Byte-Array of the image</param>
        /// <param name="pStride">Bitmapwidth inclusive offset (=Image.Width+Offset)</param>
        /// <param name="pPos">The position of nucleus of the USAN</param>
        /// <param name="pDifferenceThreshold">Threshold for the allowed difference between two Pixel in a USAN.</param>
        /// <returns>The calculated USAN</returns>
        private static Usan calcUSAN(int pPos, Byte[] pArray, int pStride, float pDifferenceThreshold)
        {
            Usan tUsan = new Usan(pPos, pArray[pPos], 0);

            int tRadius;
            int tPos;
            for (int y = -3; y <= 3; y++)
            {
                tPos = pPos + y * pStride;
                switch (Math.Abs(y))
                {
                    case 3: tRadius = 1; break;
                    case 2: tRadius = 2; break;
                    default: tRadius = 3; break;
                }
                for (int x = -tRadius; x <= tRadius; x++)
                {
                    if (Math.Abs(pArray[pPos] - pArray[tPos + x]) <= pDifferenceThreshold)
                    {
                        //c is always 1, N = Sum of all C's
                        tUsan.mN += 1;
                        Point tActualPos = ArrayToPoint.transformInXYCoord(tPos + x, pArray.Length, pStride);
                        tUsan.mSumOfMultOfCandR.Offset(tActualPos);
                    }
                }
            }

            //Centre of Gravity
            tUsan.mCentreOfGravityCoords.X = tUsan.mSumOfMultOfCandR.X / (int)tUsan.mN;
            tUsan.mCentreOfGravityCoords.Y = tUsan.mSumOfMultOfCandR.Y / (int)tUsan.mN;
            tUsan.mCentreOfGravity = ArrayToPoint.transformXYCoordInBytes(tUsan.mCentreOfGravityCoords, pStride);

            return tUsan;
        }

        /// <summary>
        /// Helperfunction, that converts an image into a grayscale image.
        /// </summary>
        /// <param name="pBitmap">The original image</param>
        /// <returns>The grayscale image.</returns>
        private static Bitmap ConvertToGrayscale(Bitmap pBitmap)
        {
            GrayscaleBT709 tGrayscale = new GrayscaleBT709();
            return tGrayscale.Apply(pBitmap);
        }

        /// <summary>
        /// Overloaded. Helperfunction, that converts a BitmapData into a grayscale image.
        /// </summary>
        /// <param name="pBitmapData">The BitmapData</param>
        /// <returns>The grayscale image.</returns>
        private static Bitmap ConvertToGrayscale(BitmapData pBitmapData)
        {
            if (pBitmapData.PixelFormat == PixelFormat.Format8bppIndexed)
                return AForge.Imaging.Image.Clone(pBitmapData);

            GrayscaleBT709 tGrayscale = new GrayscaleBT709();
            return tGrayscale.Apply(pBitmapData);
        }

        /// <summary>
        /// Helperfunction, which compares the position of the 
        /// Centre-of-gravity with the position of the nucleus.
        /// When the Centre-of-gravity is not in the radius area of the nucleus
        /// the function returns true, otherwise false.
        /// </summary>
        /// <param name="pUsan">The nucleus</param>
        /// <param name="pStride">Bitmapwidth inclusive offset (=Image.Width+Offset)</param>
        /// <param name="pRadius">The radius, where the comparsion is done</param>
        /// <returns>True - when the Centre-of-gravity is not in the radius area of the nucleus</returns>
        /// <returns>False - when the Centre-of-gravity is in the radius area of the nucleus</returns>
        private static bool controlCentreOfGravity(Usan pUsan, int pStride, DISTANCE pRadius)
        {            
            int tCoG = pUsan.mCentreOfGravity;

            switch (pRadius)
            {
                case DISTANCE.ZERO:
                    if (pUsan.Index == tCoG)
                        return false;
                    break;
                case DISTANCE.SMALL:
                    if (pUsan.Index == tCoG ||
                        //vertical
                        pUsan.Index + 1 == tCoG ||
                        pUsan.Index - 1 == tCoG ||
                        //horricontal
                        pUsan.Index + pStride == tCoG ||
                        pUsan.Index - pStride == tCoG)
                        return false;
                    break;
                case DISTANCE.BIG:
                    if (pUsan.Index == tCoG ||
                        //ONE PIXEL
                        //vertical
                        pUsan.Index + 1 == tCoG ||
                        pUsan.Index - 1 == tCoG ||
                        //horricontal
                        pUsan.Index + pStride == tCoG ||
                        pUsan.Index - pStride == tCoG || 
                        //TWO PIXEL
                        //vertical                       
                        pUsan.Index + 2 == tCoG ||
                        pUsan.Index - 2 == tCoG ||
                        //horricontal
                        pUsan.Index + 2 * pStride == tCoG ||
                        pUsan.Index - 2 * pStride == tCoG ||
                        //ONE PIXEL DIAGONAL
                        pUsan.Index + pStride + 1 == tCoG ||
                        pUsan.Index + pStride - 1 == tCoG ||
                        pUsan.Index - pStride + 1 == tCoG ||
                        pUsan.Index - pStride - 1 == tCoG)
                        return false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Return true, if all pixels in the line of the neuclues to the
        /// CoG belong to the USAN, otherwise returns false.
        /// This counteracts Image-Noise.
        /// </summary>
        /// <param name="pUsan"></param>
        /// <param name="pValues"></param>
        /// <param name="pData"></param>
        /// <param name="pDifferenceThreshold">Threshold for the allowed difference between two Pixel in a USAN.</param>
        /// <returns></returns>
        private static bool counteractImageNoise(Usan pUsan, byte[] pValues, BitmapData pData, float pDifferenceThreshold)
        {
            Point tNucleus = ArrayToPoint.transformInXYCoord(pUsan.Index, pValues.Length, pData.Stride);
            Point tCoG = ArrayToPoint.transformInXYCoord((int)pUsan.mCentreOfGravity, pValues.Length, pData.Stride);

            float tDiffX = (float)(tCoG.X - tNucleus.X) / 3f;
            float tDiffY = (float)(tCoG.Y - tNucleus.Y) / 3f;

            //Take 3 to check until the CoG, take 4 fot one pixel more
            for (byte i = 0; i < 4; i++)
            {
                int tNewPos = pUsan.Index + (int)tDiffY * pData.Stride + (int)tDiffX;
                tDiffX += tDiffX;
                tDiffY += tDiffY;
                try
                {
                    if (Math.Abs(pValues[pUsan.Index] - pValues[tNewPos]) > pDifferenceThreshold)
                        return false;
                }
                catch (IndexOutOfRangeException)
                { };
            }

                return true;
        }

        /// <summary>
        /// Helperfunction, which eliminates doubled corners in a region with a given radius.
        /// </summary>
        /// <param name="pStride">Bitmapwidth inclusive offset (=Image.Width+Offset)</param>
        /// <param name="pRadius">The given radius of a region</param>
        private static List<Usan> checkRegion(List<Usan> pCorners, int pStride, int pRadius)
        {
            for (int i = 0; i < pCorners.Count; i++)
            {
                for (int y = -pRadius; y <= pRadius; y++)
                {
                    int tPos = pCorners[i].Index + y * pStride;
                    for (int x = -pRadius; x <= pRadius; x++)
                    {
                        if (y == 0 && x == 0)
                            continue;
                        for (int k = 0; k < pCorners.Count; k++)
                        {
                            if (pCorners[k].Index == (tPos + x))
                                pCorners.RemoveAt(k);
                        }
                    }
                }
            }

            return pCorners;
        }
    }
}
