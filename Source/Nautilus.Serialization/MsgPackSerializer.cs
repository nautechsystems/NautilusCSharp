// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.IO;
    using MsgPack.Serialization;

    /// <summary>
    /// Provides a serializer for Message Pack specification.
    /// </summary>
    internal static class MsgPackSerializer
    {
        /// <summary>
        /// Serializes the given object to the Message Pack specification.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">The return type of T.</typeparam>
        /// <returns>The serialized object as a byte array.</returns>
        internal static byte[] Serialize<T>(T obj)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var byteStream = new MemoryStream())
            {
                serializer.Pack(byteStream, obj);
                return byteStream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the given Message Pack byte array to the specified type.
        /// </summary>
        /// <param name="bytes">The bytes to deserialize.</param>
        /// <typeparam name="T">The return type of T.</typeparam>
        /// <returns>The deserialized object of type T.</returns>
        internal static T Deserialize<T>(byte[] bytes)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var byteStream = new MemoryStream(bytes))
            {
                return serializer.Unpack(byteStream);
            }
        }
    }
}
