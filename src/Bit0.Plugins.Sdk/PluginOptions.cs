using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Sdk
{
    public class PluginOptions : IPluginOptions
    {
        public IEnumerable<DirectoryInfo> Directories { get; set; }
    }
}
