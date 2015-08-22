# Land Use extension

This repository contains the source code for the [Land Use extension][] for the [LANDIS-II simulation model][].

[Land Use extension]: http://www.landis-ii.org/extensions/land-use-change
[LANDIS-II simulation model]: http:/www.landis-ii.org

## Users

Information about the extension, including its User Guide, is available at the extension's [web page][Land Use Extension].

## Developers

### Building the extension

To build the extension, you need:

* the extension's source code, either bycloning this repository (if you are
  working on a new version), or by downloading a source archive from the
  [Releases page][] (if you are building an existing release)
* LANDIS-II version 6 installed
* [LANDIS-II SDK][]
* C# IDE (Visual Studio on Windows, MonoDevelop on OS X and Linux)

[Releases page]: https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Change/releases
[LANDIS-II SDK]: http://sourceforge.net/p/landis-ii-archive/wiki/SoftwareDevelopmentKit/

### Configuring the IDE

You need to configure the IDE to [add the landis-ii feed at myget.org][].
This configuration enables the IDE to automatically locate and retrieve the
libraries that the extension uses and ships with.  The first time you build
the extension, those libraries will be downloaded and installed by the NuGet
package manager.

[add the landis-ii feed at myget.org]: http://sourceforge.net/p/landis-ii-archive/wiki/MyGet/

### Developing a New Version

If you are working on a new version of the extension, you will need to modify
the project's properties so the "Assembly name" has the new version.  For
example, if the latest release is version 1.2, and you're going to work on
version 1.3, then the assembly name should be:

    Landis.Extension.LandUse-1.3-DEV

The `-DEV` indicates that it's a development version that hasn't been
released yet.  This distinguishes it from the assembly from the released
version (which will not have the `-DEV`).

### Exposing the built extension

When the extension is compiled, it is staged into the LANDIS-II build
directory for testing.  The LANDIS-II model needs to be informed that this
built extension is available for loading.  To do this, you need to run this
command at the command prompt in the project's `deploy/` folder:

    landis-extensions add "Land Use.txt"

If you are working on a new version, you need to first update that text file
with the extension's info.  You need to change 3 lines in the file to reflect
the new version; for example, if you're developing version 1.3:

    Name         "Land Use 1.3 [DEV]"
    Version      "1.3 (development)"
    ...
    Assembly     Landis.Extension.LandUse-1.3-DEV

If you're just building an existing release, then there's no need to edit the
extension info file before running the `landis-extensions` above.

_(Under construction)_

* Info about packaging and releasing the extension
  * How-to guide (step-by-step instructions) for making and deploying a release
