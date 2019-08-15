using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bit0.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        private PluginAttribute _info;

        public PluginAttribute Info
        {
            get
            {
                if (_info == null)
                {
                    _info = GetType().GetCustomAttribute<PluginAttribute>();
                }

                return _info;
            }
        }

        public abstract IServiceCollection Register(IServiceCollection services);
    }
}
