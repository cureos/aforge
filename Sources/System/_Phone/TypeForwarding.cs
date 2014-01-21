//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

// System
[assembly: TypeForwardedTo(typeof(DBNull))]

// System.IO
[assembly: TypeForwardedTo(typeof(File))]
[assembly: TypeForwardedTo(typeof(FileAccess))]
[assembly: TypeForwardedTo(typeof(FileMode))]
[assembly: TypeForwardedTo(typeof(FileShare))]

// System.Runtime.InteropServices
[assembly: TypeForwardedTo(typeof(GCHandle))]
[assembly: TypeForwardedTo(typeof(GCHandleType))]
[assembly: TypeForwardedTo(typeof(GuidAttribute))]
[assembly: TypeForwardedTo(typeof(Marshal))]
[assembly: TypeForwardedTo(typeof(MarshalAsAttribute))]
[assembly: TypeForwardedTo(typeof(UnmanagedType))]

// System.Threading
[assembly: TypeForwardedTo(typeof(Thread))]
