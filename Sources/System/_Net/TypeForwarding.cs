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
using System.Threading;
using System.Threading.Tasks;

// System
using System.Timers;
using Timer = System.Timers.Timer;

[assembly: TypeForwardedTo(typeof(ApplicationException))]
[assembly: TypeForwardedTo(typeof(Converter<,>))]
[assembly: TypeForwardedTo(typeof(DBNull))]
[assembly: TypeForwardedTo(typeof(ICloneable))]
[assembly: TypeForwardedTo(typeof(NonSerializedAttribute))]
[assembly: TypeForwardedTo(typeof(NotFiniteNumberException))]
[assembly: TypeForwardedTo(typeof(SerializableAttribute))]

// System.Collections.Concurrent
[assembly: TypeForwardedTo(typeof(ConcurrentBag<>))]
[assembly: TypeForwardedTo(typeof(ConcurrentStack<>))]
[assembly: TypeForwardedTo(typeof(ConcurrentDictionary<,>))]

// System.ComponentModel
[assembly: TypeForwardedTo(typeof(BrowsableAttribute))]
[assembly: TypeForwardedTo(typeof(CategoryAttribute))]
[assembly: TypeForwardedTo(typeof(DisplayNameAttribute))]
[assembly: TypeForwardedTo(typeof(ISynchronizeInvoke))]

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
[assembly: TypeForwardedTo(typeof(FileAccess))]
[assembly: TypeForwardedTo(typeof(FileMode))]
[assembly: TypeForwardedTo(typeof(FileShare))]
[assembly: TypeForwardedTo(typeof(FileStream))]

// System.Runtime.InteropServices
[assembly: TypeForwardedTo(typeof(GCHandle))]
[assembly: TypeForwardedTo(typeof(GCHandleType))]
[assembly: TypeForwardedTo(typeof(GuidAttribute))]
[assembly: TypeForwardedTo(typeof(Marshal))]
[assembly: TypeForwardedTo(typeof(MarshalAsAttribute))]
[assembly: TypeForwardedTo(typeof(UnmanagedType))]

// System.Runtime.Serialization
[assembly: TypeForwardedTo(typeof(ISerializable))]
[assembly: TypeForwardedTo(typeof(OptionalFieldAttribute))]
[assembly: TypeForwardedTo(typeof(SerializationInfo))]

// System.Runtime.Serialization.Formatters.Binary
[assembly: TypeForwardedTo(typeof(BinaryFormatter))]

// System.Threading
[assembly: TypeForwardedTo(typeof(Thread))]
[assembly: TypeForwardedTo(typeof(ThreadStart))]

// System.Threading.Tasks
[assembly: TypeForwardedTo(typeof(Parallel))]
[assembly: TypeForwardedTo(typeof(ParallelLoopResult))]
[assembly: TypeForwardedTo(typeof(ParallelLoopState))]
[assembly: TypeForwardedTo(typeof(ParallelOptions))]

// System.Timers
[assembly: TypeForwardedTo(typeof(ElapsedEventArgs))]
[assembly: TypeForwardedTo(typeof(ElapsedEventHandler))]
[assembly: TypeForwardedTo(typeof(Timer))]