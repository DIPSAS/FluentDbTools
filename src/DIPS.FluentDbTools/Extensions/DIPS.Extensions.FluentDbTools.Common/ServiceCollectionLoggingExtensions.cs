using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DIPS.Extensions.FluentDbTools.Common
{
    public static class ServiceCollectionLoggingExtensions
    {
        public static IServiceCollection UseDefaultLogging(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddLogging(configure => configure
                    .AddConsole());
        }
    }
}