using DIPS.Extensions.FluentDbTools.MSDependencyInjection.DefaultConfigs;
using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Database
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