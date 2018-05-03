// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Represents a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    [Immutable]
    public sealed class MessageBus<T> : ReceiveActor
        where T : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="component">The component.</param>
        /// <param name="container">The container.</param>
        /// <param name="messageStoreRef">The message store actor address.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public MessageBus(
            Enum serviceContext,
            Label component,
            IComponentryContainer container,
            IActorRef messageStoreRef)
        {
            Validate.NotNull(component, nameof(component));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messageStoreRef, nameof(messageStoreRef));

            this.Service = serviceContext;
            this.Component = component;
            this.Clock = container.Clock;
            this.Logger = container.LoggerFactory.Create(this.Service, component);
            this.MessageStore = messageStoreRef;
            this.CommandHandler = new CommandHandler(this.Logger);

            this.SetupMessageHandling();
        }

        /// <summary>
        /// Gets the message bus bounded context.
        /// </summary>
        private Enum Service { get; }

        /// <summary>
        /// Gets the message bus component.
        /// </summary>
        private Label Component { get; }

        /// <summary>
        /// Gets the message bus clock.
        /// </summary>
        private IZonedClock Clock { get; }

        /// <summary>
        /// Gets the message bus logger.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets or sets the message bus switchboard.
        /// </summary>
        private ISwitchboard Switchboard { get; set; }

        /// <summary>
        /// Gets the message bus message store.
        /// </summary>
        private IActorRef MessageStore { get; }

        /// <summary>
        /// Gets the message bus command handler.
        /// </summary>
        private CommandHandler CommandHandler { get; }

        /// <summary>
        /// Gets or sets the message count.
        /// </summary>
        private int MessageCount { get; set; }

        /// <summary>
        /// Runs pre-start of the receive actor.
        /// </summary>
        protected override void PreStart()
        {
            this.Logger.Log(LogLevel.Debug, $"{typeof(T).Name}Bus initializing...");
        }

        /// <summary>
        /// Logs unhandled messages.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void Unhandled(object message)
        {
            this.Logger.Log(LogLevel.Error, $"Unhandled message {message}");
        }

        /// <summary>
        /// Sets up all <see cref="Message"/> handling methods.
        /// </summary>
        private void SetupMessageHandling()
        {
            this.Receive<InitializeMessageSwitchboard>(msg => this.OnMessage(msg));
            this.Receive<Envelope<T>>(envelope => this.OnReceive(envelope));
        }

        private void OnMessage(InitializeMessageSwitchboard message)
        {
            Debug.NotNull(message.Switchboard, nameof(message.Switchboard));

            this.CommandHandler.Execute(() =>
            {
                this.Switchboard = message.Switchboard;

                this.Log(LogLevel.Information, $"{this.Switchboard.GetType().Name} initialized");
            });
        }

        private void LogEnvelope(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.CommandHandler.Execute(() =>
            {
                this.MessageCount++;
                this.MessageStore.Tell(envelope);

                foreach (var receiver in envelope.Receivers)
                {
                    this.Log(LogLevel.Debug, $"[{this.MessageCount}] {envelope.Sender} -> {envelope} -> {receiver}");
                }
            });
        }

        private void OnReceive(Envelope<T> envelope)
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.CommandHandler.Execute(() =>
            {
                this.Switchboard.SendToReceivers(envelope);

                this.LogEnvelope(envelope);
            });
        }

        private void Log(LogLevel logLevel, string logText)
        {
            Debug.NotNull(logText, nameof(logText));

            this.CommandHandler.Execute(() =>
            {
                this.Logger.Log(logLevel, logText);
            });
        }
    }
}
