//
// Shim.System
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

namespace System.Runtime.Serialization
{
    public interface ISerializable
    {
        void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}