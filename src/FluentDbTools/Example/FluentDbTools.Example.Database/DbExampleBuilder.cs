using System;
using System.Collections.Generic;
using FluentDbTools.Example.Common;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Database
{
    public static class DbExampleBuilder
    {
        public static IServiceProvider BuildDbExample(
            Dictionary<string, string> overrideConfig = null, 
            string additionalJsonConfig = null)
        {
            return new ServiceCollection()
                .Register(ServiceRegistration.Register)
                .UseExampleConfiguration(
                    overrideConfig, 
                    additionalJsonConfig)
                .UseDefaultLogging()
                .BuildServiceProvider();
        }
    }
}