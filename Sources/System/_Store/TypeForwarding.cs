//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

// System.Collections.Concurrent
[assembly: TypeForwardedTo(typeof(ConcurrentBag<>))]
[assembly: TypeForwardedTo(typeof(ConcurrentStack<>))]
[assembly: TypeForwardedTo(typeof(ConcurrentDictionary<,>))]

// System.Runtime.InteropServices
[assembly: TypeForwardedTo(typeof(GCHandle))]
[assembly: TypeForwardedTo(typeof(GCHandleType))]
[assembly: TypeForwardedTo(typeof(GuidAttribute))]
[assembly: TypeForwardedTo(typeof(Marshal))]
[assembly: TypeForwardedTo(typeof(MarshalAsAttribute))]
[assembly: TypeForwardedTo(typeof(UnmanagedType))]

// System.Threading.Tasks
[assembly: TypeForwardedTo(typeof(Parallel))]
[assembly: TypeForwardedTo(typeof(ParallelLoopResult))]
[assembly: TypeForwardedTo(typeof(ParallelLoopState))]
[assembly: TypeForwardedTo(typeof(ParallelOptions))]
