// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    internal class MessageProcessor
    {
        private readonly ActionBlock<object> processor;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly List<KeyValuePair<Type, Handler>> registeredHandlers = new List<KeyValuePair<Type, Handler>>();

        private Action<object> unhandled;
        private Handler[] handlers = { };
        private int handlersLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        internal MessageProcessor()
        {
            this.unhandled = this.AddToUnhandledMessages;

            this.processor = new ActionBlock<object>(
                async message =>
                {
                    await this.HandleMessage(message);
                },
                new ExecutionDataflowBlockOptions { CancellationToken = this.cts.Token });

            this.Endpoint = new Endpoint(this.processor.Post);
        }

        /// <summary>
        /// Gets the processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the message input count for the processor.
        /// </summary>
        public int InputCount => this.processor.InputCount;

        /// <summary>
        /// Gets the message processed count for the processor.
        /// </summary>
        public int ProcessedCount { get; private set; }

        /// <summary>
        /// Gets the types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.registeredHandlers.Select(h => h.Key);

        /// <summary>
        /// Gets the list of unhandled messages.
        /// </summary>
        public List<object> UnhandledMessages { get; } = new List<object>();

        /// <summary>
        /// Register the given message type with the gin handler.
        /// </summary>ve
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);
            if (this.registeredHandlers.Any(h => h.Key == type))
            {
                throw new ArgumentException($"Cannot register handler " +
                                            $"(the internal handlers already contain a handler for {type} type messages).");
            }

            this.registeredHandlers.Add(new KeyValuePair<Type, Handler>(type, Handler.Create(handler)));

            this.MoveHandlerToEndOfList(typeof(object));
            this.BuildHandlers();
        }

        /// <summary>
        /// Register the given delegate to receive unhandled messages.
        /// </summary>ve
        /// <param name="handler">The handler delegate.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.unhandled = handler;
        }

        /// <summary>
        /// Adds the given message to the list of unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        public void AddToUnhandledMessages(object message)
        {
            this.UnhandledMessages.Add(message);
        }

        private void Unhandled(object message)
        {
            this.unhandled(message);
        }

        private void BuildHandlers()
        {
            this.handlers = this.registeredHandlers.Select(h => h.Value).ToArray();
            this.handlersLength = this.handlers.Length;
        }

        private Task HandleMessage(object message)
        {
            this.ProcessedCount++;

            for (var i = 0; i < this.handlersLength; i++)
            {
                if (this.handlers[i].Handle(message))
                {
                    return Task.CompletedTask;
                }
            }

            this.Unhandled(message);

            return Task.CompletedTask;
        }

        private void MoveHandlerToEndOfList(Type handlerType)
        {
            if (this.registeredHandlers.Any(h => h.Key == handlerType))
            {
                // Move handle object to the end of the handlers
                var handleObject = this.registeredHandlers.FirstOrDefault(h => h.Key == handlerType);
                this.registeredHandlers.Remove(handleObject);
                this.registeredHandlers.Add(handleObject);
            }
        }
    }
}
