//--------------------------------------------------------------------------------------------------
// <copyright file="FixGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a factory for FIX gateways.
    /// </summary>
    public static class FixGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new FIX gateway.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <param name="tickBus">The tick bus endpoint.</param>
        /// <param name="dataBus">The data bus endpoint.</param>
        /// <returns>The created FIX gateway.</returns>
        public static FixGateway Create(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient fixClient,
            IEndpoint tickBus,
            IEndpoint dataBus)
        {
            var gateway = new FixGateway(
                container,
                messagingAdapter,
                fixClient,
                tickBus,
                dataBus);

            fixClient.InitializeGateway(gateway);

            return gateway;
        }
    }
}
