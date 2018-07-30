﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TradeType.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a unique trade strategy type.
    /// </summary>
    [Immutable]
    public sealed class TradeType : ValidString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeType"/> class.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        public TradeType(string tradeType)
            : base(tradeType)
        {
            Debug.NotNull(tradeType, nameof(tradeType));
        }
    }
}
