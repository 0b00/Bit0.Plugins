using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bit0.Plugins.Core
{
    public class PluginLoader : IPluginLoader
    {
        public PluginLoader(DirectoryInfo pluginsFolder)
        {
            PluginsFolder = pluginsFolder;

            var plugins = pluginsFolder.GetFiles("*.dll", SearchOption.AllDirectories)
                .SelectMany(file => Assembly.LoadFile(file.FullName).GetTypes())
                .Where(t => typeof(IPlugin).IsAssignableFrom(t))
                .Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin);

            foreach (var plugin in plugins)
            {
                Plugins.Add(plugin.GetInfo().FullId, plugin);
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
