using System;
using System.Collections.Generic;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Example.Common;
using FluentDbTools.Example.Migration.MigrationModels;
using FluentDbTools.Extensions.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace FluentDbTools.Example.Migration
{
    public static class MigrationBuilder
    {
        private static IEnumerable<Assembly> MigrationAssemblies => new[] {typeof(AddPersonTable).Assembly};
        
        public static IServiceProvider BuildMigration(
            SupportedDatabaseTypes databaseType,
            Dictionary<string, string> overrideConfig = null)
        {
            return new ServiceCollection()
                .ConfigureWithMigration(MigrationAssemblies)
                .UseExampleConfiguration(
                    databaseType,
                    overrideConfig)
                .BuildServiceProvider();
        }
    }
}