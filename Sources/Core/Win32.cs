// AForge Core Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	/// Windows API functions and structures.
	/// </summary>
	/// 
	/// <remarks>Some Win32 API function's are used by the library to perform memory
	/// related routines.</remarks>
	/// 
	public class Win32
	{
		// Avoid class instantiation
		private Win32( ) { }

		/// <summary>
		/// Memory allocation attributes.
		/// </summary>
		[Flags]
		public enum MemoryFlags
		{
			/// <summary>
			/// Allocates fixed memory
			/// </summary>
			Fixed		= 0,

			/// <summary>
			/// Initializes memory contents to zero
			/// </summary>
			ZeroInit	= 0x40
		}

		/// <summary>
		/// Allocate the specified number of bytes from the heap.
		/// </summary>
		/// 
		/// <param name="flags">Memory allocation attributes (see <see cref="MemoryFlags"/>).</param>
		/// <param name="size">Number of bytes to allocate.</param>
		/// 
		/// <returns>If the function succeeds, the return value is a handle to the newly allocated memory object.
		/// If the function fails, the return value is <b>IntPtr.Zero</b>.</returns>
		/// 
		[DllImport("kernel32.dll")]
		public static extern IntPtr LocalAlloc(
			[In, MarshalAs(UnmanagedType.I4)] MemoryFlags flags,
			int size );

		/// <summary>
		/// Free the specified local memory object and invalidate its handle.
		/// </summary>
		/// 
		/// <param name="memBlock">Handle to the local memory object.</param>
		/// 
		/// <returns>If the function succeeds, the return value is <b>IntPtr.Zero</b>.</returns>
		/// 
		[DllImport("kernel32.dll")]
		public static extern IntPtr LocalFree(
			IntPtr memBlock );

		/// <summary>
		/// Copy a block of memory.
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer.</param>
		/// <param name="src">Source pointer.</param>
		/// <param name="count">Memory block's length to copy.</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern IntPtr memcpy(
			IntPtr dst,
			IntPtr src,
			int count);

		/// <summary>
		/// Copy a block of memory.
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer.</param>
		/// <param name="src">Source pointer.</param>
		/// <param name="count">Memory block's length to copy.</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern unsafe byte * memcpy(
			byte * dst,
			byte * src,
			int count );

        /// <summary>
        /// Copy a block of memory.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="src">Source pointer.</param>
        /// <param name="count">Memory block's length to copy.</param>
        /// 
        /// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
        /// 
        [DllImport( "ntdll.dll" )]
        public static extern int memcpy(
            int dst,
            int src,
            int count );
        
        /// <summary>
		/// Fills buffer with a specified value.
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer.</param>
		/// <param name="filler">Value to use fo buffer filling.</param>
		/// <param name="count">Size of buffer to fill.</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern IntPtr memset(
			IntPtr dst,
			int filler,
			int count );

        /// <summary>
        /// Fills buffer with a specified value.
        /// </summary>
        /// 
        /// <param name="dst">Destination pointer.</param>
        /// <param name="filler">Value to use fo buffer filling.</param>
        /// <param name="count">Size of buffer to fill.</param>
        /// 
        /// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
        /// 
        [DllImport( "ntdll.dll" )]
        public static extern unsafe byte * memset(
            byte * dst,
            int filler,
            int count );
    }
}
