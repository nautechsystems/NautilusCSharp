// -------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a messaging endpoint.
    /// </summary>
    [Immutable]
    public class Endpoint : IEndpoint
    {
        private readonly Func<object, bool> target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="target">The target delegate for the end point.</param>
        public Endpoint(Func<object, bool> target)
        {
            this.target = target;
        }

        /// <summary>
        /// Send the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message)
        {
            this.target.Invoke(message);
        }

        /// <summary>
        /// Sends the given envelope to the endpoint.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        public void Send<T>(Envelope<T> envelope)
            where T : Message
        {
            this.target.Invoke(envelope);
        }
    }
}
