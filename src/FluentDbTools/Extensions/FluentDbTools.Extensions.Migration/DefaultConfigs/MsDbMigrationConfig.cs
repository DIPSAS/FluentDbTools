using System;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Migration.Abstractions;
using Microsoft.Extensions.Configuration;
using FluentDbTools.Migration.Common;

namespace FluentDbTools.Extensions.Migration.DefaultConfigs
{
    public class MsDbMigrationConfig : IDbMigrationConfig
    {
        private readonly IConfiguration Configuration;

        public Func<IDbConfig> GetDbConfig { get; }

        public MsDbMigrationConfig(IConfiguration configuration, IDbConfig dbConfig = null)
        {
            Configuration = configuration;

            GetDbConfig = () => dbConfig ?? new MsDbConfig(Configuration);
        }

        private string SchemaPasswordField;

        public virtual string SchemaPassword
        {
            get => SchemaPasswordField ?? Configuration.GetMigrationSchemaPassword() ?? GetDbConfig().Password;
            set => SchemaPasswordField = value;
        }

        private string TablespaceField;
        public virtual string DefaultTablespace
        {
            get => TablespaceField ?? Configuration.GetMigrationDefaultTablespace();
            set => TablespaceField = value;
        }

        private string TempTablespaceField;
        public virtual string TempTablespace
        {
            get => TempTablespaceField ?? Configuration.GetDbTempTablespace();
            set => TempTablespaceField = value;
        }

        public SupportedDatabaseTypes DbType => GetDbConfig().DbType;
        public string Schema => GetDbConfig().Schema;

        public string DatabaseName => GetDbConfig().DatabaseName;

        public string DatabaseOwner => Configuration.GetMigrationDatabaseOwner() ?? GetDbConfig().AdminUser;
        public string ConnectionString => GetDbConfig().GetAdminConnectionString();
        public string ProcessorId => DbType.GetProcessorId();
    }
}