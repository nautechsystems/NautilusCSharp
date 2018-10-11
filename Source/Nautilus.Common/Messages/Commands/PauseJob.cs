//--------------------------------------------------------------------------------------------------
// <copyright file="PauseJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Quartz;

    /// <summary>
    /// Represents a command to pause a job.
    /// </summary>
    [Immutable]
    public sealed class PauseJob : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PauseJob"/> class.
        /// </summary>
        /// <param name="jobKey">The job key to pause.</param>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public PauseJob(
            JobKey jobKey,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(jobKey, nameof(jobKey));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.JobKey = jobKey;
        }

        /// <summary>
        /// Gets the job to pause key.
        /// </summary>
        public JobKey JobKey { get; }
    }
}