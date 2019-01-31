using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Core
{
    public interface IPluginLoader
    {
        IDictionary<System.String, IPlugin> Plugins { get; }
        DirectoryInfo PluginsFolder { get; }

        IPlugin GetPlugin(System.String id, System.String version);
    }
}