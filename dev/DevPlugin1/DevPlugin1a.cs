using Bit0.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DevPlugin1
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin1a", Name = "Dev Plugin 1a", Version = "1.1.0", Implementing = typeof(List<Int32>))]
    public class DevPlugin1a : PluginBase
    {
        public override IServiceCollection Register(IServiceCollection services)
        {
            services.AddSingleton<IList<String>>(new List<String> { "a", "b", "c" });
            return services;
        }
    }
}
