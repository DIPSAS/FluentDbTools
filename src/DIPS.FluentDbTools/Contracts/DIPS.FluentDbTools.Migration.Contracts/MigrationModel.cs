using System;
using System.Linq;
using System.Reflection;
using DIPS.FluentDbTools.Migration.Abstractions;
using FluentMigrator.Builders;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Migration.Contracts
{
    public abstract class MigrationModel : FluentMigrator.Migration, IMigrationModel
    {
        private string SchemaNameField;
        private string ConfiguredDatabaseTypeField;
        private IVersionTableMetaData Version;

        public string ConfiguredDatabaseType
        {
            get
            {
                if (string.IsNullOrEmpty(ConfiguredDatabaseTypeField))
                {
                    IfDatabase(x =>
                    {
                        ConfiguredDatabaseType = x;
                        return false;
                    });
                }

                return ConfiguredDatabaseTypeField;
            }
            set => ConfiguredDatabaseTypeField = value;
        }

        public string SchemaName
        {
            get
            {
                if (!string.IsNullOrEmpty(SchemaNameField))
                {
                    return SchemaNameField;
                }

                if (Version != null)
                {
                    SchemaNameField = Version.SchemaName;
                }

                if (!string.IsNullOrEmpty(SchemaNameField))
                {
                    return SchemaNameField;
                }

                var type = typeof(FluentMigrator.Migration);
                var property = type.GetProperty("Context", BindingFlags.Instance | BindingFlags.NonPublic);
                if (property != null)
                {
                    Version = (property.GetValue(this) as MigrationContext)?.ServiceProvider?.GetService<IVersionTableMetaData>();
                    if (Version != null)
                    {
                        SchemaNameField = Version.SchemaName;
                    }

                }

                return SchemaNameField;
            }
        }

        protected MigrationModel()
        {

        }

        protected MigrationModel(IVersionTableMetaData version)
        {
            Version = version;
        }

        public bool IsOracle()
        {
            return IsOracle(this.GetConfigurtedDatabaseType());
        }

        public bool IsPostgres()
        {
            return IsPostgres(this.GetConfigurtedDatabaseType());
        }
        
        private static bool IsOracle(string configuredDatabaseType)
        {
            return IsDatabase(configuredDatabaseType, "oracle");
        }

        private static bool IsMsSql(string configuredDatabaseType)
        {
            return IsDatabase(configuredDatabaseType, "mssql");
        }
        
        private static bool IsPostgres(string configuredDatabaseType)
        {
            return IsDatabase(configuredDatabaseType, "postgres");
        }

        private static bool IsDatabase(string configuredDatabaseType, params string[] matchingDatabaseTypes)
        {
            return !string.IsNullOrEmpty(configuredDatabaseType) && matchingDatabaseTypes.Any(matchingDatabaseType => configuredDatabaseType.StartsWith(matchingDatabaseType, StringComparison.OrdinalIgnoreCase));
        }

        internal TNext AsDatabaseDateTime<TNext>(IColumnTypeSyntax<TNext> column) where TNext : IFluentSyntax
        {
            if (IsOracle(ConfiguredDatabaseType))
            {
                return column.AsCustom(MigrationConsts.DateTimeTypeForOracle);
            }

            return column.AsDateTime();
        }
        
        internal TNext AsDatabaseBlob<TNext>(IColumnTypeSyntax<TNext> column) where TNext : IFluentSyntax
        {
            if (IsOracle(ConfiguredDatabaseType))
            {
                return column.AsCustom(MigrationConsts.BlobTypeForOracle);
            }

            if (IsMsSql(ConfiguredDatabaseType))
            {
                return column.AsCustom(MigrationConsts.BlobTypeForMsSql);
            }

            if (IsPostgres(ConfiguredDatabaseType))
            {
                return column.AsCustom(MigrationConsts.BlobTypeForPostgres);
            }

            return column.AsBinary();
        }
    }


    public static class ColumnName
    {
        public const string Id = "Id";

    }
}