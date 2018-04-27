﻿// -------------------------------------------------------------------------------------------------
// <copyright file="Position.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Aggregates
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.CQS;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="Position"/> class. Represents a financial market position.
    /// </summary>
    public sealed class Position : Aggregate<Position>
    {
        private int relativeQuantity;

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> class.
        /// </summary>
        /// <param name="symbol">The position symbol.</param>
        /// <param name="fromEntryOrderId">The position entry order identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="timestamp">The position timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Position(
            Symbol symbol,
            EntityId fromEntryOrderId,
            EntityId positionId,
            ZonedDateTime timestamp)
            : base(
                  positionId,
                  timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(fromEntryOrderId, nameof(fromEntryOrderId));
            Validate.NotNull(positionId, nameof(positionId));
            Validate.NotEqualTo(timestamp, nameof(timestamp), default(ZonedDateTime));

            this.Symbol = symbol;
            this.FromEntryOrderId = fromEntryOrderId;
            this.EntryTime = Option<ZonedDateTime?>.None();
            this.AverageEntryPrice = Price.Zero();
        }

        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the positions entry order identifier.
        /// </summary>
        public EntityId FromEntryOrderId { get; }

        /// <summary>
        /// Gets the positions quantity.
        /// </summary>
        public Quantity Quantity => Quantity.Create(Math.Abs(this.relativeQuantity));

        /// <summary>
        /// Returns the positions market position.
        /// </summary>
        public MarketPosition MarketPosition => this.GetMarketPosition();

        /// <summary>
        /// Gets the positions entry time (optional).
        /// </summary>
        public Option<ZonedDateTime?> EntryTime { get; private set; }

        /// <summary>
        /// Gets the positions average entry price.
        /// </summary>
        public Price AverageEntryPrice { get; private set; }

        /// <summary>
        /// Returns the positions timestamp.
        /// </summary>
        public ZonedDateTime PositionTimestamp => this.EntityTimestamp;

        /// <summary>
        /// Applies the given <see cref="Event"/> to this position.
        /// </summary>
        /// <param name="event">The position event.</param>
        /// <returns>A <see cref="CommandResult"/> result.</returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed.")]
        public override CommandResult Apply(Event @event)
        {
            Validate.NotNull(@event, nameof(@event));

            switch (@event)
            {
                case OrderFilled orderFilled:
                    return this.When(orderFilled);

                case OrderPartiallyFilled orderPartiallyFilled:
                    return this.When(orderPartiallyFilled);

                default: return CommandResult.Fail(
                    $"The event is not recognized by the position {this}");
            }
        }

        private CommandResult When(OrderFilled positionEvent)
        {
            Debug.NotNull(positionEvent, nameof(positionEvent));

            this.UpdatePosition(
                positionEvent.OrderSide,
                positionEvent.FilledQuantity.Value,
                positionEvent.AveragePrice,
                positionEvent.ExecutionTime);
            this.Events.Add(positionEvent);
            return CommandResult.Ok();
        }

        private CommandResult When(OrderPartiallyFilled positionEvent)
        {
            Debug.NotNull(positionEvent, nameof(positionEvent));

            this.UpdatePosition(
                positionEvent.OrderSide,
                positionEvent.FilledQuantity.Value,
                positionEvent.AveragePrice,
                positionEvent.ExecutionTime);
            this.Events.Add(positionEvent);
            return CommandResult.Ok();
        }

        private void UpdatePosition(
            OrderSide orderSide,
            int quantity,
            Price averagePrice,
            ZonedDateTime eventTime)
        {
            if (orderSide == OrderSide.Buy)
            {
                this.relativeQuantity += quantity;
            }

            if (orderSide == OrderSide.Sell)
            {
                this.relativeQuantity -= quantity;
            }

            if (this.EntryTime.HasNoValue)
            {
                this.EntryTime = Option<ZonedDateTime?>.Some(eventTime);
            }

            this.AverageEntryPrice = averagePrice;
        }

        private MarketPosition GetMarketPosition()
        {
            if (this.relativeQuantity > 0)
            {
                return MarketPosition.Long;
            }

            if (this.relativeQuantity < 0)
            {
                return MarketPosition.Short;
            }

            return MarketPosition.Flat;
        }
    }
}