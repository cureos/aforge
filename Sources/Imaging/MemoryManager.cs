// AForge Image Processing Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//

namespace AForge.Imaging
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Internal memory manager used by image processing routines.
    /// </summary>
    /// 
    /// <remarks></remarks>
    /// 
    public class MemoryManager
    {
        /// <summary>
        /// Allocate unmanaged memory.
        /// </summary>
        /// 
        /// <param name="size">Memory size to allocate.</param>
        /// 
        /// <returns>Return's pointer to the allocated memory buffer.</returns>
        /// 
        /// <exception cref="OutOfMemoryException">There is insufficient memory to satisfy the request.</exception>
        /// 
        /// <remarks>The method allocates requested amount of memory and returns pointer to it. It may avoid allocation
        /// in the case some caching scheme is uses and there is already enough allocated memory available.</remarks>
        /// 
        public static IntPtr Alloc( int size )
        {
            return Marshal.AllocHGlobal( size );
        }

        /// <summary>
        /// Free unmanaged memory.
        /// </summary>
        /// 
        /// <param name="pointer">Pointer to memory buffer to free.</param>
        /// 
        /// <remarks>This method may skip actual deallocation of memory and keep it for future <see cref="Alloc"/> requests,
        /// if some caching scheme is used.</remarks>
        /// 
        public static void Free( IntPtr pointer )
        {
            Marshal.FreeHGlobal( pointer );
        }
    }
}
