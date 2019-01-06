using Microsoft.Extensions.DependencyInjection;

namespace Bit0.Plugins.Core
{
    public interface ICrunchPlugin
    {
        void Register(IServiceCollection services);
    }
}
