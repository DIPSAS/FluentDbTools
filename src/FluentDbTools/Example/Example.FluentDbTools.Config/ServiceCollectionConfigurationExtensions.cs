using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Example.FluentDbTools.Config
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
    }
}