using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DIPS.Extensions.FluentDbTools.MSDependencyInjection
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