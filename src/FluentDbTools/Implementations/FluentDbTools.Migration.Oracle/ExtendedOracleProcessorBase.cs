using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
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
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Oracle
{
    internal class ExtendedOracleProcessorBase : OracleProcessorBase, IExtendedMigrationProcessor<ExtendedOracleProcessorBase>, IExtendedMigrationProcessorOracle
    {
        private IMigrationMetadata MigrationMetadata;
        private readonly IOptionsSnapshot<RunnerOptions> RunnerOptions;
        private readonly IDbMigrationConfig MigrationConfig;
        private readonly IVersionTableMetaData VersionTableMetaData;
        private readonly IExtendedMigrationGenerator ExtendedGenerator;
        private ICustomMigrationProcessor CustomMigrationProcessor;

        [SuppressMessage("ReSharper", "InconsistentNaming")] private bool Initialize_Initialized;
        private readonly Func<string> ConnectionStringFunc;
        private string LatestComment;
        protected string SchemaPrefix => MigrationConfig?.GetSchemaPrefixId();
        protected string SchemaPrefixUniqueId => MigrationConfig?.GetSchemaPrefixUniqueId();

        public ExtendedOracleProcessorBase(OracleBaseDbFactory factory,
            IMigrationGenerator generator,
            ILogger logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IOptionsSnapshot<RunnerOptions> runnerOptions,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator> extendedGenerator,
            IDbMigrationConfig migrationConfig,
            IMigrationSourceItem migrationSourceItem = null,
            IVersionTableMetaData versionTableMetaData = null) : base(ProcessorIds.OracleProcessorId, factory, generator, logger, options, connectionStringAccessor)
        {
            RunnerOptions = runnerOptions;
            MigrationConfig = migrationConfig;
            VersionTableMetaData = versionTableMetaData;
            ExtendedGenerator = extendedGenerator;
            MigrationMetadata = new MigrationMetadata(migrationSourceItem).InitMetadata(MigrationConfig);

            ConnectionStringFunc = () => connectionStringAccessor.ConnectionString;
            Initialize();
        }

        private bool NoConnection => RunnerOptions?.Value?.NoConnection ?? true;

        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            try
            {
                return base.ReadTableData(schemaName, tableName);
            }
            catch (Exception exception)
            {
                Logger?.LogTrace(exception, $"Something went wrong when reading ReadTableData DataSet [schemaName: {schemaName}, tableName:{tableName}]. Return Empty ReadTableData DataSet...");
#pragma warning disable 618
                var metaData = VersionTableMetaData ?? new DefaultVersionTableMetaData(schemaName);
#pragma warning restore 618

                var dataSet = new DataSet();
                var table = dataSet.Tables.Add(tableName);

                if (tableName.EqualsIgnoreCase(metaData.TableName) == false)
                {
                    return dataSet;
                }

                table.Columns.Add(metaData.ColumnName, typeof(long));
                table.Columns.Add(metaData.AppliedOnColumnName, typeof(DateTime));
                table.Columns.Add(metaData.DescriptionColumnName, typeof(string));
                try
                {
                    var splitOn = $"{metaData.AppliedOnColumnName}, {metaData.DescriptionColumnName}";
                    var sql = $"select {metaData.ColumnName}, {metaData.AppliedOnColumnName}, {metaData.DescriptionColumnName} FROM {schemaName}.{tableName}";
                    var rows = Connection.Query<decimal, DateTime, string, FluentDbToolsVersionTableMetadataRow>(
                        sql,
                        (version, appliedOn, description) => new FluentDbToolsVersionTableMetadataRow(version, appliedOn, description),
                        splitOn: splitOn);


                    foreach (var row in rows)
                    {

                        var dataRow = table.NewRow();
                        table.Rows.Add(dataRow);
                        dataRow.AcceptChanges();
                        dataRow.ItemArray = new object[] { row.Version, row.AppliedOn, row.Description };
                    }
                }
                catch
                {
                    //
                }

                return dataSet;
            }
        }

        public override DataSet Read(string template, params object[] args)
        {
            try
            {
                return base.Read(template, args);
            }
            catch (Exception exception)
            {
                try
                {
                    Logger?.LogTrace(exception, $"Something went wrong when reading DataSet Sql:{string.Format(template, args)} [template:{template}, args:{string.Join(", ", args.Where(x => x != null).Select(x => $"{x}(type={x.GetType().Name})"))}]");
                }
                catch
                {
                    Logger?.LogTrace(exception, $"Something went wrong when reading DataSet [template:{template}, args:{string.Join(", ", args.Where(x => x != null).Select(x => $"{x}(type={x.GetType().Name})"))}]");
                }

                throw;
            }
        }

        public void ExtendedBeginTransaction()
        {
            if (NoConnection)
            {
                return;
            }

            base.BeginTransaction();
        }

        public void ExtendedCommitTransaction()
        {
            if (NoConnection)
            {
                return;
            }
            base.CommitTransaction();
        }

        public void ExtendedEnsureConnectionIsOpen()
        {
            if (NoConnection)
            {
                return;
            }

            base.EnsureConnectionIsOpen();
        }

        public void ExtendedEnsureConnectionIsClosed()
        {
            if (NoConnection)
            {
                return;
            }

            base.EnsureConnectionIsClosed();
        }

        public override void BeginTransaction()
        {
            ExtendedBeginTransaction();
        }

        public override void CommitTransaction()
        {
            ExtendedCommitTransaction();
        }

        protected override void EnsureConnectionIsOpen()
        {
            ExtendedEnsureConnectionIsOpen();
        }

        protected override void EnsureConnectionIsClosed()
        {
            ExtendedEnsureConnectionIsClosed();
        }

        public override bool Exists(string template, params object[] args)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (NoConnection)
            {
                return false;
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
                    Process(changeLogDependedExpression);
                }

                if (expression is MigrationMetadataChangedExpression migrationMetadataChangedExpression)
                {
                    Process(migrationMetadataChangedExpression);
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

        public bool IsExists(string template, params object[] args)
        {
            return Exists(template, args);
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

        private void Process(IChangeLogTabledExpression expressionExt)
        {
            if (CustomMigrationProcessor == null || expressionExt == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(expressionExt.DbOperation))
            {
                expressionExt.DbOperation = expressionExt.Expression.GetDbOperationFromExpression();
            }

            RunCustomAction(() => CustomMigrationProcessor.Process(expressionExt));
        }

        private void Process(MigrationMetadataChangedExpression expression)
        {
            if (CustomMigrationProcessor == null || expression == null)
            {
                return;
            }

            if (MigrationMetadata.Equals(expression.MigrationMetadata))
            {
                return;
            }

            RunCustomAction(() =>
            {
                MigrationMetadata = expression.MigrationMetadata;

                CustomMigrationProcessor.MigrationMetadataChanged(MigrationMetadata, this);
            });
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

        public void ProcessSql(string sql, string logTitle)
        {
            Logger.LogSay(logTitle);
            Process(sql);
        }

        public void Initialize()
        {
            if (!NoConnection)
            {
                return;
            }

            if (Initialize_Initialized)
            {
                return;
            }

            Initialize_Initialized = true;

            var field = GetType().SearchForField(typeof(Lazy<IDbConnection>), "_lazyConnection");
            if (field != null)
            {
                field.SetValue(this, new Lazy<IDbConnection>(() =>
                {
                    var connection = DbProviderFactory.CreateConnection() ?? throw new NullReferenceException("No Connection available");
                    connection.ConnectionString = ConnectionStringFunc?.Invoke();
                    return connection;
                }));
            }
        }

        public void Initialize(ICustomMigrationProcessor customMigrationProcessor)
        {
            CustomMigrationProcessor = customMigrationProcessor;

            RunCustomAction(() =>
            {
                CustomMigrationProcessor?.ConfigureSqlExecuteAction(sql => Process(new SqlStatement { Sql = sql, IsExternal = true }));
                CustomMigrationProcessor?.MigrationMetadataChanged(MigrationMetadata, this);
            });

            Initialize();
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
                        SchemaPrefixId = SchemaPrefix,
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
                            SchemaPrefixId = SchemaPrefix,
                            SchemaPrefixUniqueId = SchemaPrefixUniqueId
                        }));
                },
                ex => Logger.LogError(ex, $"Dropping Oracle schema(user) '{expression.SchemaName}' failed with exception :-("));

        }


        protected override void Process(string sql)
        {
            Process(new SqlStatement { Sql = sql });
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

        private SqlStatement GetCreateSchemaSql(CreateSchemaExpression expression, bool forceDefault = false)
        {
            var sql = forceDefault ? null : RunCustomFunc(
                () => CustomMigrationProcessor?.GenerateSql(
                    new CreateSchemaWithPrefixExpression
                    {
                        SchemaName = expression.SchemaName,
                        SchemaPrefixId = SchemaPrefix,
                        SchemaPrefixUniqueId = SchemaPrefixUniqueId
                    }));
            var statement = new SqlStatement
            {
                IsExternal = !string.IsNullOrEmpty(sql),
                Sql = !string.IsNullOrEmpty(sql) ? sql : ExtendedGenerator.Generate(expression)
            };
            return statement;
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

        private void Process(SqlStatement sqlStatement)
        {
            var sql = sqlStatement.Sql;
            var runningSql = string.Empty;
            try
            {
                if (Options.PreviewOnly || NoConnection || string.IsNullOrEmpty(sql))
                {
                    if (sql.IsNotEmpty())
                    {
                        if (sqlStatement.IsExternal)
                        {
                            Logger.LogSqlInternal(sql);
                            return;
                        }

                        Logger.LogSql(sql);
                        return;
                    }
                    return;
                }

                EnsureConnectionIsOpen();

                var statementsSql = Regex.Split(sql, @"^\s*;\s*$", RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x)).ToArray();

                LatestComment = null;

                if (!sqlStatement.IsExternal)
                {
                    foreach (var commandText in statementsSql)
                    {
                        if (Regex.IsMatch(commandText, @"^\s*\/\s*$"))
                        {
                            foreach (var commandText2 in commandText.ExtractSqlStatements())
                            {
                                ExecuteCommand(runningSql = commandText2);
                            }
                        }
                        else
                        {
                            ExecuteCommand(runningSql = commandText.ConvertSimpleSqlComment());
                        }
                    }
                    return;
                }

                foreach (var commandText in sqlStatement.Sql.ExtractSqlStatements())
                {
                    ExecuteCommand(runningSql = commandText, true);
                }

            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"Fail executing sql \"{runningSql}\"");
                throw;
            }

        }

        private void ExecuteCommand(string commandText, bool logSqlInternal = false)
        {

            if (Options.PreviewOnly || NoConnection)
            {
                if (logSqlInternal)
                {
                    Logger.LogSqlInternal(commandText);
                    return;
                }
                Logger.LogSql(commandText);
                return;
            }


            using (var command = CreateCommand(commandText))
            {
                if (commandText.IsSqlComment())
                {
                    LatestComment = commandText;
                }
                else
                {
                    command.ExecuteNonQuery(LatestComment);
                    LatestComment = null;
                }

                if (logSqlInternal)
                {
                    Logger.LogSqlInternal(commandText);
                    return;
                }
                Logger.LogSql(commandText);
            }
        }
    }
}