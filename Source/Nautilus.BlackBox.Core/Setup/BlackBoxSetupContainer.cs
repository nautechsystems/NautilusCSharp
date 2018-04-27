﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxSetupContainer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Setup
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="BlackBoxSetupContainer"/> class. A container to store and
    /// transport the information and infrastructure needed to instantiate a <see cref="BlackBox"/>
    /// instance and its required services and components.
    /// </summary>
    [Immutable]
    public sealed class BlackBoxSetupContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlackBoxSetupContainer"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="loggerFactory">The logger Factory.</param>
        /// <param name="guidFactory">The globally unique identifier factory.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="quoteProvider">The quote provider.</param>
        /// <param name="riskModel">The risk model.</param>
        /// <param name="account">The account.</param>
        public BlackBoxSetupContainer(
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggerFactory loggerFactory,
            IGuidFactory guidFactory,
            IInstrumentRepository instrumentRepository,
            IQuoteProvider quoteProvider,
            IReadOnlyRiskModel riskModel,
            IReadOnlyBrokerageAccount account)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(loggerFactory, nameof(loggerFactory));
            Validate.NotNull(guidFactory, nameof(guidFactory));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));
            Validate.NotNull(quoteProvider, nameof(quoteProvider));
            Validate.NotNull(riskModel, nameof(riskModel));
            Validate.NotNull(account, nameof(account));

            this.Environment = environment;
            this.Clock = clock;
            this.LoggerFactory = loggerFactory;
            this.GuidFactory = guidFactory;
            this.InstrumentRepository = instrumentRepository;
            this.QuoteProvider = quoteProvider;
            this.RiskModel = riskModel;
            this.Account = account;
        }

        /// <summary>
        /// Gets the black box environment.
        /// </summary>
        public BlackBoxEnvironment Environment { get; }

        /// <summary>
        /// Gets the black box clock.
        /// </summary>
        public IZonedClock Clock { get; }

        /// <summary>
        /// Gets the black box logger factory.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the black box <see cref="Guid"/> factory.
        /// </summary>
        public IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the black box risk model.
        /// </summary>
        public IReadOnlyRiskModel RiskModel { get; }

        /// <summary>
        /// Gets the black box account.
        /// </summary>
        public IReadOnlyBrokerageAccount Account { get; }

        /// <summary>
        /// Gets the black box instrument repository.
        /// </summary>
        public IInstrumentRepository InstrumentRepository { get; }

        /// <summary>
        /// Gets the black box quote provider.
        /// </summary>
        public IQuoteProvider QuoteProvider { get; }
    }
}
