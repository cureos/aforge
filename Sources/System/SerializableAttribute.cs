//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System
{
    [AttributeUsageAttribute(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate,
        Inherited = false)]
    public sealed class SerializableAttribute : Attribute
    {
    }
}
