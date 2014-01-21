//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.IO
{
    [Flags]
    public enum FileAccess
    {
        Read = 0x01,
        Write = 0x02,
        ReadWrite = Read | Write
    }
}