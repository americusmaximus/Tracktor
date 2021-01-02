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
    public class SimplexNoise : AbstractNoise
    {
        protected const float F2 = 0.5f * (SQRT3 - 1);

        protected const float G2 = (3 - SQRT3) / 6;

        protected const float SQRT3 = 1.7320508075688772935274463415059f;

        public SimplexNoise(IConverter converter) : base(converter) { }

        protected override XY Convert(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return Converter.Convert(new XY(xy.X * Frequency + (xy.X + xy.Y) * Frequency * F2, xy.Y * Frequency + (xy.X + xy.Y) * Frequency * F2));
        }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var i = Math.Floor(xy.X);
            var j = Math.Floor(xy.Y);
            var xi = (float)(xy.X - i);
            var yi = (float)(xy.Y - j);

            var t = (xi + yi) * G2;
            var x0 = (float)(xi - t);
            var y0 = (float)(yi - t);

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;

            var n1 = 0.0f;

            var a = 0.5f - x0 * x0 - y0 * y0;
            var n0 = a <= 0 ? 0 : (a * a) * (a * a) * Constants.GradCoord(seed, i, j, x0, y0);

            var c = (float)(2 * (1 - 2 * G2) * (1 / G2 - 2)) * t + ((float)(-2 * (1 - 2 * G2) * (1 - 2 * G2)) + a);
            var n2 = c <= 0 ? 0 : (c * c) * (c * c) * Constants.GradCoord(seed, i + Constants.PrimeX, j + Constants.PrimeY, x0 + (2 * G2 - 1), y0 + (2 * G2 - 1));

            if (y0 > x0)
            {
                var x1 = x0 + G2;
                var y1 = y0 + (G2 - 1);
                var b = 0.5f - x1 * x1 - y1 * y1;

                n1 = b <= 0 ? 0 : (b * b) * (b * b) * Constants.GradCoord(seed, i, j + Constants.PrimeY, x1, y1);
            }
            else
            {
                var x1 = x0 + (G2 - 1);
                var y1 = y0 + G2;
                var b = 0.5f - x1 * x1 - y1 * y1;

                n1 = b <= 0 ? 0 : (b * b) * (b * b) * Constants.GradCoord(seed, i + Constants.PrimeX, j, x1, y1);
            }

            return (n0 + n1 + n2) * 99.83685446303647f;
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var i = Math.Round(xyz.X);
            var j = Math.Round(xyz.Y);
            var k = Math.Round(xyz.Z);

            var x0 = xyz.X - i;
            var y0 = xyz.Y - j;
            var z0 = xyz.Z - k;

            var xNSign = (int)(-1.0f - x0) | 1;
            var yNSign = (int)(-1.0f - y0) | 1;
            var zNSign = (int)(-1.0f - z0) | 1;

            var ax0 = xNSign * -x0;
            var ay0 = yNSign * -y0;
            var az0 = zNSign * -z0;

            i *= Constants.PrimeX;
            j *= Constants.PrimeY;
            k *= Constants.PrimeZ;

            var value = 0.0f;
            var a = (0.6f - x0 * x0) - (y0 * y0 + z0 * z0);

            for (int l = 0; ; l++)
            {
                if (a > 0)
                {
                    value += (a * a) * (a * a) * Constants.GradCoord(seed, i, j, k, x0, y0, z0);
                }

                if (ax0 >= ay0 && ax0 >= az0)
                {
                    var b = a + ax0 + ax0;

                    if (b > 1)
                    {
                        b -= 1;
                        value += (b * b) * (b * b) * Constants.GradCoord(seed, i - xNSign * Constants.PrimeX, j, k, x0 + xNSign, y0, z0);
                    }
                }
                else if (ay0 > ax0 && ay0 >= az0)
                {
                    var b = a + ay0 + ay0;

                    if (b > 1)
                    {
                        b -= 1;
                        value += (b * b) * (b * b) * Constants.GradCoord(seed, i, j - yNSign * Constants.PrimeY, k, x0, y0 + yNSign, z0);
                    }
                }
                else
                {
                    var b = a + az0 + az0;

                    if (b > 1)
                    {
                        b -= 1;
                        value += (b * b) * (b * b) * Constants.GradCoord(seed, i, j, k - zNSign * Constants.PrimeZ, x0, y0, z0 + zNSign);
                    }
                }

                if (l == 1) break;

                ax0 = 0.5f - ax0;
                ay0 = 0.5f - ay0;
                az0 = 0.5f - az0;

                x0 = xNSign * ax0;
                y0 = yNSign * ay0;
                z0 = zNSign * az0;

                a += (0.75f - ax0) - (ay0 + az0);

                i += (xNSign >> 1) & Constants.PrimeX;
                j += (yNSign >> 1) & Constants.PrimeY;
                k += (zNSign >> 1) & Constants.PrimeZ;

                xNSign = -xNSign;
                yNSign = -yNSign;
                zNSign = -zNSign;

                seed = ~seed;
            }

            return value * 32.69428253173828125f;
        }
    }
}
