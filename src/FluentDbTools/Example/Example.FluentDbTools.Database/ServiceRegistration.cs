using System.Data;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace Example.FluentDbTools.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection, bool useDbProviderFactory)
        {
            return serviceCollection
                .AddScoped<IDbConfig, MSDbConfig>()
                .AddScoped<IPersonRepository, PersonRepository>()
                .AddOracleDbProvider()
                .AddPostgresDbProvider();
        }
    }
}