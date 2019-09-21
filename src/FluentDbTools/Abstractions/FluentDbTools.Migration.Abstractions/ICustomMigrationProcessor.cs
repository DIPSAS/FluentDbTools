using System;
using System.Collections.Generic;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Model;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Custom migration utility methods
    /// </summary>
    public interface ICustomMigrationProcessor
    {
        /// <summary>
        /// Will be called when Migration running a new assembly<br/>
        /// </summary>
        /// <param name="migrationMetadata"></param>
        void MigrationMetadataChanged(IMigrationMetadata migrationMetadata);
        /// <summary>
        /// Can be used to setup a common Sql execute method
        /// </summary>
        /// <param name="executeSqlAction"></param>
        void ConfigureSqlExecuteAction(Action<string> executeSqlAction);

        /// <summary>
        /// Can be used to expand tables with fixed columns
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IList<ColumnDefinition> GetDefaultColumns(string tableName = null);

        /// <summary>
        /// Can be used to implementing custom logging on all table operations
        /// </summary>
        /// <param name="expression"></param>
        void Process(IChangeLogTabledExpression expression);

        /// <summary>
        /// Will be called just after the Schema is created<br/>
        /// Can be used to execute custom grants or other database operations.
        /// </summary>
        /// <param name="expression"></param>
        void ProcessAfter(ICreateSchemaWithPrefixExpression expression);

        /// <summary>
        /// Will be called just after the Schema is deleted/dropped <br/>
        /// Can be used to do database operation in other (dependent) schema.
        /// </summary>
        /// <param name="expression"></param>
        void ProcessAfter(IDropSchemaWithPrefixExpression expression);

        /// <summary>
        /// Will be called just before schema is generated <br/>
        /// Can be used to generate a custom CreateSchema Sql
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        string GenerateSql(ICreateSchemaWithPrefixExpression expression);
    }

    /// <summary>
    /// Custom migration utility methods for IMigrationProcessor
    /// </summary>
    public interface ICustomMigrationProcessor<T> : ICustomMigrationProcessor where T : IMigrationProcessor
    {
    }
}