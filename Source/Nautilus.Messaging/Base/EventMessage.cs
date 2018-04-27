﻿﻿// -------------------------------------------------------------------------------------------------
// <copyright file="EventMessage.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Base
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// The message wrapper for all <see cref="Event"/>(s) messages system.
    /// </summary>
    [Immutable]
    public sealed class EventMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="id">The event message identifier.</param>
        /// <param name="timestamp">The event message timestamp.</param>
        public EventMessage(
            Event @event,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Validate.NotNull(@event, nameof(@event));
            Validate.NotEqualTo(timestamp, nameof(timestamp), default(ZonedDateTime));

            this.Event = @event;
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public Event Event { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="EventMessage"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Event.ToString();
    }
}
