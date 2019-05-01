//--------------------------------------------------------------------------------------------------
// <copyright file="Event.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using System.Diagnostics;
    using Nautilus.Core.Annotations;
    using NodaTime;

    /// <summary>
    /// The base class for all events.
    /// </summary>
    [Immutable]
    public abstract class Event : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        protected Event(Guid id, ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.Assert(id != default, AssertMsg.IsDefault(nameof(id)));
            Debug.Assert(timestamp != default, AssertMsg.IsDefault(nameof(timestamp)));
        }
    }
}
