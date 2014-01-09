![AForge.NET Framework](http://www.aforgenet.com/img/aforgenetf.jpg)

Portable AForge.NET Framework
=============================

Copyright (c) 2006-2013 AForge.NET; Portable Class Library, WPF, Windows Store and Windows Phone adaptations (c) 2013 Anders Gustafsson, Cureos AB.

This project is a fork of Andrew Kirillow's original [AForge.NET Framework](https://code.google.com/p/aforge/) project. 
For general information and tutorials, see [here](http://www.aforgenet.com/aforge/framework/).

The repository currently provides:

* Portable Class Libraries for base and imaging functionality functionality (Core, Math, Genetic, Fuzzy, MachineLearning, Neuro, Imaging, Imaging.Formats, Vision), 
* Portable Class Libraries *AForge.System* and *AForge.System.Drawing* to substitute .NET *System* core and *System.Drawing* types not covered by PCL, and
* Target specific (type forwarding) *AForge.System* and *AForge.System.Drawing* libraries.

The portable class libraries reference the portable *AForge.System* and/or *AForge.System.Drawing* assemblies. In applications however, the target specific (Windows Store, Windows Phone or WPF)
*AForge.System* and *AForge.System.Drawing* assemblies should be referenced, to ensure that the target specific version of each type is used.
 
`WriteableBitmap`:s provide input and output to the imaging functionality in the WPF, Windows Store and Windows Store libraries. The target specific *AForge.System.Drawing* assembly 
incorporates implicit cast operators between `WriteableBitmap` and `System.Drawing.Bitmap`.

All image processing is performed on the mock `System.Drawing.Bitmap` class, `WriteableBitmap` objects should only be used as initial input to and final output from the
image processing.

When using the WPF `AForge.System.Drawing` assembly, the real `System.Drawing` assembly from .NET Framework cannot be referenced for obvious reasons. If there is a need to reference 
the real `System.Drawing` assembly, you are recommended to use the original *AForge.NET Framework* libraries and use WPF hosting controls to display image processing results instead.

Example usage
-------------

This same approach can be applied in WPF, Windows Store and Windows Phone applications.

    // Use implicit operator to convert from WriteableBitmap to Bitmap
    Bitmap bitmap = aWriteableBitmapObject;

    // Apply one or more filter functions on the Bitmap object
    var filter1 = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.RMY;
    bitmap = filter1.Apply(bitmap);
    var filter2 = new AForge.Imaging.Filters.CannyEdgeDetector();
    filter2.ApplyInPlace(bitmap);

    // Use implicit operator to convert back from Bitmap to WriteableBitmap
    aWriteableBitmapObject = bitmap;

Building the libraries
----------------------

Open the *Portable Build All.sln* solution file located in the *Sources* folder and build the entire solution or selected projects. Visual Studio 2012 Professional or higher is required.

IMPORTANT!
----------

* To be able to reference the target specific (Windows Store, WP8, .NET/WPF) *AForge.System* and *AForge.System.Drawing* assemblies, all underlying assemblies need to have a strong name, i.e. be signed. For WP8, the official WP8 *WriteableBitmapEx* assembly is *not* signed. A signed *WriteableBitmapEx* assembly for WP8 is instead available in the binary release of this project, which you can download [here](https://github.com/cureos/aforge/releases/tag/v2.2.5).
* PCL projects cannot be built in Express versions of Visual Studio, but prebuilt PCL binaries can still be referenced. Prebuilt binaries for Portable AForge can be downloaded [here](https://github.com/cureos/aforge/releases/tag/v2.2.5).
