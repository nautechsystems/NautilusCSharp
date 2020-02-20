﻿//--------------------------------------------------------------------------------------------------
// <copyright file="IComponentryContainer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The componentry container for constructing service components.
    /// </summary>
    public interface IComponentryContainer
    {
        /// <summary>
        /// Gets the containers clock.
        /// </summary>
        IZonedClock Clock { get; }

        /// <summary>
        /// Gets the containers GUID factory.
        /// </summary>
        IGuidFactory GuidFactory { get; }

        /// <summary>
        /// Gets the containers logger.
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
    }
}
