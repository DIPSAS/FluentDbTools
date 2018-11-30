using System;
using System.Collections.Generic;
using DIPS.FluentDbTools.Example.Common;
using DIPS.Extensions.FluentDbTools.MSDependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Database
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