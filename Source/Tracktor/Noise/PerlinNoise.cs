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
    public class PerlinNoise : AbstractNoise
    {
        public PerlinNoise(IConverter converter) : base(converter) { }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var x0 = Math.Floor(xy.X);
            var y0 = Math.Floor(xy.Y);

            var xd0 = (float)(xy.X - x0);
            var yd0 = (float)(xy.Y - y0);
            var xd1 = xd0 - 1;
            var yd1 = yd0 - 1;

            var xs = Math.InterpQuintic(xd0);
            var ys = Math.InterpQuintic(yd0);

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;

            var xf0 = Math.Lerp(Constants.GradCoord(seed, x0, y0, xd0, yd0), Constants.GradCoord(seed, x1, y0, xd1, yd0), xs);
            var xf1 = Math.Lerp(Constants.GradCoord(seed, x0, y1, xd0, yd1), Constants.GradCoord(seed, x1, y1, xd1, yd1), xs);

            return Math.Lerp(xf0, xf1, ys) * 1.4247691104677813f;
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var x0 = Math.Floor(xyz.X);
            var y0 = Math.Floor(xyz.Y);
            var z0 = Math.Floor(xyz.Z);

            var xd0 = (float)(xyz.X - x0);
            var yd0 = (float)(xyz.Y - y0);
            var zd0 = (float)(xyz.Z - z0);
            var xd1 = xd0 - 1;
            var yd1 = yd0 - 1;
            var zd1 = zd0 - 1;

            var xs = Math.InterpQuintic(xd0);
            var ys = Math.InterpQuintic(yd0);
            var zs = Math.InterpQuintic(zd0);

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;
            z0 *= Constants.PrimeZ;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;
            var z1 = z0 + Constants.PrimeZ;

            var xf00 = Math.Lerp(Constants.GradCoord(seed, x0, y0, z0, xd0, yd0, zd0), Constants.GradCoord(seed, x1, y0, z0, xd1, yd0, zd0), xs);
            var xf10 = Math.Lerp(Constants.GradCoord(seed, x0, y1, z0, xd0, yd1, zd0), Constants.GradCoord(seed, x1, y1, z0, xd1, yd1, zd0), xs);
            var xf01 = Math.Lerp(Constants.GradCoord(seed, x0, y0, z1, xd0, yd0, zd1), Constants.GradCoord(seed, x1, y0, z1, xd1, yd0, zd1), xs);
            var xf11 = Math.Lerp(Constants.GradCoord(seed, x0, y1, z1, xd0, yd1, zd1), Constants.GradCoord(seed, x1, y1, z1, xd1, yd1, zd1), xs);

            var yf0 = Math.Lerp(xf00, xf10, ys);
            var yf1 = Math.Lerp(xf01, xf11, ys);

            return Math.Lerp(yf0, yf1, zs) * 0.964921414852142333984375f;
        }
    }
}
