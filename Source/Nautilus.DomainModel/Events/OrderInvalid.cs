//--------------------------------------------------------------------------------------------------
// <copyright file="OrderInvalid.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
    /// Represents an event where an order had been invalidated by the system.
    /// </summary>
    [Immutable]
    public sealed class OrderInvalid : OrderEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderInvalid"/> class.
        /// </summary>
        /// <param name="orderId">The event order identifier.</param>
        /// <param name="invalidReason">The event invalid reason.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="eventTimestamp">The event timestamp.</param>
        public OrderInvalid(
            OrderId orderId,
            string invalidReason,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(
                orderId,
                typeof(OrderInvalid),
                eventId,
                eventTimestamp)
        {
            Debug.NotDefault(eventId, nameof(eventId));
            Debug.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.InvalidReason = invalidReason;
        }

        /// <summary>
        /// Gets the events message.
        /// </summary>
        public string InvalidReason { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"InvalidReason={this.InvalidReason})";
    }
}
