using System;
using System.IO;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Oracle.CustomProcessor;
using FluentDbTools.Migration.Oracle.CustomProcessor.Resources;

namespace FluentDbTools.Migration.Oracle
{
    /// <summary>
    /// Sql resource front class for Sql embedded resources
    /// </summary>
    public static class SqlResources
    {
        private static readonly Assembly CurrentAssembly = typeof(SqlResources).Assembly;
        /// <summary>
        /// Returns the default LogonScript sql
        /// </summary>
        public static string LogonScriptSql =>
            CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(LogonScriptSql)}.txt");


        /// <summary>
        /// Returns the default SchemaPrefix sql
        /// </summary>
        public static string SchemaPrefixSql =>
            CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(SchemaPrefixSql)}.txt");

    }
}