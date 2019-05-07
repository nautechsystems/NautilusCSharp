//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageReceiver.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using NautilusMQ;

    /// <summary>
    /// Provides a mock message receiver for testing.
    /// </summary>
    public class MockMessageReceiver : MessageReceiver
    {
        private readonly int workDelayMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockMessageReceiver"/> class.
        /// </summary>
        /// <param name="workDelayMilliseconds">The work delay for the receiver.</param>
        public MockMessageReceiver(int workDelayMilliseconds = 1000)
        {
            this.workDelayMilliseconds = workDelayMilliseconds;
        }

        /// <summary>
        /// Gets the list of received messages.
        /// </summary>
        public List<object> Messages { get; } = new List<object>();

        /// <summary>
        /// Add the message to the received messages list.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void OnMessage(object message)
        {
            this.Messages.Add(message);
        }

        /// <summary>
        /// Add the message to the received messages list.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void OnMessage(int message)
        {
            this.Messages.Add(message);
        }

        /// <summary>
        /// Add the message to the received messages list.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void OnMessageWithWorkDelay(object message)
        {
            this.Messages.Add(message);
            Thread.Sleep(1000);
        }
    }
}
