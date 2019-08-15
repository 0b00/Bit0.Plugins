using Microsoft.Extensions.DependencyInjection;

namespace Bit0.Plugins
{
    public interface IPlugin
    {
        PluginAttribute Info { get; }
        IServiceCollection Register(IServiceCollection services);
    }
}
