using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FluentDbTools.Extensions.MSDependencyInjection
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