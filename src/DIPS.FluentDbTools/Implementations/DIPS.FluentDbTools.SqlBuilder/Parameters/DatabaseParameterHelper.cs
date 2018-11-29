using System;
using System.Linq;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.SqlBuilder.Abstractions.Parameters;
using DIPS.FluentDbTools.SqlBuilder.Common;
using DIPS.FluentDbTools.SqlBuilder.DbTypeTranslators;

namespace DIPS.FluentDbTools.SqlBuilder.Parameters
{
    internal class DatabaseParameterHelper : IDatabaseParameterHelper
    {
        private readonly IDbConfig DbConfig;

        
        public DatabaseParameterHelper(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
            DatabaseType = dbConfig?.DbType ?? SupportedDatabaseTypes.Postgres;
            DbTypeTranslator = DatabaseType.GetDbTypeTranslator();
        }

        public SupportedDatabaseTypes DatabaseType { get; }
        public IDbTypeTranslator DbTypeTranslator { get; }

        public string WithParameters(params string[] parameters)
        {
            var prefix = GetParameterPrefix();

            parameters = parameters.Select(x => $"{prefix}{x.Replace(prefix, "")}").ToArray();

            return string.Join(", ", parameters);
        }

        public string WithNextSequence(string sequenceName = null)
        {
            if (string.IsNullOrEmpty(sequenceName))
            {
                sequenceName = "default_seq";
            }
            switch (DatabaseType)
            {
                case SupportedDatabaseTypes.Oracle:
                    return $"{sequenceName}.nextval";
                case SupportedDatabaseTypes.Postgres:
                    return $"nextval('{sequenceName}')";
                default:
                    return $"nextval('{sequenceName}')";
            }
        }

        public string WithNextTableSequence<T>(string postfix = "_seq")
        {
            return WithNextSequence(typeof(T).Name.ToLower() + postfix);
        }

        public string AsForeignKey<T>(string postfix = "Id")
        {
            return typeof(T).Name + postfix;
        }

        public object WithGuidParameterValue(Guid guid)
        {
            if (DatabaseType == SupportedDatabaseTypes.Oracle)
            {
                return guid.ToByteArray();
            }

            return guid;
        }

        public string GetParameterPrefix()
        {
            return SqlBuilderHelper.GetParameterPrefixIfNull(DbConfig?.GetParameterPrefix());
        }
    }
}