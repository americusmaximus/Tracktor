# Tracktor
Tracktor is A C# height map image generation library with a wide range of options for user to use. Tracktor is a library, as well as CLI and GUI interfaces for it.

Tracktor is a library that runs on .Net Framework 4.0, 4.5, 4.7, 4.8, .Net Core 3.1, as well as .Net 5. Please see Build and Usage sections below for details.

## Example
The following examples show a few result of height map generation outputs.

![Height Map](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Cellular.png)
![Height Map](https://github.com/americusmaximus/Tracktor/blob/main/Docs/CubicValue.png)
![Height Map](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Perlin.png)

## Build
### Windows
#### Visual Studio
Open one of the solutions and build it. Please see `<TargetFrameworks>` node in the `.csproj` files to add or remove target frameworks for the build.
#### CLI
To build the solution please use following command:

> dotnet build Tracktor.CLI.sln --configuration Release

To build the solution for only one of the target frameworks please use the following command that shows an example of building for .Net 5.

> dotnet build Tracktor.CLI.sln --framework net50 -- configuration Release

To publish the code you always have to specify the target framework since `dotnet` doesn't support publishing multi-target projects.

> dotnet publish Tracktor.CLI.sln --framework net50 --configuration Release

**Note**: `dotnet` is unable to build the UI for any of the target frameworks.

### Linux
#### CLI
Please see the CLI section of building the code under Windows.

### Dependencies
#### Linux
##### LibGdiPlus
.Net on Linux depends on `libgdiplus` for image manipulation.

In case you see errors mentioning the following:

> The type initializer for 'Gdip' threw an exception.

or

> Unable to load DLL 'libgdiplus': The specified module could not be found.

you have to install libgdiplus library on your computer, which you can do by executing the following command:

> sudo apt install libgdiplus
 
## Use
### Windows
#### CLI
Tracktor CLI on Windows 7

![Tracktor CLI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.CLI.Win.7.png)

Below is the output of running a help command
>Tracktor.CLI.exe h

##### 3d
A boolean flag indicating whether the 3D generation mode is enabled. Default value is "False".

##### amptitude
A floating-point value of a warp fractal amptitude. Default value is "30.0".

##### cellulardistance
A cellular distance type for the "Cellular" noise generation. Possible values are "Euclidean", "EuclideanSquared", "Hybrid", and "Manhattan". Default value is "Euclidean".

##### cellularjitter
A floating-point value of a cellular jitter for the "Cellular" noise generation. Default value is "1.0".

##### cellularreturn
A cellular distance return type for the "Cellular" noise generation. Possible values are "Decrease", "Increase", "Maximum", "Minimum", "Product", "Quotient", and "Value". Default value is "Minimum".

##### fractal
A fractal generation type. Possible values are "None", "FractionalBrownianMotion", "PingPong", and "Ridged". Default value is "None".

##### frequency
A floating-point value of the noise generation frequency. Default value is "0.02".

##### gain
A floating-point value of a fractal gain. Default value is "0.5".

##### height
A positive integer value of output image height specified in pixels. Default value is "1024".

##### inverted
A boolean flag indicating whether the noise image colors have to be inverted. The parameter has no effect in "Warp" mode. Default value is "False".

##### lacunarity
A floating-point value of a fractal lacunarity. Default value is "2.0".

##### mode
A mode of Tracktor execution. Mode is a required parameter. Possible values are "Noise", and "Warp". "Noise" mode allows the user to generate a noise image with provided parameters, while "Warp" mode allows to generate a warp image.

##### noise
A noise generation type. Possible values are "Cellular", "Perlin", "Simplex", "SimplexSmooth", "Value", and "CubicValue". Default value is "Perlin".

##### octaves
A non-negative integer value specifying a number of octaves for the fractal generation. Default value is "5".

##### output
A path to the output image file. Possible values for the image file extension are BMP, EMF, EXIF, GIF, ICO, JPEG, PNG, TIF, and WMF.

##### pingpongstrength
A floating point value of a Ping Pong strength for the "PingPong" fractal. Default value is "2.0".

##### rotation
A type of coordinate rotation in the 3D mode. Possible values are "None", "ImproveXYPlanes", "ImproveXZPlanes", and "Simplex". Default value is "None".

##### seed
An integer value of noise generation seed value. Default value is "1337".

##### strength
A floating-point value of a fractal strength. Default value is "0" (zero).

##### warp
A warp type. Possible values are "None", "BasicGrid", "Radial", "Simplex", and "SimplexReduced". Default value is "None".

##### warpfractal
A warp fractal type. Possible values are "None", "Independent", and "Progressive". Default value is "None".

##### width
A positive integer value of output image width specified in pixels. Default value is "1024".

##### z
A floating-point value for the z coordinate during the 3D generation mode. Default value is "0" (zero).

#### UI
Tracktor UI runs on Windows exclusively. It allows for easy and dynamic preview when applicable, as well as ease of use without a need to remember CLI options.

Tracktor on Windows 7

![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E1.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E2.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E3.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E4.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E5.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E6.png)
![Tracktor UI on Windows 7](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.UI.Win.7.E7.png)

### Linux
#### CLI
Tracktor CLI on xUbuntu 20.04

![Tracktor CLI on xUbuntu 20.04](https://github.com/americusmaximus/Tracktor/blob/main/Docs/Tracktor.CLI.xUbuntu.20.04.png)

Please see detailed description and example of the calls in Windows CLI section. Please note the differences in calling the CLI.

On Linux you have to call dotnet and provide path to the Tracktor.CLI.dll as a first parameter, the Tracktor parameters must follow afterward, please see example below:

>dotnet Tracktor.CLI.dll [parameters]