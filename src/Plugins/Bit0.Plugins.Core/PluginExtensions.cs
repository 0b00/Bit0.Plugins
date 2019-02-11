using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bit0.Plugins.Core
{
    public static class PluginExtensions
    {
        public static PluginAttribute GetInfo(this IPlugin plugin)
        {
            return plugin.GetType().GetCustomAttribute<PluginAttribute>();
        }

        public static IServiceCollection LoadPlugins(this IServiceCollection services, DirectoryInfo pluginsDir)
        {
            var pluginLoader = new PluginLoader(pluginsDir);

            foreach (var plugin in pluginLoader.Plugins.Values)
            {
                plugin.Register(services);
            }

            services.AddTransient<IPluginLoader>(factory => pluginLoader);
            
            return services;
        }
    }
}
