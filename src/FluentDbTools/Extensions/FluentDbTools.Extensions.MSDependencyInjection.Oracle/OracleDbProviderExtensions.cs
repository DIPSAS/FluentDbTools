using System.Data;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
#pragma warning disable 1591

namespace FluentDbTools.Extensions.MSDependencyInjection.Oracle
{
    public static class OracleDbProviderExtensions
    {
        public static IServiceCollection AddOracleDbProvider(this IServiceCollection serviceCollection)
        {
            OracleClientFactory.Instance.Register(SupportedDatabaseTypes.Oracle);

            serviceCollection.AddDbProvider();

            return serviceCollection;
        }
    }
}

namespace System.Data
{
}