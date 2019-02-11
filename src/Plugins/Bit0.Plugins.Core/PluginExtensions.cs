using Microsoft.Extensions.DependencyInjection;
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

        public static IServiceCollection AddPluginLoader(this IServiceCollection services, DirectoryInfo pluginsDir)
        {
            var pluginLoader = new PluginLoader(pluginsDir);
            services.AddTransient<IPluginLoader>(factory => pluginLoader);

            pluginLoader.RegisterAll(services);

            return services;
        }
    }
}
