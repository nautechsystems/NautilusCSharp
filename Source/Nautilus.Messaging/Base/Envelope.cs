﻿// -------------------------------------------------------------------------------------------------
// <copyright file="MessageEnvelope.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Base
{
    using System;
    using System.Collections.Generic;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// Provides an envelope wrapper for all messages sent via the messaging service.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    public sealed class Envelope<T> where T : Message
    {
        private readonly T message;

        public Envelope(
            IReadOnlyCollection<Enum> receivers,
            Enum sender,
            T message,
            Guid envelopeId,
            ZonedDateTime timestamp)
        {
            Debug.ReadOnlyCollectionNotNullOrEmpty(receivers, nameof(receivers));
            Debug.NotNull(sender, nameof(sender));
            Debug.NotNull(message, nameof(message));
            Debug.NotDefault(envelopeId, nameof(envelopeId));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Receivers = receivers;
            this.Sender = sender;
            this.EnvelopeId = envelopeId;
            this.Timestamp = timestamp;
            this.message = message;
        }

        /// <summary>
        /// Gets the envelope receiver(s).
        /// </summary>
        public IReadOnlyCollection<Enum> Receivers { get; }

        /// <summary>
        /// Gets the envelope sender.
        /// </summary>
        public Enum Sender { get; }

        /// <summary>
        /// Gets the envelope identifier.
        /// </summary>
        public Guid EnvelopeId { get; }

        /// <summary>
        /// Gets the envelope creation timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Gets the envelopes opened time.
        /// </summary>
        public Option<ZonedDateTime?> OpenedTime { get; private set; }

        /// <summary>
        /// Gets a valid indicating whether the envelope has been opened.
        /// </summary>
        public bool IsOpened => this.OpenedTime.HasValue;

        /// <summary>
        /// Opens the envelope and returns the contained message (records the opened time if not
        /// already opened).
        /// </summary>
        /// <param name="currentTime">The current time (cannot be default).</param>
        /// <returns>The contained message.</returns>
        /// <exception cref="ValidationException">Throws if the current time is the default value.</exception>
        public T Open(ZonedDateTime currentTime)
        {
            Validate.NotDefault(currentTime, nameof(currentTime));

            if (this.OpenedTime.HasNoValue)
            {
                this.OpenedTime = currentTime;
            }

            return this.message;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Envelope{T}"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"[{this.message}]";
    }
}
