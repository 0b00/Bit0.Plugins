using Bit0.Plugins;
using System.Diagnostics.CodeAnalysis;

namespace DevPlugin1
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin1a", Name = "Dev Plugin 1a", Version = "1.0.0")]
    public class DevPlugin1a : PluginBase, IPlugin
    {
    }
}
