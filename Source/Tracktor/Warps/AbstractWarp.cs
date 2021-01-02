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
    public abstract class AbstractWarp : IWarp
    {
        public AbstractWarp(IConverter converter)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public float Amptitude { get; set; }

        public IConverter Converter { get; protected set; }

        public float Frequency { get; set; }

        public int Seed { get; set; }

        public virtual XY Get(float amptitude, float frequency, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return xy + GetValue(Seed, amptitude, frequency, Convert(xy));
        }

        public virtual XY Get(int seed, float amptitude, float frequency, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return xy + GetValue(seed, amptitude, frequency, Convert(xy));
        }

        public virtual XY Get(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return xy + GetValue(Seed, Amptitude, Frequency, Convert(xy));
        }

        public virtual XYZ Get(float amptitude, float frequency, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return xyz + GetValue(Seed, amptitude, frequency, Convert(xyz));
        }

        public virtual XYZ Get(int seed, float amptitude, float frequency, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return xyz + GetValue(seed, amptitude, frequency, Convert(xyz));
        }

        public virtual XYZ Get(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return xyz + GetValue(Seed, Amptitude, Frequency, Convert(xyz));
        }

        protected virtual XYZ Convert(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            return Converter.Convert(xyz);
        }

        protected virtual XY Convert(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return Converter.Convert(xy);
        }

        protected abstract XY GetValue(int seed, float amptitude, float frequency, XY xy);

        protected abstract XYZ GetValue(int seed, float amptitude, float frequency, XYZ xyz);
    }
}
