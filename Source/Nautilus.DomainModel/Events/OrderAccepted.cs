﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderAccepted.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order had been accepted by the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderAccepted : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAccepted"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="acceptedTime">The event accepted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderAccepted(
            OrderId orderId,
            ZonedDateTime acceptedTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderAccepted),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(acceptedTime, nameof(acceptedTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.AcceptedTime = acceptedTime;
        }

        /// <summary>
        /// Gets the events order accepted time.
        /// </summary>
        public ZonedDateTime AcceptedTime { get; }
    }
}
