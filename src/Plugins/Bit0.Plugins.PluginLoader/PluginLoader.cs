using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bit0.Plugins.PluginLoader
{
    public class PluginLoader : IPluginLoader
    {
        public PluginLoader(IEnumerable<DirectoryInfo> pluginsFolders, ILogger<IPluginLoader> logger)
        {

            logger.LogInformation(new EventId(4000), "Start loading plugins");

            PluginsFolders = pluginsFolders.Where(d => d.Exists);
            IList<IPlugin> plugins = new List<IPlugin>();

            plugins = PluginsFolders.SelectMany(d => d.GetFiles("*.dll", SearchOption.AllDirectories))
                .Where(file => file.Name.ToLowerInvariant().Contains("plugin".ToLowerInvariant()))
                .SelectMany(file => Assembly.LoadFile(file.FullName).GetTypes())
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && t.IsAbstract)
                .Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin)
                .ToList();

            logger.LogInformation(new EventId(4001), $"Found {plugins.Count} plugins.");

            if (!plugins.Any())
            {
                var exp = new NoPluginsFoundException(pluginsFolders);
                logger.LogWarning(exp.EventId, exp, exp.Message);
            }

            foreach (var plugin in plugins)
            {
                logger.LogInformation(new EventId(4002), $"Loading: {plugin.FullId}");

                Plugins.Add(plugin.FullId, plugin);

                logger.LogInformation(new EventId(4002), $"Loaded: {plugin.FullId}");

                if (plugin.Implementing != null)
                {
                    Implementations.Add(plugin.Implementing, plugin);
                    logger.LogInformation(new EventId(4002), $"{plugin.FullId} => {plugin.Implementing.FullName}");
                }
            }
        }

        public IEnumerable<DirectoryInfo> PluginsFolders { get; }

        public IDictionary<String, IPlugin> Plugins { get; } = new Dictionary<String, IPlugin>();
        public IDictionary<Type, IPlugin> Implementations { get; } = new Dictionary<Type, IPlugin>();

        public IPlugin GetPlugin(String id, String version)
        {
            return Plugins[$"{id}@{version}"];
        }
    }
}
