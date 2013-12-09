![AForge.NET Framework](http://www.aforgenet.com/img/aforgenetf.jpg)

Portable AForge.NET Framework
=============================

Copyright (c) AForge.NET, 2006-2013; Portable Class Library, WPF and Windows Store adaptations (c) 2013 Anders Gustafsson, Cureos AB.

This project is a fork of Andrew Kirillow's original [AForge.NET Framework](https://code.google.com/p/aforge/) project. 
For general information and tutorials, see [here](http://www.aforgenet.com/aforge/framework/).

The repository currently provides:

* Portable Class Libraries for base functionality, and
* WPF and Windows Store libraries for imaging functionality. 

`WriteableBitmap`:s provide input and output to the imaging functionality in the WPF and Windows Store libraries. To reduce rewriting of the original *AForge.NET Framework* code, 
this repository provides a mock implementation of the `System.Drawing` assembly.  `WriteableBitmap` and `Bitmap` in the `System.Drawing` assembly are interchangeable through
implicit casting.

All image processing is performed on the mock `System.Drawing.Bitmap` class, `WriteableBitmap` objects should only be used as initial input to and final output fromthe
image processing.

When using the WPF `System.Drawing` mock assembly, the real `System.Drawing` assembly from .NET Framework cannot be referenced, for obvious reasons. If there is a need to reference 
the real `System.Drawing` assembly, you are recommended to use the original *AForge.NET Framework* libraries and use WPF hosting controls to display image processing results instead.

Example usage
-------------

The same approach can be applied in both WPF and Windows Store applications.

    // Use implicit operator to convert from WriteableBitmap to Bitmap
    Bitmap bitmap = aWriteableBitmapObject;

    // Apply one or more filter functions on the Bitmap object
    var filter1 = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.RMY;
    bitmap = filter1.Apply(bitmap);
    var filter2 = new AForge.Imaging.Filters.CannyEdgeDetector();
    filter2.ApplyInPlace(bitmap);

    // Use implicit operator to convert back from Bitmap to WriteableBitmap
    aWriteableBitmapObject = bitmap;
