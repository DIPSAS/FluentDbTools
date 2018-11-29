using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IDbProvider, DbProvider>()
                .AddScoped<IPersonRepository, PersonRepository>();
        }
    }
}