using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentDbTools.Migration.Contracts.MigrationExpressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.Logging;
// ReSharper disable CommentTypo

namespace FluentDbTools.Migration.Oracle.CustomProcessor
{
    /// <inheritdoc />
    public class DefaultOracleCustomMigrationProcessor : ICustomMigrationProcessor<OracleProcessor>
    {
        /// <summary>
        /// IsNullable for "TIDSSTEMPEL" and "DIPS" collumn. Default is TRUE
        /// </summary>
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        public bool IsTidsStempelAndDipsIdIdNullable { get; set; } = true;

        /// <summary>
        /// The SchemaName of CommonSchema (Default "DIPSCOREDB")
        /// </summary>
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public string CommonSchemaName { get; set; } = "DIPSCOREDB";

        /// <summary>
        /// The SchemaName of AuthCodeSchema (Default "CORE_CODE")
        /// </summary>
        public string AuthCodeSchemaName { get; set; } = "CORE_CODE";

        /// <summary>
        /// The Logger
        /// </summary>
        protected ILogger<DefaultOracleCustomMigrationProcessor> Logger { get; }

        /// <summary>
        /// The current IExtendedMigrationProcessorOracle 
        /// </summary>
        protected IExtendedMigrationProcessor Processor { get; set; }

        /// <summary>
        ///  Contains important Migration config values
        /// </summary>
        protected IDbMigrationConfig MigrationConfig { get; }

        /// <summary>
        /// Is Only enabled if DbType is Oracle
        /// </summary>
        protected bool Enabled { get; set; }

        /// <summary>
        /// The method for running Sql statemnsts
        /// </summary>
        protected Action<string> Execute { get; set; }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public DefaultOracleCustomMigrationProcessor(
            ILogger<DefaultOracleCustomMigrationProcessor> logger,
            IExtendedMigrationProcessorOracle processor,
            IDbMigrationConfig migrationConfig)
        {
            Logger = logger;
            Processor = processor;
            MigrationConfig = migrationConfig;
            Enabled = MigrationConfig.GetDbConfig().DbType == SupportedDatabaseTypes.Oracle;
        }

        /// <inheritdoc />
        public virtual void MigrationMetadataChanged(IMigrationMetadata migrationMetadata, IExtendedMigrationProcessor extendedMigrationProcessor)
        {
            Processor = extendedMigrationProcessor;

            if (!Enabled)
            {
                return;
            }

            if (!Processor.SchemaExists(CommonSchemaName) ||
                !Processor.SchemaExists(AuthCodeSchemaName))
            {
                return;
            }

            var logonSql = MigrationConfig
                .PrepareSql(SqlResources.LogonScriptSql)
                .ReplaceIgnoreCase("{CommonSchemaName}", CommonSchemaName)
                .ReplaceIgnoreCase("{AuthCodeSchemaName}", AuthCodeSchemaName);

            Execute(logonSql);

            if (Processor.SchemaExists(MigrationConfig.Schema))
            {
                ProcessAfter(new CreateSchemaWithPrefixExpression
                {
                    SchemaName = MigrationConfig.Schema,
                    SchemaPrefixId = MigrationConfig.GetSchemaPrefixId(),
                    SchemaPrefixUniqueId = MigrationConfig.GetSchemaPrefixUniqueId()
                });
            }
        }

        /// <inheritdoc />
        public virtual void ConfigureSqlExecuteAction(Action<string> executeSqlAction)
        {
            Execute = executeSqlAction;
        }

        /// <inheritdoc />
        public virtual void Process(IChangeLogTabledExpression expression)
        {
            if (!Enabled)
            {
                return;
            }

            if (expression.ChangeLogContext == null)
            {
                return;
            }

            Logger.LogInformation($"Process TableName: {expression.TableName}, " +
                                  (expression.TableDescription != null ? $"TableDescription: {expression.TableDescription}, " : "") +
                                  $"ShortName: {expression.ChangeLogContext.ShortName}, " +
                                  $"GlobalId: {expression.ChangeLogContext.GlobalId}, " +
                                  $"DbOperation: {expression.DbOperation}, " +
                                  (expression.OldRenamedName != null ? $"OldRenamedName: {expression.OldRenamedName}" : ""));

            if (expression.Columns.Any())
            {
                foreach (var column in expression.Columns)
                {
                    Logger.LogInformation($"Column:{column.Name} Table:{column.TableName}, Type: {column.Type}, Description: {column.ColumnDescription}");
                }
            }

            //Execute(string.Empty);
        }

        /// <summary>
        /// Will be called just after the Schema is created<br/>
        /// Can be used to execute custom grants or other database operations.
        /// </summary>
        /// <param name="expression"></param>
        public virtual void ProcessAfter(ICreateSchemaWithPrefixExpression expression)
        {
            if (!Enabled)
            {
                return;
            }

            if (!Processor.SchemaExists(CommonSchemaName))
            {
                return;
            }

            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");

            var sqlAll = MigrationConfig
                .PrepareSql(SqlResources.SchemaPrefixSql)
                .ReplaceIgnoreCase("{CommonSchemaName}", CommonSchemaName)
                .ReplaceIgnoreCase("{AuthCodeSchemaName}", AuthCodeSchemaName);


            Execute(sqlAll);
        }

        /// <summary>
        /// Will be called just after the Schema is deleted/dropped <br/>
        /// Can be used to do database operation in other (dependent) schema.
        /// </summary>
        /// <param name="expression"></param>
        public virtual void ProcessAfter(IDropSchemaWithPrefixExpression expression)
        {
            if (!Enabled)
            {
                return;
            }

            if (!Processor.SchemaExists(CommonSchemaName))
            {
                return;
            }

            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");

        }

        /// <inheritdoc />
        public virtual string GenerateSql(ICreateSchemaWithPrefixExpression expression)
        {
            return null;
        }

        /// <inheritdoc />
        public virtual IList<ColumnDefinition> GetDefaultColumns(string tableName = null)
        {
            if (!Enabled)
            {
                return null;
            }

            if (!Processor.SchemaExists(CommonSchemaName))
            {
                return null;
            }

            return new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "TIDSSTEMPEL", TableName = tableName, ColumnDescription = "Timestamp for", Type = DbType.Int32, IsNullable = IsTidsStempelAndDipsIdIdNullable},
                new ColumnDefinition { Name = "OPPRETTETAV", TableName = tableName, ColumnDescription = "Row created by", Type = DbType.AnsiString, Size = 20, IsNullable = true},
                new ColumnDefinition { Name = "OPPRETTETTID", TableName = tableName, ColumnDescription = "Created timestamp", Type = DbType.Date, IsNullable = true},
                new ColumnDefinition { Name = "SISTENDRETAV", TableName = tableName, ColumnDescription = "Last created by", Type = DbType.AnsiString, Size = 20, IsNullable = true},
                new ColumnDefinition { Name = "SISTENDRETTID", TableName = tableName, ColumnDescription = "Last created timestamp", Type = DbType.Date, IsNullable = true},
                new ColumnDefinition { Name = "DIPSID", TableName = tableName, ColumnDescription = "Row sequence", Type = DbType.Int32, IsNullable = IsTidsStempelAndDipsIdIdNullable},
            };

        }
    }
}