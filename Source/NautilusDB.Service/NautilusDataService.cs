﻿//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusDataService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB.Service
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using ServiceStack;

    /// <summary>
    /// Provides a REST API for the <see cref="NautilusDB"/> system.
    /// </summary>
    public class NautilusDataService : Service
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
            Validate.NotNull(setupContainer, nameof(setupContainer));

            this.clock = setupContainer.Clock;
            this.guidFactory = setupContainer.GuidFactory;
            this.logger = setupContainer.LoggerFactory.Create(
                NautilusService.Data,
                LabelFactory.Component(nameof(NautilusDataService)));
        }
    }
}