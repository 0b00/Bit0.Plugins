using Bit0.Plugins;
using Bit0.Plugins.PluginLoader;
using Divergic.Logging.Xunit;
using FluentAssertions;
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
    public class PluginLoaderTests
    {
        private readonly ICacheLogger<IPluginLoader> _logger;
        private readonly IEnumerable<DirectoryInfo> _paths;

        public PluginLoaderTests(ITestOutputHelper output)
        {
            _logger = output.BuildLoggerFor<IPluginLoader>();
            _paths = new[] { new FileInfo(Assembly.GetExecutingAssembly().Location).Directory };
        }

        [Fact]
        public void PluginsFolder()
        {
            var loader = new PluginLoader(_paths, _logger);
            loader.PluginsFolders.Should().HaveCount(_paths.Count());
            loader.PluginsFolders.First().FullName.Should().Be(_paths.First().FullName);
        }

        [Fact]
        public void PluginsFolderNotFound()
        {
            var paths = new[] { new DirectoryInfo(DateTime.Now.ToBinary().ToString()) };
            Action action = () => new PluginLoader( paths, _logger);

            action.Should().Throw<NoPluginsFoundException>();

            _logger.Last.EventId.Id.Should().Be(4004);
            (_logger.Last.Exception as NoPluginsFoundException).PluginsFolders.First().FullName.Should().Be(paths.First().FullName);
        }

        [Fact]
        public void PluginsCount()
        {
            var loader = new PluginLoader(_paths, _logger);
            loader.Plugins.Count.Should().Be(2);
        }

        [Theory]
        [InlineData("dev-plugin1a", "1.0.0", "Dev Plugin 1a", null)]
        [InlineData("dev-plugin1b", "1.0.0", "Dev Plugin 1b", typeof(IPlugin))]
        public void GetPlugin(String id, String version, String name, Type implementing)
        {
            var loader = new PluginLoader(_paths, _logger);

            var plugin = loader.GetPlugin(id, version);
            var info = plugin.GetInfo();

            info.ToString().Should().Be($"'{name}' ({id}@{version})");

            info.Id.Should().Be(id);
            info.Version.Should().Be(version);
            info.Name.Should().Be(name);
            info.FullId.Should().Be($"{id}@{version}");
            info.Implementing.Should().Be(implementing);

            plugin.FullId.Should().Be($"{id}@{version}");
            plugin.Implementing.Should().Be(implementing);
        }
    }
}
