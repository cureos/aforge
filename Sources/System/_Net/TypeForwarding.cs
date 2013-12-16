//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

// System
[assembly: TypeForwardedTo(typeof(Converter<,>))]
[assembly: TypeForwardedTo(typeof(DBNull))]
[assembly: TypeForwardedTo(typeof(ICloneable))]
[assembly: TypeForwardedTo(typeof(NonSerializedAttribute))]
[assembly: TypeForwardedTo(typeof(NotFiniteNumberException))]
[assembly: TypeForwardedTo(typeof(SerializableAttribute))]

// System.Collections.Concurrent
[assembly: TypeForwardedTo(typeof(ConcurrentStack<>))]
[assembly: TypeForwardedTo(typeof(ConcurrentDictionary<,>))]

// System.ComponentModel
[assembly: TypeForwardedTo(typeof(BrowsableAttribute))]
[assembly: TypeForwardedTo(typeof(DisplayNameAttribute))]

// System.Data
[assembly: TypeForwardedTo(typeof(DataColumn))]
[assembly: TypeForwardedTo(typeof(DataColumnCollection))]
[assembly: TypeForwardedTo(typeof(DataRow))]
[assembly: TypeForwardedTo(typeof(DataRowCollection))]
[assembly: TypeForwardedTo(typeof(DataTable))]
[assembly: TypeForwardedTo(typeof(DataView))]

// System.Diagnostics
[assembly: TypeForwardedTo(typeof(Trace))]

// System.IO
[assembly: TypeForwardedTo(typeof(File))]
[assembly: TypeForwardedTo(typeof(FileMode))]
[assembly: TypeForwardedTo(typeof(FileStream))]

// System.Runtime.InteropServices
[assembly: TypeForwardedTo(typeof(GCHandle))]
[assembly: TypeForwardedTo(typeof(GCHandleType))]
[assembly: TypeForwardedTo(typeof(GuidAttribute))]
[assembly: TypeForwardedTo(typeof(Marshal))]

// System.Runtime.Serialization
[assembly: TypeForwardedTo(typeof(OptionalFieldAttribute))]

// System.Runtime.Serialization.Formatters.Binary
[assembly: TypeForwardedTo(typeof(BinaryFormatter))]

// System.Threading.Tasks
[assembly: TypeForwardedTo(typeof(Parallel))]
[assembly: TypeForwardedTo(typeof(ParallelOptions))]
