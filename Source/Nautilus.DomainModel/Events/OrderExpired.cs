﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderExpired.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents an event where an order had expired at the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderExpired : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpired"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="expiredTime">The event order expired time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderExpired(
            OrderId orderId,
            ZonedDateTime expiredTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderExpired),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(expiredTime, nameof(expiredTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.ExpiredTime = expiredTime;
        }

        /// <summary>
        /// Gets the events order expired time.
        /// </summary>
        public ZonedDateTime ExpiredTime { get; }
    }
}
