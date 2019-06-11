//--------------------------------------------------------------------------------------------------
// <copyright file="FixDataGatewayFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// Provides a factory for FIX gateways.
    /// </summary>
    public static class FixDataGatewayFactory
    {
        /// <summary>
        /// Creates and returns a new FIX gateway.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="dataBusAdapter">The data bus adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        /// <returns>The created FIX gateway.</returns>
        public static FixDataGateway Create(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IDataBusAdapter dataBusAdapter,
            IFixClient fixClient)
        {
            var gateway = new FixDataGateway(
                container,
                messageBusAdapter,
                dataBusAdapter,
                fixClient);

            fixClient.InitializeGateway(gateway);

            return gateway;
        }
    }
}