﻿//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageRouter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using Nautilus.Fix;

namespace Nautilus.Fxcm
{
    using System;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Fxcm.MessageFactories;
    using QuickFix;

    /// <summary>
    /// Provides a router for FXCM FIX messages.
    /// </summary>
    public sealed class FxcmFixMessageRouter : Component, IFixMessageRouter
    {
        private const string Sent = "-->";
        private const string Protocol = "[FIX]";

        private readonly AccountId accountId;
        private readonly SymbolMapper symbolMapper;

        private Session? session;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageRouter"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the router.</param>
        /// <param name="symbolMapper">The symbol mapper.</param>
        public FxcmFixMessageRouter(
            IComponentryContainer container,
            AccountId accountId,
            SymbolMapper symbolMapper)
        : base(container)
        {
            this.accountId = accountId;
            this.symbolMapper = symbolMapper;
        }

        /// <inheritdoc />
        public void InitializeSession(Session newSession)
        {
            this.session = newSession;
        }

        /// <inheritdoc />
        public void CollateralInquiry()
        {
            this.SendFixMessage(CollateralInquiryFactory.Create(this.TimeNow()));
        }

        /// <inheritdoc />
        public void TradingSessionStatusRequest()
        {
            this.SendFixMessage(TradingSessionStatusRequestFactory.Create(this.TimeNow()));
        }

        /// <inheritdoc />
        public void RequestForOpenPositionsSubscribe()
        {
            this.SendFixMessage(RequestForPositionsFactory.OpenAll(this.TimeNow(), this.accountId.AccountNumber));
        }

        /// <inheritdoc />
        public void RequestForClosedPositionsSubscribe()
        {
            this.SendFixMessage(RequestForPositionsFactory.ClosedAll(this.TimeNow(), this.accountId.AccountNumber));
        }

        /// <inheritdoc />
        public void SecurityListRequestSubscribe(Symbol symbol)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {symbol.Code}.");
                return;
            }

            var message = SecurityListRequestFactory.Create(brokerSymbolCode, this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void SecurityListRequestSubscribeAll()
        {
            var message = SecurityListRequestFactory.Create(this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void MarketDataRequestSubscribe(Symbol symbol)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {symbol.Code}.");
                return;
            }

            var message = MarketDataRequestFactory.Create(
                brokerSymbolCode,
                0,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void MarketDataRequestSubscribeAll()
        {
            foreach (var brokerSymbol in this.symbolMapper.BrokerSymbolCodes)
            {
                var message = MarketDataRequestFactory.Create(
                    brokerSymbol,
                    0,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
        }

        /// <inheritdoc />
        public void NewOrderSingle(Order order, PositionIdBroker? positionIdBroker)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = NewOrderSingleFactory.Create(
                brokerSymbolCode,
                this.accountId.AccountNumber,
                order,
                positionIdBroker,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void NewOrderList(AtomicOrder atomicOrder)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(atomicOrder.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {atomicOrder.Symbol.Code}.");
                return;
            }

            if (atomicOrder.TakeProfit != null)
            {
                var message = NewOrderListEntryFactory.CreateWithStopLossAndTakeProfit(
                    brokerSymbolCode,
                    this.accountId.AccountNumber,
                    atomicOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
            else
            {
                var message = NewOrderListEntryFactory.CreateWithStopLoss(
                    brokerSymbolCode,
                    this.accountId.AccountNumber,
                    atomicOrder,
                    this.TimeNow());

                this.SendFixMessage(message);
            }
        }

        /// <inheritdoc />
        public void OrderCancelReplaceRequest(Order order, Quantity modifiedQuantity, Price modifiedPrice)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = OrderCancelReplaceRequestFactory.Create(
                brokerSymbolCode,
                order,
                modifiedQuantity.Value,
                modifiedPrice.Value,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        /// <inheritdoc />
        public void OrderCancelRequest(Order order)
        {
            var brokerSymbolCode = this.symbolMapper.GetBrokerCode(order.Symbol.Code);
            if (brokerSymbolCode is null)
            {
                this.Logger.LogError($"Cannot find broker symbol for {order.Symbol.Code}.");
                return;
            }

            var message = OrderCancelRequestFactory.Create(
                brokerSymbolCode,
                order,
                this.TimeNow());

            this.SendFixMessage(message);
        }

        private void SendFixMessage(Message message)
        {
            if (this.session is null)
            {
                this.Logger.LogError("Cannot send FIX message (the session is null).");
                return;
            }

            try
            {
                this.session.Send(message);
                this.LogMessageSent(message);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(LogId.Networking, ex.Message, ex);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void LogMessageSent(Message message)
        {
            this.Logger.LogDebug($"{Protocol}{Sent} {message.GetType().Name}");
        }
    }
}
