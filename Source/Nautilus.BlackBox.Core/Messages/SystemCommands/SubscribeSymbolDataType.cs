﻿//--------------------------------------------------------------------------------------------------
// <copyright file="SubscribeSymbolDataType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="SubscribeSymbolDataType"/> class.
    /// </summary>
    [Immutable]
    public sealed class SubscribeSymbolDataType : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeSymbolDataType"/> class.
        /// </summary>
        /// <param name="symbol">The message symbol.</param>
        /// <param name="barSpec">The message bar profile.</param>
        /// <param name="tradeType">The message trade type.</param>
        /// <param name="tickSize">The message tick size (cannot be zero or negative).</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public SubscribeSymbolDataType(
            Symbol symbol,
            BarSpecification barSpec,
            TradeType tradeType,
            decimal tickSize,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Symbol = symbol;
            this.BarSpecification = barSpec;
            this.TradeType = tradeType;
            this.TickSize = tickSize;
        }

        /// <summary>
        /// Gets the messages instrument.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the messages bar profile.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the messages trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the messages tick size.
        /// </summary>
        public decimal TickSize { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="SubscribeSymbolDataType"/>
        /// service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(SubscribeSymbolDataType);
    }
}
