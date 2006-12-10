// AForge Image Processing Library
//
// Copyright © Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	/// Windows API functions and structures
	/// </summary>
	/// 
	/// <remarks>Some Win32 API function's are used by the library to perform memory
	/// related routines,</remarks>
	/// 
	internal class Win32
	{
		/// <summary>
		/// Copy a block of memory
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer</param>
		/// <param name="src">Source pointer</param>
		/// <param name="count">Memory block's length to copy</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern IntPtr memcpy(
			IntPtr dst,
			IntPtr src,
			int count);

		/// <summary>
		/// Copy a block of memory
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer</param>
		/// <param name="src">Source pointer</param>
		/// <param name="count">Memory block's length to copy</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern unsafe byte * memcpy(
			byte * dst,
			byte * src,
			int count);

		/// <summary>
		/// Fills buffer with a specified value
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer</param>
		/// <param name="filler">Value to use fo buffer filling</param>
		/// <param name="count">Size of buffer to fill</param>
		/// 
		/// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
		/// 
		[DllImport("ntdll.dll")]
		public static extern IntPtr memset(
			IntPtr dst,
			int filler,
			int count);
	}
}
