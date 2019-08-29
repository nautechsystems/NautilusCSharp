//--------------------------------------------------------------------------------------------------
// <copyright file="StrategyId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// Represents a valid strategy identifier.
    /// </summary>
    [Immutable]
    public sealed class StrategyId : Identifier<Execution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrategyId"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public StrategyId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}