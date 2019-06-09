﻿//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MockMessagingAdapter : IMessagingAdapter
    {
        private readonly IEndpoint testEndpoint;

        public MockMessagingAdapter(IEndpoint testEndpoint)
        {
            this.testEndpoint = testEndpoint;
        }

        public void Subscribe<T>(T messageType, IEndpoint subscriber, Guid id, ZonedDateTime timestamp)
        {
            this.testEndpoint.Send(messageType);
        }

        public void Unsubscribe<T>(T messageType, IEndpoint subscriber, Guid id, ZonedDateTime timestamp)
        {
            this.testEndpoint.Send(messageType);
        }

        public void Send<T>(T message, Address receiver, Address sender, ZonedDateTime timestamp)
            where T : Message
        {
            this.testEndpoint.Send(message);
        }

        public void SendToBus<T>(T message, Address sender, ZonedDateTime timestamp)
            where T : Message
        {
            this.testEndpoint.Send(message);
        }
    }
}
