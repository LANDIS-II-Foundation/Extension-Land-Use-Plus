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

### Deploying the extension

The extension is packaged and distributed as a Windows installer (i.e., a
setup program).  To build the installer, you'll need:

* software to make a PDF for the user guide.  It comes built into OS X, but
  on Windows, [CutePDF][] is one free option that's recommended.  Of course,
  if you have a license for Adobe Acrobat, that'll do too.
* [Inno Setup][] to actually generate the installer.  Install the QuickStart
  Pack (`ispack...`) rather than just Inno Setup itself (`isetup...`).  The
  former includes the Inno Script Studio editor which is quite handy.

[CutePDF]: http://www.cutepdf.com/
[Inno Setup]: http://www.jrsoftware.org/isinfo.php

#### Deployment steps:

1. Determine the status (i.e., type) of the next release.  Available statuses
   are:

  * alpha
  * beta
  * candidate (for official release)
  * official

   All release types except official can have multiple releases, which are
   numbered.  There is only one official release for each version.

  * alpha 1 release
  * alpha 2 release
  * ...
  * beta 1 release
  * beta 2 release
  * ...
  * release candidate 1
  * release candidate 2
  * ...
  * official release

2. Remove the `-DEV` from the assembly's name in the IDE.
3. Review all issues in the issue tracker assigned to this release.  Ensure
   that each issue has been addressed and closed, or has been re-assigned to a
   later release.  If the issue is being postponed to a future release,
   document the reason in the issue tracker.
4. Compile the appropriate configuration for the extension:
  * Debug configuration if alpha or beta release, or
  * Release configuration if release candidate or official release
5. Add an entry for this release in the User Guide.  In the entry, describe
   any notable changes in language understandable for users (avoid technical
   jargon only developers would know).

  > Should alpha & beta releases be in Versions section of the User Guide?
  > In other words, should all releases be documented in the User Guide?
  > Or should the User Guide contain just official releases, and all the
  > pre-releases (alpha, beta, release candidates) be listed in a change log
  > log of sorts?

6. Generate the PDF for the User Guide, overwriting the existing
   `deploy/docs/Land Use vX.Y - User Guide.pdf` file.  Commit it to the
   the repository.
7. Update the extension info file (`deploy/Land Use.txt`):
  * Remove `-DEV` notation from the `Name` and `Assembly` lines.
  * Add the release info to the `Version` line.
8. Open the `deploy/Land Use.iss` file with Inno Script Studio.  In that
   application, compile the script to make the installer in the `deploy/`
   folder.  The installer's file name will be based on the release info
   in the extension info file.
9. Test the installer by running it.  Verify that the installed release is
   available by running:

        landis-extensions list

10. Verify that the installed extension can be used in a scenario.
11. Upload the installer to [BinTray][].  This will require creating a new
    BinTray version for the release.
12. Tag the release in the repository.
13. Update the extension's [web page][Land Use Extension] for the new release.

[BinTray]: https://bintray.com/landis-ii/extensions/Land-Use/view