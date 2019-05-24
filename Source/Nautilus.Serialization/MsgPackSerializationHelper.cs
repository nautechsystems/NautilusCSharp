// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackSerializationHelper.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides helper methods for objects to serialize.
    /// </summary>
    internal static class MsgPackSerializationHelper
    {
        private const string NONE = nameof(NONE);

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="symbolString">The symbol string.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Symbol GetSymbol(string symbolString)
        {
            Debug.NotEmptyOrWhiteSpace(symbolString, nameof(symbolString));

            var splitSymbol = symbolString.Split('.');
            return new Symbol(splitSymbol[0], splitSymbol[1].ToEnum<Venue>());
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="priceString">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Price GetPrice(string priceString)
        {
            Debug.NotEmptyOrWhiteSpace(priceString, nameof(priceString));

            return Price.Create(Convert.ToDecimal(priceString));
        }

        /// <summary>
        /// Return a string from the given price.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns>The optional price.</returns>
        internal static string GetPriceString(Price? price)
        {
            return price is null
                ? NONE
                : price.ToString();
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="expireTimeString">The expire time string.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static ZonedDateTime? GetExpireTime(string expireTimeString)
        {
            Debug.NotEmptyOrWhiteSpace(expireTimeString, nameof(expireTimeString));

            return expireTimeString == NONE
                ? null
                : (ZonedDateTime?)expireTimeString.ToZonedDateTimeFromIso();
        }

        /// <summary>
        /// Returns the date time ISO 8601 string from the given expire time.
        /// </summary>
        /// <param name="expireTime">The optional expire time.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static string GetExpireTimeString(ZonedDateTime? expireTime)
        {
            return expireTime is null
                ? NONE
                : expireTime.Value.ToIsoString();
        }
    }
}