﻿// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarRepository.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Linq;
    using Nautilus.Core.CQS;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;
    using Nautilus.Database.Types;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a repository for persisting <see cref="Bar"/> objects into Redis.
    /// </summary>
    public sealed class RedisBarRepository : IBarRepository
    {
        private readonly RedisBarClient barClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarRepository"/> class.
        /// </summary>
        /// <param name="clientsManager">The clients manager.</param>
        /// <param name="compressor">The data compressor.</param>
        public RedisBarRepository(IRedisClientsManager clientsManager, IDataCompressor compressor)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));
            Validate.NotNull(compressor, nameof(compressor));

            this.barClient = new RedisBarClient(clientsManager, compressor);
        }

//        /// <summary>
//        /// Warning: Flushes ALL data from the <see cref="Redis"/> database.
//        /// </summary>
//        /// <param name="areYouSure">The are you sure string.</param>
//        /// <returns>A <see cref="CommandResult"/> result.</returns>
//        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
//        public CommandResult FlushAll(string areYouSure)
//        {
//            Debug.NotNull(areYouSure, nameof(areYouSure));
//
//            if (areYouSure == "YES")
//            {
//                this.barClient.FlushAll("YES");
//
//                return CommandResult.Ok();
//            }
//
//            return CommandResult.Fail("Database Flush not confirmed");
//        }

        /// <summary>
        /// Returns the total count of bars persisted within the database.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public long AllBarsCount()
        {
            return this.barClient.AllBarsCount();
        }

        /// <summary>
        /// Returns the count of bars persisted within the database with the given
        /// <see cref="BarSpecification"/>.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <returns>A <see cref="int"/>.</returns>
        public long BarsCount(BarType barType)
        {
            Debug.NotNull(barType, nameof(barType));

            return this.barClient.BarsCount(barType);
        }

        /// <summary>
        /// Adds the given bar to the repository.
        /// </summary>
        /// <param name="barType">The bar type to add.</param>
        /// <param name="bar">The bar to add.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public CommandResult Add(BarType barType, Bar bar)
        {
            Debug.NotNull(barType, nameof(barType));
            Debug.NotNull(bar, nameof(bar));

            return this.barClient.AddBar(barType, bar);
        }

        /// <summary>
        /// Adds the given bar(s) to the repository.
        /// </summary>
        /// <param name="barData">The data data to add.</param>
        /// <returns>A <see cref="CommandResult"/>.</returns>
        public CommandResult Add(BarDataFrame barData)
        {
            Debug.NotNull(barData, nameof(barData));

            return this.barClient.AddBars(barData.BarType, barData.Bars);
        }

        /// <summary>
        /// Returns a market data frame populated with the given bar info specification.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>A <see cref="QueryResult{MarketDataFrame}"/>.</returns>
        public QueryResult<BarDataFrame> Find(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            var barsQuery = this.barClient.GetBars(barType, fromDateTime, toDateTime);

            return barsQuery.IsSuccess
                 ? QueryResult<BarDataFrame>.Ok(barsQuery.Value)
                 : QueryResult<BarDataFrame>.Fail(barsQuery.Message);
        }

        /// <summary>
        /// Finds and returns all bars matching the given bar type.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <returns>The query result of bars.</returns>
        public QueryResult<BarDataFrame> FindAll(BarType barType)
        {
            var barsQuery = this.barClient.GetAllBars(barType);

            return barsQuery.IsSuccess
                ? QueryResult<BarDataFrame>.Ok(barsQuery.Value)
                : QueryResult<BarDataFrame>.Fail(barsQuery.Message);
        }

        /// <summary>
        /// Returns a query result containing the <see cref="ZonedDateTime"/> timestamp of the last
        /// bar within <see cref="Redis"/> for the given <see cref="BarSpecification"/> (if successful).
        /// </summary>
        /// <param name="barType">The requested bar type.</param>
        /// <returns>A <see cref="QueryResult{T}"/> containing the <see cref="Bar"/>.</returns>
        public QueryResult<ZonedDateTime> LastBarTimestamp(BarType barType)
        {
            Debug.NotNull(barType, nameof(barType));

            var barKeysQuery = this.barClient.GetAllSortedKeys(barType);

            if (barKeysQuery.IsFailure)
            {
                return QueryResult<ZonedDateTime>.Fail(barKeysQuery.Message);
            }

            var lastKey = barKeysQuery.Value.Last();

            var barsQuery = this.barClient.GetBarsByDay(lastKey);

            return barsQuery.IsSuccess
                ? QueryResult<ZonedDateTime>.Ok(barsQuery.Value.Last().Timestamp)
                : QueryResult<ZonedDateTime>.Fail(barsQuery.Message);
        }
    }
}
