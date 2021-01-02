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

namespace Tracktor
{
    public static class Math
    {
        public const int HashValue = 668265261;

        public static float PI { get { return (float)System.Math.PI; } }

        public static float Abs(float value) { return value < 0 ? -value : value; }

        public static float Atan2(float y, float x) { return (float)System.Math.Atan2(y, x); }

        public static int Ceiling(float value) { return (int)System.Math.Ceiling(value); }

        public static int Ceiling(double value) { return (int)System.Math.Ceiling(value); }

        public static float Clamp(float value, int min, int max) { return value < min ? min : (value > max) ? max : value; }

        public static int Floor(float value) { return value >= 0 ? (int)value : (int)value - 1; }

        public static int Hash(int seed, int xPrimed, int yPrimed)
        {
            return (seed ^ xPrimed ^ yPrimed) * HashValue;
        }

        public static int Hash(int seed, int xPrimed, int yPrimed, int zPrimed)
        {
            return (seed ^ xPrimed ^ yPrimed ^ zPrimed) * HashValue;
        }

        public static float InterpHermite(float value) { return value * value * (3 - 2 * value); }

        public static float InterpQuintic(float value) { return value * value * value * (value * (value * 6 - 15) + 10); }

        public static float Lerp(float a, float b, float t) { return a + t * (b - a); }

        public static float LerpCubic(float a, float b, float c, float d, float t)
        {
            var p = (d - c) - (a - b);

            return t * t * t * p + t * t * ((a - b) - p) + t * (c - a) + b;
        }

        public static float Max(float a, float b) { return a > b ? a : b; }

        public static int Max(int a, int b) { return a > b ? a : b; }

        public static float Min(float a, float b) { return a < b ? a : b; }

        public static int Min(int a, int b) { return a < b ? a : b; }

        public static int Round(float value) { return value >= 0 ? (int)(value + 0.5f) : (int)(value - 0.5f); }

        public static float Sqrt(float value) { return (float)System.Math.Sqrt(value); }
    }
}
