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
        /// <param name="prioritizedConfigValues"></param>
        public MsDbConfigCredentials(
            IConfiguration configuration, 
            DefaultDbConfigValues defaultDbConfigValues = null,
            IConfigurationChangedHandler configurationChangedHandler = null,
            IPrioritizedConfigValues prioritizedConfigValues = null)
            : base(defaultDbConfigValues ?? new MsDefaultDbConfigValues(configuration, prioritizedConfigValues))
        {
            IgnoreManualCallOnConfigurationChanged = true;
            configurationChangedHandler?.RegisterConfigurationChangedCallback(OnConfigurationChanged);
        }
    }
}