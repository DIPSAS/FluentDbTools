using System.Data;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Processors.Oracle;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Important database Processing methods<br/>
    /// Important database Exists methods
    /// </summary>
    public interface IExtendedMigrationProcessor : IExtendedMigrationProcessorExists
    {
        /// <summary>
        /// Create schema Process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateSchemaExpression expression);

        /// <summary>
        /// Delete schema Process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(DeleteSchemaExpression expression);

        /// <summary>
        /// Delete data Process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(DeleteDataExpression expression);
        void Process(CreateConstraintExpression expression);
        void Process(CreateIndexExpression expression);
        void Process(CreateTableExpression expression);
        void Process(CreateSequenceExpression expression);
        void Process(DeleteTableExpression expression);
        void Process(PerformDBOperationExpression expression);

        IDbConnection GetMigrationDbConnection();
        void ProcessSql(string sql);
        void ProcessSql(string sql, string logTitle);
    }

    public interface IExtendedMigrationProcessor<T> : IExtendedMigrationProcessor
    {
        void Initialize(ICustomMigrationProcessor customMigrationProcessor);
    }


    public interface IExtendedMigrationProcessorOracle : IExtendedMigrationProcessor
    {
    }

}