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
    public class ARGB : IEquatable<ARGB>
    {
		public ARGB(int a, int r, int g, int b) : this((byte)a, (byte)r, (byte)g, (byte)b) { }

		public ARGB(int r, int g, int b) : this((byte)r, (byte)g, (byte)b) { }

		public ARGB(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            B = b;
            G = g;
        }

        public ARGB(byte r, byte g, byte b)
        {
            R = r;
            B = b;
            G = g;
        }

        public ARGB(AHSB value)
        {
            if (value == default) { throw new ArgumentNullException(nameof(value)); }

            if (value.S <= 0)
            {
                // Gray Scale
                R = G = B = (byte)(value.B * 255);
            }
            else
            {
                // The color wheel consists of 6 sectors. Figure out which sector you're in
                var position = value.H / 60.0f;
                var number = Math.Floor(position);

                // Get the fractional part of the sector
                var fraction = position - number;

                // Calculate values for the three axes of the color
                var p = (int)(255 * value.B * (1.0f - value.S));
                var q = (int)(255 * value.B * (1.0f - (value.S * fraction)));
                var t = (int)(255 * value.B * (1.0f - (value.S * (1 - fraction))));

                // Assign the fractional colors to r, g, and b based on the sector the angle is in
                switch (number)
                {
                    case 0:
                        {
                            R = (byte)(255 * value.B);
                            G = (byte)t;
                            B = (byte)p;

                            break;
                        }
                    case 1:
                        {
                            R = (byte)q;
                            G = (byte)(255 * value.B);
                            B = (byte)p;

                            break;
                        }
                    case 2:
                        {
                            R = (byte)p;
                            G = (byte)(255 * value.B);
                            B = (byte)t;

                            break;
                        }
                    case 3:
                        {
                            R = (byte)p;
                            G = (byte)q;
                            B = (byte)(255 * value.B);

                            break;
                        }
                    case 4:
                        {
                            R = (byte)t;
                            G = (byte)p;
                            B = (byte)(255 * value.B);

                            break;
                        }
                    case 5:
                        {
                            R = (byte)(255 * value.B);
                            G = (byte)p;
                            B = (byte)q;

                            break;
                        }
                }
            }

            A = (byte)(255 * value.A);
        }

        public virtual byte A { get; set; }

        public virtual byte B { get; set; }

        public virtual byte G { get; set; }

        public virtual byte R { get; set; }


        public static implicit operator AHSB(ARGB value)
        {
            return value == default ? default : new AHSB(value);
        }

        public static implicit operator int(ARGB value)
        {
            return value.A << 24 | value.R << 16 | value.G << 8 | value.B;
        }

        public static bool operator !=(ARGB left, ARGB right)
        {
			return !(left == right);
		}

		public static bool operator ==(ARGB left, ARGB right)
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
			return other is ARGB argb && Equals(argb);
		}

		public bool Equals(ARGB other)
		{
			if (ReferenceEquals(other, default)) { return false; }
			if (ReferenceEquals(this, other)) { return true; }

			return A == other.A && R == other.R && G == other.G && B == other.B;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = -1743314642;

				result = result * -1521134295 + A.GetHashCode();
				result = result * -1521134295 + R.GetHashCode();
				result = result * -1521134295 + G.GetHashCode();
				result = result * -1521134295 + B.GetHashCode();

				return result;
			}
		}
        public override string ToString()
        {
			return string.Format("A: {0}, R: {1}, G: {2}, B: {3}", A, R, G, B);
		}
	}
}

