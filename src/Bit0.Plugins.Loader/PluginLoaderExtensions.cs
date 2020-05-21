using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Loader
{
    public static class PluginLoaderExtensions
    {
        public static IServiceCollection LoadPlugins(this IServiceCollection services, IEnumerable<DirectoryInfo> pluginDirectories, ILogger<IPluginLoader> logger)
        {
            var pluginLoader = new PluginLoader(pluginDirectories, logger);
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
