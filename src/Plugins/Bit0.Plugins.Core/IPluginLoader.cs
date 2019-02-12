using System;
using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Core
{
    public interface IPluginLoader
    {
        IDictionary<String, IPlugin> Plugins { get; }

        DirectoryInfo PluginsFolder { get; }

        IPlugin GetPlugin(String id, String version);
    }
}