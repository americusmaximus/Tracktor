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

namespace Tracktor.Converters
{
    public class XZConverter : AbstractConverter
    {
        public override XYZ Convert(XYZ xyz)
        {
            if (xyz == default) { throw new ArgumentNullException(nameof(xyz)); }

            var xz = xyz.X + xyz.Z;
            var s2 = xz * -0.211324865405187f;

            var result = new XYZ();

            result.Y = xyz.Y * 0.577350269189626f;
            result.X = xyz.X + s2 - result.Y;
            result.Z = xyz.Z + s2 - result.Y;
            result.Y += xz * 0.577350269189626f;

            return result;
        }
    }
}
