using Bit0.Plugins.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace PluginTests
{
    [ExcludeFromCodeCoverage]
    public class DependencyInjectionTests
    {
        [Fact]
        public void ServiceProviderTest()
        {
            var services = new ServiceCollection();
            services.LoadPlugins(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory);

            var provider = services.BuildServiceProvider();

            var pluginLoader = provider.GetService<IPluginLoader>();
            pluginLoader.Should().BeOfType<PluginLoader>();
            pluginLoader.Plugins.Should().HaveCount(2);

            var url = provider.GetService<Uri>();
            url.Should().NotBeNull();
            url.Host.Should().Be("b1thunt3r.se");

            var plugins = provider.GetServices<IPlugin>();
            plugins.Should().HaveCount(2);
            plugins.Where(p => p.FullId.StartsWith("dev-plugin1")).Should().HaveCount(2);
            plugins.Any(p => p.FullId == "dev-plugin1a@1.0.0").Should().BeTrue();
            plugins.Any(p => p.FullId == "dev-plugin1b@1.0.0").Should().BeTrue();
        }
    }
}
