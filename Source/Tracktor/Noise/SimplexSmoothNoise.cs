#region License
/*
MIT License

Copyright (c) 2020, 2021 Americus Maximus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion

using System;
using Tracktor.Converters;

namespace Tracktor.Noise
{
    public class SimplexSmoothNoise : SimplexNoise
    {
        public SimplexSmoothNoise(IConverter converter) : base(converter) { }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var i = Math.Floor(xy.X);
            var j = Math.Floor(xy.Y);

            var xi = xy.X - i;
            var yi = xy.Y - j;

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;

            var i1 = i + Constants.PrimeX;
            var j1 = j + Constants.PrimeY;

            var t = (xi + yi) * G2;
            var x0 = xi - t;
            var y0 = yi - t;

            var a0 = (2.0f / 3.0f) - x0 * x0 - y0 * y0;
            var value = (a0 * a0) * (a0 * a0) * Constants.GradCoord(seed, i, j, x0, y0);

            var a1 = (float)(2 * (1 - 2 * G2) * (1 / G2 - 2)) * t + ((float)(-2 * (1 - 2 * G2) * (1 - 2 * G2)) + a0);
            var x1 = x0 - (float)(1 - 2 * G2);
            var y1 = y0 - (float)(1 - 2 * G2);
            value += (a1 * a1) * (a1 * a1) * Constants.GradCoord(seed, i1, j1, x1, y1);

            // Nested conditionals were faster than compact bit logic/arithmetic
            var xmyi = xi - yi;

            if (t > G2)
            {
                if (xi + xmyi > 1)
                {
                    var x2 = x0 + (3 * G2 - 2);
                    var y2 = y0 + (3 * G2 - 1);
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i + (Constants.PrimeX << 1), j + Constants.PrimeY, x2, y2);
                    }
                }
                else
                {
                    var x2 = x0 + G2;
                    var y2 = y0 + (G2 - 1);
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i, j + Constants.PrimeY, x2, y2);
                    }
                }

                if (yi - xmyi > 1)
                {
                    var x3 = x0 + (3 * G2 - 1);
                    var y3 = y0 + (3 * G2 - 2);
                    var a3 = (2.0f / 3.0f) - x3 * x3 - y3 * y3;

                    if (a3 > 0)
                    {
                        value += (a3 * a3) * (a3 * a3) * Constants.GradCoord(seed, i + Constants.PrimeX, j + (Constants.PrimeY << 1), x3, y3);
                    }
                }
                else
                {
                    var x3 = x0 + (G2 - 1);
                    var y3 = y0 + G2;
                    var a3 = (2.0f / 3.0f) - x3 * x3 - y3 * y3;

                    if (a3 > 0)
                    {
                        value += (a3 * a3) * (a3 * a3) * Constants.GradCoord(seed, i + Constants.PrimeX, j, x3, y3);
                    }
                }
            }
            else
            {
                if (xi + xmyi < 0)
                {
                    var x2 = x0 + (1 - G2);
                    var y2 = y0 - G2;
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i - Constants.PrimeX, j, x2, y2);
                    }
                }
                else
                {
                    var x2 = x0 + (G2 - 1);
                    var y2 = y0 + G2;
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i + Constants.PrimeX, j, x2, y2);
                    }
                }

                if (yi < xmyi)
                {
                    var x2 = x0 - G2;
                    var y2 = y0 - (G2 - 1);
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i, j - Constants.PrimeY, x2, y2);
                    }
                }
                else
                {
                    var x2 = x0 + G2;
                    var y2 = y0 + (G2 - 1);
                    var a2 = (2.0f / 3.0f) - x2 * x2 - y2 * y2;

                    if (a2 > 0)
                    {
                        value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i, j + Constants.PrimeY, x2, y2);
                    }
                }
            }

            return value * 18.24196194486065f;
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var i = Math.Floor(xyz.X);
            var j = Math.Floor(xyz.Y);
            var k = Math.Floor(xyz.Z);

            var xi = (float)(xyz.X - i);
            var yi = (float)(xyz.Y - j);
            var zi = (float)(xyz.Z - k);

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;
            k *= Constants.PrimeZ;

            var seed2 = seed + 1293373;

            var xNMask = (int)(-0.5f - xi);
            var yNMask = (int)(-0.5f - yi);
            var zNMask = (int)(-0.5f - zi);

            var x0 = xi + xNMask;
            var y0 = yi + yNMask;
            var z0 = zi + zNMask;
            var a0 = 0.75f - x0 * x0 - y0 * y0 - z0 * z0;

            var value = (a0 * a0) * (a0 * a0) * Constants.GradCoord(seed, i + (xNMask & Constants.PrimeX), j + (yNMask & Constants.PrimeY), k + (zNMask & Constants.PrimeZ), x0, y0, z0);

            var x1 = xi - 0.5f;
            var y1 = yi - 0.5f;
            var z1 = zi - 0.5f;
            var a1 = 0.75f - x1 * x1 - y1 * y1 - z1 * z1;

            value += (a1 * a1) * (a1 * a1) * Constants.GradCoord(seed2, i + Constants.PrimeX, j + Constants.PrimeY, k + Constants.PrimeZ, x1, y1, z1);

            var xAFlipMask0 = ((xNMask | 1) << 1) * x1;
            var yAFlipMask0 = ((yNMask | 1) << 1) * y1;
            var zAFlipMask0 = ((zNMask | 1) << 1) * z1;
            var xAFlipMask1 = (-2 - (xNMask << 2)) * x1 - 1.0f;
            var yAFlipMask1 = (-2 - (yNMask << 2)) * y1 - 1.0f;
            var zAFlipMask1 = (-2 - (zNMask << 2)) * z1 - 1.0f;

            var skip5 = false;
            var a2 = xAFlipMask0 + a0;

            if (a2 > 0)
            {
                var x2 = x0 - (xNMask | 1);
                var y2 = y0;
                var z2 = z0;

                value += (a2 * a2) * (a2 * a2) * Constants.GradCoord(seed, i + (~xNMask & Constants.PrimeX), j + (yNMask & Constants.PrimeY), k + (zNMask & Constants.PrimeZ), x2, y2, z2);
            }
            else
            {
                var a3 = yAFlipMask0 + zAFlipMask0 + a0;

                if (a3 > 0)
                {
                    var x3 = x0;
                    var y3 = y0 - (yNMask | 1);
                    var z3 = z0 - (zNMask | 1);

                    value += (a3 * a3) * (a3 * a3) * Constants.GradCoord(seed, i + (xNMask & Constants.PrimeX), j + (~yNMask & Constants.PrimeY), k + (~zNMask & Constants.PrimeZ), x3, y3, z3);
                }

                var a4 = xAFlipMask1 + a1;

                if (a4 > 0)
                {
                    var x4 = (xNMask | 1) + x1;
                    var y4 = y1;
                    var z4 = z1;

                    value += (a4 * a4) * (a4 * a4) * Constants.GradCoord(seed2, i + (xNMask & (Constants.PrimeX * 2)), j + Constants.PrimeY, k + Constants.PrimeZ, x4, y4, z4);

                    skip5 = true;
                }
            }

            var skip9 = false;
            var a6 = yAFlipMask0 + a0;

            if (a6 > 0)
            {
                var x6 = x0;
                var y6 = y0 - (yNMask | 1);
                var z6 = z0;

                value += (a6 * a6) * (a6 * a6) * Constants.GradCoord(seed, i + (xNMask & Constants.PrimeX), j + (~yNMask & Constants.PrimeY), k + (zNMask & Constants.PrimeZ), x6, y6, z6);
            }
            else
            {
                var a7 = xAFlipMask0 + zAFlipMask0 + a0;

                if (a7 > 0)
                {
                    var x7 = x0 - (xNMask | 1);
                    var y7 = y0;
                    var z7 = z0 - (zNMask | 1);

                    value += (a7 * a7) * (a7 * a7) * Constants.GradCoord(seed, i + (~xNMask & Constants.PrimeX), j + (yNMask & Constants.PrimeY), k + (~zNMask & Constants.PrimeZ), x7, y7, z7);
                }

                var a8 = yAFlipMask1 + a1;

                if (a8 > 0)
                {
                    var x8 = x1;
                    var y8 = (yNMask | 1) + y1;
                    var z8 = z1;

                    value += (a8 * a8) * (a8 * a8) * Constants.GradCoord(seed2, i + Constants.PrimeX, j + (yNMask & (Constants.PrimeY << 1)), k + Constants.PrimeZ, x8, y8, z8);

                    skip9 = true;
                }
            }

            var skipD = false;
            var aA = zAFlipMask0 + a0;

            if (aA > 0)
            {
                var xA = x0;
                var yA = y0;
                var zA = z0 - (zNMask | 1);

                value += (aA * aA) * (aA * aA) * Constants.GradCoord(seed, i + (xNMask & Constants.PrimeX), j + (yNMask & Constants.PrimeY), k + (~zNMask & Constants.PrimeZ), xA, yA, zA);
            }
            else
            {
                var aB = xAFlipMask0 + yAFlipMask0 + a0;

                if (aB > 0)
                {
                    var xB = x0 - (xNMask | 1);
                    var yB = y0 - (yNMask | 1);
                    var zB = z0;

                    value += (aB * aB) * (aB * aB) * Constants.GradCoord(seed, i + (~xNMask & Constants.PrimeX), j + (~yNMask & Constants.PrimeY), k + (zNMask & Constants.PrimeZ), xB, yB, zB);
                }

                var aC = zAFlipMask1 + a1;

                if (aC > 0)
                {
                    var xC = x1;
                    var yC = y1;
                    var zC = (zNMask | 1) + z1;

                    value += (aC * aC) * (aC * aC) * Constants.GradCoord(seed2, i + Constants.PrimeX, j + Constants.PrimeY, k + (zNMask & (Constants.PrimeZ << 1)), xC, yC, zC);

                    skipD = true;
                }
            }

            if (!skip5)
            {
                var a5 = yAFlipMask1 + zAFlipMask1 + a1;

                if (a5 > 0)
                {
                    var x5 = x1;
                    var y5 = (yNMask | 1) + y1;
                    var z5 = (zNMask | 1) + z1;

                    value += (a5 * a5) * (a5 * a5) * Constants.GradCoord(seed2, i + Constants.PrimeX, j + (yNMask & (Constants.PrimeY << 1)), k + (zNMask & (Constants.PrimeZ << 1)), x5, y5, z5);
                }
            }

            if (!skip9)
            {
                var a9 = xAFlipMask1 + zAFlipMask1 + a1;

                if (a9 > 0)
                {
                    var x9 = (xNMask | 1) + x1;
                    var y9 = y1;
                    var z9 = (zNMask | 1) + z1;

                    value += (a9 * a9) * (a9 * a9) * Constants.GradCoord(seed2, i + (xNMask & (Constants.PrimeX * 2)), j + Constants.PrimeY, k + (zNMask & (Constants.PrimeZ << 1)), x9, y9, z9);
                }
            }

            if (!skipD)
            {
                var aD = xAFlipMask1 + yAFlipMask1 + a1;

                if (aD > 0)
                {
                    var xD = (xNMask | 1) + x1;
                    var yD = (yNMask | 1) + y1;
                    var zD = z1;

                    value += (aD * aD) * (aD * aD) * Constants.GradCoord(seed2, i + (xNMask & (Constants.PrimeX << 1)), j + (yNMask & (Constants.PrimeY << 1)), k + Constants.PrimeZ, xD, yD, zD);
                }
            }

            return value * 9.046026385208288f;
        }
    }
}
