//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}