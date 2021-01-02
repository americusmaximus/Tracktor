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
    public class NoneNoise : AbstractNoise
    {
        public NoneNoise(IConverter converter) : base(converter) { }

        protected override float GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var result = Math.Abs(seed.GetHashCode() ^ xy.X.GetHashCode() ^ xy.Y.GetHashCode());

            while (result > 1)
            {
                result /= seed;
            }

            return result;
        }

        protected override float GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var result = Math.Abs(seed.GetHashCode() ^ xyz.X.GetHashCode() ^ xyz.Y.GetHashCode() ^ xyz.Z.GetHashCode());

            while (result > 1)
            {
                result /= seed;
            }

            return result;
        }
    }
}
