﻿//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Data.Aggregators;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class InstrumentBuilderTests
    {
        private readonly ITestOutputHelper output;

        public InstrumentBuilderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Build_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder.Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder
               .Update(audusd)
               .Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithChanges_ReturnsNewInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            var instrument = new Instrument(
                new Symbol("AUDUSD", Venue.FXCM),
                new InstrumentId("NONE"),
                new BrokerSymbol("NONE"),
                CurrencyCode.CAD,
                SecurityType.Bond,
                2,
                0.25m,
                10m,
                3,
                1000,
                1,
                1,
                1,
                1,
                10,
                10000,
                90,
                45,
                45,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = instrumentBuilder
               .Update(instrument)
               .Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(12, instrumentBuilder.Changes.Count);

            foreach (var change in instrumentBuilder.Changes)
            {
                this.output.WriteLine(change);
            }
        }
    }
}