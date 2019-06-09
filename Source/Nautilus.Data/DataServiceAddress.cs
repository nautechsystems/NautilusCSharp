//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceAddress.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using Nautilus.Core.Annotations;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides data service component messaging addresses.
    /// </summary>
    [PerformanceOptimized]
    public static class DataServiceAddress
    {
        /// <summary>
        /// Gets the <see cref="Core"/> component messaging address.
        /// </summary>
        public static Address Core { get; } = new Address(nameof(Core));

        /// <summary>
        /// Gets the <see cref="Scheduler"/> component messaging address.
        /// </summary>
        public static Address Scheduler { get; } = new Address(nameof(Scheduler));

        /// <summary>
        /// Gets the <see cref="FixGateway"/> component messaging address.
        /// </summary>
        public static Address FixGateway { get; } = new Address(nameof(FixGateway));

        /// <summary>
        /// Gets the <see cref="TickBus"/> component messaging address.
        /// </summary>
        public static Address TickBus { get; } = new Address(nameof(TickBus));

        /// <summary>
        /// Gets the <see cref="DataBus"/> component messaging address.
        /// </summary>
        public static Address DataBus { get; } = new Address(nameof(DataBus));

        /// <summary>
        /// Gets the <see cref="TickPublisher"/> component messaging address.
        /// </summary>
        public static Address TickStore { get; } = new Address(nameof(TickStore));

        /// <summary>
        /// Gets the <see cref="DatabaseTaskManager"/> component messaging address.
        /// </summary>
        public static Address DatabaseTaskManager { get; } = new Address(nameof(DatabaseTaskManager));

        /// <summary>
        /// Gets the <see cref="BarAggregationController"/> component messaging address.
        /// </summary>
        public static Address BarAggregationController { get; } = new Address(nameof(BarAggregationController));

        /// <summary>
        /// Gets the <see cref="BarProvider"/> component messaging address.
        /// </summary>
        public static Address BarProvider { get; } = new Address(nameof(BarProvider));

        /// <summary>
        /// Gets the <see cref="BarPublisher"/> component messaging address.
        /// </summary>
        public static Address BarPublisher { get; } = new Address(nameof(BarPublisher));

        /// <summary>
        /// Gets the <see cref="TickProvider"/> component messaging address.
        /// </summary>
        public static Address TickProvider { get; } = new Address(nameof(TickProvider));

        /// <summary>
        /// Gets the <see cref="TickPublisher"/> component messaging address.
        /// </summary>
        public static Address TickPublisher { get; } = new Address(nameof(TickPublisher));

        /// <summary>
        /// Gets the <see cref="InstrumentProvider"/> component messaging address.
        /// </summary>
        public static Address InstrumentProvider { get; } = new Address(nameof(InstrumentProvider));

        /// <summary>
        /// Gets the <see cref="InstrumentPublisher"/> component messaging address.
        /// </summary>
        public static Address InstrumentPublisher { get; } = new Address(nameof(InstrumentPublisher));
    }
}
