using System;
using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Loader
{
    public interface IPluginLoader
    {
        IDictionary<String, IPlugin> Plugins { get; }

        IEnumerable<DirectoryInfo> PluginsFolders { get; }

        IPlugin GetPlugin(String id, String version);
    }
}
