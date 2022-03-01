using System;
using System.IO;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Oracle;
using Test.FluentDbTools.Migration.Resources;

namespace Test.FluentDbTools.Migration
{
    /// <summary>
    /// Sql resource front class for Sql embedded resources
    /// </summary>
    public static class TestSqlResources
    {
        private static readonly Assembly CurrentAssembly = typeof(TestSqlResources).Assembly;

        /// <summary>
        /// Returns the default LargeScriptSql sql
        /// </summary>
        public static string LargeScriptSql => CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(LargeScriptSql)}.txt");

        /// <summary>
        /// Returns the default Large2ScriptSql sql
        /// </summary>
        public static string Large2ScriptSql => CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(Large2ScriptSql)}.txt");

        /// <summary>
        /// Returns the default Large2ScriptSql sql
        /// </summary>
        public static string Large3ScriptSql => CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(Large3ScriptSql)}.txt");

        /// <summary>
        /// Returns the default AdvanceScript sql
        /// </summary>
        public static string SmallScriptSql => CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(SmallScriptSql)}.txt");

        /// <summary>
        /// Returns the CreateTableWithErrorFilter sql
        /// </summary>
        public static string CreateTableWithErrorFilter => CurrentAssembly.GetStringFromEmbeddedResource($"{_ResourceHelper.Location}.{nameof(CreateTableWithErrorFilter)}.txt");
    }
}