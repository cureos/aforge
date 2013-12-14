//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

namespace System
{
    public delegate TOutput Converter<in TInput, out TOutput>(TInput input);
}