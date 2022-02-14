using System.Data;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator;
using FluentMigrator.Expressions;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Important database Processing methods<br/>
    /// Important database Exists methods
    /// Important database connections methods
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
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

        /// <summary>
        /// Create constraint process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateConstraintExpression expression);

        /// <summary>
        /// Create Foreign key Process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateForeignKeyExpression expression);


        /// <summary>
        /// Create index process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateIndexExpression expression);

        /// <summary>
        /// Create table process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateTableExpression expression);

        /// <summary>
        /// Create column process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateColumnExpression expression);


        /// <summary>
        /// Create sequence process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(CreateSequenceExpression expression);

        /// <summary>
        /// Delete table process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(DeleteTableExpression expression);
        
        /// <summary>
        /// Database operation process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(PerformDBOperationExpression expression);

        /// <summary>
        /// AlterColumn process method
        /// </summary>
        /// <param name="expression"></param>
        void Process(AlterColumnExpression expression);


        /// <summary>
        /// Return the default Migration IDbConnection
        /// </summary>
        /// <returns></returns>
        IDbConnection GetMigrationDbConnection();


        /// <summary>
        /// Process the sql (sql will be logged if logging is enable )
        /// </summary>
        /// <param name="sql"></param>
        void ProcessSql(string sql);

        /// <summary>
        /// Process the sql (sql with title will be logged if logging is enable )
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="logTitle"></param>
        void ProcessSql(string sql, string logTitle);

        /// <summary>
        /// Initialize method
        /// </summary>
        void Initialize();

        /// <summary>
        /// Start a transaction if No-Connection is turned off
        /// </summary>
        void ExtendedBeginTransaction();

        /// <summary>
        /// Commit the transaction if No-Connection is turned off
        /// </summary>
        void ExtendedCommitTransaction();

        /// <summary>
        /// Ensure that connection is Opened if No-Connection is turned off
        /// </summary>
        void ExtendedEnsureConnectionIsOpen();

        /// <summary>
        /// Ensure that connection is Closed if No-Connection is turned off
        /// </summary>
        void ExtendedEnsureConnectionIsClosed();

        /// <inheritdoc cref="IMigrationProcessor.ReadTableData"/>
        DataSet ReadTableData(string schemaName, string tableName);

        /// <inheritdoc cref="IMigrationProcessor.Read"/>
        DataSet Read(string template, params object[] args);

    }

    /// <summary>
    /// Represent <see cref="IExtendedMigrationProcessor"/> of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IExtendedMigrationProcessor<T> : IExtendedMigrationProcessor
    {
        /// <summary>
        /// Initialize method
        /// </summary>
        /// <param name="customMigrationProcessor"></param>
        void Initialize(ICustomMigrationProcessor customMigrationProcessor);
    }


    /// Represent <see cref="IExtendedMigrationProcessor"/> of Oracle
    public interface IExtendedMigrationProcessorOracle : IExtendedMigrationProcessor
    {
    }

}