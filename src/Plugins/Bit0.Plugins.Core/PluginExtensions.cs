using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;

namespace Bit0.Plugins.Core
{
    public static class PluginExtensions
    {
        public static PluginAttribute GetInfo(this IPlugin plugin)
        {
            return plugin.GetType().GetCustomAttribute<PluginAttribute>();
        }

        public static IServiceCollection LoadPlugins(this IServiceCollection services, DirectoryInfo pluginsDir, ILogger<IPluginLoader> logger)
        {
            var pluginLoader = new PluginLoader(pluginsDir, logger);

            foreach (var plugin in pluginLoader.Plugins.Values)
            {
                plugin.Register(services);
                logger.LogInformation(new EventId(4003), $"Registered: {plugin.FullId}");
            }

            services.AddSingleton<IPluginLoader>(pluginLoader);
            
            return services;
        }
    }
}
