using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbConfig, DefaultDbConfig>()
                .AddScoped<IDbProvider, DbProvider>()
                .AddScoped<IPersonRepository, PersonRepository>();
        }
    }
}