using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using Microsoft.Extensions.DependencyInjection;
#pragma warning disable CS1591

namespace Example.FluentDbTools.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddDbProvider(serviceCollection.GetDependecyInjectionDbConfigType())
                .AddScoped<IPersonRepository, PersonRepository>()
                .AddOracleDbProvider()
                .AddPostgresDbProvider();
        }
    }
}