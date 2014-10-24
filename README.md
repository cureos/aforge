![AForge.NET Framework](http://www.aforgenet.com/img/aforgenetf.jpg)

![Accord.NET Framework](https://camo.githubusercontent.com/7ca11aa81f840d4b423f9b187b6ceb75d6b9e3d3/687474703a2f2f6163636f72642d6672616d65776f726b2e6e65742f646f63732f69636f6e732f6c6f676f2e706e67)

![Accord.NET Extensions](https://raw.githubusercontent.com/dajuric/accord-net-extensions/master/Deployment/Logo/logo-big.png)

ai4unity
=============================

*AForge.NET Framework*: Copyright (c) 2006-2013 AForge.NET; licensed under Lesser General Public License (LGPL) version 3.

This project is a fork of  Anders Gustafsson, Cureo AB's [Portable AForge.NET Framework] (https://github.com/cureos/aforge) 
project, which is a fork of Andrew Kirillow's original [AForge.NET Framework](https://code.google.com/p/aforge/) project. 
For general information and tutorials, see [here](http://www.aforgenet.com/aforge/framework/). After the initial commit,
I continued adding Cureo AB's [Portable Accord.NET Framework](https://github.com/cureos/accord) (a fork of the original
César Souzas's [Accord.NET Framework](https://github.com/accord-net/framework)) and Dajuric's 
[Accord.NET Extensions](https://github.com/dajuric/accord-net-extensions) to the project.

The main goal of this project is providing an AI framework that works on [Unity game engine] (http://www.unity3d.com) 
right out of the box, supporting as many platforms as possible with a single codebase. That means that all unsafe code
will be removed from this project, as well as all code that uses an API not supported by Unity (such as System.Drawing). 

A secondary goal of the project is providing an AI framework that can be used on commercial projects without requiring 
the developer to release the source code of his project. So, to avoid confusion, code about GPL license has been removed
from the project too. If a suitable replacement is found, removed features will be included again at some point in the future.


Working features:
* AForge.Core: 
	* The SystemTools class has been removed because it included unsafe code, but the rest of the code is working correctly.
* AForge.Fuzzy
* AForge.Genetic
* AForge.MachineLearning
* AForge.Math: 
	* The Complex class doesn't implements the ISerializable interface since it isn't supported on Windows Phone.
* AForge.Neuro
* Accord.Core
* Accord.MachineLearning
* Accord.Math
* Accord.Neuro
* Accord.Statistics
* Accord.Extensions.Core: 
	* The Platform class has been removed because it were using native functions.
	* The PinneArray class has been removed because it required the AForge.Core.SystemTools class.
	* The XmlSerialization has been removed from the SerializationExtensions because it isn't supported in some Unity platforms.
	* The PathExtensions class has been converted into an Editor Script because it was using some functions not available on Webplayer builds.
* Accord.Extensions.Math
* Accord.Extensions.Statistics


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
* Accord.Audio
* Accord.Audio.Formats
* Accord.Audition
* Accord.Controls.Audio
* Accord.Controls.Imaging
* Accord.Controls.Statistics
* Accord.Controls.Vision
* Accord.DirectSound
* Accord.Formats
* Accord.Imaging
* Accord.MachineLearning.GPL
* Accord.Math.NonCommercial
* Accord.Vision
* Accord.Extensions.Imaging
* Accord.Extensions.Visio


Building the libraries
----------------------

1) Open the Unity Editor.
2) Select the Window -> AI4Unity -> Build DLLs menu option.
3) Select the DLLs that should be compiled and the path were the DLLs will be created.



Notes on commercial use
-----------------------

Please also note that *AForge.NET Framework* itself is licensed under LGPL version 3, and the copyright holder states the following on the *AForge.NET Framework* web site:

> Regarding collaboration, contribution, offers, partnering, custom work/consulting, none GPL/LGPL licensing, etc., please, contact using the next e-mail:
aforge.net [at] gmail {dot} com
