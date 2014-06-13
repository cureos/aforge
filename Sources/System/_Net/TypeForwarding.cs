/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
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
[assembly: TypeForwardedTo(typeof(DescriptionAttribute))]
[assembly: TypeForwardedTo(typeof(DisplayNameAttribute))]
[assembly: TypeForwardedTo(typeof(ISynchronizeInvoke))]

// System.ComponentModel.DataAnnotations
[assembly: TypeForwardedTo(typeof(RangeAttribute))]

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

// System.Net
[assembly: TypeForwardedTo(typeof(IWebProxy))]

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
[assembly: TypeForwardedTo(typeof(ThreadAbortException))]
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