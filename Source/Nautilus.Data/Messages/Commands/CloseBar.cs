﻿//--------------------------------------------------------------------------------------------------
// <copyright file="CloseBar.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The command message to close the bar of the given bar specification.
    /// </summary>
    [Immutable]
    public sealed class CloseBar : Command, IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseBar"/> class.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <param name="scheduledTime">The scheduled job time.</param>
        /// <param name="identifier">The close identifier.</param>
        /// <param name="timestamp">The close timestamp.</param>
        public CloseBar(
            BarSpecification barSpec,
            ZonedDateTime scheduledTime,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            this.ScheduledTime = scheduledTime;
            this.BarSpecification = barSpec;
        }

        /// <summary>
        /// Gets the messages bar specification to close.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }
    }
}