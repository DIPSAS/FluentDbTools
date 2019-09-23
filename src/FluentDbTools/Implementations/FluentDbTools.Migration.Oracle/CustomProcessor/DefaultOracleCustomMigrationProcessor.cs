using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.Logging;

namespace FluentDbTools.Migration.Oracle.CustomProcessor
{
    /// <inheritdoc />
    public class DefaultOracleCustomMigrationProcessor : ICustomMigrationProcessor<OracleProcessor>
    {
        [SuppressMessage("ReSharper", "IdentifierTypo")] 
        public virtual bool IsTidsStempelAndDipsIdIdNullable { get; set; } = true;

        [SuppressMessage("ReSharper", "StringLiteralTypo")] 
        public virtual string CommonSchemaName { get; set; } = "DIPSCOREDB";

        public virtual string AuthCodeSchemaName { get; set; } = "CORE_CODE";

        private readonly ILogger<DefaultOracleCustomMigrationProcessor> Logger;
        private IExtendedMigrationProcessor Processor;
        private Action<string> Execute;

        /// <inheritdoc />
        public DefaultOracleCustomMigrationProcessor(
            ILogger<DefaultOracleCustomMigrationProcessor> logger,
            IExtendedMigrationProcessorOracle processor)
        {
            Logger = logger;
            Processor = processor;
        }

        public virtual void MigrationMetadataChanged(IMigrationMetadata migrationMetadata, IExtendedMigrationProcessor extendedMigrationProcessor)
        {
            Processor = extendedMigrationProcessor;
            if (Processor.SchemaExists(CommonSchemaName) && Processor.SchemaExists(AuthCodeSchemaName))
            {
                var logonSql = SqlResources.LogonScriptSql
                                    .ReplaceIgnoreCase("{CommonSchemaName}", CommonSchemaName)
                                    .ReplaceIgnoreCase("{AuthCodeSchemaName}", AuthCodeSchemaName)
                                    .ReplaceIgnoreCase("{User}", migrationMetadata.MigrationName.SubstringTo("."));

                Execute(logonSql);
            }
        }

        public virtual void ConfigureSqlExecuteAction(Action<string> executeSqlAction)
        {
            Execute = executeSqlAction;
        }

        public virtual void Process(IChangeLogTabledExpression expression)
        {
            if (expression.ChangeLogContext != null)
            {

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
        }

        /// <summary>
        /// Will be called just after the Schema is created<br/>
        /// Can be used to execute custom grants or other database operations.
        /// </summary>
        /// <param name="expression"></param>
        public virtual void ProcessAfter(ICreateSchemaWithPrefixExpression expression)
        {
            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");
            if (Processor.SchemaExists(CommonSchemaName))
            {
                var sqlAll = SqlResources.SchemaPrefixSql;

                sqlAll = sqlAll.Replace("{CommonSchemaName}", CommonSchemaName);
                sqlAll = sqlAll.Replace("{SchemaName}", expression.SchemaName.ToUpper());
                sqlAll = sqlAll.Replace("{SchemaPrefixUniqueId}", expression.SchemaPrefixUniqueId);
                sqlAll = sqlAll.Replace("{SchemaPrefixId}", expression.SchemaPrefixId);

                Execute(sqlAll);

            }
        }

        /// <summary>
        /// Will be called just after the Schema is deleted/dropped <br/>
        /// Can be used to do database operation in other (dependent) schema.
        /// </summary>
        /// <param name="expression"></param>
        public virtual void ProcessAfter(IDropSchemaWithPrefixExpression expression)
        {
            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");

        }

        public virtual string GenerateSql(ICreateSchemaWithPrefixExpression expression)
        {
            if (Processor.SchemaExists(CommonSchemaName))
            {
                var sqlAll = SqlResources.CreateSchemaSql;

                sqlAll = sqlAll.Replace("{CommonSchemaName}", CommonSchemaName);
                sqlAll = sqlAll.Replace("{SchemaName}", expression.SchemaName);
                sqlAll = sqlAll.Replace("{SchemaPrefixUniqueId}", expression.SchemaPrefixUniqueId);
                sqlAll = sqlAll.Replace("{SchemaPrefix}", expression.SchemaPrefixId);
            
                return sqlAll;
            }

            return null;
        }

        public virtual IList<ColumnDefinition> GetDefaultColumns(string tableName = null)
        {
            if (Processor.SchemaExists(CommonSchemaName))
            {
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

            return new List<ColumnDefinition>();

        }
    }
}