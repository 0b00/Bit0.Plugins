using Bit0.Plugins;
using System.Diagnostics.CodeAnalysis;

namespace DevPlugin1
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin1b", Name = "Dev Plugin 1b", Version = "1.0.0", Implementing = typeof(IPlugin))]
    public class DevPlugin1b : PluginBase
    {
    }

}
