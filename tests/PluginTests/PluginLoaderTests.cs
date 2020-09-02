using Bit0.Plugins.Loader;
using Bit0.Plugins.Sdk;
using Divergic.Logging.Xunit;
using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace PluginTests
{
    [ExcludeFromCodeCoverage]
    public class PluginLoaderTests
    {
        private readonly DirectoryInfo _path;
        private readonly ICacheLogger<IPluginLoader> _logger;
        private readonly IPluginOptions _pluginOptions;

        public PluginLoaderTests(ITestOutputHelper output)
        {
            _logger = output.BuildLoggerFor<IPluginLoader>();
            _path = FindPluginsDir();
            _pluginOptions = new PluginOptions
            {
                Directories = new DirectoryInfo[] { _path }
            };
        }

        public static DirectoryInfo FindPluginsDir()
        {
            var wd = new DirectoryInfo(Environment.CurrentDirectory);
            DirectoryInfo path = null;

            while (path == null)
            {
                var dirs = wd.GetDirectories("plugins");
                if (dirs.Any())
                {
                    path = dirs.First();
                } else
                {
                    wd = wd.Parent;
                }
            }

            return path;
        }

        [Fact]
        public void PluginsFolder()
        {
            var loader = new PluginLoader(_pluginOptions, _logger);
            loader.PluginsFolders.Should().Contain(_path);
        }

        [Fact]
        public void PluginsCount()
        {
            var loader = new PluginLoader(_pluginOptions, _logger);
            loader.Plugins.Count.Should().Be(3);
        }

        [Theory]
        [InlineData("dev-plugin1a", "1.1.0", "Dev Plugin 1a")]
        [InlineData("dev-plugin1b", "1.0.0", "Dev Plugin 1b")]
        [InlineData("dev-plugin2" , "1.0.1", "Dev Plugin 2" )]
        public void GetPlugin(String id, String version, String name)
        {
            var loader = new PluginLoader(_pluginOptions, _logger); ;

            var plugin = loader.GetPlugin(id, version);
            var info = plugin.Info;

            info.ToString().Should().Be($"'{name}' ({id}@{version})");

            info.Id.Should().Be(id);
            info.Version.Should().Be(version);
            info.Name.Should().Be(name);
            info.Implementing.Should().NotBeNull();
        }

        [Fact]
        public void NoPluginsInDir()
        {
            var dir = new DirectoryInfo(Path.GetTempFileName().Replace(".tmp", ""));
            dir.Create();

            var loader = new PluginLoader(new PluginOptions
            {
                Directories = new DirectoryInfo[] { dir }
            }, _logger);
            loader.Plugins.Count.Should().Be(0);
            _logger.Last.Message.Should().Be("No plugins loaded.");
            _logger.Last.EventId.Id.Should().Be(4004);
            _logger.Last.EventId.Name.Should().Be("NoPluginsFound");

            dir.Delete();
        }
    }
}
