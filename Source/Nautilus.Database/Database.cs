﻿//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDatabase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Database.Enums;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The main macro object which contains the <see cref="Database"/> and presents its API.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Database : ComponentBusConnectedBase, IDisposable
    {
        private readonly ActorSystem actorSystem;
        private readonly Dictionary<Enum, IActorRef> addresses;
        private readonly IFixClient fixClient;
        private readonly int rollingWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="addresses">The system service addresses.</param>
        /// <param name="fixClient">The data client.</param>
        /// <param name="rollingWindow">The rolling window size of data to be maintained.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public Database(
            DatabaseSetupContainer setupContainer,
            ActorSystem actorSystem,
            MessagingAdapter messagingAdapter,
            Dictionary<Enum, IActorRef> addresses,
            IFixClient fixClient,
            int rollingWindow)
            : base(
                ServiceContext.Database,
                LabelFactory.Component(nameof(Database)),
                setupContainer,
                messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(addresses, nameof(addresses));
            Validate.Int32NotOutOfRange(rollingWindow, nameof(rollingWindow), 0, int.MaxValue, RangeEndPoints.LowerExclusive);

            this.actorSystem = actorSystem;
            this.addresses = addresses;
            this.fixClient = fixClient;
            this.rollingWindow = rollingWindow;

            messagingAdapter.Send(new InitializeMessageSwitchboard(
                new Switchboard(addresses),
                setupContainer.GuidFactory.NewGuid(),
                this.TimeNow()));
        }

        /// <summary>
        /// Start the database.
        /// </summary>
        public void Start()
        {
            this.fixClient.Connect();

            while (!this.fixClient.IsConnected)
            {
                // Wait for connection.
            }

            this.fixClient.UpdateInstrumentsSubscribeAll();
            this.fixClient.RequestMarketDataSubscribeAll();

            this.Send(
                DatabaseService.CollectionManager,
                new StartSystem(
                    Guid.NewGuid(),
                    this.TimeNow()));

            var barSpecs = new List<BarSpecification>
            {
                new BarSpecification(QuoteType.Bid, Resolution.Second, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Second, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Second, 1),
                new BarSpecification(QuoteType.Bid, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Minute, 1),
                new BarSpecification(QuoteType.Bid, Resolution.Hour, 1),
                new BarSpecification(QuoteType.Ask, Resolution.Hour, 1),
                new BarSpecification(QuoteType.Mid, Resolution.Hour, 1)
            };

            foreach (var symbol in this.fixClient.GetAllSymbols())
            {
                foreach (var barSpec in barSpecs)
                {
                    var barType = new BarType(symbol, barSpec);
                    var subscribe = new Subscribe<BarType>(
                        barType,
                        this.NewGuid(),
                        this.TimeNow());

                    this.Send(DatabaseService.CollectionManager, subscribe);
                }
            }
        }

        /// <summary>
        /// Gracefully shuts down the <see cref="Database"/> system.
        /// </summary>
        public void Shutdown()
        {
            this.fixClient.Disconnect();

            // Placeholder for the log events (do not refactor away).
            var actorSystemName = this.actorSystem.Name;

            this.Log.Information($"{actorSystemName} ActorSystem shutting down...");

            var shutdownTasks = this.addresses.Select(
                address => address.Value.GracefulStop(TimeSpan.FromSeconds(10)))
                .Cast<Task>()
                .ToList();

            this.Log.Information($"Waiting for actors to shut down...");
            Task.WhenAll(shutdownTasks);

            this.actorSystem.Terminate();
            this.Log.Information($"{actorSystemName} terminated.");

            this.Dispose();
        }

        /// <summary>
        /// Disposes the <see cref="Database"/> object.
        /// </summary>
        public void Dispose()
        {
            this.actorSystem?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
