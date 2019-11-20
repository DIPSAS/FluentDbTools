using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Migration.Contracts.MigrationExpressions.Execute
{
    internal class InternalExecuteExpressionRoot : IExecuteExpressionRoot
    {
        private readonly IMigrationContext _context;
        private IEnumerable<ICustomSqlTitleConverter> TitleConverters;

        public InternalExecuteExpressionRoot(IMigrationContext context)
        {
            _context = context;
            TitleConverters = context.ServiceProvider.GetServices<ICustomSqlTitleConverter>();
        }

        /// <inheritdoc />
        public void Sql(string sqlStatement)
        {
            var expression = new InternalExecuteSqlStatementExpression
            {
                SqlStatement = sqlStatement,
                AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
            };
            _context.Expressions.Add(expression);
        }

        /// <inheritdoc />
        public void Script(string pathToSqlScript, IDictionary<string, string> parameters)
        {
            var expression = new InternalExecuteSqlScriptExpression
            {
                SqlScript = pathToSqlScript,
                Parameters = parameters,
                AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
            };

            _context.Expressions.Add(expression);
        }

        /// <inheritdoc />
        public void Script(string pathToSqlScript)
        {
            var expression = new InternalExecuteSqlScriptExpression
            {
                SqlScript = pathToSqlScript,
                AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
            };
            _context.Expressions.Add(expression);
        }

        /// <inheritdoc />
        public void WithConnection(Action<IDbConnection, IDbTransaction> operation)
        {
            var expression = new PerformDBOperationExpression { Operation = operation };
            _context.Expressions.Add(expression);
        }

        /// <inheritdoc />
        public void EmbeddedScript(string embeddedSqlScriptName)
        {
            var embeddedResourceProviders = _context.ServiceProvider.GetService<IEnumerable<IEmbeddedResourceProvider>>();
            if (embeddedResourceProviders == null)
            {
#pragma warning disable 612
                Debug.Assert(_context.MigrationAssemblies != null, "_context.MigrationAssemblies != null");
                var expression = new InternalExecuteEmbeddedSqlScriptExpression(_context.MigrationAssemblies)
                {
                    SqlScript = embeddedSqlScriptName,
                    AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
                };
#pragma warning restore 612
                _context.Expressions.Add(expression);
            }
            else
            {
                var expression = new InternalExecuteEmbeddedSqlScriptExpression(embeddedResourceProviders)
                {
                    SqlScript = embeddedSqlScriptName,
                    AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
                };
                _context.Expressions.Add(expression);
            }
        }

        /// <inheritdoc />
        public void EmbeddedScript(string embeddedSqlScriptName, IDictionary<string, string> parameters)
        {
            var embeddedResourceProviders = _context.ServiceProvider.GetService<IEnumerable<IEmbeddedResourceProvider>>();
            InternalExecuteEmbeddedSqlScriptExpression expression;
            if (embeddedResourceProviders == null)
            {
#pragma warning disable 612
                Debug.Assert(_context.MigrationAssemblies != null, "_context.MigrationAssemblies != null");
                expression = new InternalExecuteEmbeddedSqlScriptExpression(_context.MigrationAssemblies)
                {
                    SqlScript = embeddedSqlScriptName,
                    Parameters = parameters,
                    AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
                };
#pragma warning restore 612
            }
            else
            {
                expression = new InternalExecuteEmbeddedSqlScriptExpression(embeddedResourceProviders)
                {
                    SqlScript = embeddedSqlScriptName,
                    Parameters = parameters,
                    AdditionalSqlTitleConverterFunc = AdditionalSqlTitleConverter
                };
            }

            _context.Expressions.Add(expression);
        }

        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        private string AdditionalSqlTitleConverter(string sql)
        {
            if (TitleConverters == null || !TitleConverters.Any())
            {
                return sql;
            }

            foreach (var customSqlTitleConverter in TitleConverters)
            {
                sql = customSqlTitleConverter?.ConvertToTitle(sql) ?? sql;
            }

            return sql;
        }
    }

    /// <summary>
    /// Convert sql to a more readable title
    /// </summary>
    public interface ICustomSqlTitleConverter
    {
        /// <summary>
        /// Convert sql to a more readable title
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string ConvertToTitle(string sql);
    }
}