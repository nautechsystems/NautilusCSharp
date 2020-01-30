// -------------------------------------------------------------------------------------------------
// <copyright file="BsonInstrumentSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a data serializer for the BSON specification.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    public class BsonInstrumentSerializer : IDataSerializer<Instrument>
    {
        /// <inheritdoc />
        public DataEncoding DataEncoding => DataEncoding.Bson;

        /// <inheritdoc />
        public byte[] Serialize(Instrument instrument)
        {
            var bsonMap = new BsonDocument
            {
                { nameof(Instrument.Symbol), instrument.Symbol.Value },
                { nameof(Instrument.BrokerSymbol), instrument.BrokerSymbol.Value },
                { nameof(Instrument.QuoteCurrency), instrument.QuoteCurrency.ToString() },
                { nameof(Instrument.SecurityType), instrument.SecurityType.ToString() },
                { nameof(Instrument.PricePrecision), instrument.PricePrecision },
                { nameof(Instrument.SizePrecision), instrument.SizePrecision },
                { nameof(Instrument.MinStopDistanceEntry), instrument.MinStopDistanceEntry },
                { nameof(Instrument.MinStopDistance), instrument.MinStopDistance },
                { nameof(Instrument.MinLimitDistanceEntry), instrument.MinLimitDistanceEntry },
                { nameof(Instrument.MinLimitDistance), instrument.MinLimitDistance },
                { nameof(Instrument.TickSize), instrument.TickSize.ToString() },
                { nameof(Instrument.RoundLotSize), instrument.RoundLotSize.ToString() },
                { nameof(Instrument.MinTradeSize), instrument.MinTradeSize.ToString() },
                { nameof(Instrument.MaxTradeSize), instrument.MaxTradeSize.ToString() },
                { nameof(Instrument.RolloverInterestBuy), instrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.RolloverInterestSell), instrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture) },
                { nameof(Instrument.Timestamp), instrument.Timestamp.ToIsoString() },
            }.ToDictionary();

            if (instrument is ForexInstrument forexCcy)
            {
                bsonMap.Add(nameof(ForexInstrument.BaseCurrency), forexCcy.BaseCurrency.ToString());
            }

            return bsonMap.ToBson();
        }

        /// <inheritdoc />
        public Instrument Deserialize(byte[] serialized)
        {
            Debug.NotEmpty(serialized, nameof(serialized));

            var unpacked = BsonSerializer.Deserialize<BsonDocument>(serialized);

            var securityType = unpacked[nameof(Instrument.SecurityType)].AsString.ToEnum<SecurityType>();
            if (securityType == SecurityType.Forex)
            {
                return new ForexInstrument(
                    Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
                    new BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)].AsString),
                    unpacked[nameof(Instrument.PricePrecision)].AsInt32,
                    unpacked[nameof(Instrument.SizePrecision)].AsInt32,
                    unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32,
                    unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32,
                    unpacked[nameof(Instrument.MinStopDistance)].AsInt32,
                    unpacked[nameof(Instrument.MinLimitDistance)].AsInt32,
                    Price.Create(unpacked[nameof(Instrument.TickSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.RoundLotSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.MinTradeSize)].AsString),
                    Quantity.Create(unpacked[nameof(Instrument.MaxTradeSize)].AsString),
                    Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                    Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                    unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
            }

            return new Instrument(
                Symbol.FromString(unpacked[nameof(Instrument.Symbol)].AsString),
                new BrokerSymbol(unpacked[nameof(Instrument.BrokerSymbol)].AsString),
                unpacked[nameof(Instrument.QuoteCurrency)].AsString.ToEnum<Currency>(),
                securityType,
                unpacked[nameof(Instrument.PricePrecision)].AsInt32,
                unpacked[nameof(Instrument.SizePrecision)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistanceEntry)].AsInt32,
                unpacked[nameof(Instrument.MinStopDistance)].AsInt32,
                unpacked[nameof(Instrument.MinLimitDistance)].AsInt32,
                Price.Create(unpacked[nameof(Instrument.TickSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.RoundLotSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.MinTradeSize)].AsString),
                Quantity.Create(unpacked[nameof(Instrument.MaxTradeSize)].AsString),
                Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestBuy)].AsString),
                Convert.ToDecimal(unpacked[nameof(Instrument.RolloverInterestSell)].AsString),
                unpacked[nameof(Instrument.Timestamp)].AsString.ToZonedDateTimeFromIso());
        }
    }
}
