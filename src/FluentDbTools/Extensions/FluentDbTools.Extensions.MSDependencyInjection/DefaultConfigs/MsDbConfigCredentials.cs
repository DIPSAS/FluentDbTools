using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using Microsoft.Extensions.Configuration;

namespace FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs
{
    /// <summary>
    /// Overrides properties in <see cref="DbConfigCredentials"/> with default functions from <see cref="MsDefaultDbConfigValues"/>
    /// </summary>
    internal class MsDbConfigCredentials : DbConfigCredentials
    {
        /// <summary>
        /// Overrides properties in <see cref="DbConfigCredentials"/> with default functions from <see cref="MsDefaultDbConfigValues"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="defaultDbConfigValues"></param>
        /// <param name="configurationChangedHandler"></param>
        public MsDbConfigCredentials(
            IConfiguration configuration, 
            DefaultDbConfigValues defaultDbConfigValues = null,
            IConfigurationChangedHandler configurationChangedHandler = null)
            : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration))
        {
            IgnoreManualCallOnConfigurationChanged = true;
            configurationChangedHandler?.RegisterConfigurationChangedCallback(OnConfigurationChanged);
        }
    }
}