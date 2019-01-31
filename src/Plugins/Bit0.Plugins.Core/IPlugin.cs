using Microsoft.Extensions.DependencyInjection;

namespace Bit0.Plugins.Core
{
    public interface IPlugin
    {
        void Register(IServiceCollection services);
    }
}
