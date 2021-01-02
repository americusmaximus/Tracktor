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
    public class ValueNoise : AbstractNoise
    {
        public ValueNoise(IConverter converter) : base(converter) { }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var x0 = Math.Floor(xy.X);
            var y0 = Math.Floor(xy.Y);

            var xs = Math.InterpHermite((float)(xy.X - x0));
            var ys = Math.InterpHermite((float)(xy.Y - y0));

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;

            var xf0 = Math.Lerp(Constants.ValCoord(seed, x0, y0), Constants.ValCoord(seed, x1, y0), xs);
            var xf1 = Math.Lerp(Constants.ValCoord(seed, x0, y1), Constants.ValCoord(seed, x1, y1), xs);

            return Math.Lerp(xf0, xf1, ys);
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var x0 = Math.Floor(xyz.X);
            var y0 = Math.Floor(xyz.Y);
            var z0 = Math.Floor(xyz.Z);

            var xs = Math.InterpHermite((float)(xyz.X - x0));
            var ys = Math.InterpHermite((float)(xyz.Y - y0));
            var zs = Math.InterpHermite((float)(xyz.Z - z0));

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;
            z0 *= Constants.PrimeZ;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;
            var z1 = z0 + Constants.PrimeZ;

            var xf00 = Math.Lerp(Constants.ValCoord(seed, x0, y0, z0), Constants.ValCoord(seed, x1, y0, z0), xs);
            var xf10 = Math.Lerp(Constants.ValCoord(seed, x0, y1, z0), Constants.ValCoord(seed, x1, y1, z0), xs);
            var xf01 = Math.Lerp(Constants.ValCoord(seed, x0, y0, z1), Constants.ValCoord(seed, x1, y0, z1), xs);
            var xf11 = Math.Lerp(Constants.ValCoord(seed, x0, y1, z1), Constants.ValCoord(seed, x1, y1, z1), xs);

            var yf0 = Math.Lerp(xf00, xf10, ys);
            var yf1 = Math.Lerp(xf01, xf11, ys);

            return Math.Lerp(yf0, yf1, zs);
        }
    }
}
