using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bit0.Plugins.Core
{
    public interface IPlugin
    {
        String FullId { get; }

        void Register(IServiceCollection services);
    }
}
