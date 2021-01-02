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
    public class CubicValueNoise : AbstractNoise
    {
        public CubicValueNoise(IConverter converter) : base(converter) { }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var x1 = Math.Floor(xy.X);
            var y1 = Math.Floor(xy.Y);

            var xs = (float)(xy.X - x1);
            var ys = (float)(xy.Y - y1);

            x1 *= Constants.PrimeX;
            y1 *= Constants.PrimeY;

            var x0 = x1 - Constants.PrimeX;
            var y0 = y1 - Constants.PrimeY;
            var x2 = x1 + Constants.PrimeX;
            var y2 = y1 + Constants.PrimeY;
            var x3 = x1 + unchecked(Constants.PrimeX * 2);
            var y3 = y1 + unchecked(Constants.PrimeY * 2);

            return Math.LerpCubic(
                Math.LerpCubic(Constants.ValCoord(seed, x0, y0), Constants.ValCoord(seed, x1, y0), Constants.ValCoord(seed, x2, y0), Constants.ValCoord(seed, x3, y0),
                xs),
                Math.LerpCubic(Constants.ValCoord(seed, x0, y1), Constants.ValCoord(seed, x1, y1), Constants.ValCoord(seed, x2, y1), Constants.ValCoord(seed, x3, y1),
                xs),
                Math.LerpCubic(Constants.ValCoord(seed, x0, y2), Constants.ValCoord(seed, x1, y2), Constants.ValCoord(seed, x2, y2), Constants.ValCoord(seed, x3, y2),
                xs),
                Math.LerpCubic(Constants.ValCoord(seed, x0, y3), Constants.ValCoord(seed, x1, y3), Constants.ValCoord(seed, x2, y3), Constants.ValCoord(seed, x3, y3),
                xs),
                ys) * (1 / (1.5f * 1.5f));
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var x1 = Math.Floor(xyz.X);
            var y1 = Math.Floor(xyz.Y);
            var z1 = Math.Floor(xyz.Z);

            var xs = (float)(xyz.X - x1);
            var ys = (float)(xyz.Y - y1);
            var zs = (float)(xyz.Z - z1);

            x1 *= Constants.PrimeX;
            y1 *= Constants.PrimeY;
            z1 *= Constants.PrimeZ;

            var x0 = x1 - Constants.PrimeX;
            var y0 = y1 - Constants.PrimeY;
            var z0 = z1 - Constants.PrimeZ;
            var x2 = x1 + Constants.PrimeX;
            var y2 = y1 + Constants.PrimeY;
            var z2 = z1 + Constants.PrimeZ;

            var x3 = x1 + unchecked(Constants.PrimeX * 2);
            var y3 = y1 + unchecked(Constants.PrimeY * 2);
            var z3 = z1 + unchecked(Constants.PrimeZ * 2);

            return Math.LerpCubic(
                Math.LerpCubic(
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y0, z0), Constants.ValCoord(seed, x1, y0, z0), Constants.ValCoord(seed, x2, y0, z0), Constants.ValCoord(seed, x3, y0, z0), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y1, z0), Constants.ValCoord(seed, x1, y1, z0), Constants.ValCoord(seed, x2, y1, z0), Constants.ValCoord(seed, x3, y1, z0), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y2, z0), Constants.ValCoord(seed, x1, y2, z0), Constants.ValCoord(seed, x2, y2, z0), Constants.ValCoord(seed, x3, y2, z0), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y3, z0), Constants.ValCoord(seed, x1, y3, z0), Constants.ValCoord(seed, x2, y3, z0), Constants.ValCoord(seed, x3, y3, z0), xs),
                    ys),
                Math.LerpCubic(
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y0, z1), Constants.ValCoord(seed, x1, y0, z1), Constants.ValCoord(seed, x2, y0, z1), Constants.ValCoord(seed, x3, y0, z1), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y1, z1), Constants.ValCoord(seed, x1, y1, z1), Constants.ValCoord(seed, x2, y1, z1), Constants.ValCoord(seed, x3, y1, z1), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y2, z1), Constants.ValCoord(seed, x1, y2, z1), Constants.ValCoord(seed, x2, y2, z1), Constants.ValCoord(seed, x3, y2, z1), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y3, z1), Constants.ValCoord(seed, x1, y3, z1), Constants.ValCoord(seed, x2, y3, z1), Constants.ValCoord(seed, x3, y3, z1), xs),
                    ys),
                Math.LerpCubic(
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y0, z2), Constants.ValCoord(seed, x1, y0, z2), Constants.ValCoord(seed, x2, y0, z2), Constants.ValCoord(seed, x3, y0, z2), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y1, z2), Constants.ValCoord(seed, x1, y1, z2), Constants.ValCoord(seed, x2, y1, z2), Constants.ValCoord(seed, x3, y1, z2), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y2, z2), Constants.ValCoord(seed, x1, y2, z2), Constants.ValCoord(seed, x2, y2, z2), Constants.ValCoord(seed, x3, y2, z2), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y3, z2), Constants.ValCoord(seed, x1, y3, z2), Constants.ValCoord(seed, x2, y3, z2), Constants.ValCoord(seed, x3, y3, z2), xs),
                    ys),
                Math.LerpCubic(
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y0, z3), Constants.ValCoord(seed, x1, y0, z3), Constants.ValCoord(seed, x2, y0, z3), Constants.ValCoord(seed, x3, y0, z3), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y1, z3), Constants.ValCoord(seed, x1, y1, z3), Constants.ValCoord(seed, x2, y1, z3), Constants.ValCoord(seed, x3, y1, z3), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y2, z3), Constants.ValCoord(seed, x1, y2, z3), Constants.ValCoord(seed, x2, y2, z3), Constants.ValCoord(seed, x3, y2, z3), xs),
                    Math.LerpCubic(Constants.ValCoord(seed, x0, y3, z3), Constants.ValCoord(seed, x1, y3, z3), Constants.ValCoord(seed, x2, y3, z3), Constants.ValCoord(seed, x3, y3, z3), xs),
                    ys),
                zs) * (1 / (1.5f * 1.5f * 1.5f));
        }
    }
}
