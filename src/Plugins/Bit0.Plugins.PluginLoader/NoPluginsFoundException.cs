using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.PluginLoader
{
    public class NoPluginsFoundException : Exception
    {
        public IEnumerable<DirectoryInfo> PluginsFolders { get; }

        public EventId EventId => new EventId(4004, "NoPluginsFound");

        public NoPluginsFoundException(IEnumerable<DirectoryInfo> pluginsFolders)
        {
            PluginsFolders = pluginsFolders;
        }
    }
}
