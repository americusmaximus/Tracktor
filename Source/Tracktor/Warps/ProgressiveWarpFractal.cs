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

namespace Tracktor.Warps
{
    public class ProgressiveWarpFractal : AbstractWarpFractal
    {
        public ProgressiveWarpFractal(IWarp warp) : base(warp) { }

        protected override XY GetValue(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var amp = Bounding * Warp.Amptitude;
            var freq = Warp.Frequency;
            var seed = Warp.Seed;

            var result = new XY(xy);

            for (var i = 0; i < Octaves; i++)
            {
                result = Warp.Get(seed, amp, freq, result);

                seed++;
                amp *= Gain;
                freq *= Lacunarity;
            }

            return result;
        }

        protected override XYZ GetValue(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var amp = Bounding * Warp.Amptitude;
            var freq = Warp.Frequency;
            var seed = Warp.Seed;

            var result = new XYZ(xyz);

            for (var i = 0; i < Octaves; i++)
            {
                result = Warp.Get(seed, amp, freq, result);

                seed++;
                amp *= Gain;
                freq *= Lacunarity;
            }

            return result;
        }
    }
}
