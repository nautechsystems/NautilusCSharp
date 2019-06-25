﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Money.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Primitives;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents the concept of money.
    /// </summary>
    [Immutable]
    public sealed class Money : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Money"/> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        private Money(decimal amount, Currency currency)
            : base(amount)
        {
            Debug.NotNegativeDecimal(amount, nameof(amount));
            Debug.True(amount % 0.01m == 0, nameof(amount));
            Debug.NotDefault(currency, nameof(currency));

            this.Currency = currency;
        }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Money"/> objects are equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(Money left, Money right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value indicating whether the <see cref="Money"/> objects are not equal.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator !=(Money left, Money right) => !(left == right);

        /// <summary>
        /// Returns a new <see cref="Money"/> object with a value of zero.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public static Money Zero(Currency currency)
        {
            return new Money(decimal.Zero, currency);
        }

        /// <summary>
        /// Returns a new <see cref="Money"/> object with the given amount, of the given currency.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="currency">The currency.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public static Money Create(decimal amount, Currency currency)
        {
            return new Money(amount, currency);
        }

        /// <summary>
        /// Adds the given money to this money.
        /// </summary>
        /// <param name="other">The amount.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money Add(Money other)
        {
            return new Money(this.Value + other.Value, this.Currency);
        }

        /// <summary>
        /// Subtracts the given money from this money.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money Subtract(Money other)
        {
            return new Money(this.Value - other.Value, this.Currency);
        }

        /// <summary>
        /// Multiplies this money by the given multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money MultiplyBy(int multiplier)
        {
            return new Money(this.Value * multiplier, this.Currency);
        }

        /// <summary>
        /// Divides this money by the given divisor.
        /// </summary>
        /// <param name="divisor">The divisor.</param>
        /// <returns>A new <see cref="Money"/> object.</returns>
        public Money DivideBy(int divisor)
        {
            Debug.PositiveInt32(divisor, nameof(divisor));

            return new Money(this.Value / divisor, this.Currency);
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="Money"/> is equal to the given <see cref="object"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object other) => other != null && this.Equals(other);

        /// <summary>
        /// Returns a value indicating whether this <see cref="Money"/> is equal to the given <see cref="Money"/>.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>The result of the equality check.</returns>
        public bool Equals(Money other) => this.Value.Equals(other.Value) && this.Currency.Equals(other.Currency);

        /// <summary>
        /// Returns the hash code for this <see cref="DecimalNumber"/>.
        /// </summary>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public override int GetHashCode() => Hash.GetCode(this.Value, this.Currency);

        /// <summary>
        /// Returns a string representation of the <see cref="Money"/> object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{this.Value:N2}({this.Currency})";
        }
    }
}
