// -------------------------------------------------------------------------------------------------
// <copyright file="NetworkAddress.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Represents a valid network address.
    /// </summary>
    [Immutable]
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public sealed class NetworkAddress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkAddress"/> class.
        /// </summary>
        /// <param name="address">The network address.</param>
        /// <exception cref="ArgumentException">If the address is empty or white space.</exception>
        public NetworkAddress(string address)
        {
            Condition.NotEmptyOrWhiteSpace(address, nameof(address));

            this.Value = address;
        }

        /// <summary>
        /// Gets the local host 127.0.0.1 network address.
        /// </summary>
        /// <returns>The local host network address.</returns>
        public static NetworkAddress LocalHost { get; } = new NetworkAddress("127.0.0.1");

        /// <summary>
        /// Gets the network address value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="NetworkAddress"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Value;
    }
}
