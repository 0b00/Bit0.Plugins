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

            foreach (var file in pluginsFolder.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                var assembly = Assembly.LoadFile(file.FullName);
                var assemblyPlugins = assembly.GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t));

                foreach (var pluginType in assemblyPlugins)
                {
                    var plugin = Activator.CreateInstance(pluginType) as IPlugin;
                    var info = plugin.GetInfo();
                    Plugins.Add(info.FullId, plugin);
                }
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
