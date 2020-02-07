//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusServiceBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Service
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Events;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Fix;
    using Nautilus.Messaging;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a data service.
    /// </summary>
    public abstract class NautilusServiceBase : MessageBusConnected
    {
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledConnect;
        private readonly (IsoDayOfWeek Day, LocalTime Time) scheduledDisconnect;
        private readonly List<Address> connectionAddresses = new List<Address>();

        private ZonedDateTime nextConnectTime;
        private ZonedDateTime nextDisconnectTime;
        private bool autoReconnect;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusServiceBase"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="config">The service configuration.</param>
        /// <exception cref="ArgumentException">If the addresses is empty.</exception>
        protected NautilusServiceBase(
            IComponentryContainer container,
            MessageBusAdapter messageBusAdapter,
            IScheduler scheduler,
            FixConfiguration config)
            : base(container, messageBusAdapter)
        {
            this.Scheduler = scheduler;
            this.scheduledConnect = config.ConnectTime;
            this.scheduledDisconnect = config.DisconnectTime;
            this.nextConnectTime = TimingProvider.GetNextUtc(
                this.scheduledConnect.Day,
                this.scheduledConnect.Time,
                this.InstantNow());
            this.nextDisconnectTime = TimingProvider.GetNextUtc(
                this.scheduledDisconnect.Day,
                this.scheduledDisconnect.Time,
                this.InstantNow());
            this.autoReconnect = true;

            // Commands
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Events
            this.RegisterHandler<SessionConnected>(this.OnMessage);
            this.RegisterHandler<SessionDisconnected>(this.OnMessage);

            // Event Subscriptions
            this.Subscribe<SessionConnected>();
            this.Subscribe<SessionDisconnected>();
        }

        /// <summary>
        /// Gets the services scheduler.
        /// </summary>
        protected IScheduler Scheduler { get; }

        /// <summary>
        /// Register the given address to receiver generated connect messages.
        /// </summary>
        /// <param name="receiver">The receiver to register.</param>
        protected void RegisterConnectionAddress(Address receiver)
        {
            if (!this.connectionAddresses.Contains(receiver))
            {
                this.connectionAddresses.Add(receiver);
            }
            else
            {
                this.Log.Warning($"Connection address {receiver} was already registered.");
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.Execute(() =>
            {
                if (TimingProvider.IsOutsideWeeklyInterval(
                    this.scheduledDisconnect,
                    this.scheduledConnect,
                    this.InstantNow()))
                {
                    // Outside disconnection schedule weekly interval
                    this.CreateDisconnectFixJob();
                    this.CreateConnectFixJob();
                }
                else
                {
                    // Inside disconnection schedule weekly interval
                    this.CreateConnectFixJob();
                    this.CreateDisconnectFixJob();
                }

                this.OnServiceStart(start);
            });
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.autoReconnect = false;  // Avoid immediate reconnection
            this.OnServiceStop(stop);
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        /// <param name="start">The start message.</param>
        protected virtual void OnServiceStart(Start start)
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Stop"/> message.
        /// </summary>
        /// <param name="stop">The start message.</param>
        protected virtual void OnServiceStop(Stop stop)
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        protected virtual void OnConnected()
        {
            // Do nothing if not overridden
        }

        /// <summary>
        /// Called when the service receives a <see cref="Start"/> message.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            // Do nothing if not overridden
        }

        private void OnMessage(Connect connect)
        {
            // Forward message
            this.autoReconnect = true;
            this.Send(connect, this.connectionAddresses);
        }

        private void OnMessage(Disconnect disconnect)
        {
            // Forward message
            this.autoReconnect = false;  // Avoid immediate reconnection
            this.Send(disconnect, this.connectionAddresses);
        }

        private void OnMessage(SessionConnected message)
        {
            this.Log.Information($"Connected to session {message.SessionId}.");

            if (this.nextDisconnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateDisconnectFixJob();
            }

            this.OnConnected();
        }

        private void OnMessage(SessionDisconnected message)
        {
            if (this.autoReconnect)
            {
                this.Log.Warning($"Disconnected from session {message.SessionId}. Initiating auto reconnect...");

                var connect = new Connect(
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(connect, this.connectionAddresses);
            }
            else
            {
                this.Log.Information($"Disconnected from session {message.SessionId}.");
            }

            if (this.nextConnectTime.IsLessThanOrEqualTo(this.TimeNow()))
            {
                this.CreateConnectFixJob();
            }

            this.OnDisconnected();
        }

        private void CreateConnectFixJob()
        {
            this.Execute(() =>
            {
                var now = this.InstantNow();
                var nextTime = TimingProvider.GetNextUtc(
                    this.scheduledConnect.Day,
                    this.scheduledConnect.Time,
                    now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

                var job = new Connect(
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.Scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    job,
                    this.Endpoint);

                this.nextConnectTime = nextTime;

                this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
            });
        }

        private void CreateDisconnectFixJob()
        {
            this.Execute(() =>
            {
                var now = this.InstantNow();
                var nextTime = TimingProvider.GetNextUtc(
                    this.scheduledDisconnect.Day,
                    this.scheduledDisconnect.Time,
                    now);
                var durationToNext = TimingProvider.GetDurationToNextUtc(nextTime, now);

                var job = new Disconnect(
                    nextTime,
                    this.NewGuid(),
                    this.TimeNow());

                this.Scheduler.ScheduleSendOnceCancelable(
                    durationToNext,
                    this.Endpoint,
                    job,
                    this.Endpoint);

                this.nextDisconnectTime = nextTime;

                this.Log.Information($"Created scheduled job {job} for {nextTime.ToIsoString()}");
            });
        }
    }
}
