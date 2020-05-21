using Bit0.Plugins.Loader;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace PluginTests
{

    [ExcludeFromCodeCoverage]
    public class IoCTests
    {
        private readonly ICacheLogger<IPluginLoader> _logger;
        private readonly DirectoryInfo _path;

        public IoCTests(ITestOutputHelper output)
        {
            _logger = output.BuildLoggerFor<IPluginLoader>();
            _path = PluginLoaderTests.FindPluginsDir();
        }

        [Fact]
        public void IoC1()
        {
            IServiceCollection services = new ServiceCollection();
            services.LoadPlugins(new List<DirectoryInfo> { _path }, _logger);

            var provider = services.BuildServiceProvider();

            var loader = provider.GetService<IPluginLoader>();
            loader.Plugins.Count.Should().Be(3);
        }

        [Fact]
        public void PluginWithDeps()
        {
            IServiceCollection services = new ServiceCollection();
            services.LoadPlugins(new List<DirectoryInfo> { _path }, _logger);

            var provider = services.BuildServiceProvider();

            var loader = provider.GetService<IPluginLoader>();

            var name = "Dev Plugin 2";
            var id = "dev-plugin2";
            var version = "1.0.1";

            var plugin = loader.GetPlugin(id, version);
            var info = plugin.Info;

            info.ToString().Should().Be($"'{name}' ({id}@{version})");

            info.Id.Should().Be(id);
            info.Version.Should().Be(version);
            info.Name.Should().Be(name);
            info.Implementing.Should().NotBeNull();
            info.Implementing.Name.Should().Be("IMapper");

            var charList = provider.GetService<IList<Char>>();
            charList.Should().NotBeEmpty();
            charList.First().Should().Be('a');
        }
    }
}
