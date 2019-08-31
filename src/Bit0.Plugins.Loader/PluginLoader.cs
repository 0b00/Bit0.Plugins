using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bit0.Plugins.Loader
{
    public class PluginLoader : IPluginLoader
    {
        private readonly ILogger<IPluginLoader> _logger;

        public PluginLoader(IEnumerable<DirectoryInfo> pluginsFolders, ILogger<IPluginLoader> logger)
        {
            Setup();
            _logger = logger;

            _logger.LogInformation(new EventId(4000), "Start loading plugins");

            PluginsFolders = pluginsFolders.Where(d => d.Exists);
            IList<IPlugin> plugins = new List<IPlugin>();

            plugins = PluginsFolders.SelectMany(d => d.GetFiles("*.dll", SearchOption.AllDirectories))
                .Where(file => file.Name.ToLowerInvariant().Contains("plugin".ToLowerInvariant()))
                .Select(file => Assembly.LoadFile(file.FullName))
                .SelectMany(GetLoadableTypes)
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(pluginType => Activator.CreateInstance(pluginType) as IPlugin).ToList();

            _logger.LogInformation(new EventId(4001), $"Found {plugins.Count} plugins.");

            if (!plugins.Any())
            {
                var evId = new EventId(4004, "NoPluginsFound");
                _logger.LogWarning(evId, "No plugins loaded.");
            }

            foreach (var plugin in plugins)
            {
                _logger.LogInformation(new EventId(4002), $"Loading: {plugin.Info.FullId}");

                Plugins.Add(plugin.Info.FullId, plugin);

                _logger.LogInformation(new EventId(4002), $"Loaded: {plugin.Info.FullId}");

                //if (plugin.Info.Implementing != null)
                //{
                //    Implementations.Add(plugin.Info.Implementing, plugin);
                //    logger.LogInformation(new EventId(4002), $"{plugin.Info.FullId} => {plugin.Info.Implementing.FullName}");
                //}
            }
        }

        private void Setup()
        {
            var domain = AppDomain.CurrentDomain;

            domain.AssemblyLoad += (sender, e) =>
            {
                _logger.LogDebug(new EventId(6000), $"Loaded: {e.LoadedAssembly.FullName}");
            };

            domain.AssemblyResolve += (sender, e) =>
            {
                var dir = new FileInfo(e.RequestingAssembly.Location).Directory;
                var file = dir.GetFiles(e.Name.Split(',').FirstOrDefault() + ".dll", SearchOption.TopDirectoryOnly).FirstOrDefault();
                return Assembly.LoadFile(file.FullName);
            };
        }

        private IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public IEnumerable<DirectoryInfo> PluginsFolders { get; }

        public IDictionary<String, IPlugin> Plugins { get; } = new Dictionary<String, IPlugin>();
        // public IDictionary<Type, IPlugin> Implementations { get; } = new Dictionary<Type, IPlugin>();

        public IPlugin GetPlugin(String id, String version)
        {
            return Plugins[$"{id}@{version}"];
        }
    }
}
