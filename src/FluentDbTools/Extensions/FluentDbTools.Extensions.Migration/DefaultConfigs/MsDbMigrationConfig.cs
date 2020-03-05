using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Common.Abstractions.PrioritizedConfig;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using Microsoft.Extensions.Configuration;
using FluentDbTools.Migration.Common;

namespace FluentDbTools.Extensions.Migration.DefaultConfigs
{
    /// <inheritdoc />
    public class MsDbMigrationConfig : IDbMigrationConfig
    {
        private readonly DefaultDbConfigValues Defaults;
        private readonly IPrioritizedConfigValues PrioritizedConfigValues;
        internal readonly IConfiguration Configuration;
        private IDictionary<string, string> AllConfigValuesField;

        /// <inheritdoc />
        public Func<IDbConfig> GetDbConfig { get; }

        /// <summary>
        /// Constructor. <br/>
        /// If <paramref name="dbConfig"/> is not Null, GetDbConfig func will be initialized from <paramref name="dbConfig"/><br/>
        /// If <paramref name="dbConfig"/> is Null, GetDbConfig func will be initialized from <paramref name="configuration"/><br/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dbConfig"></param>
        /// <param name="configurationChangedHandler"></param>
        /// <param name="prioritizedConfigValues"></param>
        /// <param name="prioritizedConfigKeys"></param>
        public MsDbMigrationConfig(
            IConfiguration configuration,
            IDbConfig dbConfig = null,
            IConfigurationChangedHandler configurationChangedHandler = null,
            IPrioritizedConfigValues prioritizedConfigValues = null,
            IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeys = null)
        {
            var prioritizedConfigKeysArray = (prioritizedConfigKeys ?? new[] { new PrioritizedConfigKeys() }).ToArray();
            PrioritizedConfigValues = prioritizedConfigValues ??
                                      new PrioritizedConfigValues(configuration.GetConfigValue, prioritizedConfigKeysArray);

            Configuration = configuration;

            if (dbConfig == null)
            {
                var newConfig = new MsDbConfig(
                    Configuration,
                    configurationChangedHandler,
                    CreateDefaultDbConfigValues(prioritizedConfigKeysArray));

                Defaults = newConfig.Defaults;
                dbConfig = newConfig;
            }

            if (Defaults == null)
            {
                Defaults = (dbConfig as MsDbConfig)?.Defaults ?? CreateDefaultDbConfigValues(prioritizedConfigKeysArray);
            }

            GetDbConfig = () => dbConfig;
            configurationChangedHandler?.RegisterConfigurationChangedCallback(OnConfigurationChanged);
        }

        private string SchemaPasswordField;

        /// <inheritdoc />
        public virtual string SchemaPassword
        {
            get => SchemaPasswordField ??
                        PrioritizedConfigValues.GetDbSchemaPassword()
                       .WithDefault(Configuration.GetMigrationSchemaPassword())
                       .WithDefault(Configuration.GetSecret(Schema))
                       .WithDefault(GetDbConfig().Password.EndsWithIgnoreCase("_APP") == false ? GetDbConfig().Password : Schema);
            set => SchemaPasswordField = value;
        }

        private string TablespaceField;

        /// <inheritdoc />
        public virtual string DefaultTablespace
        {
            get => TablespaceField ?? Configuration.GetMigrationDefaultTablespace();
            set => TablespaceField = value;
        }

        private string TempTablespaceField;

        /// <inheritdoc />
        public virtual string TempTablespace
        {
            get => TempTablespaceField ?? Configuration.GetDbTempTablespace();
            set => TempTablespaceField = value;
        }


        /// <inheritdoc />
        public SupportedDatabaseTypes DbType => GetDbConfig().DbType;

        /// <inheritdoc />
        public string Schema => GetDbConfig().Schema;

        /// <inheritdoc />
        public string DatabaseName => GetDbConfig().DatabaseName;

        /// <inheritdoc />
        public string GetSchemaPrefixId() => PrioritizedConfigValues.GetDbSchemaPrefixIdString() ??
                                             GetAllMigrationConfigValues().GetValue("schemaPrefix:Id") ??
                                             GetDbConfig().GetSchemaPrefixId() ??
                                             Defaults?.GetDefaultSchemaPrefixIdString.Invoke() ?? string.Empty;

        /// <inheritdoc />
        public string DatabaseOwner => Configuration.GetMigrationDatabaseOwner() ?? GetDbConfig().AdminUser;

        /// <inheritdoc />
        public string ConnectionString => GetDbConfig().GetAdminConnectionString();

        /// <inheritdoc />
        public string ProcessorId => DbType.GetProcessorId();

        /// <inheritdoc />
        public IDictionary<string, string> GetAllMigrationConfigValues(bool reload = false)
        {
            if (AllConfigValuesField == null || reload)
            {
                AllConfigValuesField = Configuration.GetMigrationSection().GetDbAllConfigValues();
            }

            return AllConfigValuesField;
        }

        /// <inheritdoc />
        public string GetMigrationName()
        {
            return GetAllMigrationConfigValues().GetValue("migrationName", "name");
        }

        /// <inheritdoc />
        public string GetSchemaPrefixUniqueId()
        {
            return PrioritizedConfigValues.GetDbSchemaUniquePrefixIdString() ??
                   GetAllMigrationConfigValues().GetValue("schemaPrefix:UniqueId") ??
                   GetDbConfig().GetAllDatabaseConfigValues().GetValue("schemaPrefix:UniqueId") ??
                   Defaults?.GetDefaultSchemaPrefixUniqueIdString.Invoke() ?? string.Empty; 

        }

        private void OnConfigurationChanged(Func<string[], string> getValueFunc)
        {
            TempTablespaceField = null;
            TablespaceField = null;
            SchemaPasswordField = null;

            GetAllMigrationConfigValues(true);
        }

        private MsDefaultDbConfigValues CreateDefaultDbConfigValues(IEnumerable<IPrioritizedConfigKeys> prioritizedConfigKeysArray)
        {
            return new MsDefaultDbConfigValues(Configuration, PrioritizedConfigValues, prioritizedConfigKeysArray);
        }

    }
}