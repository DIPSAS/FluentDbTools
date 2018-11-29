using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbConfig, DbConfig>()
                .AddScoped<IMigrationConfig, MigrationConfig>();
        }
    }
}