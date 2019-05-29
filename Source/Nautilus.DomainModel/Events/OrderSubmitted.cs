﻿//--------------------------------------------------------------------------------------------------
// <copyright file="OrderSubmitted.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents an event where an order had been submitted to the broker.
    /// </summary>
    [Immutable]
    public sealed class OrderSubmitted : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSubmitted"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="submittedTime">The event submitted time.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderSubmitted(
            OrderId orderId,
            ZonedDateTime submittedTime,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderSubmitted),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(submittedTime, nameof(submittedTime));
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.SubmittedTime = submittedTime;
        }

        /// <summary>
        /// Gets the events order submitted time.
        /// </summary>
        public ZonedDateTime SubmittedTime { get; }
    }
}
