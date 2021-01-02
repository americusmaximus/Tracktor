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
    public abstract class AbstractNoise : INoise
    {
        public AbstractNoise(IConverter converter)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public IConverter Converter { get; protected set; }

        public virtual float Frequency { get; set; }

        public virtual int Seed { get; set; }

        public virtual float Get(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return GetValue(seed, Convert(xy));
        }

        public virtual float Get(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return GetValue(seed, Convert(xyz));
        }

        public virtual float Get(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return GetValue(Convert(xy));
        }

        public virtual float Get(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return GetValue(Convert(xyz));
        }

        protected virtual XY Convert(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return Converter.Convert(xy * Frequency);
        }

        protected virtual XYZ Convert(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return Converter.Convert(xyz * Frequency);
        }

        protected virtual float GetValue(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return GetValue(Seed, xy);
        }

        protected virtual float GetValue(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return GetValue(Seed, xyz);
        }

        protected abstract float GetValue(int seed, XY xy);

        protected abstract float GetValue(int seed, XYZ xyz);
    }
}

