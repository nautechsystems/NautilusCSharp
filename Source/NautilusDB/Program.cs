﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusDB
{
    using System.Diagnostics.CodeAnalysis;
    using global::Serilog;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Nautilus.Common.Enums;
    using Nautilus.Serilog;
    using Serilog.Events;

    /// <summary>
    /// The main entry point for the program.
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Reviewed. Program is never instantiated.")]
    public class Program
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        public static void Main(string[] args)
        {
            var logger = new SerilogLogger(LogEventLevel.Information);
            logger.Information(NautilusService.AspCoreHost, "Building ASP.NET Core Web Host...");

            BuildWebHost(args).Run();

            logger.Information(NautilusService.AspCoreHost, "Closing and flushing Serilog...");
            Log.CloseAndFlush();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
