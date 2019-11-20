using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs.NotInUse
{
    /// <inheritdoc />
    internal class MsDbConnectionStringBuilderConfig : DbConnectionStringBuilderConfig
    {
        /// <inheritdoc />
        public MsDbConnectionStringBuilderConfig(
            IConfiguration configuration,
            DefaultDbConfigValues defaultDbConfigValues = null,
            DbConfigCredentials dbConfigCredentials = null)
           : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration), dbConfigCredentials) 
        {

        }
    }
}