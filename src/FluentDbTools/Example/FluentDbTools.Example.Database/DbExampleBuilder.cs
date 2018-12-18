using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Common;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Database
{
    public static class DbExampleBuilder
    {
        public static IServiceProvider BuildDbExample(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            return new ServiceCollection()
                .Register(ServiceRegistration.Register)
                .UseExampleConfiguration(
                    databaseType,
                    overrideConfig)
                .UseDefaultLogging()
                .BuildServiceProvider();
        }
    }
}