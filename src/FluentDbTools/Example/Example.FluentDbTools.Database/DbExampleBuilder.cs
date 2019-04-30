using System;
using System.Collections.Generic;
using Example.FluentDbTools.Config;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Example.FluentDbTools.Database
{
    public static class DbExampleBuilder
    {
        public static IServiceProvider BuildDbExample(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            var services = new ServiceCollection();
            ServiceRegistration.Register(services);
            return services
                .UseExampleConfiguration(
                    databaseType,
                    overrideConfig)                
                .AddLogging(configure => configure
                    .AddConsole())
                .BuildServiceProvider();
        }
    }
}