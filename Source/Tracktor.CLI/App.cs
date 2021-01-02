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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Tracktor.Colors;
using Tracktor.Converters;
using Tracktor.Enums;
using Tracktor.Noise;
using Tracktor.Noise.Cellular;
using Tracktor.Warps;

namespace Tracktor.CLI
{
    public static class App
    {
        //var cellularDistance = DistanceType.Euclidean;
        //var cellularReturn = ReturnType.Minimum;



        static readonly Dictionary<string, string> Help = new Dictionary<string, string>()
        {
            { "3d",               "A boolean flag indicating whether the 3D gneration mode\n                    is enabled. Default value is \"False\"." },
            { "amptitude",        "A floating-point value of a warp fractal amptitude.\n                    Default value is \"30.0\"." },
            { "cellulardistance", "A cellular distance type for the \"Cellular\" noise\n                    generation. Possible values are \"Euclidean\",\n                    \"EuclideanSquared\", \"Hybrid\", and \"Manhattan\".\n                    Default value is \"Euclidean\"." },
            { "cellularjitter",   "A floating-point value of a cellular jitter for the\n                    \"Cellular\" noise generation. Default value is \"1.0\"." },
            { "cellularreturn",   "A cellulart distance return type for the \"Cellular\" noise\n                    generation. Possible values are \"Decrease\", \"Increase\",\n                    \"Maximum\", \"Minimum\", \"Product\", \"Quotient\", and \"Value\".\n                    Default value is \"Minimum\"." },
            { "fractal" ,         "A fractal generation type. Possible values are \"None\",\n                    \"FractionalBrownianMotion\", \"PingPong\", and \"Ridged\".\n                    Default value is \"None\"." },
            { "frequency",        "A floating-point value of the noise generation frequency.\n                    Default value is \"0.02\"." },
            { "gain",             "A floating-point value of a fractal gain.\n                    Default value is \"0.5\"." },
            { "height",           "A positive integer value of output image height specified\n                    in pixels. Default value is \"1024\"." },
            { "inverted",         "A boolean flag indicating whether the noise image colors\n                    have to be inverted. The parameter has no effect in \"Warp\"\n                    mode. Default value is \"False\"." },
            { "lacunarity",       "A floating-point value of a fractal lacunarity.\n                    Default value is \"2.0\"." },
            { "mode",             "A mode of Tracktor execution. Mode is a required parameter.\n                    Possible values are \"Noise\", and \"Warp\".\n                    \"Noise\" mode allows the user to generate a noise image with\n                    provided parameters, while \"Warp\" mode allows to generate\n                    a warp image."},
            { "noise",            "A noise generation type. Possible values are \"Cellular\",\n                    \"Perlin\", \"Simplex\", \"SimplexSmooth\", \"Value\", and\n                    \"CubicValue\". Default value is \"Perlin\"." },
            { "octaves",          "A non-negative integer value specifiying a number of\n                    octaves for the fractal generation. Default value is \"5\"." },
            { "output",           "A path to the output image file. Possible values for the\n                    image file extension are BMP, EMF, EXIF, GIF, ICO, JPEG,\n                    PNG, TIFF, and WMF."},
            { "pingpongstrength", "A floating point value of a Ping Pong strength for the\n                    \"PingPong\" fractal. Default value is \"2.0\"." },
            { "rotation",         "A type of coordinate rotation in the 3d mode. Possible\n                    values are \"None\", \"ImproveXYPlanes\", \"ImproveXZPlanes\",\n                    and \"Simplex\". Default value is \"None\"." },
            { "seed",             "An integer value of noise generation seed value.\n                    Default value is \"1337\"." },
            { "strength",         "A floating-point value of a fractal strength.\n                    Default value is \"0\" (zero)." },
            { "warp",             "A warp type. Possible values are \"None\", \"BasicGrid\",\n                    \"Radial\", \"Simplex\", and \"SimplexReduced\".\n                    Default value is \"None\"." },
            { "warpfractal",      "A warp fractal type. Possible values are \"None\",\n                    \"Independent\", and \"Progressive\". Default value is \"None\"." },
            { "width",            "A positive integer value of output image width specified in\n                    pixels. Default value is \"1024\"." },
            { "z",                "A floating-point value for the z coordinate during the 3D\n                    generation mode. Default valoe is \"0\" (zero)." },
        };

        public static string GetHelpString(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter)) { return "No help available for <blank> parameter."; }

            if (parameter == "m") { parameter = "mode"; }

            if (Help.TryGetValue(parameter, out var result))
            {
                return result;
            }

            return string.Format("No help available for <{0}> parameter.", parameter);
        }

        public static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(string.Format("Tracktor [Version {0}]", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            Console.WriteLine("Copyright © 2021 Americus Maximus.");
            Console.WriteLine();

            if (args == default || args.Length == 0)
            {
                Console.WriteLine("Usage: Tracktor.CLI [options|parameters].");
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine(" h|help                              Display help.");
                Console.WriteLine(" h=[parameter]|help=[parameter]      Display help for a specified parameter.");
                Console.WriteLine(" m=[mode]|mode=[mode] [parameters]   Run Tracktor in a specific mode with\n                                  specified parameters.");
                Console.WriteLine(" v|version                           Display version.");

                return 0;
            }

            var parameters = args.ToArray();

            // Version takes priority over anything else
            if (parameters.Any(a => a.ToLowerInvariant() == "v") || parameters.Any(a => a.ToLowerInvariant() == "version"))
            {
                Console.WriteLine(string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

                return 0;
            }

            // Help is a second highest priority
            if (parameters.Any(a => a.ToLowerInvariant() == "h") || parameters.Any(a => a.ToLowerInvariant() == "help"))
            {
                Console.WriteLine("Help:");

                foreach (var x in Help.OrderBy(h => h.Key))
                {
                    Console.Write(x.Key.PadRight(20));
                    Console.WriteLine(x.Value);
                    Console.WriteLine();
                }

                return 0;
            }

            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("h=")) || parameters.Any(a => a.ToLowerInvariant().StartsWith("help=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("h=") || a.ToLowerInvariant().StartsWith("help=")).OrderBy(a => a).ToArray();

                Console.WriteLine("Help:");

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("h=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("help=")) { ars[x] = ars[x].Substring(5, ars[x].Length - 5); }

                    Console.Write(ars[x].PadRight(20));
                    Console.WriteLine(GetHelpString(ars[x]));
                    Console.WriteLine();
                }

                return 0;
            }

            // Execution mode
            var appMode = AppMode.Noise;

            if (!parameters.Any(a => a.ToLowerInvariant().StartsWith("m=")) && !parameters.Any(a => a.ToLowerInvariant().StartsWith("mode=")))
            {
                Console.WriteLine("Mode parameter is required. Please see help for details.");

                return -1;
            }
            else
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("m=") || a.ToLowerInvariant().StartsWith("mode=")).OrderBy(a => a).ToArray();

                if (ars.Length != 1)
                {
                    Console.WriteLine("There can be only one mode parameter.");

                    return -1;
                }

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("m=")) { ars[x] = ars[x].Substring(2, ars[x].Length - 2); }
                    if (ars[x].StartsWith("mode=")) { ars[x] = ars[x].Substring(5, ars[x].Length - 5); }
                }

                if (!Enum.TryParse<AppMode>(ars[0], true, out appMode))
                {
                    Console.WriteLine(string.Format("Unable to parse value <{0}> as a mode.", ars[0]));

                    return -1;
                }
            }

            // Output
            var output = string.Empty;
            if (parameters.Any(a => a.ToLowerInvariant().StartsWith("output=")))
            {
                var ars = parameters.Where(a => a.ToLowerInvariant().StartsWith("output=")).OrderBy(a => a).ToArray();

                if (ars.Length != 1)
                {
                    Console.WriteLine("There can be only one output parameter.");

                    return -1;
                }

                for (var x = 0; x < ars.Length; x++)
                {
                    if (ars[x].StartsWith("output=")) { ars[x] = ars[x].Substring(7, ars[x].Length - 7); }
                }

                if (string.IsNullOrWhiteSpace(ars[0]))
                {
                    Console.WriteLine("Output path cannot be empty.");

                    return -1;
                }

                output = NormalizeFileName(ars[0]);
            }

            // Process parameters
            var height = 1024;
            var width = 1024;
            var isInverted = false;
            var is3D = false;
            var z = 0;

            var seed = 1337;
            var frequency = 0.02f;

            var noise = NoiseType.Perlin;
            var rotation = RotationType.None;
            var fractal = FractalType.None;

            var octaves = 5;
            var lacunarity = 2.0f;
            var gain = 0.5f;
            var strength = 0.0f;
            var pingPongStrength = 2.0f;

            var amptitude = 30.0f;

            var cellularDistance = DistanceType.Euclidean;
            var cellularReturn = ReturnType.Minimum;
            var cellularJitter = 1.0f;

            var warp = WarpType.None;
            var warpFractal = WarpFractalType.None;

            foreach (var p in parameters)
            {
                if (!p.Contains("="))
                {
                    Console.WriteLine(string.Format("Unable to parse <{0}> parameter. Skipping it.", p));
                    continue;
                }

                var key = p.Substring(0, p.IndexOf("=")).ToLowerInvariant();

                if (key == "m" || key == "mode" || key == "output") { continue; }

                var pms = parameters.Where(ar => ar.ToLowerInvariant().StartsWith(key + "=")).ToArray();

                if (pms.Length != 1)
                {
                    Console.WriteLine(string.Format("There can be only one parameter <{0}>", key));

                    return -1;
                }

                var value = p.Substring(key.Length + 1, p.Length - key.Length - 1);

                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine(string.Format("Empty value for <{0}> is not allowed.", key));

                    return -1;
                }

                if (key == "width")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue > 0)
                        {
                            width = intValue;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Unable to parse value <{0}> as a positive integer number.", value));

                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number.", value));

                        return -1;
                    }
                }
                else if (key == "height")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue > 0)
                        {
                            height = intValue;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Unable to parse value <{0}> as a positive integer number.", value));

                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number.", value));

                        return -1;
                    }
                }
                else if (key == "inverted")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        isInverted = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean value.", value));

                        return -1;
                    }
                }
                else if (key == "3d")
                {
                    if (bool.TryParse(value, out var boolValue))
                    {
                        is3D = boolValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a boolean value.", value));

                        return -1;
                    }
                }
                else if (key == "z")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        z = intValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number.", value));

                        return -1;
                    }
                }
                else if (key == "seed")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        height = seed;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number.", value));

                        return -1;
                    }
                }
                else if (key == "frequency")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        frequency = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "amptitude")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        amptitude = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "octaves")
                {
                    if (int.TryParse(value, out var intValue))
                    {
                        if (intValue >= 0)
                        {
                            octaves = intValue;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("Unable to parse value <{0}> as a non-negative integer number.", value));

                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as an integer number.", value));

                        return -1;
                    }
                }
                else if (key == "lacunarity")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        lacunarity = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "gain")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        gain = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "strength")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        strength = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "pingpongstrength")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        pingPongStrength = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "noise")
                {
                    if (Enum.TryParse<NoiseType>(value, true, out var noiseValue))
                    {
                        noise = noiseValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a noise type.", value));

                        return -1;
                    }
                }
                else if (key == "rotation")
                {
                    if (Enum.TryParse<RotationType>(value, true, out var rotationValue))
                    {
                        rotation = rotationValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a rotation type.", value));

                        return -1;
                    }
                }
                else if (key == "fractal")
                {
                    if (Enum.TryParse<FractalType>(value, true, out var fractalValue))
                    {
                        fractal = fractalValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a fractal type.", value));

                        return -1;
                    }
                }
                else if (key == "distance")
                {
                    if (Enum.TryParse<DistanceType>(value, true, out var distanceValue))
                    {
                        cellularDistance = distanceValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a distance type.", value));

                        return -1;
                    }
                }
                else if (key == "return")
                {
                    if (Enum.TryParse<ReturnType>(value, true, out var returnValue))
                    {
                        cellularReturn = returnValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a return type.", value));

                        return -1;
                    }
                }
                else if (key == "jitter")
                {
                    if (float.TryParse(value, out var floatValue))
                    {
                        cellularJitter = floatValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a floating-point number.", value));

                        return -1;
                    }
                }
                else if (key == "warp")
                {
                    if (Enum.TryParse<WarpType>(value, true, out var warpValue))
                    {
                        warp = warpValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a warp type.", value));

                        return -1;
                    }
                }
                else if (key == "warpfractal")
                {
                    if (Enum.TryParse<WarpFractalType>(value, true, out var warpFractalValue))
                    {
                        warpFractal = warpFractalValue;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Unable to parse value <{0}> as a warp fractal type.", value));

                        return -1;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("Unknown parameter <{0}>. Skipping it.", key));
                }
            }

            // Execution
            switch (appMode)
            {
                case AppMode.Noise:
                    {
                        return Noise(width, height,
                            new NoiseRequest(width, height,
                                GetNoiseFractal(fractal, gain, lacunarity, octaves, strength, pingPongStrength, noise, seed, frequency, rotation, cellularDistance, cellularJitter, cellularReturn),
                                GetWarpFractal(warpFractal, gain, lacunarity, octaves, warp, seed, amptitude, frequency, rotation))
                        {
                            Is3D = is3D,
                            IsInverted = isInverted,
                            Z = z
                        });
                    }
                case AppMode.Warp:
                    {
                        return Warp(width, height,
                            new WarpRequest(width, height,
                                GetWarpFractal(warpFractal, gain, lacunarity, octaves, warp, seed, amptitude, frequency, rotation))
                        {
                            Is3D = is3D,
                            IsInverted = isInverted,
                            Z = z
                        });
                    }
            }

            return 0;
        }

        public static string NormalizeFileName(string fileName)
        {
            return string.IsNullOrWhiteSpace(Path.GetDirectoryName(fileName)) ? Path.Combine(Environment.CurrentDirectory, fileName) : fileName;
        }

        private static Image GenerateImage(int width, int height, ARGB[] colors)
        {
            var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    result.SetPixel(x, y, Color.FromArgb(colors[y * width + x]));
                }
            }

            return result;
        }

        private static IDistanceCalculator GetCellularDistance(DistanceType distanceType, float jitter, ReturnType returnType)
        {
            var converter = GetCellularDistanceConverter(returnType);

            switch (distanceType)
            {
                case DistanceType.Euclidean:
                    {
                        return new EuclideanDistanceCalculator(converter, jitter);
                    }
                case DistanceType.EuclideanSquared:
                    {
                        return new EuclideanSquaredDistanceCalculator(converter, jitter);
                    }
                case DistanceType.Hybrid:
                    {
                        return new HybridDistanceCalculator(converter, jitter);
                    }
                case DistanceType.Manhattan:
                    {
                        return new ManhattanDistanceCalculator(converter, jitter);
                    }
            }

            return new NoneDistanceCalculator(converter, jitter);
        }

        private static IDistanceConverter GetCellularDistanceConverter(ReturnType returnType)
        {
            switch (returnType)
            {
                case ReturnType.Decrease:
                    {
                        return new DecreaseDistanceConverter();
                    }
                case ReturnType.Increase:
                    {
                        return new IncreaseDistanceConverter();
                    }
                case ReturnType.Maximum:
                    {
                        return new MaximumDistanceConverter();
                    }
                case ReturnType.Minimum:
                    {
                        return new MinimumDistanceConverter();
                    }
                case ReturnType.Product:
                    {
                        return new ProductDistanceConverter();
                    }
                case ReturnType.Quotient:
                    {
                        return new QuotientDistanceConverter();
                    }
                case ReturnType.Value:
                    {
                        return new ValueDistanceConverter();
                    }
            }

            return new NoneDistanceConverter();
        }

        private static IConverter GetConverter(RotationType type)
        {
            switch (type)
            {
                case RotationType.ImproveXYPlanes:
                    {
                        return new XYConverter();
                    }
                case RotationType.ImproveXZPlanes:
                    {
                        return new XZConverter();
                    }
                case RotationType.Simplex:
                    {
                        return new SimplexConverter();
                    }
            }

            return new NoneConverter();
        }

        private static INoise GetNoise(NoiseType noiseType, int seed, float frequency, RotationType rotationType, DistanceType distanceType, float jitter, ReturnType returnType)
        {
            var converter = GetConverter(rotationType);

            switch (noiseType)
            {
                case NoiseType.Cellular:
                    {
                        return new CellularNoise(GetCellularDistance(distanceType, jitter, returnType), converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case NoiseType.CubicValue:
                    {
                        return new CubicValueNoise(converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case NoiseType.Perlin:
                    {
                        return new PerlinNoise(converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case NoiseType.Simplex:
                    {
                        return new SimplexNoise(converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case NoiseType.SimplexSmooth:
                    {
                        return new SimplexSmoothNoise(converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case NoiseType.Value:
                    {
                        return new ValueNoise(converter)
                        {
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
            }

            return new NoneNoise(converter)
            {
                Frequency = frequency,
                Seed = seed
            };
        }

        private static INoiseFractal GetNoiseFractal(FractalType fractalType, float gain, float lacunarity, int octaves, float strength, float pingPongStrength, NoiseType noiseType, int seed, float frequency, RotationType rotationType, DistanceType distanceType, float jitter, ReturnType returnType)
        {
            var noise = GetNoise(noiseType, seed, frequency, rotationType, distanceType, jitter, returnType);

            switch (fractalType)
            {
                case FractalType.FractionalBrownianMotion:
                    {
                        return new FractionalBrownianMotionFractal(noise)
                        {
                            Gain = gain,
                            Lacunarity = lacunarity,
                            Octaves = octaves,
                            Strength = strength
                        };
                    }
                case FractalType.PingPong:
                    {
                        return new PingPongNoiseFractal(noise)
                        {
                            Gain = gain,
                            Lacunarity = lacunarity,
                            Octaves = octaves,
                            Strength = strength,

                            PingPongStength = pingPongStrength
                        };
                    }
                case FractalType.Ridged:
                    {
                        return new RidgedNoiseFractal(noise)
                        {
                            Gain = gain,
                            Lacunarity = lacunarity,
                            Octaves = octaves,
                            Strength = strength
                        };
                    }
            }

            return new NoneNoiseFractal(noise)
            {
                Gain = gain,
                Lacunarity = lacunarity,
                Octaves = octaves,
                Strength = strength
            };
        }

        private static IWarp GetWarp(WarpType warpType, int seed, float amptitude, float frequency, RotationType rotationType)
        {
            var converter = GetConverter(rotationType);

            switch (warpType)
            {
                case WarpType.BasicGrid:
                    {
                        return new BasicGridWarp(converter)
                        {
                            Amptitude = amptitude,
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case WarpType.Radial:
                    {
                        return new RadialWarp(converter)
                        {
                            Amptitude = amptitude,
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case WarpType.Simplex:
                    {
                        return new SimplexWarp(converter)
                        {
                            Amptitude = amptitude,
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
                case WarpType.SimplexReduced:
                    {
                        return new SimplexReducedWarp(converter)
                        {
                            Amptitude = amptitude,
                            Frequency = frequency,
                            Seed = seed
                        };
                    }
            }

            return new NoneWarp(converter)
            {
                Amptitude = amptitude,
                Frequency = frequency,
                Seed = seed
            };
        }

        private static IWarpFractal GetWarpFractal(WarpFractalType warpFractalType, float gain, float lacunarity, int octaves, WarpType warpType, int seed, float amptitude, float frequency, RotationType rotationType)
        {
            var warp = GetWarp(warpType, seed, amptitude, frequency, rotationType);

            switch (warpFractalType)
            {
                case WarpFractalType.Independent:
                    {
                        return new IndependentWarpFractal(warp)
                        {
                            Gain = gain,
                            Lacunarity = lacunarity,
                            Octaves = octaves
                        };
                    }
                case WarpFractalType.Progressive:
                    {
                        return new ProgressiveWarpFractal(warp)
                        {
                            Gain = gain,
                            Lacunarity = lacunarity,
                            Octaves = octaves
                        };
                    }
            }

            return new NoneWarpFractal(warp)
            {
                Gain = gain,
                Lacunarity = lacunarity,
                Octaves = octaves
            };
        }

        private static int Noise(int width, int height, NoiseRequest noiseRequest)
        {
            try
            {
                GenerateImage(width, height, new Tracktor().Noise(noiseRequest).Image);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return -1;
            }

            return 0;
        }

        private static int Warp(int width, int height, WarpRequest warpRequest)
        {
            try
            {
                GenerateImage(width, height, new Tracktor().Warp(warpRequest).Image);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return -1;
            }

            return 0;
        }
    }
}
