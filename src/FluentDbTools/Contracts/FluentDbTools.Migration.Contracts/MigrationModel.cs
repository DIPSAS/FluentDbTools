using System;
using System.Collections.Generic;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Migration.Contracts
{
    /// <summary>
    /// Abstract class with Extended props and methods to <see cref="FluentMigrator.Migration"/>
    /// </summary>
    public abstract class MigrationModel : FluentMigrator.Migration, IMigrationModel
    {
        private string SchemaNameField;
        private string SchemaPrefixIdField;
        private string SchemaPrefixUniqueIdField;
        private string ConfiguredDatabaseTypeField;
        private IVersionTableMetaData Version;
        private IMigrationContext MigrationContextField;
        private IDbMigrationConfig MigrationConfigField;
        private IServiceProvider ServiceProviderField;
        private IList<IMigrationExpression> MigrationExpressionsField;

        /// <inheritdoc />
        public ICreateOrReplaceExpressionRoot CreateOrReplace => new CreateOrReplaceExpressionRoot(GetMigrationContext());

        /// <summary>
        /// Gets the starting point for data deletions
        /// </summary>
        public new IDeleteExpressionRoot Delete
        {
            get
            {
                var root = base.Delete;
                return this.IsMigrationContextChanged(root) ? new DeleteExpressionRoot(GetMigrationContext()) : root;
            }
        }

        /// <summary>
        /// Set the configured Database type i.e. Oracle Or Postgres
        /// </summary>
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

        /// <inheritdoc />
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

                Version = GetServiceProvider()?.GetService<IVersionTableMetaData>();

                if (Version != null)
                {
                    SchemaNameField = Version.SchemaName;
                }

                return SchemaNameField;
            }
        }

        /// <inheritdoc />
        public string SchemaPrefixId
        {
            get
            {
                if (!string.IsNullOrEmpty(SchemaPrefixIdField))
                {
                    return SchemaPrefixIdField;
                }

                SchemaPrefixIdField = GetMigrationConfig()?.GetSchemaPrefixId();
                return SchemaPrefixIdField;
            }
        }

        /// <inheritdoc />
        public string SchemaPrefixUniqueId
        {
            get
            {
                if (!string.IsNullOrEmpty(SchemaPrefixUniqueIdField))
                {
                    return SchemaPrefixUniqueIdField;
                }

                SchemaPrefixUniqueIdField = GetMigrationConfig()?.GetSchemaPrefixUniqueId();
                return SchemaPrefixUniqueIdField;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbMigrationConfig GetMigrationConfig()
        {
            if (MigrationConfigField != null)
            {
                return MigrationConfigField;
            }

            MigrationConfigField = GetServiceProvider()?.GetService<IDbMigrationConfig>();
            return MigrationConfigField;
        }

        /// <inheritdoc />
        public IMigrationContext GetMigrationContext()
        {
            if (MigrationContextField != null)
            {
                return MigrationContextField;
            }

            MigrationContextField = this.GetMigrationContextFromObject(typeof(FluentMigrator.Migration));

            return MigrationContextField;
        }

        /// <inheritdoc />
        public IList<IMigrationExpression> GetExpressions()
        {
            if (MigrationExpressionsField != null)
            {
                return MigrationExpressionsField;
            }

            return MigrationExpressionsField = GetMigrationContext()?.Expressions as IList<IMigrationExpression>;
        }

        /// <inheritdoc />
        public string GetPrefixedName(string name)
        {
            return string.IsNullOrEmpty(SchemaPrefixId) ? name : name.GetPrefixedName(SchemaPrefixId);
        }

        /// <inheritdoc />
        public void Reset(IMigrationContext context)
        {
            MigrationExpressionsField = null;
            MigrationContextField = context;
        }

        /// <inheritdoc />
        protected MigrationModel()
        {
        }

        /// <inheritdoc />
        protected MigrationModel(IVersionTableMetaData version)
        {
            Version = version;
        }

        /// <inheritdoc />
        protected MigrationModel(IMigrationContext context)
        {
            MigrationContextField = context;
        }
        /// <summary>
        /// Return true if ConfiguredDatabaseType == "oracle"
        /// </summary>
        /// <returns></returns>
        public bool IsOracle()
        {
            return IsOracle(this.GetConfigurtedDatabaseType());
        }

        /// <summary>
        /// Return true if ConfiguredDatabaseType == "postgres"
        /// </summary>
        /// <returns></returns>
        public bool IsPostgres()
        {
            return IsPostgres(this.GetConfigurtedDatabaseType());
        }

        /// <summary>
        /// Return <see cref="IServiceProvider"/> from <see cref="IMigrationContext"/>
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetServiceProvider()
        {
            if (ServiceProviderField != null)
            {
                return ServiceProviderField;
            }

            ServiceProviderField = GetMigrationContext()?.ServiceProvider;
            return ServiceProviderField;
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
            return IsOracle(ConfiguredDatabaseType) ? column.AsCustom(MigrationConsts.DateTimeTypeForOracle) : column.AsDateTime();
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

            return IsPostgres(ConfiguredDatabaseType) ? column.AsCustom(MigrationConsts.BlobTypeForPostgres) : column.AsBinary();
        }
    }

    


    /// <summary>
    /// Default Column names
    /// </summary>
    public static class ColumnName
    {
        /// <summary>
        /// Id Column
        /// </summary>
        public const string Id = "Id";

    }



}