![AForge.NET Framework](http://www.aforgenet.com/img/aforgenetf.jpg)

Portable AForge.NET Framework
=============================

*AForge.NET Framework*: Copyright (c) 2006-2013 AForge.NET; licensed under Lesser General Public License (LGPL) version 3.<br/>
*Shim* and *Shim.Drawing* class libraries: Copyright (c) 2013-2014 Anders Gustafsson, Cureos AB; licensed under General Public License (GPL) version 3.

This project is a fork of Andrew Kirillow's original [AForge.NET Framework](https://code.google.com/p/aforge/) project. 
For general information and tutorials, see [here](http://www.aforgenet.com/aforge/framework/).

The repository currently provides:

* Portable Class Libraries for base and imaging functionality functionality (Core, Math, Genetic, Fuzzy, MachineLearning, Neuro, Imaging, Imaging.Formats, Vision), 
* Portable Class Libraries *Shim* and *Shim.Drawing* to substitute .NET *System* core and *System.Drawing* types not covered by PCL, and
* Target specific (type forwarding) *Shim* and *Shim.Drawing* libraries for .NET Framework, Windows Store and Windows Phone 8.

The portable class libraries reference the portable *Shim* and/or *Shim.Drawing* assemblies. In applications however, the target specific (Windows Store, Windows Phone or WPF)
*Shim* and *Shim.Drawing* assemblies should be referenced, to ensure that the target specific version of each type is used.
 
`WriteableBitmap`:s provide input and output to the imaging functionality in the WPF, Windows Store and Windows Store libraries. The target specific *Shim.Drawing* assembly 
incorporates explicit cast operators between `WriteableBitmap` and `System.Drawing.Bitmap`.

All image processing is performed on the mock `System.Drawing.Bitmap` class, `WriteableBitmap` objects should only be used as initial input to and final output from the
image processing.

When using the WPF *Shim.Drawing* assembly, the real *System.Drawing* assembly from .NET Framework cannot be referenced for obvious reasons. If there is a need to reference 
the real *System.Drawing* assembly, you are recommended to use the original *AForge.NET Framework* libraries and use WPF hosting controls to display image processing results instead.

**IMPORTANT!**<br />
* PCL projects cannot be built in Express versions of Visual Studio, but prebuilt PCL binaries can still be referenced. The latest prebuilt binaries for Portable AForge can be downloaded [here](https://github.com/cureos/aforge/releases/tag/v2.2.5.2).
* To be able to reference the target specific (Windows Store, WP8, .NET/WPF) *Shim* and *Shim.Drawing* assemblies, all underlying assemblies need to have a strong name, i.e. be signed. 

Example usage
-------------

This same approach can be applied in WPF, Windows Store and Windows Phone applications.

    // Use explicit operator to convert from WriteableBitmap to Bitmap
    Bitmap bitmap = (Bitmap)aWriteableBitmapObject;

    // Apply one or more filter functions on the Bitmap object
    var filter1 = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.RMY;
    bitmap = filter1.Apply(bitmap);
    var filter2 = new AForge.Imaging.Filters.CannyEdgeDetector();
    filter2.ApplyInPlace(bitmap);

    // Use explicit operator to convert back from Bitmap to WriteableBitmap
    aWriteableBitmapObject = (WriteableBitmap)bitmap;

Building the libraries
----------------------

Open the *Portable Build All.sln* solution file located in the *Sources* folder and build the entire solution or selected projects. Visual Studio 2012 Professional or higher is required.

Notes on commercial use
-----------------------

The *Shim* and *Shim.Drawing* assemblies that are required to build the Portable Class Library version of AForge.NET Framework are published under the General Public License, version 3.
For those interested in using the PCL libraries without having to adhere to GPL, please contact the copyright holder of the *Shim* assemblies at

licenses@cureos.com

for commercial licensing alternatives.

Please also note that *AForge.NET Framework* itself is licensed under LGPL version 3, and the copyright holder states the following on the *AForge.NET Framework* web site:

> Regarding collaboration, contribution, offers, partnering, custom work/consulting, none GPL/LGPL licensing, etc., please, contact using the next e-mail:
aforge.net [at] gmail {dot} com
