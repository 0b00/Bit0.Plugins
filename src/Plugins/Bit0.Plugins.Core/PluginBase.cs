using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bit0.Plugins.Core
{
    public abstract class PluginBase : IPlugin
    {
        private String _fullId;

        public String FullId
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_fullId))
                {
                    _fullId = this.GetInfo().FullId;
                }

                return _fullId;
            }
        }

        public abstract void Register(IServiceCollection services);
    }
}
