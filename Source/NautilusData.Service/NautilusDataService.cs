﻿//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData.Service
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a REST API for the <see cref="NautilusData"/> system.
    /// </summary>
    public class NautilusDataService
    {
        private readonly IZonedClock clock;
        private readonly IGuidFactory guidFactory;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NautilusDataService"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        public NautilusDataService(IComponentryContainer setupContainer)
        {
            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.logger = setupContainer.LoggerFactory.Create(
                NautilusService.Data,
                new Label(nameof(NautilusDataService)));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        public void Test()
        {
            var x1 = this.clock.TimeNow();
            var x2 = this.guidFactory.NewGuid();
            this.logger.Debug($"Test logger {x1} {x2}");
        }
    }
}
