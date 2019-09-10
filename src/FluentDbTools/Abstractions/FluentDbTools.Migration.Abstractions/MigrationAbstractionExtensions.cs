using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Migration.Abstractions
{
    public static class MigrationAbstractionExtensions
    {
        public static IList<IMigrationExpression> GetMigrationExpressions<T>(
            this ExpressionBuilderBase<T> builder) where T : class, IMigrationExpression
        {
            var context = builder.GetMigrationContextFromBuilder();
            return context?.Expressions as IList<IMigrationExpression> ?? throw new FieldAccessException($" Field of type 'IMigrationContext' not found in type[{builder.GetType().Name}]");
        }

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

        public static IList<ColumnDefinition> GetAlterColumns(
            this ICollection<IMigrationExpression> migrationExpressions, string tableName)
        {
            return migrationExpressions
                .Select(x => x as AlterColumnExpression)
                .Where(x => x?.TableName == tableName)
                .Select(x => x.Column).ToList();
        }

        public static bool IsFieldType(this FieldInfo type, Type fieldType)
        {
            return type?.FieldType == fieldType;
        }

        public static string GetDbOperation(this IMigrationExpression expression)
        {
            var dbOperation = expression.GetType().Name;
            var index = dbOperation.IndexOf("Expression", StringComparison.CurrentCultureIgnoreCase);
            if (index > -1)
            {
                dbOperation = dbOperation.Substring(0, index);
            }

            return dbOperation;
        }

        public static IDbMigrationConfig GetDbMigrationConfig(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IDbMigrationConfig>();
        }
    }
}