using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Runner.Initialization;

namespace FluentDbTools.Migration.Abstractions
{
    /// <inheritdoc />
    public class MigrationMetadata : IMigrationMetadata
    {
        private string MigrationCallingNameConfigField;

        /// <inheritdoc />
        public Assembly MigrationAssembly { get; private set; }


        /// <inheritdoc />
        public string MigrationName { get; private set; }


        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="migrationSourceItem"></param>
        public MigrationMetadata(IMigrationSourceItem migrationSourceItem = null)
        {
            if (migrationSourceItem != null)
            {
                MigrationAssembly = migrationSourceItem.MigrationTypeCandidates?.FirstOrDefault()?.Assembly;
                if (MigrationAssembly != null)
                {
                    MigrationName = MigrationAssembly.GetName().Name;
                    return;
                }
            }

            MigrationAssembly = Assembly.GetCallingAssembly();
            MigrationName = MigrationAssembly.GetName().Name;
        }

        /// <summary>
        /// Constructor initializing from <paramref name="model"/>
        /// </summary>
        public MigrationMetadata(IMigrationModel model)
        {
            InitMetadata(model);
        }

        /// <summary>
        /// Initialize MigrationCallingName and MigrationAssembly from <paramref name="migrationConfig"/>
        /// </summary>
        /// <param name="migrationConfig"></param>
        public MigrationMetadata InitMetadata(IDbMigrationConfig migrationConfig)
        {
            MigrationCallingNameConfigField = migrationConfig?.GetMigrationName();
            if (MigrationCallingNameConfigField.IsNotEmpty())
            {
                MigrationName = MigrationCallingNameConfigField;
            }

            return this;
        }

        /// <summary>
        /// Initialize MigrationCallingName and MigrationAssembly from <paramref name="model"/>
        /// </summary>
        /// <param name="model"></param>
        protected void InitMetadata(IMigrationModel model)
        {
            if (model == null)
            {
                return;
            }

            MigrationAssembly = model.GetType().Assembly;

            if (MigrationCallingNameConfigField.IsEmpty())
            {
                MigrationName = MigrationAssembly.GetName().Name;
            }

        }

        /// <summary>
        /// Fetch config value for <paramref name="key"/>
        /// </summary>
        /// <param name="migrationConfig"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetConfigValue(IDbMigrationConfig migrationConfig, string key)
        {
            if (migrationConfig == null)
            {
                return null;
            }

            return migrationConfig.GetAllMigrationConfigValues()?.GetValue(key) ??
                   migrationConfig.GetDbConfig().GetAllDatabaseConfigValues()?.GetValue(key);
        }


        /// <inheritdoc />
#pragma warning disable 659
        public override bool Equals(object obj)
#pragma warning restore 659
        {
            if (obj is IMigrationMetadata other)
            {
                return Equals(MigrationAssembly, other.MigrationAssembly) && string.Equals(MigrationName,
                           other.MigrationName, StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return (MigrationAssembly?.GetHashCode() ?? 0) | (MigrationName?.GetHashCode() ?? 0);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{ {nameof(MigrationMetadata)} [{nameof(MigrationAssembly)}.Name:{MigrationAssembly.GetName().Name}, {nameof(MigrationName)}:{MigrationName}] }}";
        }
    }
}
