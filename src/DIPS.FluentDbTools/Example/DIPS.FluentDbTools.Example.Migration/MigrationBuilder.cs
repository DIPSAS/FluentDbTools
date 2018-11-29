using System;
using System.Collections.Generic;
using System.Reflection;
using DIPS.FluentDbTools.Example.Common;
using DIPS.FluentDbTools.Example.Migration.MigrationModels;
using DIPS.Extensions.FluentDbTools.Migration;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Example.Migration
{
    public static class MigrationBuilder
    {
        private static IEnumerable<Assembly> MigrationAssemblies => new[] {typeof(AddPersonTable).Assembly};
        
        public static IServiceProvider BuildMigration(
            Dictionary<string, string> overrideConfig = null, 
            string additionalJsonConfig = null)
        {
            return new ServiceCollection()
                .ConfigureWithMigration(MigrationAssemblies)
                .UseExampleConfiguration(
                    overrideConfig, 
                    additionalJsonConfig)
                .BuildServiceProvider();
        }
    }
}