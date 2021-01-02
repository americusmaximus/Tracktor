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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Tracktor.Colors;
using Tracktor.Converters;
using Tracktor.Noise;
using Tracktor.Noise.Cellular;
using Tracktor.UI.Controls;
using Tracktor.Enums;
using Tracktor.UI.Properties;
using Tracktor.Warps;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Tracktor.UI.Windows
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            NoiseZoomValue = 100;
            WarpZoomValue = 100;
        }

        protected virtual bool IsAutomaticChange { get; set; }

        protected virtual double NoiseZoomValue { get; set; }

        protected virtual double WarpZoomValue { get; set; }

        protected virtual void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            new AboutWindow().ShowDialog(this);
        }

        protected virtual void ApplyChanges(object sender, EventArgs e)
        {
            // 3D controls
            var is3D = Is3DCheckBox.Checked;
            ZNumericUpDown.Enabled = is3D;

            // Fractal options
            PingPongStrengthNumericUpDown.Enabled = (FractalTypeComboBox.SelectedItem as FractalTypeComboBoxItem).Type == FractalType.PingPong;

            // Cellular
            var cellular = (NoiseTypeComboBox.SelectedItem as NoiseTypeComboBoxItem).Type == NoiseType.Cellular;
            CellularDistanceFunctionTypeComboBox.Enabled = cellular;
            CellularDistanceReturnTypeComboBox.Enabled = cellular;
            CellularJitterNumericUpDown.Enabled = cellular;

            // Labels
            ExtraLabel.Text = is3D
                ? "Visualisation of domain warp:\r\nRed = X offset, Green = Y offset, Blue = Z offset"
                : "Visualisation of domain warp:\r\nHue = Angle, Brightness = Magnitude";

            Generate();

            NoiseZoomValueChanged(sender, e);
            WarpZoomValueChanged(sender, e);
        }

        protected virtual void CellularDistanceFunctionComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void CellularJitterNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void CellularReturnTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual ExecutionResult Execute(NoiseRequest noiseRequest, WarpRequest warpRequest)
        {
            var tracktor = new Tracktor();

            return new ExecutionResult()
            {
                Noise = tracktor.Noise(noiseRequest),
                Wrap = tracktor.Warp(warpRequest)
            };
        }

        protected virtual void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected virtual void FractalComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void FrequencyNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void GainNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void Generate()
        {
            var height = (int)SizeYNumericUpDown.Value;
            var width = (int)SizeXNumericUpDown.Value;

            var noiseRequest = new NoiseRequest(width, height, GetNoiseFractal(), GetWarpFractal())
            {
                Is3D = Is3DCheckBox.Checked,
                IsInverted = IsInvertedCheckBox.Checked,
                Z = (float)ZNumericUpDown.Value
            };

            var warpRequest = new WarpRequest(width, height, GetWarpFractal())
            {
                Is3D = Is3DCheckBox.Checked,
                IsInverted = IsInvertedCheckBox.Checked,
                Z = (float)ZNumericUpDown.Value
            };

            using (var window = new WorkerWindow() { Action = () => { return Execute(noiseRequest, warpRequest); } })
            {
                window.ShowDialog(this);

                var result = window.Result;

                if (result == default) { return; }

                if (NoisePictureBox.Image != default)
                {
                    var img = NoisePictureBox.Image;
                    NoisePictureBox.Image = default;
                    img.Dispose();
                }

                if (WarpPictureBox.Image != default)
                {
                    var img = WarpPictureBox.Image;
                    WarpPictureBox.Image = default;
                    img.Dispose();
                }

                NoisePictureBox.Image = GenerateImage(width, height, result.Noise.Image);
                WarpPictureBox.Image = GenerateImage(width, height, result.Wrap.Image);

                MeanLabel.Text = "Mean: " + result.Noise.Statistics.Average.ToString();
                MinLabel.Text = "Min: " + result.Noise.Statistics.Minimum.ToString();
                MaxLabel.Text = "Max: " + result.Noise.Statistics.Maximum.ToString();
            }
        }

        protected virtual Image GenerateImage(int width, int height, ARGB[] colors)
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

        protected virtual IDistanceCalculator GetCellularDistance()
        {
            var jitter = (float)CellularJitterNumericUpDown.Value;
            var converter = GetCellularDistanceConverter();

            var type = (CellularDistanceFunctionTypeComboBox.SelectedItem as DistanceTypeComboBoxItem).Type;

            switch (type)
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

        protected virtual IDistanceConverter GetCellularDistanceConverter()
        {
            var type = (CellularDistanceReturnTypeComboBox.SelectedItem as ReturnTypeComboBoxItem).Type;

            switch (type)
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

        protected virtual IConverter GetConverter(RotationType type)
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

        protected virtual INoise GetNoise()
        {
            var seed = (int)SeedNumericUpDown.Value;
            var frequency = (float)FrequencyNumericUpDown.Value;
            var converter = GetConverter((ThreeDRotationComboBox.SelectedItem as RotationTypeComboBoxItem).Type);

            var type = (NoiseTypeComboBox.SelectedItem as NoiseTypeComboBoxItem).Type;

            switch (type)
            {
                case NoiseType.Cellular:
                    {
                        return new CellularNoise(GetCellularDistance(), converter)
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

        protected virtual INoiseFractal GetNoiseFractal()
        {
            var gain = (float)GainNumericUpDown.Value;
            var lacunarity = (float)LacunarityNumericUpDown.Value;
            var octaves = (int)OctavesNumericUpDown.Value;
            var strength = (float)StrengthNumericUpDown.Value;

            var noise = GetNoise();

            var type = (FractalTypeComboBox.SelectedItem as FractalTypeComboBoxItem).Type;

            switch (type)
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

                            PingPongStength = (float)PingPongStrengthNumericUpDown.Value
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

        protected virtual IWarp GetWarp()
        {
            var seed = (int)WarpSeedNumericUpDown.Value;
            var amptitude = (float)WarpAmplitudeNumericUpDown.Value;
            var frequency = (float)WarpFrequencyNumericUpDown.Value;
            var converter = GetConverter((WarpThreeDRotationComboBox.SelectedItem as RotationTypeComboBoxItem).Type);

            var type = (WarpTypeComboBox.SelectedItem as WarpTypeComboBoxItem).Type;

            switch (type)
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

        protected virtual IWarpFractal GetWarpFractal()
        {
            var gain = (float)WarpFractalGainNumericUpDown.Value;
            var lacunarity = (float)WarpFractalLacunarityNumericUpDown.Value;
            var octaves = (int)WarpFractalOctavesNumericUpDown.Value;

            var warp = GetWarp();

            var type = (WarpFractalTypeComboBox.SelectedItem as WarpFractalTypeComboBoxItem).Type;

            switch (type)
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

        protected virtual void Is3DCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void IsInvertedCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void LacunarityNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void MainWindowLoad(object sender, EventArgs e)
        {
            Icon = Resources.Icon;

            IsAutomaticChange = true;

            NoiseTypeComboBox.Items.AddRange(new[]
            {
                new NoiseTypeComboBoxItem() { Type = NoiseType.Perlin, Name = "Perlin" },

                new NoiseTypeComboBoxItem() { Type = NoiseType.Simplex, Name = "Simplex" },
                new NoiseTypeComboBoxItem() { Type = NoiseType.SimplexSmooth, Name = "Smooth Simplex" },

                new NoiseTypeComboBoxItem() { Type = NoiseType.Cellular, Name = "Cellular" },

                new NoiseTypeComboBoxItem() { Type = NoiseType.CubicValue, Name = "Cubic Value" },
                new NoiseTypeComboBoxItem() { Type = NoiseType.Value, Name = "Value" }
            });

            NoiseTypeComboBox.SelectedIndex = 1;

            ThreeDRotationComboBox.Items.AddRange(new[]
            {
                new RotationTypeComboBoxItem() { Type = RotationType.None, Name = "None" },
                new RotationTypeComboBoxItem() { Type = RotationType.ImproveXYPlanes, Name = "Improve XY Planes" },
                new RotationTypeComboBoxItem() { Type = RotationType.ImproveXZPlanes, Name = "Improve XZ Planes" },
                new RotationTypeComboBoxItem() { Type = RotationType.Simplex, Name = "Simplex" }
            });

            ThreeDRotationComboBox.SelectedIndex = 0;

            FractalTypeComboBox.Items.AddRange(new[]
            {
                new FractalTypeComboBoxItem() { Type = FractalType.None, Name = "None" },
                new FractalTypeComboBoxItem() { Type = FractalType.FractionalBrownianMotion, Name = "Fractional Brownian Motion" },
                new FractalTypeComboBoxItem() { Type = FractalType.PingPong, Name = "Ping Pong" },
                new FractalTypeComboBoxItem() { Type = FractalType.Ridged, Name = "Ridged" }

            });

            FractalTypeComboBox.SelectedIndex = 1;

            CellularDistanceFunctionTypeComboBox.Items.AddRange(new[]
            {
                new DistanceTypeComboBoxItem() { Type = DistanceType.Euclidean, Name = "Euclidean" },
                new DistanceTypeComboBoxItem() { Type = DistanceType.EuclideanSquared, Name = "Euclidean Squared" },
                new DistanceTypeComboBoxItem() { Type = DistanceType.Hybrid, Name = "Hybrid" },
                new DistanceTypeComboBoxItem() { Type = DistanceType.Manhattan, Name = "Manhattan" }
            });

            CellularDistanceFunctionTypeComboBox.SelectedIndex = 0;

            CellularDistanceReturnTypeComboBox.Items.AddRange(new[]
            {
                new ReturnTypeComboBoxItem() { Type = ReturnType.Minimum, Name = "Minimum" },
                new ReturnTypeComboBoxItem() { Type = ReturnType.Maximum, Name = "Maximum" },

                new ReturnTypeComboBoxItem() { Type = ReturnType.Increase, Name = "Increase" },
                new ReturnTypeComboBoxItem() { Type = ReturnType.Decrease, Name = "Decrease" },

                new ReturnTypeComboBoxItem() { Type = ReturnType.Product, Name = "Product" },
                new ReturnTypeComboBoxItem() { Type = ReturnType.Quotient, Name = "Quotient" },

                new ReturnTypeComboBoxItem() { Type = ReturnType.Value, Name = "Value" }
            });

            CellularDistanceReturnTypeComboBox.SelectedIndex = 0;

            WarpTypeComboBox.Items.AddRange(new[]
            {
                new WarpTypeComboBoxItem() { Type = WarpType.None, Name = "None" },
                new WarpTypeComboBoxItem() { Type = WarpType.BasicGrid, Name = "Basic Grid" },
                new WarpTypeComboBoxItem() { Type = WarpType.Radial, Name = "Radial" },
                new WarpTypeComboBoxItem() { Type = WarpType.Simplex, Name = "Simplex" },
                new WarpTypeComboBoxItem() { Type = WarpType.SimplexReduced, Name = "Simplex Reduced" }
            });

            WarpTypeComboBox.SelectedIndex = 0;

            WarpThreeDRotationComboBox.Items.AddRange(new[]
            {
                new RotationTypeComboBoxItem() { Type = RotationType.None, Name = "None" },
                new RotationTypeComboBoxItem() { Type = RotationType.ImproveXYPlanes, Name = "Improve XY Planes" },
                new RotationTypeComboBoxItem() { Type = RotationType.ImproveXZPlanes, Name = "Improve XZ Planes" },
                new RotationTypeComboBoxItem() { Type = RotationType.Simplex, Name = "Simplex" }
            });

            WarpThreeDRotationComboBox.SelectedIndex = 0;

            WarpFractalTypeComboBox.Items.AddRange(new[]
            {
                new WarpFractalTypeComboBoxItem() { Type = WarpFractalType.None, Name = "None" },
                new WarpFractalTypeComboBoxItem() { Type = WarpFractalType.Independent, Name = "Independent" },
                new WarpFractalTypeComboBoxItem() { Type = WarpFractalType.Progressive, Name = "Progressive" }

            });

            WarpFractalTypeComboBox.SelectedIndex = 0;

            IsAutomaticChange = false;

            ApplyChanges(sender, e);
        }

        protected virtual void NoisePanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            NoisePictureBox.Left = -e.NewValue;
        }

        protected virtual void NoisePanelResize(object sender, EventArgs e)
        {
            if (NoisePictureBox.Image == default) { return; }

            var point = new Point(
                Math.Max(0, (NoisePanel.Width - NoisePictureBox.Width) / 2),
                Math.Max(0, (NoisePanel.Height - NoisePictureBox.Height) / 2));

            var extraWidth = NoisePictureBox.Width - NoisePanel.Width;
            var extraHeight = NoisePictureBox.Height - NoisePanel.Height;

            if (extraWidth > 0)
            {
                NoisePanelHorizontalScrollBar.Maximum = NoisePictureBox.Width - NoisePanel.Width;
            }

            NoisePanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                NoisePanelVerticalScrollBar.Maximum = NoisePictureBox.Height - NoisePanel.Height;
            }

            NoisePanelVerticalScrollBar.Visible = extraHeight > 0;

            NoisePictureBox.Location = new Point(point.X - (NoisePanelHorizontalScrollBar.Visible ? NoisePanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (NoisePanelVerticalScrollBar.Visible ? NoisePanelVerticalScrollBar.Value : 0));
        }

        protected virtual void NoisePanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            NoisePictureBox.Top = -e.NewValue;
        }

        protected virtual void NoiseSaveToolStripButtonClick(object sender, EventArgs e)
        {
            if (NoisePictureBox.Image == default) { return; }

            SaveImage(NoisePictureBox.Image);
        }

        protected virtual void NoiseTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void NoiseZoom100ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 100;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom100ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = true;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom150ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 150;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom150ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = true;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom200ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 200;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom200ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = true;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom300ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 300;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom300ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = true;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom400ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 400;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom400ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = true;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom500ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 500;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom500ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = false;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = true;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoom50ToolStripMenuItemClick(object sender, EventArgs e)
        {
            NoiseZoomValue = 50;
            NoiseZoomToolStripDropDownButton.Text = NoiseZoom50ToolStripMenuItem.Text;

            NoiseZoom50ToolStripMenuItem.Checked = true;
            NoiseZoom100ToolStripMenuItem.Checked = false;
            NoiseZoom150ToolStripMenuItem.Checked = false;
            NoiseZoom200ToolStripMenuItem.Checked = false;
            NoiseZoom300ToolStripMenuItem.Checked = false;
            NoiseZoom400ToolStripMenuItem.Checked = false;
            NoiseZoom500ToolStripMenuItem.Checked = false;

            NoiseZoomValueChanged(sender, e);
        }

        protected virtual void NoiseZoomValueChanged(object sender, EventArgs e)
        {
            NoisePictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (NoisePictureBox.Image != default)
            {
                NoisePictureBox.Width = Math.Ceiling(NoisePictureBox.Image.Width * NoiseZoomValue / 100);
                NoisePictureBox.Height = Math.Ceiling(NoisePictureBox.Image.Height * NoiseZoomValue / 100);

                NoisePanelResize(sender, e);
            }
        }

        protected virtual void OctavesNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void PingPongStrengthNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void ReportAnIssueToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Tracktor/issues") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }

        protected virtual void SaveImage(Image image)
        {
            if (image == default) { return; }

            MainSaveFileDialog.FileName = string.Empty;

            if (MainSaveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var extension = Path.GetExtension(MainSaveFileDialog.FileName).ToLowerInvariant()
                                        .Replace(".", string.Empty).Replace("ico", "icon").Replace("jpg", "jpeg").Replace("tif", "tiff"); ;

                var imageFormatProperty = typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                                                                .FirstOrDefault(p => p.Name.ToLowerInvariant() == extension);

                if (imageFormatProperty == default)
                {
                    MessageBox.Show(this, string.Format("Unable to save the image in {0} format. Save as BMP.", extension.ToUpperInvariant()), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var imageFormat = imageFormatProperty != default ? (ImageFormat)imageFormatProperty.GetValue(default, default) : ImageFormat.Bmp;

                try
                {
                    image.Save(MainSaveFileDialog.FileName, imageFormat);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        protected virtual void SaveNoiseAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (NoisePictureBox.Image == default) { return; }

            SaveImage(NoisePictureBox.Image);
        }

        protected virtual void SaveWrapAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (WarpPictureBox.Image == default) { return; }

            SaveImage(WarpPictureBox.Image);
        }

        protected virtual void SeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void SizeXNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void SizeYNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void StrengthNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void ThreeDRotationComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void VisitWebsiteToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/AmericusMaximus/Tracktor") { UseShellExecute = true, Verb = "open" });
            }
            catch (Exception) { }
        }

        protected virtual void WarpAmplitudeNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpFractalGainNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpFractalLacunarityNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpFractalOctavesNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpFractalTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpFrequencyNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }
        protected virtual void WarpPanelHorizontalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            WarpPictureBox.Left = -e.NewValue;
        }

        protected virtual void WarpPanelResize(object sender, EventArgs e)
        {
            if (WarpPictureBox.Image == default) { return; }

            var point = new Point(
                Math.Max(0, (WarpPanel.Width - WarpPictureBox.Width) / 2),
                Math.Max(0, (WarpPanel.Height - WarpPictureBox.Height) / 2));

            var extraWidth = WarpPictureBox.Width - WarpPanel.Width;
            var extraHeight = WarpPictureBox.Height - WarpPanel.Height;

            if (extraWidth > 0)
            {
                WarpPanelHorizontalScrollBar.Maximum = WarpPictureBox.Width - WarpPanel.Width;
            }

            WarpPanelHorizontalScrollBar.Visible = extraWidth > 0;

            if (extraHeight > 0)
            {
                WarpPanelVerticalScrollBar.Maximum = WarpPictureBox.Height - WarpPanel.Height;
            }

            WarpPanelVerticalScrollBar.Visible = extraHeight > 0;

            WarpPictureBox.Location = new Point(point.X - (WarpPanelHorizontalScrollBar.Visible ? WarpPanelHorizontalScrollBar.Value : 0),
                                                                point.Y - (WarpPanelVerticalScrollBar.Visible ? WarpPanelVerticalScrollBar.Value : 0));
        }

        protected virtual void WarpPanelVerticalScrollBarScroll(object sender, ScrollEventArgs e)
        {
            WarpPictureBox.Top = -e.NewValue;
        }

        protected virtual void WarpSaveToolStripButtonClick(object sender, EventArgs e)
        {
            if (WarpPictureBox.Image == default) { return; }

            SaveImage(WarpPictureBox.Image);
        }

        protected virtual void WarpSeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpThreeDRotationComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }

        protected virtual void WarpTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }
        protected virtual void WarpZoom100ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 100;
            WarpZoomToolStripDropDownButton.Text = WarpZoom100ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = true;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom150ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 150;
            WarpZoomToolStripDropDownButton.Text = WarpZoom150ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = true;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom200ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 200;
            WarpZoomToolStripDropDownButton.Text = WarpZoom200ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = true;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom300ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 300;
            WarpZoomToolStripDropDownButton.Text = WarpZoom300ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = true;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom400ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 400;
            WarpZoomToolStripDropDownButton.Text = WarpZoom400ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = true;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom500ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 500;
            WarpZoomToolStripDropDownButton.Text = WarpZoom500ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = false;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = true;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoom50ToolStripMenuItemClick(object sender, EventArgs e)
        {
            WarpZoomValue = 50;
            WarpZoomToolStripDropDownButton.Text = WarpZoom50ToolStripMenuItem.Text;

            WarpZoom50ToolStripMenuItem.Checked = true;
            WarpZoom100ToolStripMenuItem.Checked = false;
            WarpZoom150ToolStripMenuItem.Checked = false;
            WarpZoom200ToolStripMenuItem.Checked = false;
            WarpZoom300ToolStripMenuItem.Checked = false;
            WarpZoom400ToolStripMenuItem.Checked = false;
            WarpZoom500ToolStripMenuItem.Checked = false;

            WarpZoomValueChanged(sender, e);
        }

        protected virtual void WarpZoomValueChanged(object sender, EventArgs e)
        {
            WarpPictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (WarpPictureBox.Image != default)
            {
                WarpPictureBox.Width = Math.Ceiling(WarpPictureBox.Image.Width * WarpZoomValue / 100);
                WarpPictureBox.Height = Math.Ceiling(WarpPictureBox.Image.Height * WarpZoomValue / 100);

                WarpPanelResize(sender, e);
            }
        }

        protected virtual void ZNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (IsAutomaticChange) { return; }

            ApplyChanges(sender, e);
        }
    }
}
