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
        public PluginLoader(IEnumerable<DirectoryInfo> pluginsFolders, ILogger<IPluginLoader> logger)
        {

            logger.LogInformation(new EventId(4000), "Start loading plugins");

            PluginsFolders = pluginsFolders.Where(d => d.Exists);
            IList<IPlugin> plugins = new List<IPlugin>();

            plugins = PluginsFolders.SelectMany(d => d.GetFiles("*.dll", SearchOption.AllDirectories))
                .SelectMany(file => Assembly.LoadFile(file.FullName).GetTypes())
                .Where(t => typeof(IPlugin).IsAssignableFrom(t))
                .Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin)
                .ToList();

            if (!plugins.Any())
            {
                var exp = new DirectoryNotFoundException($"Could not find plugins.");
                logger.LogError(new EventId(4004), exp, exp.Message);
                throw exp;
            }

            logger.LogInformation(new EventId(4001), $"Found {plugins.Count} plugins.");

            foreach (var plugin in plugins)
            {
                logger.LogInformation(new EventId(4002), $"Loading: {plugin.FullId}");
                Plugins.Add(plugin.FullId, plugin);
                logger.LogInformation(new EventId(4002), $"Loaded: {plugin.FullId}");
            }
        }

        public IEnumerable<DirectoryInfo> PluginsFolders { get; }

        public IDictionary<String, IPlugin> Plugins { get; } = new Dictionary<String, IPlugin>();

        public IPlugin GetPlugin(String id, String version)
        {
            return Plugins[$"{id}@{version}"];
        }
    }
}
