﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Subscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using System.Linq;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// Represents a command to subscribe to type T.
    /// </summary>
    /// <typeparam name="T">The subscription type.</typeparam>
    [Immutable]
    public sealed class Subscribe<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscribe{T}"/> class.
        /// </summary>
        /// <param name="subscription">The subscription type.</param>
        /// <param name="subscriber">The subscriber endpoint.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public Subscribe(
            T subscription,
            Mailbox subscriber,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(Subscribe<T>),
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Subscription = subscription;
            this.Subscriber = subscriber;
        }

        /// <summary>
        /// Gets the commands type to subscribe to.
        /// </summary>
        public T Subscription { get; }

        /// <summary>
        /// Gets the commands subscription name.
        /// </summary>
        public string SubscriptionName => this.Subscription is null ? string.Empty : this.Subscription.ToString().Split(".").Last();

        /// <summary>
        /// Gets the commands subscription type.
        /// </summary>
        public Type SubscriptionType => typeof(T);

        /// <summary>
        /// Gets the commands subscriber endpoint.
        /// </summary>
        public Mailbox Subscriber { get; }
    }
}
