using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.DbProvider.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.DbProvider
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbProvider, DbProvider>();
        }
    }
}