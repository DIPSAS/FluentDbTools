﻿using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentDbTools.Migration.Common;
using FluentDbTools.Migration.Contracts.MigrationExpressions;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Oracle
{
    internal class ExtendedOracleProcessorBase : OracleProcessorBase, IExtendedMigrationProcessor<ExtendedOracleProcessorBase>
    {
        private readonly IDbMigrationConfig MigrationConfig;
        private readonly IExtendedMigrationGenerator ExtendedGenerator;
        private readonly ICustomMigrationProcessor CustomMigrationProcessor;
        protected string SchemaPrefix => MigrationConfig?.GetSchemaPrefixId();
        protected string SchemaPrefixUniqueId => MigrationConfig?.GetSchemaPrefixUniqueId();

        public ExtendedOracleProcessorBase(OracleBaseDbFactory factory,
            IMigrationGenerator generator,
            ILogger logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator> extendedGenerator,
            IDbMigrationConfig migrationConfig,
            ICustomMigrationProcessor<OracleProcessor> customMigrationProcessor = null) : base(ProcessorIds.OracleProcessorId, factory, generator, logger, options, connectionStringAccessor)
        {
            MigrationConfig = migrationConfig;
            ExtendedGenerator = extendedGenerator;
            CustomMigrationProcessor = customMigrationProcessor;
            RunCustomAction(() => CustomMigrationProcessor?.ConfigureSqlExecuteAction(ProcessSql));
        }

        public override bool Exists(string template, params object[] args)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            EnsureConnectionIsOpen();

            using (var command = CreateCommand(string.Format(template, args)))
            using (var reader = command.ExecuteReader())
            {
                return reader.Read();
            }
        }

        public override void Process(PerformDBOperationExpression expression)
        {
            if (expression is ILinkedExpression)
            {
                if (expression is DefaultColumnsLinkedExpression defaultColumnsDependedExpression)
                {
                    AddDefaultColumns(defaultColumnsDependedExpression.Expression);
                }
                if (expression is ChangeLogLinkedExpression changeLogDependedExpression)
                {
                    ProcessChangeLogExpression(changeLogDependedExpression);
                }
                return;
            }

            base.Process(expression);
        }

        public override void Process(DeleteDataExpression expression)
        {
            if (!SchemaExists(expression.SchemaName))
            {
                return;
            }

            base.Process(expression);
        }


        public override bool SchemaExists(string schemaName)
        {
            return Exists(ExtendedGenerator.GenerateSchemaExistsSql(schemaName));
        }

        public IDbConnection GetMigrationDbConnection()
        {
            return Connection;
        }

        public override void Process(CreateTableExpression expression)
        {
            if (TableExists(expression.SchemaName, expression.TableName))
            {
                return;
            }

            base.Process(expression);
        }

        private void AddDefaultColumns(CreateTableExpression expression)
        {
            var defaultColumns =
                RunCustomFunc(() => CustomMigrationProcessor?.GetDefaultColumns(expression.TableName));

            if ((defaultColumns?.Any() ?? false) == false)
            {
                return;
            }

            var columnDefinitions = expression.Columns;
            foreach (var column in defaultColumns)
            {
                if (columnDefinitions.Any(x => (string.IsNullOrEmpty(column.TableName) || x.TableName == column.TableName) && x.Name == column.Name))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(column.TableName))
                {
                    column.TableName = expression.TableName;
                }

                columnDefinitions.Add(column);
                Logger.LogSay($"Added Default Column:{column.Name} Table:{column.TableName}, Type: {column.Type}, Description: {column.ColumnDescription}");
            }

            expression.Columns = columnDefinitions;
        }

        private void ProcessChangeLogExpression(IChangeLogTabledExpression expressionExt)
        {
            if (CustomMigrationProcessor == null || expressionExt == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(expressionExt.DbOperation))
            {
                expressionExt.DbOperation = expressionExt.Expression.GetDbOperation();
            }

            RunCustomAction(() => CustomMigrationProcessor.Process(expressionExt));
        }


        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return Exists(ExtendedGenerator.GenerateSequenceExistsSql(schemaName, sequenceName));
        }

        public override void Process(CreateSequenceExpression expression)
        {
            if (SequenceExists(expression.Sequence.SchemaName, expression.Sequence.Name))
            {
                return;
            }

            base.Process(expression);
        }

        public override void Process(CreateConstraintExpression expression)
        {
            if (ConstraintExists(expression.Constraint.SchemaName, expression.Constraint.TableName,
                expression.Constraint.ConstraintName))
            {
                return;
            }

            Process(ExtendedGenerator.Generate(expression));
        }

        public override void Process(CreateIndexExpression expression)
        {
            if (IndexExists(expression.Index.SchemaName, expression.Index.TableName, expression.Index.Name))
            {
                return;
            }

            Process(ExtendedGenerator.Generate(expression));
        }

        public void ProcessSql(string sql)
        {
            Process(sql);
        }

        public override void Process(CreateSchemaExpression expression)
        {
            if (SchemaExists(expression.SchemaName))
            {
                return;
            }

            SetupTableSpace(TableSpaceType.Default);
            SetupTableSpace(TableSpaceType.Temp);

            Logger.LogSay($"Creating Oracle schema(user) '{expression.SchemaName}'...");
            Process(GetCreateSchemaSql(expression));

            if (!SchemaExists(expression.SchemaName))
            {
                Process(GetCreateSchemaSql(expression, true));
            }

            Logger.LogSay($"Created Oracle schema(user) '{expression.SchemaName}'...");

            RunCustomAction(
                () => CustomMigrationProcessor?.ProcessAfter(
                    new CreateSchemaWithPrefixExpression
                    {
                        SchemaName = expression.SchemaName,
                        SchemaPrefix = SchemaPrefix,
                        SchemaPrefixUniqueId = SchemaPrefixUniqueId
                    }));
        }

        public override void Process(DeleteTableExpression expression)
        {
            if (!TableExists(expression.SchemaName, expression.TableName))
            {
                return;
            }
            base.Process(expression);
        }

        public override void Process(DeleteSchemaExpression expression)
        {
            this.ExecuteCodeBlockUntilNoExeception(() =>
                {
                    if (!SchemaExists(expression.SchemaName))
                    {
                        return;
                    }

                    var stopwatch = new StopWatch();

                    Logger.LogSay($"Dropping Oracle schema(user) '{expression.SchemaName}'...");
                    stopwatch.Time(() => Process(ExtendedGenerator.Generate(expression)));
                    Logger.LogSay($"Dropped Oracle schema(user) '{expression.SchemaName}'...");
                    Logger.LogElapsedTime(stopwatch.ElapsedTime());

                    RunCustomAction(() =>
                        CustomMigrationProcessor?.ProcessAfter(new DropSchemaWithPrefixExpression
                        {
                            SchemaName = expression.SchemaName,
                            SchemaPrefix = SchemaPrefix,
                            SchemaPrefixUniqueId = SchemaPrefixUniqueId
                        }));
                },
                ex => Logger.LogError(ex, $"Dropping Oracle schema(user) '{expression.SchemaName}' failed with exception :-("));

        }


        protected override void Process(string sql)
        {
            var runningSql = string.Empty;
            try
            {
                if (Options.PreviewOnly || string.IsNullOrEmpty(sql))
                {
                    if (sql.IsNotEmpty())
                    {
                        Logger.LogSql(sql);
                    }
                    return;
                }

                EnsureConnectionIsOpen();

                var batches = Regex.Split(sql, @"^\s*;\s*$", RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));

                foreach (var batch in batches)
                {
                    runningSql = batch;
                    using (var command = CreateCommand(batch))
                    {
                        command.ExecuteNonQuery();
                        Logger.LogSql(batch);
                    }
                }

            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"Fail executing sql \"{runningSql}\"");
                throw;
            }
        }

        private bool TableSpaceExists(TableSpaceType tableSpaceType)
        {
            var sql = ExtendedGenerator.GenerateTableSpaceExistsSql(tableSpaceType);
            if (string.IsNullOrEmpty(sql))
            {
                return true;
            }
            return Exists(sql);
        }

        private void SetupTableSpace(TableSpaceType tableSpaceType)
        {
            if (TableSpaceExists(tableSpaceType))
            {
                return;
            }
            Logger.LogSay($"Creating Oracle {tableSpaceType} tablespace: {ExtendedGenerator.GetTableSpaceName(tableSpaceType)}...");
            Process(ExtendedGenerator.GenerateCreateTableSpaceSql(tableSpaceType));
            Logger.LogSay($"Created Oracle {tableSpaceType} tablespace: {ExtendedGenerator.GetTableSpaceName(tableSpaceType)}...");
        }

        private string GetCreateSchemaSql(CreateSchemaExpression expression, bool forceDefault = false)
        {
            var sql = forceDefault ? null : RunCustomFunc(
                () => CustomMigrationProcessor?.GenerateSql(
                    new CreateSchemaWithPrefixExpression
                    {
                        SchemaName = expression.SchemaName,
                        SchemaPrefix = SchemaPrefix,
                        SchemaPrefixUniqueId = SchemaPrefixUniqueId
                    }));

            return !string.IsNullOrEmpty(sql) ? sql : ExtendedGenerator.Generate(expression);
        }

        private static void RunCustomAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (NotImplementedException)
            {
                //
            }
        }

        private static T RunCustomFunc<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (NotImplementedException)
            {
                //
            }

            return default;
        }

    }
}