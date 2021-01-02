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
    public abstract class AbstractWarpFractal : IWarpFractal
    {
        public AbstractWarpFractal(IWarp warp)
        {
            Warp = warp ?? throw new ArgumentNullException(nameof(warp));
        }

        public virtual float Bounding
        {
            get
            {
                var again = Math.Abs(Gain);
                var amp = Gain;
                var ampFractal = 1.0f;

                for (var i = 1; i < Octaves; i++)
                {
                    ampFractal += amp;
                    amp *= again;
                }

                return 1.0f / ampFractal;
            }
        }

        public virtual float Gain { get; set; }

        public virtual float Lacunarity { get; set; }

        public virtual float Octaves { get; set; }

        protected virtual IWarp Warp { get; set; }

        public virtual XY Get(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return GetValue(xy);
        }

        public virtual XYZ Get(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return GetValue(xyz);
        }

        protected abstract XY GetValue(XY xy);

        protected abstract XYZ GetValue(XYZ xyz);
    }
}
