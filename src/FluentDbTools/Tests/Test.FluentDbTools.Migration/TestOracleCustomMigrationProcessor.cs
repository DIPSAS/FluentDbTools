using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.Logging;

namespace Test.FluentDbTools.Migration
{
    public class TestOracleCustomMigrationProcessor : ICustomMigrationProcessor<OracleProcessor>
    {
        private readonly ILogger<TestOracleCustomMigrationProcessor> Logger;
        private Action<string> Execute;

        public TestOracleCustomMigrationProcessor(ILogger<TestOracleCustomMigrationProcessor> logger)
        {
            Logger = logger;
        }

        public void MigrationMetadataChanged(IMigrationMetadata migrationMetadata)
        {
            Logger.LogInformation(migrationMetadata.ToString());
        }

        public void ConfigureSqlExecuteAction(Action<string> executeSqlAction)
        {
            Execute = executeSqlAction;
        }

        public void Process(IChangeLogTabledExpression expression)
        {
            if (expression.ChangeLogContext != null)
            {

                Logger.LogInformation($"Process TableName: {expression.TableName}, " +
                                      (expression.TableDescription != null ? $"TableDescription: {expression.TableDescription}, " : "") +
                                      $"ShortName: {expression.ChangeLogContext.ShortName}, " +
                                      $"GlobalId: {expression.ChangeLogContext.GlobalId}, " +
                                      $"DbOperation: {expression.DbOperation}, " + 
                                      (expression.OldRenamedName != null ? $"OldRenamedName: {expression.OldRenamedName}" : "") );
                
                if (expression.Columns.Any())
                {
                    foreach (var column in expression.Columns)
                    {
                        Logger.LogInformation($"Column:{column.Name} Table:{column.TableName}, Type: {column.Type}, Description: {column.ColumnDescription}");
                    }
                }
                Execute(string.Empty);
            }
        }

        /// <summary>
        /// Will be called just after the Schema is created<br/>
        /// Can be used to execute custom grants or other database operations.
        /// </summary>
        /// <param name="expression"></param>
        public void ProcessAfter(ICreateSchemaWithPrefixExpression expression)
        {
            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");
        }

        /// <summary>
        /// Will be called just after the Schema is deleted/dropped <br/>
        /// Can be used to do database operation in other (dependent) schema.
        /// </summary>
        /// <param name="expression"></param>
        public void ProcessAfter(IDropSchemaWithPrefixExpression expression)
        {
            Logger.LogInformation($"ProcessAfter Schema:{expression.SchemaName} SchemaPrefix:{expression.SchemaPrefixId}");

        }

        public string GenerateSql(ICreateSchemaWithPrefixExpression expression)
        {
            var sqlAll = @"
            create user {SchemaName} identified by ""{SchemaName}""
            enable editions account lock;

            -- hei hei
            -- aho
            /* drop role {SchemaPrefix}_TABLE_ROLE;
            create role {SchemaPrefix}_TABLE_ROLE; */

            /*
            drop role {SchemaPrefix}_TABLE_ROLE;
            create role {SchemaPrefix}_TABLE_ROLE; 
            */

            grant connect, resource to {SchemaName};
            grant unlimited tablespace to {SchemaName};
";
            sqlAll = sqlAll.Replace("{SchemaName}", expression.SchemaName);
            sqlAll = sqlAll.Replace("{SchemaPrefixUniqueId}", expression.SchemaPrefixUniqueId);
            sqlAll = sqlAll.Replace("{SchemaPrefix}", expression.SchemaPrefixId);
            return sqlAll;
        }

        public IList<ColumnDefinition> GetDefaultColumns(string tableName = null)
        {
            return new List<ColumnDefinition>
            {
                new ColumnDefinition { Name = "TIDSSTEMPEL", TableName = tableName, ColumnDescription = "Timestamp for", Type = DbType.Int32, IsNullable = true},
                new ColumnDefinition { Name = "OPPRETTETAV", TableName = tableName, ColumnDescription = "Row created by", Type = DbType.AnsiString, Size = 20, IsNullable = true},
                new ColumnDefinition { Name = "OPPRETTETTID", TableName = tableName, ColumnDescription = "Created timestamp", Type = DbType.Date, IsNullable = true},
                new ColumnDefinition { Name = "SISTENDRETAV", TableName = tableName, ColumnDescription = "Last created by", Type = DbType.AnsiString, Size = 20, IsNullable = true},
                new ColumnDefinition { Name = "SISTENDRETTID", TableName = tableName, ColumnDescription = "Last created timestamp", Type = DbType.Date, IsNullable = true},
                new ColumnDefinition { Name = "DIPSID", TableName = tableName, ColumnDescription = "Row sequence", Type = DbType.Int32, IsNullable = true},
            };
        }
    }
}