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

namespace Tracktor.Noise.Cellular
{
    public class NoneDistanceCalculator : AbstractDistanceCalculator
    {
        public NoneDistanceCalculator(IDistanceConverter converter, float jitter) : base(converter, jitter) { }

        protected override IDistance GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var xr = Math.Round(Math.Abs(xy.X)) * Constants.PrimeX;
            var yr = Math.Round(Math.Abs(xy.Y)) * Constants.PrimeY;

            var hash = Math.Hash(seed, xr, yr);

            var xf = (float)xr;
            var yf = (float)yr;

            while (xf > 1)
            {
                xf /= 10.0f;
            }

            while (yf > 1)
            {
                yf /= 10.0f;
            }

            return new Distance()
            {
                Minimum = (xf + yf) / xf,
                Maximum = (xf + yf) / yf,
                Hash = hash
            };
        }

        protected override IDistance GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var xr = Math.Round(Math.Abs(xyz.X)) * Constants.PrimeX;
            var yr = Math.Round(Math.Abs(xyz.Y)) * Constants.PrimeY;
            var zr = Math.Round(Math.Abs(xyz.Z)) * Constants.PrimeZ;

            var hash = Math.Hash(seed, xr, yr, zr);

            var xf = (float)xr;
            var yf = (float)yr;
            var zf = (float)zr;

            while (xf > 1)
            {
                xf /= 10.0f;
            }

            while (yf > 1)
            {
                yf /= 10.0f;
            }

            while (zf > 1)
            {
                zf /= 10.0f;
            }

            return new Distance()
            {
                Minimum = (xf + yf) / (xf + zf),
                Maximum = (xf + yf) / (yf + zf),
                Hash = hash
            };
        }
    }
}
