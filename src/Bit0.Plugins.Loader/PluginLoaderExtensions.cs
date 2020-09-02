using Bit0.Plugins.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bit0.Plugins.Loader
{
    public static class PluginLoaderExtensions
    {
        public static IServiceCollection LoadPlugins(this IServiceCollection services, IPluginOptions options)
        {
            var provider = services.BuildServiceProvider();
            var logger = provider.GetService<ILogger<IPluginLoader>>();

            if (logger == null)
            {
                logger = new LoggerFactory().CreateLogger<IPluginLoader>();
            }

            var pluginLoader = new PluginLoader(options, logger);
            foreach (var plugin in pluginLoader.Plugins.Values)
            {
                services = plugin.Register(services);
                logger.LogInformation(new EventId(4003), $"Registered: {plugin.Info.FullId}");
            }
            services.AddSingleton<IPluginLoader>(pluginLoader);

            return services;
        }
    }
}
