﻿#region License
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

namespace Tracktor.Noise
{
    public class PingPongNoiseFractal : AbstractNoiseFractal
    {
        public PingPongNoiseFractal(INoise noise) : base(noise) { }

        public virtual float PingPongStength { get; set; }

        protected override float GetValue(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var sum = 0.0f;
            var amp = Bounding;
            var seed = Noise.Seed;

            for (var i = 0; i < Octaves; i++)
            {
                var noise = PingPong((Noise.Get(seed++, xyz) + 1) * PingPongStength);

                sum += (noise - 0.5f) * 2 * amp;
                amp *= Math.Lerp(1.0f, noise, Strength);

                xyz *= Lacunarity;
                amp *= Gain;
            }

            return sum;
        }

        protected override float GetValue(XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var sum = 0.0f;
            var amp = Bounding;
            var seed = Noise.Seed;

            for (var i = 0; i < Octaves; i++)
            {
                var noise = PingPong((Noise.Get(seed++, xy) + 1) * PingPongStength);

                sum += (noise - 0.5f) * 2 * amp;
                amp *= Math.Lerp(1.0f, noise, Strength);

                xy *= Lacunarity;
                amp *= Gain;
            }

            return sum;
        }

        protected virtual float PingPong(float value)
        {
            value -= (int)(value * 0.5f) * 2;
            return value < 1 ? value : 2 - value;
        }
    }
}
