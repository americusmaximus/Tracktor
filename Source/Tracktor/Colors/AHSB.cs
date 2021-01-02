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

namespace Tracktor.Colors
{
    public class AHSB : IEquatable<AHSB>
	{
		public const float Epsilon = 1.0f / 256.0f;

        public AHSB(float hue, float saturation, float brightness, float alpha = 1f)
        {
            H = hue;
            S = saturation;
            B = brightness;
            A = alpha;
        }

        public AHSB(ARGB color)
        {
            var max = Math.Max(color.R, Math.Max(color.G, color.B));
            var min = Math.Min(color.R, Math.Min(color.G, color.B));

            var delta = max - min;
            var h = 0.0f;
            var s = 0.0f;

            if (delta > float.Epsilon)
            {
                if (Math.Abs(max - color.R) < Epsilon && color.G >= color.B)
                {
                    h = 60 * (color.G - color.B) / delta;
                }
                else if (Math.Abs(max - color.R) < Epsilon && color.G < color.B)
                {
                    h = 60 * (color.G - color.B) / delta + 360;
                }
                else if (Math.Abs(max - color.G) < Epsilon)
                {
                    h = 60 * (color.B - color.R) / delta + 120;
                }
                else if (Math.Abs(max - color.B) < Epsilon)
                {
                    h = 60 * (color.R - color.G) / delta + 240;
                }
                else
                {
                    h = 0f;
                }

                s = (Math.Abs(max) < Epsilon) ? 0f : (1.0f - (min / max));
            }

            H = h;
            S = s;
            B = max;
            A = color.A * Epsilon;
        }

        public float A { get; set; }

        public float B { get; set; }

        public float H { get; set; }

        public float S { get; set; }


        public static implicit operator AHSB(ARGB value)
        {
            return value == default ? default : new AHSB(value);
        }

        public static implicit operator ARGB(AHSB value)
        {
            return value == default ? default : new ARGB(value);
        }

        public static bool operator !=(AHSB left, AHSB right)
        {
            return !(left == right);
        }

        public static bool operator ==(AHSB left, AHSB right)
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
            return other is AHSB ahsb && Equals(ahsb);
        }

        public bool Equals(AHSB other)
        {
            if (ReferenceEquals(other, default)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return A == other.A && H == other.H && S == other.S && B == other.B;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = -1743314642;

                result = result * -1521134295 + A.GetHashCode();
                result = result * -1521134295 + H.GetHashCode();
                result = result * -1521134295 + S.GetHashCode();
                result = result * -1521134295 + B.GetHashCode();

                return result;
            }
        }
        public override string ToString()
        {
            return string.Format("A: {0}, H: {1}, S: {2}, B: {3}", A, H, S, B);
        }
	}
}
