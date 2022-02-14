using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Common;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators.Oracle;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.Processors.Oracle;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
[assembly: InternalsVisibleTo("FluentDbTools.Extensions.Migration")]
namespace FluentDbTools.Migration.Oracle
{
    internal class ExtendedOracleManagedProcessor : OracleManagedProcessor, IExtendedMigrationProcessor<ExtendedOracleManagedProcessor>
    {
        private readonly IExtendedMigrationProcessor<ExtendedOracleProcessorBase> ExtendedMigrationProcessor;
        private readonly IVersionTableMetaData VersionTableMetaData;

        public ExtendedOracleManagedProcessor(
            OracleManagedDbFactory factory,
            OracleGenerator generator,
            ILogger<ExtendedOracleManagedProcessor> logger,
            IOptionsSnapshot<ProcessorOptions> options,
            IConnectionStringAccessor connectionStringAccessor,
            IExtendedMigrationProcessor<ExtendedOracleProcessorBase> extendedMigrationProcessor,
            ICustomMigrationProcessor<OracleProcessor> customMigrationProcessor = null,
            IVersionTableMetaData versionTableMetaData = null
        )
            : base(factory, generator, logger, options, connectionStringAccessor)
        {
            ExtendedMigrationProcessor = extendedMigrationProcessor;
            VersionTableMetaData = versionTableMetaData;
            Initialize(customMigrationProcessor);
        }

        public override IList<string> DatabaseTypeAliases => new List<string> { ProcessorIds.OracleProcessorId };
        public override string DatabaseType => ProcessorIds.OracleProcessorId;

        public override DataSet ReadTableData(string schemaName, string tableName)
        {
            return ExtendedMigrationProcessor.ReadTableData(schemaName, tableName);
        }

        public override DataSet Read(string template, params object[] args)
        {
            return ExtendedMigrationProcessor.Read(template, args);
        }


        public override bool Exists(string template, params object[] args)
        {
            return ExtendedMigrationProcessor.IsExists(template, args);
        }

        public void ExtendedBeginTransaction()
        {
            ExtendedMigrationProcessor.ExtendedBeginTransaction();
        }

        public void ExtendedCommitTransaction()
        {
            ExtendedMigrationProcessor.ExtendedCommitTransaction();
        }

        public void ExtendedEnsureConnectionIsOpen()
        {
            ExtendedMigrationProcessor.ExtendedEnsureConnectionIsOpen();
        }

        public void ExtendedEnsureConnectionIsClosed()
        {
            ExtendedMigrationProcessor.ExtendedEnsureConnectionIsClosed();
        }

        public override void BeginTransaction()
        {
            ExtendedBeginTransaction();
        }

        public override void CommitTransaction()
        {
            ExtendedCommitTransaction();
        }

        protected override void EnsureConnectionIsClosed()
        {
            ExtendedEnsureConnectionIsClosed();
        }

        protected override void EnsureConnectionIsOpen()
        {
            ExtendedEnsureConnectionIsOpen();
        }

        public override void Process(PerformDBOperationExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateTableExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateColumnExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateSequenceExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        public override void Process(CreateForeignKeyExpression expression)
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

        public override void Process(AlterColumnExpression expression)
        {
            ExtendedMigrationProcessor.Process(expression);
        }

        protected override void Process(string sql)
        {
            ProcessSql(sql);
        }

        public void ProcessSql(string sql)
        {
            ExtendedMigrationProcessor.ProcessSql(sql);
        }

        public void ProcessSql(string sql, string logTitle)
        {
            ExtendedMigrationProcessor.ProcessSql(sql, logTitle);
        }

        public IDbConnection GetMigrationDbConnection()
        {
            return Connection;
        }

        public void Initialize()
        {
            ExtendedMigrationProcessor.Initialize();           
        }

        public void Initialize(ICustomMigrationProcessor customMigrationProcessor)
        {
            Initialize();
            ExtendedMigrationProcessor.Initialize(customMigrationProcessor);
        }

        public bool IsExists(string template, params object[] args)
        {
            return Exists(template, args);
        }

        public override void RollbackTransaction()
        {
            try
            {
                base.RollbackTransaction();
            }
            catch
            {
                //
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            try
            {
                base.Dispose(isDisposing);
            }
            catch
            {
                //
            }
        }
    }
}