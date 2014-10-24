#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides serialization extensions. 
    /// </summary>
    public static class SerializationExtensions
    {
        #region Binary formatter

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Memory stream containing serialized object.</returns>
        public static MemoryStream ToBinary<T>(this T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            obj.ToBinary(memoryStream);
            return memoryStream;
        }

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// <para>If the file exists it will be overwritten.</para>
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="fileName">The name of the file to save serialized object.</param>
        public static void ToBinary<T>(this T obj, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                obj.ToBinary(fileStream);
                fileStream.Flush();
            }
        }

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="stream">The existing stream to serialize to.</param>
        public static void ToBinary<T>(this T obj, Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, obj);
        }

        /// <summary>
        /// De-serializes the object from the specified stream.
        /// <para>When de-serializing multiple objects the position within stream must not be tampered by the user.</para>
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="stream">The stream which contains object data.</param>
        /// <returns>De-serialized object.</returns>
        public static T FromBinary<T>(this Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var obj = (T)binaryFormatter.Deserialize(stream);
            return obj;
        }

        #endregion
    }
}
