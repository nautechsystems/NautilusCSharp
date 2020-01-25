﻿//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataFrame.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Frames
{
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A container for <see cref="Bars"/> of a certain <see cref="BarType"/>.
    /// </summary>
    [Immutable]
    public sealed class BarDataFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataFrame"/> class.
        /// </summary>
        /// <param name="barType">The symbol bar data.</param>
        /// <param name="bars">The bars dictionary.</param>
        public BarDataFrame(BarType barType, Bar[] bars)
        {
            this.BarType = barType;
            this.Bars = bars;
        }

        /// <summary>
        /// Gets the data frames symbol.
        /// </summary>
        public BarType BarType { get; }

        /// <summary>
        /// Gets the data frames bars.
        /// </summary>
        public Bar[] Bars { get; }

        /// <summary>
        /// Gets the data frames count of bars held.
        /// </summary>
        public int Count => this.Bars.Length;

        /// <summary>
        /// Gets the data frames start time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Bars.First().Timestamp;

        /// <summary>
        /// Gets the data frames end time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Bars.Last().Timestamp;
    }
}
