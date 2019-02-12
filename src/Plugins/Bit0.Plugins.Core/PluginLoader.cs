using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bit0.Plugins.Core
{
    public class PluginLoader : IPluginLoader
    {
        public PluginLoader(DirectoryInfo pluginsFolder, ILogger<IPluginLoader> logger)
        {
            PluginsFolder = pluginsFolder;

            logger.LogInformation(new EventId(4000), "Start loading plugins");

            var plugins = pluginsFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                .SelectMany(file => Assembly.LoadFile(file.FullName).GetTypes())
                .Where(t => typeof(IPlugin).IsAssignableFrom(t))
                .Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin)
                .ToList();
            
            logger.LogInformation(new EventId(4001), $"Found {plugins.Count} plugins.");

            foreach (var plugin in plugins)
            {
                logger.LogInformation(new EventId(4002), $"Loading: {plugin.FullId}");
                Plugins.Add(plugin.FullId, plugin);
                logger.LogInformation(new EventId(4002), $"Loaded: {plugin.FullId}");
            }
        }

        public DirectoryInfo PluginsFolder { get; }

        public IDictionary<String, IPlugin> Plugins { get; } = new Dictionary<String, IPlugin>();

        public IPlugin GetPlugin(String id, String version)
        {
            return Plugins[$"{id}@{version}"];
        }
    }
}
