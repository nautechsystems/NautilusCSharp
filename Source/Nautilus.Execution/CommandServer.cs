// -------------------------------------------------------------------------------------------------
// <copyright file="CommandServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Execution.Network;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using NodaTime;

    /// <summary>
    /// Provides a <see cref="Command"/> message server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class CommandServer : ComponentBusConnected
    {
        private readonly IEndpoint commandRouter;
        private readonly Throttler commandThrottler;
        private readonly Throttler newOrderThrottler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandServer"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="orderManager">The order manager endpoint.</param>
        /// <param name="config">The service configuration.</param>
        public CommandServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEndpoint orderManager,
            Configuration config)
            : base(container, messagingAdapter)
        {
            this.commandRouter = new CommandRouter(
                container,
                commandSerializer,
                this.Endpoint,
                config.ServerAddress,
                config.CommandsPort).Endpoint;

            this.commandThrottler = new Throttler(
                container,
                orderManager,
                Duration.FromSeconds(1),
                config.CommandsPerSecond);

            this.newOrderThrottler = new Throttler(
                container,
                this.commandThrottler.Endpoint,
                Duration.FromSeconds(1),
                config.NewOrdersPerSecond);

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<CollateralInquiry>(this.OnMessage);
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.Log.Information($"Starting from {message}...");

            this.commandRouter.Send(message);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Information($"Stopping from {message}...");

            this.commandRouter.Send(message);
        }

        private void OnMessage(SubmitOrder message)
        {
            this.newOrderThrottler.Endpoint.Send(message);
        }

        private void OnMessage(SubmitAtomicOrder message)
        {
            this.newOrderThrottler.Endpoint.Send(message);
        }

        private void OnMessage(CancelOrder message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.commandThrottler.Endpoint.Send(message);
        }
    }
}
