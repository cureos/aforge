//
// Portability Class Library
//
// Copyright © Cureos AB, 2013
// info at cureos dot com
//

using System.IO;

namespace System.Runtime.Serialization.Formatters.Binary
{
    public sealed class BinaryFormatter
    {
        #region METHODS

        public object Deserialize(Stream serializationStream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}