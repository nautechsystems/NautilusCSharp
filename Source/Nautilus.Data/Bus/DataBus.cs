// -------------------------------------------------------------------------------------------------
// <copyright file="DataBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Bus
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a data bus.
    /// </summary>
    public sealed class DataBus : Component
    {
        private readonly List<Type> dataTypes;
        private readonly List<IEndpoint> barSubscriptions;
        private readonly List<IEndpoint> instrumentSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBus"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public DataBus(IComponentryContainer container)
        : base(container)
        {
            this.dataTypes = new List<Type> { typeof(Bar), typeof(Instrument) };
            this.barSubscriptions = new List<IEndpoint>();
            this.instrumentSubscriptions = new List<IEndpoint>();

            this.RegisterHandler<ISubscribe>(this.OnMessage);
            this.RegisterHandler<IUnsubscribe>(this.OnMessage);
            this.RegisterHandler<(BarType, Bar)>(this.Publish);
            this.RegisterHandler<Instrument>(this.Publish);
        }

        private void OnMessage(ISubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (!this.dataTypes.Contains(type))
            {
                this.Log.Error($"Cannot subscribe to {type} data (only Bar or Instrument data).");
                return;
            }

            switch (type.Name)
            {
                case nameof(Bar):
                    if (this.barSubscriptions.Contains(subscriber))
                    {
                        this.Log.Warning($"{subscriber} is already subscribed to {type} data.");
                        return; // Design time error
                    }

                    this.barSubscriptions.Add(subscriber);
                    break;
                case nameof(Instrument):
                    if (this.instrumentSubscriptions.Contains(subscriber))
                    {
                        this.Log.Warning($"{subscriber} is already subscribed to {type} data.");
                        return; // Design time error
                    }

                    this.instrumentSubscriptions.Add(subscriber);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(type.Name, nameof(type.Name));
            }
        }

        private void OnMessage(IUnsubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            switch (type.Name)
            {
                case nameof(Bar):
                    if (!this.barSubscriptions.Contains(subscriber))
                    {
                        this.Log.Warning($"{subscriber} is already unsubscribed from {type} data.");
                        return; // Design time error
                    }

                    this.barSubscriptions.Remove(subscriber);
                    break;
                case nameof(Instrument):
                    if (!this.instrumentSubscriptions.Contains(subscriber))
                    {
                        this.Log.Warning($"{subscriber} is already unsubscribed from {type} data.");
                        return; // Design time error
                    }

                    this.instrumentSubscriptions.Remove(subscriber);
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(type.Name, nameof(type.Name));
            }
        }

        private void Publish((BarType, Bar) data)
        {
            if (this.barSubscriptions.Count == 0)
            {
                return; // No subscribers.
            }

            for (var i = 0; i < this.barSubscriptions.Count; i++)
            {
                this.barSubscriptions[i].Send(data);

                this.Log.Verbose(
                    $"[{this.ProcessedCount}] {typeof((BarType, Bar)).Name} -> {this.barSubscriptions[i]}");
            }
        }

        private void Publish(Instrument data)
        {
            if (this.instrumentSubscriptions.Count == 0)
            {
                return; // No subscribers.
            }

            for (var i = 0; i < this.instrumentSubscriptions.Count; i++)
            {
                this.instrumentSubscriptions[i].Send(data);

                this.Log.Verbose(
                    $"[{this.ProcessedCount}] {typeof(Instrument).Name} -> {this.instrumentSubscriptions[i]}");
            }
        }
    }
}
