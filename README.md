# Land Use extension

This repository contains the source code for the [Land Use extension][] for the [LANDIS-II simulation model][].

[Land Use extension]: https://github.com/LANDIS-II-Foundation/Extension-Land-Use-Plus/blob/master/docs/index.md
[LANDIS-II simulation model]: http:/www.landis-ii.org

## Users

Information about the extension, including its User Guide, is available at the extension's [web page][Land Use Extension].

### Building the extension

Follow the instructions posted to the Developer's Group for Core v7 posted on the LANDIS-II web site.

### Exposing the built extension

When the extension is compiled, it is staged into the LANDIS-II build
directory for testing.  The LANDIS-II model needs to be informed that this
built extension is available for loading.  To do this, you need to run this
command at the command prompt in the project's `deploy/` folder:

    landis-extensions add "Land Use.txt"

If you're just building an existing release, then there's no need to edit the
extension info file before running the `landis-extensions` above.

### Deploying the extension

The extension is packaged and distributed as a Windows installer (i.e., a
setup program).  To build the installer, you'll need:

* [Inno Setup][] to actually generate the installer.  Install the QuickStart
  Pack (`ispack...`) rather than just Inno Setup itself (`isetup...`).  The
  former includes the Inno Script Studio editor which is quite handy.

[Inno Setup]: http://www.jrsoftware.org/isinfo.php

Next, test the installer by running it.  Verify that the installed release is
   available by running:

        landis-extensions list

Please verify that the installed extension can be used in a scenario.
