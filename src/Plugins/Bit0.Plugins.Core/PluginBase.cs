using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bit0.Plugins.Core
{
    public abstract class PluginBase : IPlugin
    {
        public String FullId => this.GetInfo().FullId;

        public abstract void Register(IServiceCollection services);
    }
}
