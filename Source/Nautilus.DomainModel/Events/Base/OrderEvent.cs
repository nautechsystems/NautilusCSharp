﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all order events.
    /// </summary>
    [Immutable]
    public abstract class OrderEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderEvent"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="symbol">The event symbol.</param>
        /// <param name="eventType">The event type.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        protected OrderEvent(
            OrderId orderId,
            Symbol symbol,
            Type eventType,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                eventType,
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.OrderId = orderId;
            this.Symbol = symbol;
        }

        /// <summary>
        /// Gets the events order identifier.
        /// </summary>
        public OrderId OrderId { get; }

        /// <summary>
        /// Gets the events order symbol.
        /// </summary>
        public Symbol Symbol { get; }
    }
}
