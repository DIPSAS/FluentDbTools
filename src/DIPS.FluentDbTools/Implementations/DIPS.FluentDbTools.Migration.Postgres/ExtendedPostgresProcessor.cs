using System.Data;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Migration.Abstractions;
using DIPS.FluentDbTools.Migration.Common;
using FluentMigrator.Expressions;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Generators.Postgres;
using FluentMigrator.Runner.Helpers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Postgres;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DIPS.FluentDbTools.Migration.Postgres
{
    internal class ExtendedPostgresProcessor : PostgresProcessor, IExtendedMigrationProcessor<ExtendedPostgresProcessor>
    {
        private readonly IExtendedMigrationGenerator ExtendedGeneratorField;
        private readonly IDbConfig DbConfig;

        public ExtendedPostgresProcessor(
            IDbConfig dbConfig,
            PostgresQuoter quoter,
            PostgresDbFactory factory,
            PostgresGenerator generator,
            ILogger<ExtendedPostgresProcessor> logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationGenerator<ExtendedPostgresGenerator> extendedGenerator)
            : base(factory, generator, logger, options, connectionStringAccessor)
        {
            ExtendedGeneratorField = extendedGenerator;
            DbConfig = dbConfig;

            if (dbConfig.DbType.GetProcessorId() == ProcessorIds.PostgresProcessorId)
            {
                PostgresDatabaseCreator.CreateDatabase(dbConfig);
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

        public override void Process(DeleteSchemaExpression expression)
        {
            this.ExecuteCodeBlockUntilNoExeception(() =>
                {
                    if (!SchemaExists(expression.SchemaName))
                    {
                        return;
                    }

                    Logger.LogSay($"Dropping Postgres schema(user) '{expression.SchemaName}'...");
                    Process(ExtendedGeneratorField.Generate(expression));
                    Logger.LogSay($"Dropped Postgres schema(user) '{expression.SchemaName}'...");

                    Process(new DeleteUserExpression(expression));
                    PostgresDatabaseCreator.DropDatabase(DbConfig);
                },
                ex => Logger.LogError(ex, $"Dropping Postgres schema(user) '{expression.SchemaName}' failed with exception :-("));
        }

        private void Process(CreateUserExpression expression)
        {
            if (Exists(ExtendedGeneratorField.GenerateUserExistsSql(expression.SchemaName)))
            {
                return;
            }

            Logger.LogSay($"Creating Postgres user '{expression.SchemaName}'...");
            Process(ExtendedGeneratorField.Generate(expression));
            Logger.LogSay($"Created Postgres user '{expression.SchemaName}'...");

        }

        private void Process(DeleteUserExpression expression)
        {
            if (!Exists(ExtendedGeneratorField.GenerateUserExistsSql(expression.SchemaName)))
            {
                return;
            }

            Logger.LogSay($"Dropping Postgres user '{expression.SchemaName}'...");
            Process(ExtendedGeneratorField.Generate(expression));
            Logger.LogSay($"Dropping Postgres user '{expression.SchemaName}'...");
        }

    }
}