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
using ImagePixelEnumerator.ColorCaches.Common;

namespace ImagePixelEnumerator.Helpers
{
    internal class ColorModelHelper
    {
        #region | Constants |

        private const int X = 0;
        private const int Y = 1;
        private const int Z = 2;

        private const float Epsilon = 1E-05f;
        private const float OneThird = 1.0f / 3.0f;
        private const float TwoThirds = 2.0f * OneThird;
        public const Double HueFactor = 1.4117647058823529411764705882353;

        private static readonly float[] XYZWhite = new[] { 95.05f, 100.00f, 108.90f };

        private static readonly float[,] Rgb2Xyz = 
        { 
            { 0.41239083F, 0.35758433F, 0.18048081F },
	        { 0.21263903F, 0.71516865F, 0.072192319F },
	        { 0.019330820F, 0.11919473F, 0.95053220F }
        };

        private static readonly float[,] Xyz2Rgb = 
        {
	        { 3.2409699F, -1.5373832F, -0.49861079F },
	        { -0.96924376F, 1.8759676F, 0.041555084F },
	        { 0.055630036F, -0.20397687F, 1.0569715F }
        };

        #endregion

        #region | -> RGB |

        private static Int32 GetColorComponent(Single v1, Single v2, Single hue)
        {
            Single preresult;

            if (hue < 0.0f) hue++;
            if (hue > 1.0f) hue--;

            if ((6.0f * hue) < 1.0f)
            {
                preresult = v1 + (((v2 - v1) * 6.0f) * hue);
            }
            else if ((2.0f * hue) < 1.0f)
            {
                preresult = v2;
            }
            else if ((3.0f * hue) < 2.0f)
            {
                preresult = v1 + (((v2 - v1) * (TwoThirds - hue)) * 6.0f);
            }
            else
            {
                preresult = v1;
            }

            return Convert.ToInt32(255.0f * preresult);
        }

        public static Color HSBtoRGB(Single hue, Single saturation, Single brightness)
        {
            // initializes the default black
            Int32 red = 0;
            Int32 green = 0;
            Int32 blue = 0;

            // only if there is some brightness; otherwise leave it pitch black
            if (brightness > 0.0f)
            {
                // if there is no saturation; leave it gray based on the brightness only
                if (Math.Abs(saturation - 0.0f) < Epsilon)
                {
                    red = green = blue = Convert.ToInt32(255.0f * brightness);
                }
                else // the color is more complex
                {
                    // converts HSL cylinder to one slice (its factors)
                    Single factorHue = hue / 360.0f;
                    Single factorA = brightness < 0.5f ? brightness * (1.0f + saturation) : (brightness + saturation) - (brightness * saturation);
                    Single factorB = (2.0f * brightness) - factorA;

                    // maps HSL slice to a RGB cube
                    red = GetColorComponent(factorB, factorA, factorHue + OneThird);
                    green = GetColorComponent(factorB, factorA, factorHue);
                    blue = GetColorComponent(factorB, factorA, factorHue - OneThird);
                }
            }

            Int32 argb = 255 << 24 | red << 16 | green << 8 | blue;
            return Color.FromArgb(argb);
        }

        #endregion

        #region | RGB -> |

        public static void RGBtoLab(Int32 red, Int32 green, Int32 blue, out Single l, out Single a, out Single b)
        {
            Single x, y, z;
            RGBtoXYZ(red, green, blue, out x, out y, out z);
            XYZtoLab(x, y, z, out l, out a, out b);
        }

        public static void RGBtoXYZ(Int32 red, Int32 green, Int32 blue, out Single x, out Single y, out Single z)
        {
            // normalize red, green, blue values
            Double redFactor = red / 255.0;
            Double greenFactor = green / 255.0;
            Double blueFactor = blue / 255.0;

            // convert to a sRGB form
            Double sRed = (redFactor > 0.04045) ? Math.Pow((redFactor + 0.055) / (1 + 0.055), 2.2) : (redFactor / 12.92);
            Double sGreen = (greenFactor > 0.04045) ? Math.Pow((greenFactor + 0.055) / (1 + 0.055), 2.2) : (greenFactor / 12.92);
            Double sBlue = (blueFactor > 0.04045) ? Math.Pow((blueFactor + 0.055) / (1 + 0.055), 2.2) : (blueFactor / 12.92);

            // converts
            x = Convert.ToSingle(sRed * 0.4124 + sGreen * 0.3576 + sBlue * 0.1805);
            y = Convert.ToSingle(sRed * 0.2126 + sGreen * 0.7152 + sBlue * 0.0722);
            z = Convert.ToSingle(sRed * 0.0193 + sGreen * 0.1192 + sBlue * 0.9505);
        }

        #endregion

        #region | XYZ -> |

        private static Single GetXYZValue(Single value)
        {
            return value > 0.008856f ? (Single)Math.Pow(value, OneThird) : (7.787f * value + 16.0f / 116.0f);
        }

        public static void XYZtoLab(Single x, Single y, Single z, out Single l, out Single a, out Single b)
        {
            l = 116.0f * GetXYZValue(y / XYZWhite[Y]) - 16.0f;
            a = 500.0f * (GetXYZValue(x / XYZWhite[X]) - GetXYZValue(y / XYZWhite[Y]));
            b = 200.0f * (GetXYZValue(y / XYZWhite[Y]) - GetXYZValue(z / XYZWhite[Z]));
        }

        #endregion

        #region | Methods |

        public static Int64 GetColorEuclideanDistance(ColorModel colorModel, Color requestedColor, Color realColor)
        {
            Single componentA, componentB, componentC;
            GetColorComponents(colorModel, requestedColor, realColor, out componentA, out componentB, out componentC);
            return (Int64)(componentA * componentA + componentB * componentB + componentC * componentC);
        }

        public static Int32 GetEuclideanDistance(Color color, ColorModel colorModel, IList<Color> palette)
        {
            // initializes the best difference, set it for worst possible, it can only get better
            Int64 leastDistance = Int64.MaxValue;
            Int32 result = 0;

            for (Int32 index = 0; index < palette.Count; index++)
            {
                Color targetColor = palette[index];
                Int64 distance = GetColorEuclideanDistance(colorModel, color, targetColor);

                // if a difference is zero, we're good because it won't get better
                if (distance == 0)
                {
                    result = index;
                    break;
                }

                // if a difference is the best so far, stores it as our best candidate
                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    result = index;
                }
            }

            return result;
        }

        public static Int32 GetComponentA(ColorModel colorModel, Color color)
        {
            Int32 result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.R;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetHue() / HueFactor);
                    break;

                case ColorModel.LabColorSpace:
                    Single l, a, b;
                    RGBtoLab(color.R, color.G, color.B, out l, out a, out b);
                    result = Convert.ToInt32(l * 255.0f);
                    break;
            }

            return result;
        }

        public static Int32 GetComponentB(ColorModel colorModel, Color color)
        {
            Int32 result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.G;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetSaturation() * 255);
                    break;

                case ColorModel.LabColorSpace:
                    Single l, a, b;
                    RGBtoLab(color.R, color.G, color.B, out l, out a, out b);
                    result = Convert.ToInt32(a * 255.0f);
                    break;
            }

            return result;
        }

        public static Int32 GetComponentC(ColorModel colorModel, Color color)
        {
            Int32 result = 0;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    result = color.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    result = Convert.ToInt32(color.GetBrightness() * 255);
                    break;

                case ColorModel.LabColorSpace:
                    Single l, a, b;
                    RGBtoLab(color.R, color.G, color.B, out l, out a, out b);
                    result = Convert.ToInt32(b * 255.0f);
                    break;
            }

            return result;
        }

        public static void GetColorComponents(ColorModel colorModel, Color color, out Single componentA, out Single componentB, out Single componentC)
        {
            componentA = 0.0f;
            componentB = 0.0f;
            componentC = 0.0f;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    componentA = color.R;
                    componentB = color.G;
                    componentC = color.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    componentA = color.GetHue();
                    componentB = color.GetSaturation();
                    componentC = color.GetBrightness();
                    break;

                case ColorModel.LabColorSpace:
                    RGBtoLab(color.R, color.G, color.B, out componentA, out componentB, out componentC);
                    break;

                case ColorModel.XYZ:
                    RGBtoXYZ(color.R, color.G, color.B, out componentA, out componentB, out componentC);
                    break;
            }
        }

        public static void GetColorComponents(ColorModel colorModel, Color color, Color targetColor, out Single componentA, out Single componentB, out Single componentC)
        {
            componentA = 0.0f;
            componentB = 0.0f;
            componentC = 0.0f;

            switch (colorModel)
            {
                case ColorModel.RedGreenBlue:
                    componentA = color.R - targetColor.R;
                    componentB = color.G - targetColor.G;
                    componentC = color.B - targetColor.B;
                    break;

                case ColorModel.HueSaturationBrightness:
                    componentA = color.GetHue() - targetColor.GetHue();
                    componentB = color.GetSaturation() - targetColor.GetSaturation();
                    componentC = color.GetBrightness() - targetColor.GetBrightness();
                    break;

                case ColorModel.LabColorSpace:

                    Single sourceL, sourceA, sourceB;
                    Single targetL, targetA, targetB;

                    RGBtoLab(color.R, color.G, color.B, out sourceL, out sourceA, out sourceB);
                    RGBtoLab(targetColor.R, targetColor.G, targetColor.B, out targetL, out targetA, out targetB);

                    componentA = sourceL - targetL;
                    componentB = sourceA - targetA;
                    componentC = sourceB - targetB;

                    break;

                case ColorModel.XYZ:

                    Single sourceX, sourceY, sourceZ;
                    Single targetX, targetY, targetZ;

                    RGBtoXYZ(color.R, color.G, color.B, out sourceX, out sourceY, out sourceZ);
                    RGBtoXYZ(targetColor.R, targetColor.G, targetColor.B, out targetX, out targetY, out targetZ);

                    componentA = sourceX - targetX;
                    componentB = sourceY - targetY;
                    componentC = sourceZ - targetZ;

                    break;
            }
        }

        public static void HighColorAmplification(ref Int32 red, ref Int32 green, ref Int32 blue)
        {
            Single redfloatValue = red / 8192.0f;
            Single greenfloatValue = green / 8192.0f;
            Single bluefloatValue = blue / 8192.0f;

            Single[] result = new Single[3];

            for (Int32 index = 0; index < 3; index++)
            {
                result[index] += Rgb2Xyz[index, 0] * redfloatValue;
                result[index] += Rgb2Xyz[index, 1] * greenfloatValue;
                result[index] += Rgb2Xyz[index, 2] * bluefloatValue;
            }

            Single x = result[0] + result[1] + result[2];
            Single y = result[1];

            if (x > 0)
            {
                redfloatValue = y;
                greenfloatValue = result[0] / x;
                bluefloatValue = result[1] / x;
            }
            else
            {
                redfloatValue = 0.0f;
                greenfloatValue = 0.0f;
                bluefloatValue = 0.0f;
            }

            Single bias = (Single)(Math.Log(0.85f) / -0.693147f);
            Single exposure = 1.0f;
            Single lumAvg = redfloatValue;
            Single lumMax = redfloatValue;
            Single lumNormal = lumMax / lumAvg;
            Single divider = (Single)Math.Log10(lumNormal + 1.0);

            Double yw = (redfloatValue / lumAvg) * exposure;
            Double interpol = Math.Log(2 + Math.Pow(yw / lumNormal, bias) * 8);
            Double l = PadeLog(yw);
            redfloatValue = (Single)((l / interpol) / divider);

            Single z;
            y = redfloatValue;
            Array.Clear(result, 0, 3);

            result[1] = greenfloatValue;
            result[2] = bluefloatValue;

            if ((y > 1e-06f) && (result[1] > 1e-06F) && (result[2] > 1e-06F))
            {
                x = (result[1] * y) / result[2];
                z = (x / result[1]) - x - y;
            }
            else
            {
                x = z = 1e-06F;
            }

            redfloatValue = x;
            greenfloatValue = y;
            bluefloatValue = z;

            Array.Clear(result, 0, 3);

            for (int i = 0; i < 3; i++)
            {
                result[i] += Xyz2Rgb[i, 0] * redfloatValue;
                result[i] += Xyz2Rgb[i, 1] * greenfloatValue;
                result[i] += Xyz2Rgb[i, 2] * bluefloatValue;
            }

            redfloatValue = result[0] > 1 ? 1 : result[0];
            greenfloatValue = result[1] > 1 ? 1 : result[1];
            bluefloatValue = result[2] > 1 ? 1 : result[2];

            red = (Byte)(255 * redfloatValue + 0.5);
            green = (Byte)(255 * greenfloatValue + 0.5);
            blue = (Byte)(255 * bluefloatValue + 0.5);
        }

        private static Double PadeLog(Double value)
        {
            if (value < 1)
            {
                return (value * (6 + value) / (6 + 4 * value));
            }

            if (value < 2)
            {
                return (value * (6 + 0.7662 * value) / (5.9897 + 3.7658 * value));
            }

            return Math.Log(value + 1);
        }

        #endregion
    }
}