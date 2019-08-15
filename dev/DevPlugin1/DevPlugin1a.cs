using Bit0.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DevPlugin1
{
    [ExcludeFromCodeCoverage]
    [Plugin(Id = "dev-plugin1a", Name = "Dev Plugin 1a", Version = "1.0.0")]
    public class DevPlugin1a : PluginBase
    {
         public override IServiceCollection Register(IServiceCollection services) => throw new NotImplementedException();
    }
}
