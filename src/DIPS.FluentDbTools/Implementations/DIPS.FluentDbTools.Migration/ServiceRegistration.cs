using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DIPS.Extensions.FluentDbTools.Common;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace DIPS.FluentDbTools.Migration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection Register(IServiceCollection serviceCollection, IEnumerable<Assembly> assembliesWithMigrationModels)
        {
            return serviceCollection
                .AddFluentMigratorCore()
                .ConfigureFluentMigrationWithDatabaseType()
                .ConfigureRunner(builder => builder
                    .WithMigrationsIn(assembliesWithMigrationModels.ToArray()))
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .Register(Common.ServiceRegistration.Register);
        }
    }
}