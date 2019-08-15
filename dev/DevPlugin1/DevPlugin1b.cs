using Bit0.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DevPlugin1
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin1b", Name = "Dev Plugin 1b", Version = "1.0.0")]
    public class DevPlugin1b : PluginBase
    {
        public override IServiceCollection Register(IServiceCollection services) => throw new NotImplementedException();
    }

}
