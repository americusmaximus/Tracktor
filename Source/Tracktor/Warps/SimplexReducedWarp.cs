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

namespace Tracktor.Warps
{
    public class SimplexReducedWarp : AbstractWarp
    {
        protected const float F2 = 0.5f * (SQRT3 - 1);

        protected const float G2 = (3 - SQRT3) / 6;

        protected const float SQRT3 = 1.7320508075688772935274463415059f;

        public SimplexReducedWarp(IConverter converter) : base(converter) { }

        protected override XY Convert(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return new XY(xy.X + (xy.X + xy.Y) * F2, xy.Y + (xy.X + xy.Y) * F2);
        }

        protected override XY GetValue(int seed, float amptitude, float frequency, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            xy *= frequency;

            var i = Math.Floor(xy.X);
            var j = Math.Floor(xy.Y);

            var xi = xy.X - i;
            var yi = xy.Y - j;

            var t = (xi + yi) * G2;
            var x0 = (float)(xi - t);
            var y0 = (float)(yi - t);

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;

            var a = 0.5f - x0 * x0 - y0 * y0;

            var result = new XY();

            if (a > 0)
            {
                var aaaa = a * a * a * a;

                result += aaaa * Constants.GradCoord(seed, i, j);
            }

            var c = 2 * (1 - 2 * G2) * (1 / G2 - 2) * t + ((-2 * (1 - 2 * G2) * (1 - 2 * G2)) + a);

            if (c > 0)
            {
                var cccc = c * c * c * c;

                result += cccc * Constants.GradCoord(seed, i + Constants.PrimeX, j + Constants.PrimeY);
            }

            if (y0 > x0)
            {
                var x1 = x0 + G2;
                var y1 = y0 + (G2 - 1);
                var b = 0.5f - x1 * x1 - y1 * y1;

                if (b > 0)
                {
                    var bbbb = b * b * b * b;

                    result += bbbb * Constants.GradCoord(seed, i, j + Constants.PrimeY);
                }
            }
            else
            {
                var x1 = x0 + (G2 - 1);
                var y1 = y0 + G2;
                var b = 0.5f - x1 * x1 - y1 * y1;

                if (b > 0)
                {
                    var bbbb = b * b * b * b;

                    result += bbbb * Constants.GradCoord(seed, i + Constants.PrimeX, j);
                }
            }

            return new XY(result.X * amptitude * 16.0f, result.Y * amptitude * 16.0f);
        }

        protected override XYZ GetValue(int seed, float amptitude, float frequency, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            xyz *= frequency;

            var i = Math.Round(xyz.X);
            var j = Math.Round(xyz.Y);
            var k = Math.Round(xyz.Z);

            var x0 = xyz.X - i;
            var y0 = xyz.Y - j;
            var z0 = xyz.Z - k;

            var xNSign = (int)(-x0 - 1.0f) | 1;
            var yNSign = (int)(-y0 - 1.0f) | 1;
            var zNSign = (int)(-z0 - 1.0f) | 1;

            var ax0 = xNSign * -x0;
            var ay0 = yNSign * -y0;
            var az0 = zNSign * -z0;

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;
            k *= Constants.PrimeZ;

            var result = new XYZ();

            var a = (0.6f - x0 * x0) - (y0 * y0 + z0 * z0);

            for (var l = 0; ; l++)
            {
                if (a > 0)
                {
                    var aaaa = a * a * a * a;

                    result += aaaa * Constants.GradCoord(seed, i, j, k);
                }

                var b = a;
                var i1 = i;
                var j1 = j;
                var k1 = k;

                if (ax0 >= ay0 && ax0 >= az0)
                {
                    b = b + ax0 + ax0;
                    i1 -= xNSign * Constants.PrimeX;
                }
                else if (ay0 > ax0 && ay0 >= az0)
                {
                    b = b + ay0 + ay0;
                    j1 -= yNSign * Constants.PrimeY;
                }
                else
                {
                    b = b + az0 + az0;
                    k1 -= zNSign * Constants.PrimeZ;
                }

                if (b > 1)
                {
                    b -= 1;
                    var bbbb = (b * b) * (b * b);

                    result += bbbb * Constants.GradCoord(seed, i1, j1, k1);
                }

                if (l == 1) break;

                ax0 = 0.5f - ax0;
                ay0 = 0.5f - ay0;
                az0 = 0.5f - az0;

                a += (0.75f - ax0) - (ay0 + az0);

                i += (xNSign >> 1) & Constants.PrimeX;
                j += (yNSign >> 1) & Constants.PrimeY;
                k += (zNSign >> 1) & Constants.PrimeZ;

                xNSign = -xNSign;
                yNSign = -yNSign;
                zNSign = -zNSign;

                seed += 1293373;
            }

            return new XYZ(result.X * amptitude * 7.71604938271605f, result.Y * amptitude * 7.71604938271605f, result.Z * amptitude * 7.71604938271605f);
        }
    }
}
