using System.Collections.Generic;
using System.Data;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Common;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FluentDbTools.Migration.Oracle
{
    internal class ExtendedOracleManagedProcessor : OracleManagedProcessor, IExtendedMigrationProcessor<ExtendedOracleManagedProcessor>
    {
        private readonly IExtendedMigrationProcessor ExtendedMigrationProcessor;

        public ExtendedOracleManagedProcessor(
            OracleManagedDbFactory factory,
            OracleGenerator generator,
            ILogger<ExtendedOracleManagedProcessor> logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationProcessor<ExtendedOracleProcessorBase> extendedMigrationProcessor
        )
            : base(factory, generator, logger, options, connectionStringAccessor)
        {
            ExtendedMigrationProcessor = extendedMigrationProcessor;
        }

        public override IList<string> DatabaseTypeAliases => new List<string> { ProcessorIds.OracleProcessorId };
        public override string DatabaseType => ProcessorIds.OracleProcessorId;


        public override void Process(CreateTableExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateSequenceExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }


        public override void Process(CreateConstraintExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateIndexExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public void ProcessSql(string sql)
        {
            ExtendedMigrationProcessor.ProcessSql(sql);
        }

        public override void Process(CreateSchemaExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(DeleteSchemaExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(DeleteDataExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(DeleteTableExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public IDbConnection GetMigrationDbConnection()
        {
            return Connection;
        }

        protected override void Process(string sql)
        {
            ProcessSql(sql);
        }
    }
}