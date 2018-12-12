using DIPS.Extensions.FluentDbTools.MSDependencyInjection;
using DIPS.Extensions.FluentDbTools.MSDependencyInjection.DefaultConfigs;
using DIPS.FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Config
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbConfig, DefaultDbConfig>();
        }
    }
}