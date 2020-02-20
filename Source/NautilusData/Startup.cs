﻿//--------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusData
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nautilus.Data;

    /// <summary>
    /// The main ASP.NET Core Startup class to configure and build the web hosting services.
    /// </summary>
    public sealed class Startup
    {
        private readonly IHostingEnvironment env;
        private readonly IConfiguration config;
        private readonly ILoggerFactory loggerFactory;
        private readonly DataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public Startup(
            IHostingEnvironment env,
            IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            this.env = env;
            this.config = config;
            this.loggerFactory = loggerFactory;

            var dataServiceConfig = DataServiceConfigurator.Build(loggerFactory, config);

            this.dataService = DataServiceFactory.Create(dataServiceConfig);
            this.dataService.Start();
        }

        /// <summary>
        /// Configures the ASP.NET Core web hosting services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        /// Configures the ASP.NET Core web request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="appLifetime">The application lifetime.</param>
        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopping.Register(this.OnShutdown);

            if (this.env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

        private void OnShutdown()
        {
            this.dataService.Stop();

            Task.Delay(2000).Wait();
        }
    }
}
