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

namespace Tracktor.Noise.Cellular
{
    public class HybridDistanceCalculator : AbstractDistanceCalculator
    {
        public HybridDistanceCalculator(IDistanceConverter converter, float jitter) : base(converter, jitter) { }

        protected override IDistance GetValue(int seed, XY xy)
        {
            if (xy == default) { throw new ArgumentNullException(nameof(xy)); }

            var xr = Math.Round(xy.X);
            var yr = Math.Round(xy.Y);

            var distance0 = float.MaxValue;
            var distance1 = float.MaxValue;

            var closestHash = 0;

            var cellularJitter = 0.43701595f * Jitter;

            var xPrimed = (xr - 1) * Constants.PrimeX;
            var yPrimedBase = (yr - 1) * Constants.PrimeY;

            for (var xi = xr - 1; xi <= xr + 1; xi++)
            {
                var yPrimed = yPrimedBase;

                for (var yi = yr - 1; yi <= yr + 1; yi++)
                {
                    var hash = Math.Hash(seed, xPrimed, yPrimed);
                    var idx = hash & (255 << 1);

                    var vecX = (float)(xi - xy.X) + Constants.RandVecs2D[idx] * cellularJitter;
                    var vecY = (float)(yi - xy.Y) + Constants.RandVecs2D[idx | 1] * cellularJitter;

                    var newDistance = (Math.Abs(vecX) + Math.Abs(vecY)) + (vecX * vecX + vecY * vecY);

                    distance1 = Math.Max(Math.Min(distance1, newDistance), distance0);

                    if (newDistance < distance0)
                    {
                        distance0 = newDistance;
                        closestHash = hash;
                    }

                    yPrimed += Constants.PrimeY;
                }

                xPrimed += Constants.PrimeX;
            }

            return new Distance()
            {
                Minimum = distance0,
                Maximum = distance1,
                Hash = closestHash
            };
        }

        protected override IDistance GetValue(int seed, XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var xr = Math.Round(xyz.X);
            var yr = Math.Round(xyz.Y);
            var zr = Math.Round(xyz.Z);

            var distance0 = float.MaxValue;
            var distance1 = float.MaxValue;
            var closestHash = 0;

            var cellularJitter = 0.39614353f * Jitter;

            var xPrimed = (xr - 1) * Constants.PrimeX;
            var yPrimedBase = (yr - 1) * Constants.PrimeY;
            var zPrimedBase = (zr - 1) * Constants.PrimeZ;

            for (var xi = xr - 1; xi <= xr + 1; xi++)
            {
                var yPrimed = yPrimedBase;

                for (var yi = yr - 1; yi <= yr + 1; yi++)
                {
                    var zPrimed = zPrimedBase;

                    for (var zi = zr - 1; zi <= zr + 1; zi++)
                    {
                        var hash = Math.Hash(seed, xPrimed, yPrimed, zPrimed);
                        var idx = hash & (255 << 2);

                        var vecX = (float)(xi - xyz.X) + Constants.RandVecs3D[idx] * cellularJitter;
                        var vecY = (float)(yi - xyz.Y) + Constants.RandVecs3D[idx | 1] * cellularJitter;
                        var vecZ = (float)(zi - xyz.Z) + Constants.RandVecs3D[idx | 2] * cellularJitter;

                        var newDistance = (Math.Abs(vecX) + Math.Abs(vecY) + Math.Abs(vecZ)) + (vecX * vecX + vecY * vecY + vecZ * vecZ);

                        distance1 = Math.Max(Math.Min(distance1, newDistance), distance0);

                        if (newDistance < distance0)
                        {
                            distance0 = newDistance;
                            closestHash = hash;
                        }

                        zPrimed += Constants.PrimeZ;
                    }

                    yPrimed += Constants.PrimeY;
                }

                xPrimed += Constants.PrimeX;
            }

            return new Distance()
            {
                Minimum = distance0,
                Maximum = distance1,
                Hash = closestHash
            };
        }
    }
}
