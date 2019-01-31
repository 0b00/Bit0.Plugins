using Bit0.Plugins.Core;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Xunit;

namespace PluginTests
{
    [ExcludeFromCodeCoverage]
    public class PluginLoaderTests
    {
        private readonly DirectoryInfo _path;

        public PluginLoaderTests()
        {
            _path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
        }

        [Fact]
        public void PluginsFolder()
        {
            var loader = new PluginLoader(_path);
            loader.PluginsFolder.Should().Be(_path);
        }

        [Fact]
        public void PluginsCount()
        {
            var loader = new PluginLoader(_path);
            loader.Plugins.Count.Should().Be(2);
        }

        [Theory]
        [InlineData("dev-plugin1a", "1.0.0", "Dev Plugin 1a")]
        [InlineData("dev-plugin1b", "1.0.0", "Dev Plugin 1b")]
        public void GetPlugin(String id, String version, String name)
        {
            var loader = new PluginLoader(_path);

            var plugin = loader.GetPlugin(id, version);
            var info = plugin.GetInfo();

            info.ToString().Should().Be($"'{name}' ({id}@{version})");

            info.Id.Should().Be(id);
            info.Version.Should().Be(version);
            info.Name.Should().Be(name);
        }
    }
}
