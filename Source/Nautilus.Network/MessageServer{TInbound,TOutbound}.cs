// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer{TInbound,TOutbound}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network.Messages;
    using NetMQ;
    using NetMQ.Sockets;

    /// <summary>
    /// The base class for all messaging servers.
    /// </summary>
    /// <typeparam name="TInbound">The inbound message type.</typeparam>
    /// <typeparam name="TOutbound">The outbound response type.</typeparam>
    public abstract class MessageServer<TInbound, TOutbound> : Component, IDisposable
        where TInbound : Message
        where TOutbound : Response
    {
        private const int ExpectedFramesCount = 3;

        private readonly byte[] delimiter = { };
        private readonly CancellationTokenSource cts;
        private readonly RouterSocket socket;
        private readonly IMessageSerializer<TInbound> inboundSerializer;
        private readonly IMessageSerializer<TOutbound> outboundSerializer;
        private readonly Dictionary<Guid, Address> correlationIndex = new Dictionary<Guid, Address>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer{TInbound, TOutbound}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="inboundSerializer">The inbound message serializer.</param>
        /// <param name="outboundSerializer">The outbound message serializer.</param>
        /// <param name="host">The consumer host address.</param>
        /// <param name="port">The consumer port.</param>
        /// <param name="id">The consumer identifier.</param>
        protected MessageServer(
            IComponentryContainer container,
            IMessageSerializer<TInbound> inboundSerializer,
            IMessageSerializer<TOutbound> outboundSerializer,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(container)
        {
            Condition.NotDefault(id, nameof(id));

            this.cts = new CancellationTokenSource();
            this.socket = new RouterSocket()
            {
                Options =
                {
                    Linger = TimeSpan.Zero,
                    Identity = Encoding.Unicode.GetBytes(id.ToString()),
                },
            };

            this.inboundSerializer = inboundSerializer;
            this.outboundSerializer = outboundSerializer;

            this.NetworkAddress = new ZmqNetworkAddress(host, port);
            this.CountReceived = 0;
            this.CountSent = 0;

            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the network address for the router.
        /// </summary>
        public ZmqNetworkAddress NetworkAddress { get; }

        /// <summary>
        /// Gets the server received message count.
        /// </summary>
        public int CountReceived { get; private set; }

        /// <summary>
        /// Gets the server processed message count.
        /// </summary>
        public int CountSent { get; private set; }

        /// <summary>
        /// Dispose of the socket.
        /// </summary>
        public void Dispose()
        {
            if (!this.socket.IsDisposed)
            {
                this.socket.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.socket.Bind(this.NetworkAddress.Value);
            this.Log.Debug($"Bound {this.socket.GetType().Name} to {this.NetworkAddress}");

            Task.Run(this.StartWork, this.cts.Token);
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.cts.Cancel();
            this.socket.Unbind(this.NetworkAddress.Value);
            this.Log.Debug($"Unbound {this.socket.GetType().Name} from {this.NetworkAddress}");

            this.Dispose();
        }

        /// <summary>
        /// Sends a MessageReceived message to the given receiver address.
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected void SendReceived(Message receivedMessage)
        {
            var received = new MessageReceived(
                receivedMessage.Type.Name,
                receivedMessage.Id,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (received is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(received, receivedMessage.Id);
        }

        /// <summary>
        /// Sends a MessageRejected message to the given receiver address.
        /// </summary>
        /// <param name="failureMessage">The query failure message.</param>
        /// <param name="correlationId">The message correlation identifier.</param>
        protected void SendQueryFailure(string failureMessage, Guid correlationId)
        {
            var failure = new QueryFailure(
                failureMessage,
                correlationId,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (failure is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(failure, correlationId);
        }

        /// <summary>
        /// Sends a message with the given payload to the given receiver address.
        /// </summary>
        /// <param name="outbound">The outbound message to send.</param>
        /// <param name="correlationId">The correlation identifier for the receiver address.</param>
        protected void SendMessage(TOutbound outbound, Guid correlationId)
        {
            if (this.correlationIndex.Remove(correlationId, out var receiver))
            {
            }
            else
            {
                this.Log.Error($"Cannot send message {outbound} " +
                               $"(no receiver address found for {correlationId}).");
                return;
            }

            if (!receiver.HasBytesValue)
            {
                this.Log.Error($"Cannot send message {outbound} " +
                               $"(no receiver address found for {correlationId}).");
                return;
            }

            this.SendMessage(outbound, receiver);
        }

        private Task StartWork()
        {
            while (!this.cts.IsCancellationRequested)
            {
                this.ReceiveMessage();
            }

            this.Log.Debug("Stopped receiving messages.");
            return Task.CompletedTask;
        }

        private void ReceiveMessage()
        {
            // msg[0] reply address
            // msg[1] should be empty byte array delimiter
            // msg[2] payload
            var msg = this.socket.ReceiveMultipartBytes(ExpectedFramesCount);
            if (msg.Count != ExpectedFramesCount)
            {
                var error = $"Message was malformed (expected {ExpectedFramesCount} frames, received {msg.Count}).";
                if (msg.Count >= 1)
                {
                    this.SendRejected(error, new Address(msg[0], Encoding.ASCII.GetString));
                }

                this.Log.Error(error);
            }
            else
            {
                this.DeserializeMessage(msg[2], new Address(msg[0], Encoding.ASCII.GetString));
            }
        }

        private void DeserializeMessage(byte[] payload, Address sender)
        {
            try
            {
                var received = this.inboundSerializer.Deserialize(payload);
                this.correlationIndex[received.Id] = sender;

                if (received.Type.IsSubclassOf(typeof(TInbound)) || received.Type == typeof(TInbound))
                {
                    this.SendToSelf(received);

                    this.Log.Verbose($"[{this.CountReceived}]<-- {received} from Address({sender.StringValue}).");
                }
                else
                {
                    var errorMessage = $"Message type {received.Type} not valid at this address {this.NetworkAddress}.";
                    this.SendRejected(errorMessage, sender);

                    this.Log.Error(errorMessage);
                }

                this.CountReceived++;
            }
            catch (SerializationException ex)
            {
                var message = "Unable to deserialize message.";
                this.Log.Error(message + Environment.NewLine + ex);
                this.SendRejected(message, sender);
            }
        }

        private void SendRejected(string rejectedMessage, Address receiver)
        {
            var rejected = new MessageRejected(
                rejectedMessage,
                Guid.Empty,
                Guid.NewGuid(),
                this.TimeNow()) as TOutbound;

            // Exists to avoid warning CS8604 (this should never happen anyway due to generic type constraints)
            if (rejected is null)
            {
                throw new InvalidOperationException($"The message was not of type {typeof(TOutbound)}.");
            }

            this.SendMessage(rejected, receiver);
        }

        private void SendMessage(TOutbound outbound, Address receiver)
        {
            this.socket.SendMultipartBytes(
                receiver.BytesValue,
                this.delimiter,
                this.outboundSerializer.Serialize(outbound));

            this.CountSent++;
            this.Log.Verbose($"[{this.CountSent}]--> {outbound} to Address({receiver}).");
        }
    }
}
