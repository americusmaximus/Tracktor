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

namespace Tracktor
{
    public class XY : IEquatable<XY>
    {
        public XY() { }

        public XY(float x, float y)
        {
            X = x;
            Y = y;
        }

        public XY(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            X = xy.X;
            Y = xy.Y;
        }

        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public static XY operator -(XY left, XY right)
        {
            if (left == default) { throw new ArgumentNullException(nameof(left)); }
            if (right == default) { throw new ArgumentNullException(nameof(right)); }

            return new XY(left.X - right.X, left.Y - right.Y);
        }

        public static bool operator !=(XY left, XY right)
        {
            return !(left == right);
        }

        public static XY operator * (XY xy, int value)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return value * xy;
        }

        public static XY operator *(int value, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return new XY(xy.X * value, xy.Y * value);
        }

        public static XY operator *(XY xy, float value)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return value * xy;
        }

        public static XY operator *(float value, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            return new XY(xy.X * value, xy.Y * value);
        }

        public static XY operator +(XY left, XY right)
        {
            if (left == default) { throw new ArgumentNullException(nameof(left)); }
            if (right == default) { throw new ArgumentNullException(nameof(right)); }

            return new XY(left.X + right.X, left.Y + right.Y);
        }

        public static bool operator ==(XY left, XY right)
        {
            if (ReferenceEquals(left, default))
            {
                if (ReferenceEquals(right, default)) { return true; }

                return false;
            }

            return left.Equals(right);
        }

        public override bool Equals(object other)
        {
            return other is XY xy && Equals(xy);
        }

        public bool Equals(XY other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = -1743314642;

                result = result * -1521134295 + X.GetHashCode();
                result = result * -1521134295 + Y.GetHashCode();

                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }
    }
}
