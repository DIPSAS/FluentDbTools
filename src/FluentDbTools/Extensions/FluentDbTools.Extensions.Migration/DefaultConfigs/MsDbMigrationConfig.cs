using System;
using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Contracts;
using FluentDbTools.Extensions.DbProvider;
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
        private readonly IConfiguration Configuration;
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
        public MsDbMigrationConfig(IConfiguration configuration, IDbConfig dbConfig = null)
        {
            Configuration = configuration;

            if (dbConfig == null)
            {
                var newConfig = new MsDbConfig(Configuration);
                Defaults = newConfig.Defaults;
                dbConfig = newConfig;
            }

            if (Defaults == null)
            {
                Defaults = (dbConfig as MsDbConfig)?.Defaults ?? new MsDefaultDbConfigValues(configuration);
            }
            
            GetDbConfig = () => dbConfig;
        }

        private string SchemaPasswordField;

        /// <inheritdoc />
        public virtual string SchemaPassword
        {
            get => SchemaPasswordField ?? Configuration.GetMigrationSchemaPassword() ?? GetDbConfig().Password;
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
        public string GetSchemaPrefixId() => GetAllMigrationConfigValues().GetValue("schemaPrefix:Id") ?? Defaults?.GetDefaultSchemaPrefixIdString.Invoke() ?? string.Empty;

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
        public string GetSchemaPrefixUniqueId()
        {
            return GetAllMigrationConfigValues().GetValue("schemaPrefix:UniqueId");
        }
    }
}