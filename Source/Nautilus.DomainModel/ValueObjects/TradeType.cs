﻿// -------------------------------------------------------------------------------------------------
// <copyright file="TradeType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;

    /// <summary>
    /// The immutable sealed <see cref="TradeType"/> class. Represents a unique trade type.
    /// </summary>
    [Immutable]
    public sealed class TradeType : ValidString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeType"/> class.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <exception cref="ValidationException">Throws if the value is null or white space, or if
        /// the string values length is greater than 100 characters.</exception>
        public TradeType(string tradeType) : base(tradeType)
        {
            // Validated by base class.
        }
    }
}