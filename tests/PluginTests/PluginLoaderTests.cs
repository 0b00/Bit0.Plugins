using Bit0.Plugins.Loader;
using Divergic.Logging.Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace PluginTests
{
    [ExcludeFromCodeCoverage]
    public class PluginLoaderTests
    {
        private readonly DirectoryInfo _path;
        private readonly ICacheLogger<IPluginLoader> _logger;

        public PluginLoaderTests(ITestOutputHelper output)
        {
            _logger = output.BuildLoggerFor<IPluginLoader>();
            _path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
        }

        [Fact]
        public void PluginsFolder()
        {
            var loader = new PluginLoader(new List<DirectoryInfo> { _path }, _logger);
            loader.PluginsFolders.Should().Contain(_path);
        }

        [Fact]
        public void PluginsCount()
        {
            var loader = new PluginLoader(new List<DirectoryInfo> { _path }, _logger);
            loader.Plugins.Count.Should().Be(2);
        }

        [Theory]
        [InlineData("dev-plugin1a", "1.0.0", "Dev Plugin 1a")]
        [InlineData("dev-plugin1b", "1.0.0", "Dev Plugin 1b")]
        public void GetPlugin(String id, String version, String name)
        {
            var loader = new PluginLoader(new List<DirectoryInfo> { _path }, _logger);

            var plugin = loader.GetPlugin(id, version);
            var info = plugin.Info;

            info.ToString().Should().Be($"'{name}' ({id}@{version})");

            info.Id.Should().Be(id);
            info.Version.Should().Be(version);
            info.Name.Should().Be(name);
        }
    }
}
