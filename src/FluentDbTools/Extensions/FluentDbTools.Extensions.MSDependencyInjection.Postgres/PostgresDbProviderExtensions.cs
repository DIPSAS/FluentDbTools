using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FluentDbTools.Extensions.MSDependencyInjection.Postgres
{
    public static class PostgresDbProviderExtensions
    {
        public static IServiceCollection AddPostgresDbProvider(this IServiceCollection serviceCollection)
        {
            NpgsqlFactory.Instance.Register(SupportedDatabaseTypes.Postgres);

            serviceCollection.AddDbProvider();

            return serviceCollection;
        }
    }
}