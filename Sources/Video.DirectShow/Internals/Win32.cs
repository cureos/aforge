// AForge Video for Windows Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    /// <summary>
    /// Some Win32 API used internally.
    /// </summary>
    /// 
    internal class Win32
    {
        /// <summary>
        /// Supplies a pointer to an implementation of <b>IBindCtx</b> (a bind context object).
        /// This object stores information about a particular moniker-binding operation.
        /// </summary>
        /// 
        /// <param name="reserved">Reserved for future use; must be zero.</param>
        /// <param name="ppbc">Address of <b>IBindCtx*</b> pointer variable that receives the
        /// interface pointer to the new bind context object.</param>
        /// 
        /// <returns>Returns <b>S_OK</b> on success.</returns>
        /// 
        [DllImport( "ole32.dll" )]
        public static extern
        int CreateBindCtx( int reserved, out IBindCtx ppbc );

        /// <summary>
        /// Converts a string into a moniker that identifies the object named by the string.
        /// </summary>
        /// 
        /// <param name="pbc">Pointer to the IBindCtx interface on the bind context object to be used in this binding operation.</param>
        /// <param name="szUserName">Pointer to a zero-terminated wide character string containing the display name to be parsed. </param>
        /// <param name="pchEaten">Pointer to the number of characters of szUserName that were consumed.</param>
        /// <param name="ppmk">Address of <b>IMoniker*</b> pointer variable that receives the interface pointer
        /// to the moniker that was built from <b>szUserName</b>.</param>
        /// 
        /// <returns>Returns <b>S_OK</b> on success.</returns>
        /// 
        [DllImport( "ole32.dll", CharSet = CharSet.Unicode )]
        public static extern
        int MkParseDisplayName( IBindCtx pbc, string szUserName,
            ref int pchEaten, out IMoniker ppmk );

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
    }
}
