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
using Tracktor.Colors;

namespace Tracktor
{
    public class Tracktor
    {
        public virtual Result Noise(NoiseRequest request)
        {
            if (request == default) { throw new ArgumentNullException(nameof(request)); }

            var avg = 0.0f;
            var min = float.MaxValue;
            var max = float.MinValue;

            var index = 0;

            var result = new ARGB[request.Width * request.Height];
            var noise = new float[request.Width * request.Height];

            for (var y = request.Height / -2; y < request.Height / 2; y++)
            {
                for (var x = request.Width / -2; x < request.Width / 2; x++)
                {
                    var value = request.Is3D
                                ? request.Noise.Get(request.Warp.Get(new XYZ(x, y, request.Z)))
                                : request.Noise.Get(request.Warp.Get(new XY(x, y)));

                    avg += value;
                    max = Math.Max(max, value);
                    min = Math.Min(min, value);
                    noise[index++] = value;
                }
            }

            avg /= index - 1;
            var scale = 255.0f / (max - min);

            for (var i = 0; i < noise.Length; i++)
            {
                var value = Math.Round(Math.Clamp((noise[i] - min) * scale, 0, 255));

                if (request.IsInverted)
                {
                    value = 255 - value;
                }

                result[i] = new ARGB(255, value, value, value);
            }

            return new Result()
            {
                Image = result,
                Statistics = new Statistics()
                {
                    Average = avg,
                    Minimum = min,
                    Maximum = max
                }
            };
        }

        public virtual Result Warp(WarpRequest request)
        {
            if (request == default) { throw new ArgumentNullException(nameof(request)); }

            var avg = 0.0f;
            var min = float.MaxValue;
            var max = float.MinValue;

            var index = 0;

            var result = new ARGB[request.Width * request.Height];
            var noise = new float[request.Width * request.Height * 3];

            for (var y = -request.Height / 2; y < request.Height / 2; y++)
            {
                for (var x = -request.Width / 2; x < request.Width / 2; x++)
                {
                    var xyz = request.Is3D
                                ? request.Warp.Get(new XYZ(x, y, request.Z))
                                : new XYZ(request.Warp.Get(new XY(x, y)));

                    xyz.X -= x;
                    xyz.Y -= y;
                    xyz.Z -= request.Z;

                    avg += xyz.X + xyz.Y;
                    max = Math.Max(max, Math.Max(xyz.X, xyz.Y));
                    min = Math.Min(min, Math.Min(xyz.X, xyz.Y));

                    noise[index++] = xyz.X;
                    noise[index++] = xyz.Y;

                    if (request.Is3D)
                    {
                        avg += xyz.Z;
                        max = Math.Max(max, xyz.Z);
                        min = Math.Min(min, xyz.Z);
                        noise[index++] = xyz.Z;
                    }
                }
            }

            avg = request.Is3D ? (avg / ((index - 1) * 3)) : (avg / ((index - 1) * 2));

            index = 0;
            var scale = 1.0f / (max - min);

            for (var i = 0; i < result.Length; i++)
            {
                if (request.Is3D)
                {
                    result[i] = new ARGB(255, (int)(255 * (noise[index++] - min) * scale), (int)(255 * (noise[index++] - min) * scale), (int)(255 * (noise[index++] - min) * scale));
                }
                else
                {
                    var vx = (noise[index++] - min) / (max - min) - 0.5f;
                    var vy = (noise[index++] - min) / (max - min) - 0.5f;

                    result[i] = new AHSB(Math.Atan2(vy, vx) * (180 / Math.PI) + 180, 0.9f, Math.Min(1.0f, Math.Sqrt(vx * vx + vy * vy) * 2));
                }

                if (request.IsInverted)
                {
                    var color = result[i];
                    result[i] = new ARGB(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
                }
            }

            return new Result()
            {
                Image = result,
                Statistics = new Statistics()
                {
                    Average = avg,
                    Minimum = min,
                    Maximum = max
                }
            };
        }
    }
}