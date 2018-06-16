﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSetupContainer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// The setup container for the <see cref="Nautilus.Database"/>.
    /// </summary>
    [Immutable]
    public sealed class DatabaseSetupContainer : IComponentryContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSetupContainer"/> class.
        /// </summary>
        /// <param name="clock">The container clock.</param>
        /// <param name="guidFactory">The container GUID factory.</param>
        /// <param name="loggerFactory">The container logger factory.</param>
        public DatabaseSetupContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(guidFactory, nameof(guidFactory));
            Validate.NotNull(loggerFactory, nameof(loggerFactory));

            this.Clock = clock;
            this.GuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets the containers clock.
        /// </summary>
        public IZonedClock Clock { get; }

        /// <summary>
        /// Gets the containers GUID factory.
        /// </summary>
        public IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the containers logger factory.
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }
    }
}
