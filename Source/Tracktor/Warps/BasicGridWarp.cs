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
    public class BasicGridWarp : AbstractWarp
    {
        protected const float G2 = (3 - SQRT3) / 6;

        protected const float SQRT3 = 1.7320508075688772935274463415059f;

        public BasicGridWarp(IConverter converter) : base(converter) { }

        protected override XY GetValue(int seed, float amptitude, float frequency, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            xy *= frequency;

            var x0 = Math.Floor(xy.X);
            var y0 = Math.Floor(xy.Y);

            var xs = Math.InterpHermite(xy.X - x0);
            var ys = Math.InterpHermite(xy.Y - y0);

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;

            var hash0 = Math.Hash(seed, x0, y0) & (255 << 1);
            var hash1 = Math.Hash(seed, x1, y0) & (255 << 1);

            var lx0x = Math.Lerp(Constants.RandVecs2D[hash0], Constants.RandVecs2D[hash1], xs);
            var ly0x = Math.Lerp(Constants.RandVecs2D[hash0 | 1], Constants.RandVecs2D[hash1 | 1], xs);

            hash0 = Math.Hash(seed, x0, y1) & (255 << 1);
            hash1 = Math.Hash(seed, x1, y1) & (255 << 1);

            var lx1x = Math.Lerp(Constants.RandVecs2D[hash0], Constants.RandVecs2D[hash1], xs);
            var ly1x = Math.Lerp(Constants.RandVecs2D[hash0 | 1], Constants.RandVecs2D[hash1 | 1], xs);

            return new XY(Math.Lerp(lx0x, lx1x, ys) * amptitude, Math.Lerp(ly0x, ly1x, ys) * amptitude);
        }

        protected override XYZ GetValue(int seed, float amptitude, float frequency, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            xyz *= frequency;

            var x0 = Math.Floor(xyz.X);
            var y0 = Math.Floor(xyz.Y);
            var z0 = Math.Floor(xyz.Z);

            var xs = Math.InterpHermite(xyz.X - x0);
            var ys = Math.InterpHermite(xyz.Y - y0);
            var zs = Math.InterpHermite(xyz.Z - z0);

            x0 *= Constants.PrimeX;
            y0 *= Constants.PrimeY;
            z0 *= Constants.PrimeZ;

            var x1 = x0 + Constants.PrimeX;
            var y1 = y0 + Constants.PrimeY;
            var z1 = z0 + Constants.PrimeZ;

            var hash0 = Math.Hash(seed, x0, y0, z0) & (255 << 2);
            var hash1 = Math.Hash(seed, x1, y0, z0) & (255 << 2);

            var lx0x = Math.Lerp(Constants.RandVecs3D[hash0], Constants.RandVecs3D[hash1], xs);
            var ly0x = Math.Lerp(Constants.RandVecs3D[hash0 | 1], Constants.RandVecs3D[hash1 | 1], xs);
            var lz0x = Math.Lerp(Constants.RandVecs3D[hash0 | 2], Constants.RandVecs3D[hash1 | 2], xs);

            hash0 = Math.Hash(seed, x0, y1, z0) & (255 << 2);
            hash1 = Math.Hash(seed, x1, y1, z0) & (255 << 2);

            var lx1x = Math.Lerp(Constants.RandVecs3D[hash0], Constants.RandVecs3D[hash1], xs);
            var ly1x = Math.Lerp(Constants.RandVecs3D[hash0 | 1], Constants.RandVecs3D[hash1 | 1], xs);
            var lz1x = Math.Lerp(Constants.RandVecs3D[hash0 | 2], Constants.RandVecs3D[hash1 | 2], xs);

            var lx0y = Math.Lerp(lx0x, lx1x, ys);
            var ly0y = Math.Lerp(ly0x, ly1x, ys);
            var lz0y = Math.Lerp(lz0x, lz1x, ys);

            hash0 = Math.Hash(seed, x0, y0, z1) & (255 << 2);
            hash1 = Math.Hash(seed, x1, y0, z1) & (255 << 2);

            lx0x = Math.Lerp(Constants.RandVecs3D[hash0], Constants.RandVecs3D[hash1], xs);
            ly0x = Math.Lerp(Constants.RandVecs3D[hash0 | 1], Constants.RandVecs3D[hash1 | 1], xs);
            lz0x = Math.Lerp(Constants.RandVecs3D[hash0 | 2], Constants.RandVecs3D[hash1 | 2], xs);

            hash0 = Math.Hash(seed, x0, y1, z1) & (255 << 2);
            hash1 = Math.Hash(seed, x1, y1, z1) & (255 << 2);

            lx1x = Math.Lerp(Constants.RandVecs3D[hash0], Constants.RandVecs3D[hash1], xs);
            ly1x = Math.Lerp(Constants.RandVecs3D[hash0 | 1], Constants.RandVecs3D[hash1 | 1], xs);
            lz1x = Math.Lerp(Constants.RandVecs3D[hash0 | 2], Constants.RandVecs3D[hash1 | 2], xs);

            return new XYZ(Math.Lerp(lx0y, Math.Lerp(lx0x, lx1x, ys), zs) * amptitude, Math.Lerp(ly0y, Math.Lerp(ly0x, ly1x, ys), zs) * amptitude, Math.Lerp(lz0y, Math.Lerp(lz0x, lz1x, ys), zs) * amptitude);
        }
    }
}
