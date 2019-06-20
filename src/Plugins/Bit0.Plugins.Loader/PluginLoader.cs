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

            var dlls = PluginsFolders.SelectMany(d => d.GetFiles("*.dll", SearchOption.AllDirectories));
            var filteredDlls = dlls.Where(file => file.Name.ToLowerInvariant().Contains("plugin".ToLowerInvariant()));
            var types = filteredDlls.SelectMany(file => Assembly.LoadFile(file.FullName).GetTypes());
            var filteredTypes = types.Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

            plugins = filteredTypes.Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin).ToList();

            logger.LogInformation(new EventId(4001), $"Found {plugins.Count} plugins.");

            if (!plugins.Any())
            {
                var evId = new EventId(4004, "NoPluginsFound");
                logger.LogWarning(evId, "No plugins loaded.");
            }

            foreach (var plugin in plugins)
            {
                logger.LogInformation(new EventId(4002), $"Loading: {plugin.Info.FullId}");

                Plugins.Add(plugin.Info.FullId, plugin);

                logger.LogInformation(new EventId(4002), $"Loaded: {plugin.Info.FullId}");

                if (plugin.Info.Implementing != null)
                {
                    Implementations.Add(plugin.Info.Implementing, plugin);
                    logger.LogInformation(new EventId(4002), $"{plugin.Info.FullId} => {plugin.Info.Implementing.FullName}");
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
