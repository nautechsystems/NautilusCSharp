﻿// -------------------------------------------------------------------------------------------------
// <copyright file="StubSetupContainerFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Moq;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Logging;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Data.Market;
    using Nautilus.BlackBox.Risk;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The stub nautilus setup container.
    /// </summary>
    public class StubSetupContainerFactory
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        public MockLogger Logger { get; private set; }

        /// <summary>
        /// Gets the quote provider.
        /// </summary>
        public IQuoteProvider QuoteProvider { get; private set; }

        /// <summary>
        /// The create.
        /// </summary>
        /// <returns>
        /// The <see cref="BlackBoxSetupContainer"/>.
        /// </returns>
        public BlackBoxSetupContainer Create()
        {
            var environment = BlackBoxEnvironment.Live;

            var clock = new StubClock();
            clock.FreezeSetTime(StubDateTime.Now());

            this.Logger = new MockLogger();
            var loggerFactory = new LoggerFactory(this.Logger);

            var guidFactory = new GuidFactory();
            var instrumentRepository = new Mock<IInstrumentRepository>().Object;
            this.QuoteProvider = new QuoteProvider(Exchange.FXCM);

            var riskModel = new RiskModel(
                new EntityId("None"),
                Percentage.Create(10),
                Percentage.Create(1),
                Quantity.Create(2),
                true,
                clock.TimeNow());

            var account = StubAccountFactory.Create();

            return new BlackBoxSetupContainer(
                environment,
                clock,
                loggerFactory,
                guidFactory,
                instrumentRepository,
                this.QuoteProvider,
                riskModel,
                account);
        }
    }
}