﻿//--------------------------------------------------------------------------------------------------
// <copyright file="SystemShutdown.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Represents a command to shutdown the system.
    /// </summary>
    [Immutable]
    public sealed class SystemShutdown : SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemShutdown"/> class.
        /// </summary>
        /// <param name="messageId">The commands identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The commands timestamp (cannot be default).</param>
        public SystemShutdown(
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Debug.NotDefault(messageId, nameof(messageId));
            Debug.NotDefault(messageTimestamp, nameof(messageTimestamp));
        }
    }
}