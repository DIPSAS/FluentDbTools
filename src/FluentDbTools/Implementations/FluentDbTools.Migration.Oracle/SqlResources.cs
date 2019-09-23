using System;
using System.IO;
using System.Reflection;
using FluentDbTools.Migration.Oracle.CustomProcessor;
using FluentDbTools.Migration.Oracle.CustomProcessor.Resources;

namespace FluentDbTools.Migration.Oracle
{
    /// <summary>
    /// Sql resource front class for Sql embedded resources
    /// </summary>
    public static class SqlResources
    {
        /// <summary>
        /// Returns the default CreateSchema sql
        /// </summary>
        public static string CreateSchemaSql =>
            GetStringFromLocalEmbeddedResource($"{_ResourceHelper.Location}.{nameof(CreateSchemaSql)}.sql");

        /// <summary>
        /// Returns the default LogonScript sql
        /// </summary>
        public static string LogonScriptSql =>
            GetStringFromLocalEmbeddedResource($"{_ResourceHelper.Location}.{nameof(LogonScriptSql)}.sql");


        /// <summary>
        /// Returns the default SchemaPrefix sql
        /// </summary>
        public static string SchemaPrefixSql =>
            GetStringFromLocalEmbeddedResource($"{_ResourceHelper.Location}.{nameof(SchemaPrefixSql)}.sql");

        /// <summary>
        /// Load string from Embedded resource from <paramref name="assembly"/> at location <paramref name="location"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetStringFromEmbeddedResource(this Assembly assembly, string location)
        {
            string content;

            using (var stream = assembly.GetManifestResourceStream(location))
            {
                if (stream == null)
                {
                    return null;
                }
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }

        /// <summary>
        /// Load string from Embedded resource from <paramref name="type"/>.Assembly at location <paramref name="location"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetStringFromEmbeddedResource(Type type, string location)
        {
            return type.Assembly.GetStringFromEmbeddedResource(location);
        }

        /// <summary>
        /// Load string from Embedded resource from this.Assembly at location <paramref name="location"/>
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string GetStringFromLocalEmbeddedResource(string location)
        {
            return typeof(SqlResources).Assembly.GetStringFromEmbeddedResource(location);
        }

    }
}