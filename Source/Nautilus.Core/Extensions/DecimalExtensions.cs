﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides useful generic <see cref="decimal"/> extension methods.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Returns the number of decimal places of this <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The number of decimal places.</returns>
        public static int GetDecimalPlaces(this decimal value)
        {
            return BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        }

        /// <summary>
        /// Returns the decimal tick size from the given <see cref="int"/>.
        /// </summary>
        /// <param name="decimals">The value.</param>
        /// <returns>A decimal representation of the tick size.</returns>
        public static decimal ToTickSize(this int decimals)
        {
            Condition.NotNegativeInt32(decimals, nameof(decimals));

            decimal divisor = 1;

            for (var i = 0; i < decimals; i++)
            {
                divisor *= 10;
            }

            return 1 / divisor;
        }
    }
}
