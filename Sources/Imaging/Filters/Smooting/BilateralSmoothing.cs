// Developed by Maxim Saplin
// 2012
// LGPL License

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using M = System.Math;

namespace AForge.Imaging.Filters
{
    /// <summary>
    /// Bilaterla filter implementation. Edge preserving smoothing and noise reduction that uses chromatic and spatial factors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bilateral filter conducts "selective" gausian smoothing of areas of same color (domains) which removes noise and contrast artifacts
    /// while preserving sharp edges.
    /// </para>
    /// <para>
    /// 2 major parameters <see cref="BilateralSmoothing.SpatialFactor"/> and <see cref="BilateralSmoothing.ColorFactor"/> define the reults of the filter. 
    /// By changing the params you may achive either only noise reduction with little change to the image or get sliky look effect to the entire image.
    /// </para>
    /// <para>
    /// Although the filter incorporates parallel processing large <see cref="BilateralSmoothing.KernelSize"/> values (&gt; 25) on high resolution images
    /// may decrease speed of processing. Also on high resolution images small KernelSize values ( &lt; 9) may not provide noticible results.
    /// </para>
    /// <para>
    /// More details on the alogorithm can be found here: http://saplin.blogspot.com/2012/01/bilateral-image-filter-edge-preserving.html
    /// </para>
    /// </remarks>
    public class BilateralSmoothing : BaseUsingCopyPartialFilter
    {
        private uint kernelSize = 3;

        private const int maxKernelSize = 256;

        /// <summary>
        /// Whther to throw an exception in case a large kernel size is used which may lead to significant performance issues
        /// </summary>
        public bool LimitKernelSize {get;set;}

        /// <summary>
        /// Size of a square for limiting surrounding pixels that take part in calculations
        /// </summary>
        /// <remarks>
        /// The greater the value the more is the general power of the filter. Small values ( &lt 9) on high-res images (3000px) do not give significant results.
        /// Large values increase the number of calculations and degrade performance
        /// </remarks>
        public uint KernelSize
        {
            get
            {
                return kernelSize;
            }
            set
            {
                if (value > maxKernelSize) throw new ArgumentOutOfRangeException("KernelSize must be less than " + maxKernelSize.ToString());
                if (value < 3) throw new ArgumentOutOfRangeException("KernelSize must be greater than 3");
                if (LimitKernelSize && (value > 25)) throw new ArgumentOutOfRangeException("KernerlSize is larger then 25. Time for applying is significant and may lead to application freezing. In order to use any KernelSize value set property 'LimitKernelSize' to false");
                if (value % 2 == 0) throw new ArgumentException("KernerlSize must be an odd integer");

                kernelSize = value;
            }
        }

        private double spatialFactor;
        private bool spatialChanged = true;

        /// <summary>
        /// Determines smothing power within a color domain (neighbour pixels of similar color)
        /// </summary>
        public double SpatialFactor
        {
            get
            {
                return spatialFactor;
            }
            set
            {
                spatialFactor = value;
                spatialChanged = true;
            }
        }

        private double spatialPower;
        private bool spatialPowerChanged = true;

        /// <summary>
        /// Exponent power, used in Spatial function calucalation
        /// </summary>
        /// <remarks>
        /// The greater the value is the more distinctive drop in weigh coefficient (color, spatial closeness) is. E.g. for default value "2" weigh cofecients for colors  (128, 130),  (128,150) and (128,200) can be 0.8 and 0.7 and 0.6.
        /// For power "6" the corresponding coefficients can be 0.85, 0.8, 0.5. When evaluating color or spatial closeness (i.e. corresponding wigh coefficients) greater power the interval (spatial, color) in which surrounding pixels will have the most affect.
        /// Color values (so as distance values) in range 0-10 may have same affect on the resulting pixel while it'll significantly drop after 11.
        /// </remarks>
        public double SpatialPower
        {
            get
            {
                return spatialPower;
            }
            set
            {
                spatialPower = value;
                spatialPowerChanged = true;
            }
        }

        private double colorFactor;
        private bool colorChanged = true;

        private const int colorsCount = 256;

        /// <summary>
        /// Determines the variance of color for a color domain 
        /// </summary>
        /// <remarks>
        /// The greater the value is the more distinctive drop in weigh coefficient (color, spatial closeness) is. E.g. for default value "2" weigh cofecients for colors  (128, 130),  (128,150) and (128,200) can be 0.8 and 0.7 and 0.6.
        /// For power "6" the corresponding coefficients can be 0.85, 0.8, 0.5. When evaluating color or spatial closeness (i.e. corresponding wigh coefficients) greater power the interval (spatial, color) in which surrounding pixels will have the most affect.
        /// Color values (so as distance values) in range 0-10 may have same affect on the resulting pixel while it'll significantly drop after 11.
        /// </remarks>
        public double ColorFactor
        {
            get
            {
                return colorFactor;
            }
            set
            {
                colorFactor = value;
                colorChanged = true;
            }
        }

        private double colorPower;
        private bool colorPowerChanged = true;

        /// <summary>
        /// Exponent power, used in Color function calucalation
        /// </summary>
        /// <remarks>
        /// You may play with it if you'd like to but in most cases default values should be fine
        /// </remarks>
        public double ColorPower
        {
            get
            {
                return colorPower;
            }
            set
            {
                colorPower = value;
                colorPowerChanged = true;
            }
        }

        private Dictionary<PixelFormat, PixelFormat> formatTranslations = new Dictionary<PixelFormat, PixelFormat>();

        /// <summary>
        /// The algorithm is tweaked for and natively works only with a single format: 24bppRgb
        /// </summary>
        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public BilateralSmoothing()
        {
            LimitKernelSize = true;
            KernelSize = 9;
            SpatialFactor = 10;
            ColorFactor = 50;
            SpatialPower = 2;
            ColorPower = 2;
            formatTranslations[PixelFormat.Format8bppIndexed] = PixelFormat.Format8bppIndexed;
            formatTranslations[PixelFormat.Format24bppRgb] = PixelFormat.Format24bppRgb;
            formatTranslations[PixelFormat.Format32bppRgb] = PixelFormat.Format32bppRgb;
            formatTranslations[PixelFormat.Format32bppArgb] = PixelFormat.Format32bppArgb;
        }

        private double[,] spatialFunc;
        private double[,] colorFunc;

        /// <summary>
        /// For performance improvements Color and Spatial functions are precalculated prior to filter execution and put to 2 dimensional arrays
        /// </summary>
        private void InitSpatialFunc()
        {
            if ((spatialFunc == null) || (spatialFunc.Length != KernelSize * KernelSize) || spatialChanged || spatialPowerChanged)
            {
                if ((spatialFunc == null) || spatialFunc.Length != KernelSize * KernelSize) spatialFunc = new double[KernelSize, KernelSize];

                uint c = KernelSize / 2;

                for (int i = 0; i < KernelSize; i++)
                    for (int k = 0; k < KernelSize; k++)
                        spatialFunc[i, k] = M.Exp(-0.5 * (M.Pow(M.Sqrt((i - c) * (i - c) + (k - c) * (k - c) / spatialFactor), SpatialPower)));

                spatialChanged = false;
                spatialPowerChanged = false;
            }
        }

        /// <summary>
        /// For performance improvements Color and Spatial functions are precalculated prior to filter execution and put to 2 dimensional arrays
        /// </summary>
        private void InitColorFunc()
        {
            if ((colorFunc == null) || colorChanged || colorPowerChanged)
            {
                if (colorFunc == null) colorFunc = new double[colorsCount, colorsCount];
                for (int i = 0; i < colorsCount; i++)
                    for (int k = 0; k < colorsCount; k++)
                        colorFunc[i, k] = M.Exp(-0.5 * (M.Pow(M.Abs(i - k) / colorFactor, colorPower)));

                colorChanged = false;
                colorPowerChanged = false;
            }
        }

        private void InitFilter()
        {
            InitSpatialFunc();
            InitColorFunc();
        }

        /// <summary>
        /// Set to false to disable multithreaded filter application despite the number of cores CPU has
        /// </summary>
        public bool DisableParallelProcessing { get; set; }

        /// <summary>
        /// Applies filter
        /// </summary>
        /// <remarks>
        /// The routine automatically decies whether to use parallel processing to boost performance
        /// </remarks>
        protected override unsafe void ProcessFilter(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            InitFilter();

            int kernelHalf = (int)kernelSize / 2;

            var x = Stopwatch.StartNew();

            if (source.Width <= kernelSize || source.Height <= kernelSize) 
                ProcessWithEdgeChecks(source, destination, new Rectangle(0, 0, source.Width, source.Height));
            else
            {
                // For performance reasons there're 2 separate methods doing similar stuff 
                // For center pixels we may avoid unnecessary checks while for pixels closer to the boundaries checks a compulsory
                // This separation gives ~10% boost over single call to ProcessWithEdgeChecks

                // Parallel processing on dual core Intel Core i5 HT (4 virtual cores) gave 3.25x performance boost
                if (Environment.ProcessorCount > 1 && !DisableParallelProcessing)
                    ProcessWithoutChecksParallel(source, destination, new Rectangle(kernelHalf, kernelHalf, source.Width - (int) kernelSize + 1, source.Height - (int) kernelSize + 1)); // center
                else
                    ProcessWithoutChecks(source, destination, new Rectangle(kernelHalf, kernelHalf, source.Width - (int)kernelSize + 1, source.Height - (int)kernelSize + 1)); // center
                
                ProcessWithEdgeChecks(source, destination, new Rectangle(0, 0, source.Width, (int) kernelHalf)); // top
                ProcessWithEdgeChecks(source, destination, new Rectangle(source.Width - (int) kernelHalf, 0, (int) kernelHalf, source.Height)); // right
                ProcessWithEdgeChecks(source, destination, new Rectangle(0, source.Height - kernelHalf, source.Width, (int) kernelHalf)); // bottom
                ProcessWithEdgeChecks(source, destination, new Rectangle(0, 0, (int) kernelHalf, source.Height)); // left
            }

            x.Stop();

            ExecutionTime = x.ElapsedMilliseconds;
        }

        /// <summary>
        /// Time in ms measured during past execution
        /// </summary>
        public long ExecutionTime
        {
            get;
            private set;
        }

        // For Format32bppArgb alpha channel is not used for calculations
        private unsafe void ProcessWithoutChecksParallel(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            var pixelSize = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
            var kernelHalf = kernelSize / 2;
            var bytesInKernelRow = kernelSize * pixelSize;
            var srcRow = (byte*)source.ImageData;
            var dstRow = (byte*)destination.ImageData;

            if (pixelSize > 1) // For 24bpp, and 32bpp formats
            {
                Parallel.For(rect.Top, rect.Height + rect.Top, i =>
                {
                    uint x, y;
                    byte srcR, srcG, srcB;
                    byte dstR, dstG, dstB;
                    byte srcR0, srcG0, srcB0;
                    byte* srcPixel;
                    byte* srcPixel0;
                    byte* dstPixel0;
                    double sCoefR, sCoefG, sCoefB, sMembR, sMembG, sMembB, coefR, coefG, coefB;

                    for (uint k = (uint)rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint)(source.Stride * i) + (uint)(pixelSize * k);
                        dstPixel0 = dstRow + (uint)(destination.Stride * i) + (uint)(pixelSize * k);
                        srcPixel = srcPixel0 + kernelHalf * (source.Stride + pixelSize); // lower right corner - to start processing from that point
                        sCoefR = 0;
                        sCoefG = 0;
                        sCoefB = 0;
                        sMembR = 0;
                        sMembG = 0;
                        sMembB = 0;

                        srcR0 = *(srcPixel0 + 2);
                        srcG0 = *(srcPixel0 + 1);
                        srcB0 = *(srcPixel0);

                        y = kernelSize;
                        // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;
                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                srcR = *(srcPixel + 2);
                                srcG = *(srcPixel + 1);
                                srcB = *(srcPixel);

                                coefR = spatialFunc[x, y] * colorFunc[srcR, srcR0];
                                coefG = spatialFunc[x, y] * colorFunc[srcG, srcG0];
                                coefB = spatialFunc[x, y] * colorFunc[srcB, srcB0];

                                sCoefR += coefR;
                                sCoefG += coefG;
                                sCoefB += coefB;

                                sMembR += coefR * srcR;
                                sMembG += coefG * srcG;
                                sMembB += coefB * srcB;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstR = (byte)(sMembR / sCoefR);
                        dstG = (byte)(sMembG / sCoefG);
                        dstB = (byte)(sMembB / sCoefB);

                        *(dstPixel0 + 2) = dstR;
                        *(dstPixel0 + 1) = dstG;
                        *(dstPixel0) = dstB;
                    }
                });
            }
            else // 8bpp grayscale
            {
                Parallel.For(rect.Top, rect.Height + rect.Top, i =>
                {
                    uint x, y;
                    byte srcC;
                    byte dstC;
                    byte srcC0;
                    byte* srcPixel;
                    byte* srcPixel0;
                    byte* dstPixel0;
                    double sCoefC, sMembC, coefC;

                    for (uint k = (uint)rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint)(source.Stride * i) + (uint)(pixelSize * k);
                        dstPixel0 = dstRow + (uint)(destination.Stride * i) + (uint)(pixelSize * k);
                        srcPixel = srcPixel0 + kernelHalf * (source.Stride + pixelSize); // lower right corner - to start processing from that point
                        sCoefC = 0;
                        sMembC = 0;

                        srcC0 = *(srcPixel0);

                        y = kernelSize;
                        // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;
                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                srcC = *(srcPixel);

                                coefC = spatialFunc[x, y] * colorFunc[srcC, srcC0];

                                sCoefC += coefC;
     
                                sMembC += coefC * srcC;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstC = (byte)(sMembC / sCoefC);

                        *(dstPixel0) = dstC;
                    }
                });
            }
        }

        // For Format32bppArgb alpha channel is not used for calculations
        private unsafe void ProcessWithoutChecks(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            var pixelSize = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat) / 8;
            var kernelHalf = kernelSize / 2;
            var bytesInKernelRow = kernelSize * pixelSize;
            var srcRow = (byte*)source.ImageData;
            var dstRow = (byte*)destination.ImageData;

            if (pixelSize > 1) // For 24bpp, and 32bpp formats
            {
                uint x, y;
                byte srcR, srcG, srcB;
                byte dstR, dstG, dstB;
                byte srcR0, srcG0, srcB0;
                byte* srcPixel;
                byte* srcPixel0;
                byte* dstPixel0;
                double sCoefR, sCoefG, sCoefB, sMembR, sMembG, sMembB, coefR, coefG, coefB;

                for (uint i = (uint)rect.Top; i < rect.Height + rect.Top; i++) //lines, y
                {
                    //if (i % 3 == 0) progress = (uint)(i / rect.Height);

                    for (uint k = (uint)rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint)(source.Stride * i) + (uint)(pixelSize * k);
                        dstPixel0 = dstRow + (uint)(destination.Stride * i) + (uint)(pixelSize * k);
                        srcPixel = srcPixel0 + kernelHalf * (source.Stride + pixelSize);
                        // lower right corner - to start processing from that point
                        sCoefR = 0;
                        sCoefG = 0;
                        sCoefB = 0;
                        sMembR = 0;
                        sMembG = 0;
                        sMembB = 0;

                        srcR0 = *(srcPixel0 + 2);
                        srcG0 = *(srcPixel0 + 1);
                        srcB0 = *(srcPixel0);

                        y = kernelSize; // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;
                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                srcR = *(srcPixel + 2);
                                srcG = *(srcPixel + 1);
                                srcB = *(srcPixel);

                                coefR = spatialFunc[x, y] * colorFunc[srcR, srcR0];
                                coefG = spatialFunc[x, y] * colorFunc[srcG, srcG0];
                                coefB = spatialFunc[x, y] * colorFunc[srcB, srcB0];

                                sCoefR += coefR;
                                sCoefG += coefG;
                                sCoefB += coefB;

                                sMembR += coefR * srcR;
                                sMembG += coefG * srcG;
                                sMembB += coefB * srcB;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstR = (byte)(sMembR / sCoefR);
                        dstG = (byte)(sMembG / sCoefG);
                        dstB = (byte)(sMembB / sCoefB);

                        *(dstPixel0 + 2) = dstR;
                        *(dstPixel0 + 1) = dstG;
                        *(dstPixel0) = dstB;
                    }
                }
            }
            else // 8bpp grayscale
            {
                for (uint i = (uint) rect.Top; i < rect.Height + rect.Top; i++) //lines, y
                {
                    uint x, y;
                    byte srcC;
                    byte dstC;
                    byte srcC0;
                    byte* srcPixel;
                    byte* srcPixel0;
                    byte* dstPixel0;
                    double sCoefC, sMembC, coefC;

                    for (uint k = (uint) rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint) (source.Stride*i) + (uint) (pixelSize*k);
                        dstPixel0 = dstRow + (uint) (destination.Stride*i) + (uint) (pixelSize*k);
                        srcPixel = srcPixel0 + kernelHalf*(source.Stride + pixelSize); // lower right corner - to start processing from that point
                        sCoefC = 0;
                        sMembC = 0;

                        srcC0 = *(srcPixel0);

                        y = kernelSize;
                        // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;
                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                srcC = *(srcPixel);

                                coefC = spatialFunc[x, y]*colorFunc[srcC, srcC0];

                                sCoefC += coefC;

                                sMembC += coefC*srcC;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstC = (byte) (sMembC/sCoefC);

                        *(dstPixel0) = dstC;
                    }
                }
            }
        }

        // The method is different from ProcessCenter in 2 checks to avoid getting out of bounds
        // For Format32bppArgb alpha channel is not used for calculations
        private unsafe void ProcessWithEdgeChecks(UnmanagedImage source, UnmanagedImage destination, Rectangle rect)
        {
            var pixelSize = System.Drawing.Image.GetPixelFormatSize(source.PixelFormat)/8;
            var kernelHalf = kernelSize/2;
            var bytesInKernelRow = kernelSize*pixelSize;
            var srcRow = (byte*) source.ImageData;
            var dstRow = (byte*) destination.ImageData;

            if (pixelSize > 1) // For 24bpp, and 32bpp formats
            {
                int rx, ry;
                uint x, y;
                byte srcR, srcG, srcB;
                byte dstR, dstG, dstB;
                byte srcR0, srcG0, srcB0;
                byte* srcPixel;
                byte* srcPixel0;
                byte* dstPixel0;
                double sCoefR, sCoefG, sCoefB, sMembR, sMembG, sMembB, coefR, coefG, coefB;

                for (uint i = (uint) rect.Top; i < rect.Height + rect.Top; i++) //lines, y
                {
                    for (uint k = (uint) rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint) (source.Stride*i) + (uint) (pixelSize*k);
                        dstPixel0 = dstRow + (uint) (destination.Stride*i) + (uint) (pixelSize*k);
                        srcPixel = srcPixel0 + kernelHalf*(source.Stride + pixelSize);
                            // lower right corner - to start processing from that point
                        sCoefR = 0;
                        sCoefG = 0;
                        sCoefB = 0;
                        sMembR = 0;
                        sMembG = 0;
                        sMembB = 0;

                        srcR0 = *(srcPixel0 + 2);
                        srcG0 = *(srcPixel0 + 1);
                        srcB0 = *(srcPixel0);

                        y = kernelSize; // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;

                            ry = (int) (y - kernelHalf);
                            if ((ry + i >= source.Height) || (ry + i < 0)) // bounds check
                            {
                                srcPixel = srcPixel - source.Stride;
                                continue;
                            }

                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                rx = (int) (x - kernelHalf);

                                if ((rx + k >= source.Width) || (rx + k < 0)) // bounds check
                                {
                                    srcPixel -= pixelSize;
                                    continue;
                                }

                                srcR = *(srcPixel + 2);
                                srcG = *(srcPixel + 1);
                                srcB = *(srcPixel);

                                coefR = spatialFunc[x, y]*colorFunc[srcR, srcR0];
                                coefG = spatialFunc[x, y]*colorFunc[srcG, srcG0];
                                coefB = spatialFunc[x, y]*colorFunc[srcB, srcB0];

                                sCoefR += coefR;
                                sCoefG += coefG;
                                sCoefB += coefB;

                                sMembR += coefR*srcR;
                                sMembG += coefG*srcG;
                                sMembB += coefB*srcB;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstR = (byte) (sMembR/sCoefR);
                        dstG = (byte) (sMembG/sCoefG);
                        dstB = (byte) (sMembB/sCoefB);

                        *(dstPixel0 + 2) = dstR;
                        *(dstPixel0 + 1) = dstG;
                        *(dstPixel0) = dstB;
                    }
                }
            }
            else // 8bpp grayscale
            {
                int rx, ry;
                uint x, y;
                byte srcC;
                byte dstC;
                byte srcC0;
                byte* srcPixel;
                byte* srcPixel0;
                byte* dstPixel0;
                double sCoefC, sMembC, coefC;

                for (uint i = (uint)rect.Top; i < rect.Height + rect.Top; i++) //lines, y
                {
                    for (uint k = (uint)rect.Left; k < rect.Width + rect.Left; k++) //columns, x
                    {
                        srcPixel0 = srcRow + (uint)(source.Stride * i) + (uint)(pixelSize * k);
                        dstPixel0 = dstRow + (uint)(destination.Stride * i) + (uint)(pixelSize * k);
                        srcPixel = srcPixel0 + kernelHalf * (source.Stride + pixelSize);
                        // lower right corner - to start processing from that point
                        
                        sCoefC = 0;
                        sMembC = 0;

                        srcC0 = *(srcPixel0);

                        y = kernelSize; // move from lower right to upper left corner
                        while (y != 0)
                        {
                            y--;

                            ry = (int)(y - kernelHalf);
                            if ((ry + i >= source.Height) || (ry + i < 0)) // bounds check
                            {
                                srcPixel = srcPixel - source.Stride;
                                continue;
                            }

                            x = kernelSize;
                            while (x != 0)
                            {
                                x--;

                                rx = (int)(x - kernelHalf);

                                if ((rx + k >= source.Width) || (rx + k < 0)) // bounds check
                                {
                                    srcPixel -= pixelSize;
                                    continue;
                                }

                                srcC = *(srcPixel);

                                coefC = spatialFunc[x, y] * colorFunc[srcC, srcC0];

                                sCoefC += coefC;

                                sMembC += coefC * srcC;

                                srcPixel -= pixelSize;
                            }

                            srcPixel = srcPixel - source.Stride + bytesInKernelRow;
                        }

                        dstC = (byte)(sMembC / sCoefC);

                        *(dstPixel0) = dstC;
                    }
                }               
            }
        }
    }
}
