//--------------------------------------------------------------------------------------------------
// <copyright file="FixConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents the configuration for a FIX session.
    /// </summary>
    [Immutable]
    public sealed class FixConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixConfiguration"/> class.
        /// </summary>
        /// <param name="broker">The FIX brokerage name.</param>
        /// <param name="accountType">The FIX account type.</param>
        /// <param name="accountCurrency">The FIX account currency.</param>
        /// <param name="configPath">The FIX configuration file path.</param>
        /// <param name="credentials">The FIX credentials.</param>
        /// <param name="sendAccountTag">The option flag to send account tags with messages.</param>
        /// <param name="connectTime">The time to connect FIX sessions.</param>
        /// <param name="disconnectTime">The time to disconnect FIX sessions.</param>
        public FixConfiguration(
            Brokerage broker,
            AccountType accountType,
            Currency accountCurrency,
            string configPath,
            FixCredentials credentials,
            bool sendAccountTag,
            (IsoDayOfWeek, LocalTime) connectTime,
            (IsoDayOfWeek, LocalTime) disconnectTime)
        {
            Condition.NotEmptyOrWhiteSpace(configPath, nameof(configPath));

            this.AccountId = new AccountId(broker, credentials.AccountNumber, accountType);
            this.Broker = broker;
            this.AccountType = accountType;
            this.AccountCurrency = accountCurrency;
            this.ConfigPath = configPath;
            this.Credentials = credentials;
            this.SendAccountTag = sendAccountTag;
            this.ConnectTime = connectTime;
            this.DisconnectTime = disconnectTime;
        }

        /// <summary>
        /// Gets the FIX configuration account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the FIX brokerage name.
        /// </summary>
        public Brokerage Broker { get; }

        /// <summary>
        /// Gets the FIX account type.
        /// </summary>
        public AccountType AccountType { get; }

        /// <summary>
        /// Gets the FIX account currency.
        /// </summary>
        public Currency AccountCurrency { get; }

        /// <summary>
        /// Gets the FIX configuration file path.
        /// </summary>
        public string ConfigPath { get; }

        /// <summary>
        /// Gets the FIX account credentials.
        /// </summary>
        public FixCredentials Credentials { get; }

        /// <summary>
        /// Gets a value indicating whether the Account tag should be sent with FIX messages.
        /// </summary>
        public bool SendAccountTag { get; }

        /// <summary>
        /// Gets the day of week and time for connecting FIX sessions.
        /// </summary>
        public (IsoDayOfWeek, LocalTime) ConnectTime { get; }

        /// <summary>
        /// Gets the day of week and time for disconnecting FIX sessions.
        /// </summary>
        public (IsoDayOfWeek, LocalTime) DisconnectTime { get; }
    }
}
