using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace DIPS.Extensions.FluentDbTools.Common
{
    public static class ServiceCollectionConfigurationExtensions
    {      
        public static IConfigurationBuilder AddJsonFileIfTrue(this IConfigurationBuilder configurationBuilder,
            string path,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddJsonFile(path);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddInMemoryIfTrue(this IConfigurationBuilder configurationBuilder,
            Dictionary<string, string> dict,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddInMemoryCollection(dict);
            }
            return configurationBuilder;
        }
        
        public static IConfigurationBuilder AddConfigurationIfTrue(this IConfigurationBuilder configurationBuilder,
            IConfiguration configuration,
            Func<bool> func)
        {
            if (func.Invoke())
            {
                configurationBuilder.AddConfiguration(configuration);
            }
            return configurationBuilder;
        }
    }
}