using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Extensions.MSDependencyInjection;
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
                .AddDbProvider<MsDbConfig>()
                .AddScoped<IPersonRepository, PersonRepository>()
                .AddOracleDbProvider()
                .AddPostgresDbProvider();
        }
    }
}