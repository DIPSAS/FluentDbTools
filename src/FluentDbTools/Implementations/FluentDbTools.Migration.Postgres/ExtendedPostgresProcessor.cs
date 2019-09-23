using System;
using System.Data;
using System.IO;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Common;
using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Postgres;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FluentDbTools.Migration.Postgres
{
    internal class ExtendedPostgresProcessor : PostgresProcessor, IExtendedMigrationProcessor<ExtendedPostgresProcessor>
    {
        private readonly IExtendedMigrationGenerator ExtendedGeneratorField;
        private readonly IDbMigrationConfig DbMigrationConfig;

        public ExtendedPostgresProcessor(IDbMigrationConfig dbMigrationConfig,
            PostgresQuoter quoter,
            PostgresDbFactory factory,
            PostgresGenerator generator,
            ILogger<ExtendedPostgresProcessor> logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationGenerator<ExtendedPostgresGenerator> extendedGenerator,
            PostgresOptions pgOptions)
            : base(factory, generator, logger, options, connectionStringAccessor, pgOptions)
        {
            ExtendedGeneratorField = extendedGenerator;
            DbMigrationConfig = dbMigrationConfig;

            if (dbMigrationConfig.ProcessorId == ProcessorIds.PostgresProcessorId)
            {
                var stopWatch = new StopWatch();

                PostgresDatabaseCreator.CreateDatabase(
                    dbMigrationConfig,
                    () =>
                    {
                        Logger.LogSay($"Creating Postgres database '{dbMigrationConfig.DatabaseName.ToLower()}'...");
                        stopWatch.Start();
                    },
                    sql =>
                    {
                        stopWatch.Stop();
                        Logger.LogSql(sql);
                        Logger.LogSay($"Created Postgres database '{dbMigrationConfig.DatabaseName.ToLower()}'...");
                        Logger.LogElapsedTime(stopWatch.ElapsedTime());
                    });
            }

            this.SetPrivateFieldValue<PostgresProcessor>("_quoter", quoter);
        }

        public override string DatabaseType => ProcessorIds.PostgresProcessorId;

        public IDbConnection GetMigrationDbConnection()
        {
            return Connection;
        }

        public void ProcessSql(string sql)
        {
            Process(sql);
        }

        public void Initialize(ICustomMigrationProcessor customMigrationProcessor)
        {
            
        }

        public bool IsExists(string template, params object[] args)
        {
            return Exists(template, args);
        }

        public override bool TableExists(string schemaName, string tableName)
        {
            return base.TableExists(schemaName, tableName.ToLower());
        }

        public override bool IndexExists(string schemaName, string tableName, string indexName)
        {
            return base.IndexExists(schemaName, tableName.ToLower(), indexName.ToLower());
        }

        public override bool SequenceExists(string schemaName, string sequenceName)
        {
            return base.SequenceExists(schemaName, sequenceName.ToLower());
        }

        public override bool ConstraintExists(string schemaName, string tableName, string constraintName)
        {
            return base.ConstraintExists(schemaName, tableName.ToLower(), constraintName.ToLower());
        }

        public override bool ColumnExists(string schemaName, string tableName, string columnName)
        {
            return base.ColumnExists(schemaName, tableName.ToLower(), columnName.ToLower());
        }

        public override void Process(AlterColumnExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(CreateColumnExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(AlterTableExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(CreateTableExpression expression)
        {
            expression.ToLower();
            if (TableExists(expression.SchemaName, expression.TableName))
            {
                return;
            }
            base.Process(expression);
        }

        public override void Process(CreateForeignKeyExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(InsertDataExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(CreateSequenceExpression expression)
        {
            expression.ToLower();
            if (SequenceExists(expression.Sequence.SchemaName, expression.Sequence.Name))
            {
                return;
            }

            base.Process(expression);
        }

        public override void Process(AlterDefaultConstraintExpression expression)
        {
            expression.ToLower();
            base.Process(expression);
        }

        public override void Process(CreateConstraintExpression expression)
        {
            expression.ToLower();
            if (ConstraintExists(expression.Constraint.SchemaName, expression.Constraint.TableName,
                expression.Constraint.ConstraintName))
            {
                return;
            }

            base.Process(expression);
        }

        public override void Process(CreateIndexExpression expression)
        {
            expression.ToLower();
            if (IndexExists(expression.Index.SchemaName, expression.Index.TableName, expression.Index.Name))
            {
                return;
            }

            base.Process(expression);
        }

        public override void Process(DeleteColumnExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteConstraintExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteDefaultConstraintExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteForeignKeyExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteIndexExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteSequenceExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(RenameColumnExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(RenameTableExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(UpdateDataExpression expression)
        {
            base.Process(expression.ToLower());
        }

        public override void Process(DeleteDataExpression expression)
        {
            expression.ToLower();
            if (!TableExists(expression.SchemaName, expression.TableName))
            {
                return;
            }

            base.Process(expression);
        }


        public override void Process(DeleteTableExpression expression)
        {
            expression.ToLower();
            if (!TableExists(expression.SchemaName, expression.TableName))
            {
                return;
            }
            base.Process(expression);
        }

        public override void Process(CreateSchemaExpression expression)
        {
            Process(new CreateUserExpression(expression));

            if (SchemaExists(expression.SchemaName))
            {
                try
                {
                    Process(ExtendedGeneratorField.GenerateDefaultPrivilegesSql(expression.SchemaName));
                }
                catch
                {
                    Logger.LogSay($"Error when try to append DefaultPrivileges to schema [{expression.SchemaName}]. Probably because this is already done....");
                }
                return;
            }
            Logger.LogSay($"Creating Postgres schema '{expression.SchemaName}'...");
            Process(ExtendedGeneratorField.Generate(expression));
            Logger.LogSay($"Created Postgres schema '{expression.SchemaName}'...");
        }

        private void Debug(string s)
        {
            Logger.LogSay(s);
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
                Logger.LogSay($"Dropping Postgres schema '{expression.SchemaName}'...");
                stopwatch.Time(() => Process(ExtendedGeneratorField.Generate(expression)));
                Logger.LogSay($"Dropped Postgres schema '{expression.SchemaName}'...");
                Logger.LogElapsedTime(stopwatch.ElapsedTime());

                Process(new DeleteUserExpression(expression));
                PostgresDatabaseCreator.DropDatabase(
                    DbMigrationConfig,
                    () => 
                    {
                        stopwatch.Start();
                        Logger.LogSay($"Dropping Postgres database '{DbMigrationConfig.DatabaseName.ToLower()}'...");
                    },
                    sql =>
                    {
                        stopwatch.Stop();
                        Logger.LogSql(sql);
                        Logger.LogSay($"Dropped Postgres database '{DbMigrationConfig.DatabaseName.ToLower()}'...");
                        Logger.LogElapsedTime(stopwatch.ElapsedTime());
                    });
            },
                ex => Logger.LogError(ex, $"Dropping Postgres schema(user) '{expression.SchemaName}' failed with exception :-("));
        }

        private void Process(CreateUserExpression expression)
        {
            if (Exists(ExtendedGeneratorField.GenerateUserExistsSql(expression.SchemaName)))
            {
                return;
            }

            var stopwatch = new StopWatch();
            Logger.LogSay($"Creating Postgres user/role '{expression.SchemaName}'...");
            stopwatch.Time(() => Process(ExtendedGeneratorField.Generate(expression)));
            Logger.LogSay($"Created Postgres user/role '{expression.SchemaName}'...");
            Logger.LogElapsedTime(stopwatch.ElapsedTime());
        }

        private void Process(DeleteUserExpression expression)
        {
            if (!Exists(ExtendedGeneratorField.GenerateUserExistsSql(expression.SchemaName)))
            {
                return;
            }

            var stopwatch = new StopWatch();
            Logger.LogSay($"Dropping Postgres user/role '{expression.SchemaName}'...");
            stopwatch.Time(() => Process(ExtendedGeneratorField.Generate(expression)));
            Logger.LogSay($"Dropping Postgres user/role '{expression.SchemaName}'...");
            Logger.LogElapsedTime(stopwatch.ElapsedTime());
        }

        protected override void Process(string sql)
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

            using (var command = CreateCommand(sql))
            {
                try
                {
                    command.ExecuteNonQuery();
                    Logger.LogSql(sql);
                }
                catch (Exception ex)
                {
                    using (var message = new StringWriter())
                    {
                        message.WriteLine("An error occurred executing the following sql:");
                        message.WriteLine(sql);
                        message.WriteLine("The error was {0}", ex.Message);

                        throw new Exception(message.ToString(), ex);
                    }
                }
            }
        }
    }
}