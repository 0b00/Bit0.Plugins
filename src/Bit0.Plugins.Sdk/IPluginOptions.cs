using System.Collections.Generic;
using System.IO;

namespace Bit0.Plugins.Sdk
{
    public interface IPluginOptions
    {
        IEnumerable<DirectoryInfo> Directories { get; set; }
    }
}