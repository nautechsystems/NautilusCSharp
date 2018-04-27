﻿// -------------------------------------------------------------------------------------------------
// <copyright file="EntrySignal.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="EntrySignal"/> class. Represents a trade entry signal.
    /// </summary>
    [Immutable]
    public sealed class EntrySignal : Signal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntrySignal"/> class.
        /// </summary>
        /// <param name="symbol">The entry signal symbol.</param>
        /// <param name="signalId">The entry signal identifier.</param>
        /// <param name="signalLabel">The entry signal label.</param>
        /// <param name="tradeProfile">The entry signal trade profile.</param>
        /// <param name="orderSide">The entry signal order side.</param>
        /// <param name="entryPrice">The entry signal price.</param>
        /// <param name="stopLossPrice">The entry signal stop-loss price.</param>
        /// <param name="profitTargets">The entry signal profit targets.</param>
        /// <param name="signalTimestamp">The entry signal timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public EntrySignal(
            Symbol symbol,
            EntityId signalId,
            Label signalLabel,
            TradeProfile tradeProfile,
            OrderSide orderSide,
            Price entryPrice,
            Price stopLossPrice,
            IReadOnlyDictionary<int, Price> profitTargets,
            ZonedDateTime signalTimestamp)
            : base(
                  symbol,
                  signalId,
                  signalLabel,
                  tradeProfile.TradeType,
                  signalTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(signalId, nameof(signalId));
            Validate.NotNull(signalLabel, nameof(signalLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotDefault(orderSide, nameof(orderSide));
            Validate.NotNull(entryPrice, nameof(entryPrice));
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.NotNull(profitTargets, nameof(profitTargets));

            this.TradeProfile = tradeProfile;
            this.OrderSide = orderSide;
            this.EntryPrice = entryPrice;
            this.StopLossPrice = stopLossPrice;
            this.ProfitTargets = profitTargets.ToImmutableDictionary();
            this.ExpireTime = this.CalculateExpireTime();
        }

        /// <summary>
        /// Gets the entry signals trade profile.
        /// </summary>
        public TradeProfile TradeProfile { get; }

        /// <summary>
        /// Gets the entry signals order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the entry signals price.
        /// </summary>
        public Price EntryPrice { get; }

        /// <summary>
        /// Gets the entry signals stop-loss price.
        /// </summary>
        public Price StopLossPrice { get; }

        /// <summary>
        /// Gets the entry signals profit targets.
        /// </summary>
        public IReadOnlyDictionary<int, Price> ProfitTargets { get; }

        /// <summary>
        /// Gets the  entry signals expire time (optional).
        /// </summary>
        public Option<ZonedDateTime?> ExpireTime { get; }

        private Option<ZonedDateTime?> CalculateExpireTime()
        {
            if (this.TradeProfile.BarsValid > 0
             && this.TradeProfile.BarSpecification.TimeFrame != BarTimeFrame.Tick)
            {
                var expireTime = this.SignalTimestamp.Plus(this.TradeProfile.BarSpecification.Duration * this.TradeProfile.BarsValid);

                Validate.True(ZonedDateTime.Comparer.Instant.Compare(expireTime, this.SignalTimestamp) > 0, nameof(this.SignalTimestamp));

                return expireTime;
            }

            return Option<ZonedDateTime?>.None();
        }
    }
}