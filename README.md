![AForge.NET Framework](http://www.aforgenet.com/img/aforgenetf.jpg)

ai4unity
=============================

*AForge.NET Framework*: Copyright (c) 2006-2013 AForge.NET; licensed under Lesser General Public License (LGPL) version 3.

This project is a fork of  Anders Gustafsson, Cureo AB's [Portable AForge.NET Framework] (https://github.com/cureos/aforge) 
project, which is a fork of Andrew Kirillow's original [AForge.NET Framework](https://code.google.com/p/aforge/) project. 
For general information and tutorials, see [here](http://www.aforgenet.com/aforge/framework/).

The main goal of this project is providing an AI framework that works on [Unity game engine] (http://www.unity3d.com) 
right out of the box, supporting as many platforms as possible with a single codebase. That means that all unsafe code
will be removed from this project, as well as all code that uses an API not supported by Unity (such as System.Drawing). 

A secondary goal of the project is providing an AI framework that can be used on commercial projects without requiring 
the developer to release the source code of his project. So, to avoid confusion, code about GPL license has been removed
from the project too. If a suitable replacement is found, removed features will be included again at some point in the future.


Working features:
* AForge.Core: the SystemTools class has been removed because it included unsafe code, but the rest of the code is working correctly.
* AForge.Fuzzy
* AForge.Genetic
* AForge.MachineLearning
* AForge.Math: the Complex class doesn't implements the ISerializable interface since it isn't supported on Windows Phone.
* AForge.Neuro

Removed features:
* AForge.Controls
* AForge.Imaging
* AForge.Imaging.Formats
* AForge.Robotics.Lego
* AForge.Robotics.Surveyor
* AForge.Robotics.TeRK
* AForge.Robotics.Video
* AForge.Robotics.Video.DirectShow
* AForge.Robotics.Video.FFMPEG
* AForge.Robotics.Video.Kinect
* AForge.Robotics.Video.VFW
* AForge.Robotics.Video.Ximea
* AForge.Robotics.Vision


Building the libraries
----------------------

Open the *Portable Build All.sln* solution file located in the *Sources* folder and build the entire solution or selected projects. Visual Studio 2012 Professional or higher is required.


Notes on commercial use
-----------------------

Please also note that *AForge.NET Framework* itself is licensed under LGPL version 3, and the copyright holder states the following on the *AForge.NET Framework* web site:

> Regarding collaboration, contribution, offers, partnering, custom work/consulting, none GPL/LGPL licensing, etc., please, contact using the next e-mail:
aforge.net [at] gmail {dot} com
