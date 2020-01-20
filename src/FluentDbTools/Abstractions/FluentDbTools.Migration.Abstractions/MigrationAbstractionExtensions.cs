using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentMigrator.Builders;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Useful extension method for FluentMigration library
    /// </summary>
    public static class MigrationAbstractionExtensions
    {
        /// <summary>
        /// Execute Sql. If Sql contains line-breaks, sql is split into smaller sql statements and execute them separately
        /// </summary>
        /// <param name="migration"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this FluentMigrator.Migration migration, string sql)
        {

            ExecuteSql(migration.Execute.Sql, sql);
        }

        /// <summary>
        /// Execute Sql. If Sql contains line-breaks, sql is split into smaller sql statements and execute them separately
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this IExtendedMigrationProcessor processor, string sql)
        {
            ExecuteSql(processor.ProcessSql, sql);
        }


        /// <summary>
        /// Execute Sql. If Sql contains line-breaks, sql is split into smaller sql statements and execute them separately
        /// </summary>
        /// <param name="action"></param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this Action<string> action, string sql)
        {
            if (action == null)
            {
                return;
            }

            if (!sql.Contains("\n") &&
                !sql.Contains("\r") &&
                !sql.Contains(";"))
            {
                action(sql);
                return;
            }

            foreach (var sqlStatement in sql.ExtractSqlStatements())
            {
                action(sqlStatement);
            }
        }


        /// <summary>
        /// Replace elements by conventions<br/>
        /// Replace {SchemaName} with <paramref name="schemaName"/> or <see cref="IDbConfigSchemaTargets.Schema"/><br/>
        /// Replace {SchemaPrefixId} with <paramref name="schemaPrefixId"/> or  <see cref="IDbConfigSchemaTargets.GetSchemaPrefixId()"/><br/>
        /// Replace {SchemaPrefixUniqueId} with <paramref name="schemaPrefixUniqueId"/> or  <see cref="IDbMigrationConfig.GetSchemaPrefixUniqueId()"/><br/>
        /// Replace {MigrationName} with <paramref name="migrationName"/> or  <see cref="IDbMigrationConfig.GetMigrationName()"/><br/>
        /// Replace {User} with <paramref name="migrationName"/> or  <see cref="IDbMigrationConfig.GetMigrationName()"/><br/>
        /// </summary>
        /// <param name="migrationConfig"></param>
        /// <param name="sql"></param>
        /// <param name="schemaName"></param>
        /// <param name="schemaPrefixId"></param>
        /// <param name="schemaPrefixUniqueId"></param>
        /// <param name="migrationName"></param>
        /// <returns></returns>
        public static string PrepareSql(this IDbMigrationConfig migrationConfig, string sql, string schemaName = null, string schemaPrefixId = null, string schemaPrefixUniqueId = null, string migrationName = null)
        {
            if (sql == null)
            {
                return null;
            }

            schemaName = schemaName ?? migrationConfig.Schema;
            schemaPrefixId = schemaPrefixId ?? migrationConfig.GetSchemaPrefixId();
            schemaPrefixUniqueId = schemaPrefixUniqueId ?? migrationConfig.GetSchemaPrefixUniqueId();
            migrationName = migrationName ?? migrationConfig.GetMigrationName();
            return sql
                .ReplaceIgnoreCase("{MigrationName}", migrationName)
                .ReplaceIgnoreCase("{User}", migrationName)
                .ReplaceIgnoreCase("{SchemaName}", schemaName?.ToUpper())
                .ReplaceIgnoreCase("{SchemaPrefixId}", schemaPrefixId)
                .ReplaceIgnoreCase("{SchemaPrefixUniqueId}", schemaPrefixUniqueId);
        }



        /// <summary>
        /// Resolve all <see cref="IMigrationExpression"/>'s from a <see cref="ExpressionBuilderBase&lt;T&gt;"/><br/>
        /// <br/>
        /// Exception <see cref="FieldAccessException"/> wil be thrown if <see cref="ExpressionBuilderBase&lt;T&gt;"/> is missing the <see cref="IMigrationContext"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="FieldAccessException"></exception>
        public static IList<IMigrationExpression> GetMigrationExpressions<T>(
            this ExpressionBuilderBase<T> builder) where T : class, IMigrationExpression
        {
            var context = builder.GetMigrationContextFromBuilder();
            return context?.Expressions as IList<IMigrationExpression> ?? throw new FieldAccessException($" Field of type 'IMigrationContext' not found in type[{builder.GetType().Name}]");
        }

        /// <summary>
        /// Replace a <see cref="IMigrationExpression"/> in the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="itemToBeReplaced"></param>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public static IList<IMigrationExpression> Replace(this IList<IMigrationExpression> list,
            IMigrationExpression itemToBeReplaced,
            IMigrationExpression newItem)
        {
            var index = list.IndexOf(itemToBeReplaced);
            if (index < 0)
            {
                list.Add(newItem);
            }
            else
            {
                list[index] = newItem;
            }

            return list;
        }

        /// <summary>
        /// Insert a <see cref="IMigrationExpression"/> into the list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="indexItem"></param>
        /// <param name="newItemToInsert"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public static IList<IMigrationExpression> Insert(this IList<IMigrationExpression> list,
            IMigrationExpression indexItem,
            IMigrationExpression newItemToInsert,
            bool after = false)
        {
            if (list?.Contains(newItemToInsert) ?? true)
            {
                return list;
            }

            var index = list.IndexOf(indexItem);
            if (index < 0)
            {
                list.Add(newItemToInsert);
            }
            else
            {
                if (after)
                {
                    index++;
                }
                list.Insert(index, newItemToInsert);
            }

            return list;
        }

        /// <summary>
        /// Return all <see cref="AlterColumnExpression"/> from the collection
        /// </summary>
        /// <param name="migrationExpressions"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static IList<ColumnDefinition> GetAlterColumns(
            this ICollection<IMigrationExpression> migrationExpressions, string tableName)
        {
            return migrationExpressions
                .Select(x => x as AlterColumnExpression)
                .Where(x => x?.TableName == tableName)
                .Select(x => x.Column).ToList();
        }

        /// <summary>
        /// Return true if <paramref name="type"/>(<see cref="FieldInfo.FieldType"/> )is of type specified by <paramref name="fieldType"/> argument
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static bool IsFieldType(this FieldInfo type, Type fieldType)
        {
            return type?.FieldType == fieldType;
        }

        /// <summary>
        /// Resolve Database operation from <paramref name="expression"/><br/>
        /// <br/>
        /// i.e: AlterColumnExpression => AlterColumn, CreateTableExpression => CreateTable, AlterTableExpression => AlterTable
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetDbOperationFromExpression(this IMigrationExpression expression)
        {
            var dbOperation = expression.GetType().Name;
            var index = dbOperation.IndexOf("Expression", StringExtensions.CurrentIgnoreCaseStringComparison);
            if (index > -1)
            {
                dbOperation = dbOperation.Substring(0, index);
            }

            return dbOperation;
        }

        /// <summary>
        /// Resolve the registered implementation of <see cref="IDbMigrationConfig"/> from the DependencyInjection container <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IDbMigrationConfig GetDbMigrationConfig(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IDbMigrationConfig>();
        }


        /// <summary>
        /// Resolve table-value by <paramref name="template"/> and <paramref name="tableName"/><br/>
        /// i.e: With <paramref name="template"/> equal to "tables:{<paramref name="tableName"/>}:globalId" and <paramref name="tableName"/> equal to "Person" (and ie. <paramref name="migrationConfig"/>.GetSchemaPrefixId() (<see cref="IDbConfigSchemaTargets.GetSchemaPrefixId()"/>) returns "EX" <br/>
        /// => Will search configuration:<br/>
        /// - "database:migration:tables:Person:globalId"<br/>
        /// - "database:migration:tables:EXPerson:globalId"<br/> 
        /// - "database:tables:Person:globalId"<br/> 
        /// - "database:tables:EXPerson:globalId"<br/> 
        /// </summary>
        /// <param name="migrationConfig"></param>
        /// <param name="template">string template containing {tableName}</param>
        /// <param name="tableName"></param>
        /// <param name="fallbackTemplates"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        public static string GetTableConfigValue(this IDbMigrationConfig migrationConfig, string template, string tableName, params string[] fallbackTemplates)
        {
            if (migrationConfig == null)
            {
                return null;
            }
            var defaultKey = template.ReplaceIgnoreCase("{tableName}", tableName);
            var alternativeKey = template.ReplaceIgnoreCase("{tableName}", tableName.TrimPrefixName(migrationConfig.GetSchemaPrefixId()));
            var alternativeKey2 = template.ReplaceIgnoreCase("{tableName}", tableName.GetPrefixedName(migrationConfig.GetSchemaPrefixId()));

            var list = new List<string>(new[] { defaultKey, alternativeKey, alternativeKey2 }.Distinct())  ;
            if (fallbackTemplates.Any())
            {
                list.AddRange(fallbackTemplates.SelectMany(x => new[]
                {
                    x.ReplaceIgnoreCase("{tableName}", tableName),
                    x.ReplaceIgnoreCase("{tableName}", tableName.TrimPrefixName(migrationConfig.GetSchemaPrefixId())),
                    x.ReplaceIgnoreCase("{tableName}", tableName.GetPrefixedName(migrationConfig.GetSchemaPrefixId())),
                }.Distinct()));
            }

            var keys = list.Distinct().ToArray();

            return migrationConfig.GetAllMigrationConfigValues()?.GetValue(keys) ??
                    migrationConfig.GetDbConfig().GetAllDatabaseConfigValues()?.GetValue(keys);
        }

    }
}