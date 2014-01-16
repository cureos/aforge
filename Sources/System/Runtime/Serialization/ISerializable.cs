//
// Portability Class Library
//
// Copyright © Cureos AB, 2014
// info at cureos dot com
//

namespace System.Runtime.Serialization
{
    public interface ISerializable
    {
        void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}