//--------------------------------------------------------------------------------------------------
// <copyright file="IDataSerializer{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Common.Enums;

    /// <summary>
    /// Provides a binary serializer for data objects of type T.
    /// </summary>
    /// <typeparam name="T">The serializable data type.</typeparam>
    public interface IDataSerializer<T>
    {
        /// <summary>
        /// Gets the serializers encoding.
        /// </summary>
        DataEncoding BlobEncoding { get; }

        /// <summary>
        /// Gets the serializers encoding.
        /// </summary>
        DataEncoding ObjectEncoding { get; }

        /// <summary>
        /// Returns the serialized data object bytes.
        /// </summary>
        /// <param name="dataObject">The data object to serialize.</param>
        /// <returns>The serialized data object bytes.</returns>
        byte[] Serialize(T dataObject);

        /// <summary>
        /// Returns the serialized data object two dimensional bytes array.
        /// </summary>
        /// <param name="dataObjects">The data objects to serialize.</param>
        /// <returns>The serialized data object bytes.</returns>
        byte[][] Serialize(T[] dataObjects);

        /// <summary>
        /// Return the data objects bytes array serialized to a single array.
        /// </summary>
        /// <param name="dataObjectsArray">The data objects array to serialize.</param>
        /// <param name="metadata">The metadata for the given data objects array.</param>
        /// <returns>The serialized data bytes.</returns>
        byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata);

        /// <summary>
        /// Returns the deserialize data object of type T.
        /// </summary>
        /// <param name="dataBytes">The bytes to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        T Deserialize(byte[] dataBytes);

        /// <summary>
        /// Returns the deserialize data object of type T array.
        /// </summary>
        /// <param name="dataBytesArray">The bytes array to deserialize.</param>
        /// <returns>The deserialized data object.</returns>
        T[] Deserialize(byte[][] dataBytesArray);

        /// <summary>
        /// Return the data objects bytes array deserialized to a two dimensional array.
        /// </summary>
        /// <param name="dataBytes">The data bytes to deserialize.</param>
        /// <returns>The deserialized data bytes array.</returns>
        T[] DeserializeBlob(byte[] dataBytes);
    }
}
