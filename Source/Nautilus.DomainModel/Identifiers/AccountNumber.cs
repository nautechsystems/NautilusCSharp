//--------------------------------------------------------------------------------------------------
// <copyright file="AccountNumber.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a valid account number.
    /// </summary>
    [Immutable]
    public sealed class AccountNumber : Identifier<AccountNumber>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountNumber"/> class.
        /// </summary>
        /// <param name="value">The identifier value.</param>
        public AccountNumber(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
