using System.Data;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
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
                .AddScoped<IDbConnection>(sp =>
                {
                    var dbConfig = sp.GetService<IDbConfig>();
                    var dbConnection = useDbProviderFactory ? 
                        dbConfig.GetDbProviderFactory().CreateConnection() : 
                        dbConfig.CreateDbConnection();
                    dbConnection.Open();
                    return dbConnection;
                });
        }
    }
}