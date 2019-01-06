using Bit0.Registry.Core;
using Divergic.Logging.Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace RegistryTests
{
    public class PackageManagerTest
    {
        private readonly ICacheLogger<IPackageManager> _logger;
        private readonly DirectoryInfo _cacheDir;

        public PackageManagerTest(ITestOutputHelper output)
        {
            var factory = LogFactory.Create(output);
            _logger = output.BuildLoggerFor<IPackageManager>();
            _cacheDir = new DirectoryInfo(".packs");
        }

        [Fact]
        public void AddNonExistingFeed()
        {
            var manager = new PackageManager(_cacheDir, _logger);
            var name = "default";
            var url = "http://feed/";

            manager.AddFeed(new KeyValuePair<String, String>(name, url));

            _logger.Last.EventId.Id.Should().Be(3001);
            _logger.Last.Message.Should().Be($"Could load package feed: {url}");
        }

        [Fact]
        public void AddExistingFeed()
        {
            var manager = new PackageManager(_cacheDir, _logger);
            var name = "default";
            var url = new FileInfo(@"TestData\registry1\index.json").FullName;

            manager.AddFeed(new KeyValuePair<String, String>(name, url));

            var feed = manager.Feeds.FirstOrDefault();

            feed.Key.Should().Be(name);
            feed.Value.Title.Should().Be("Test Feed 1");
            feed.Value.Id.Should().Be("http://feed1.test/");
            feed.Value.Updated.Should().Be(DateTime.Parse("2018-12-31T00:55:19.303511+00:00"));
        }

        [Fact]
        public void AddFeeds()
        {
            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new Dictionary<String, String>
            {
                { "f404", "http://feed.test/" },
                { "default", new FileInfo(@"TestData\registry1\index.json").FullName }
            });

            var feed = manager.Feeds;

            feed.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("test", 4)]
        [InlineData("package-1", 1)]
        [InlineData("test-package-1", 1)]
        public void FindPackages(String name, Int32 count)
        {
            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var packages = manager.Find(name).ToList();
            packages.Count.Should().Be(count);
        }

        [Fact]
        public void PackageInfo()
        {
            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var package = manager.Find("test-package-1").First();
            package.Name.Should().Be("Test Package 1");
            package.Description.Should().Be("Testing packages, part 1");
            package.Type.Should().Be(PackageType.Theme);
            package.Published.Should().Be(DateTime.Parse("2018-12-31T00:55:19.2841065+00:00"));
            package.Updated.Should().Be(DateTime.Parse("2018-12-31T00:55:19.303511+00:00"));
            package.License.Name.Should().Be("MIT");
            package.License.Url.Should().Be("http://feed1.test/license/MIT");
            package.Author.Name.Should().Be("User One");
            package.Author.Alias.Should().Be("user1");
            package.Author.Email.Should().Be("one@user.test");
            package.Author.HomePage.Should().Be("http://feed1.test/users/user1");
            package.Author.Social.Should().BeNull();
            package.Homepage.Should().Be("http://feed1.test/packages/test-package-1");
            package.Icon.Should().Be("http://feed1.test/packages/test-package-1/icon.png");
            package.Screenshot.Should().Be("http://feed1.test/packages/test-package-1/screenshot.png");
            package.Tags.Count().Should().Be(4);
            package.Tags.Last().Should().Be("theme");
            package.Features.Count().Should().Be(2);
            package.Features.Last().Should().Be("theme");
            package.Screenshot.Should().Be("http://feed1.test/packages/test-package-1/screenshot.png");
            package.Versions.Count().Should().Be(4);
            
            var packageVersion = package.Versions.First();
            packageVersion.Updated.Should().Be(DateTime.Parse("2018-12-31T00:55:19.2841065+00:00"));
            packageVersion.Url.Should().Be("http://feed1.test/packages/test-package-1/test-package-1-1.0.0.zip");
            packageVersion.Size.Should().Be(382);
            packageVersion.Sha256.Should().Be("55E55E7131A6164FD6302D43E84FFA1867601BD98A282A23E0086794F2746952");
        }
        
        [Fact]
        public void GetPackage()
        {

            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var pack = manager.Get("test-package-1", "1.0.0");

            pack.Id.Should().Be("test-package-1");
            pack.Name.Should().Be("Test Package 1");
            pack.Version.Should().Be("1.0.0");
            pack.Description.Should().Be("Testing packages, part 1");
            pack.Homepage.Should().Be("http://feed1.test/packages/test-package-1");
            pack.PackFile.Should().NotBeNull();
        }

        [Fact]
        public void GetNonExistingPackage1()
        {

            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var package = manager.Get("test-package-x", "1.0.0");

            _logger.Last.EventId.Id.Should().Be(3003);
            _logger.Last.Message.Should().Be("Package not found");
        }

        [Theory]
        [InlineData("test-package-2")]
        [InlineData("test-package-3")]
        [InlineData("test-package-4")]
        public void GetNonExistingPackageVersion1(String name)
        {

            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var package = manager.Get(name, "1.0.0");

            _logger.Last.EventId.Id.Should().Be(3004);
            _logger.Last.Message.Should().Be("Package version not found");
        }
        
        [Fact]
        public void GetNonExistingPackageVersion2()
        {

            var manager = new PackageManager(_cacheDir, _logger);
            manager.AddFeed(new KeyValuePair<String, String>("default", new FileInfo(@"TestData\registry1\index.json").FullName));

            var package = manager.Get("test-package-4", "1.0.3");

            _logger.Last.EventId.Id.Should().Be(3002);
            _logger.Last.Message.Should().Be("Invalid Pack file");
        }
    }
}
