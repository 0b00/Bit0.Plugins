using Bit0.Registry.Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Version = SemVer.Version;

namespace Bit0.Registry.Core
{
    public class PackageManager : IPackageManager
    {
        private readonly DirectoryInfo _packageCacheDir;
        private readonly ILogger<IPackageManager> _logger;


        public PackageManager(DirectoryInfo packageCacheDir, ILogger<IPackageManager> logger)
        {
            _packageCacheDir = packageCacheDir;
            _logger = logger;
        }

        public void AddFeed(KeyValuePair<String, String> source)
        {
            _logger.LogInformation(new EventId(3000), $"Add feed: {source.Key} {source.Value}");

            var feed = GetFeed(new Uri(source.Value));
            if (feed != null)
            {
                Feeds.Add(source.Key, feed);
            }
        }

        public void AddFeed(IDictionary<String, String> feeds)
        {
            foreach (var source in feeds)
            {
                AddFeed(source);
            }
        }

        public IDictionary<String, PackageFeed> Feeds { get; } = new Dictionary<String, PackageFeed>();

        public IEnumerable<Package> Packages => Feeds.SelectMany(f => f.Value.Package);

        public IEnumerable<Package> Find(String name)
        {
            _logger.LogInformation(new EventId(3000), $"Find package: {name}");

            var packages = Packages.Where(p => 
                p.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()) 
                || p.Id.ToLowerInvariant().Contains(name.ToLowerInvariant()));
            
            _logger.LogInformation(new EventId(3000), $"Found packages: {packages.Count()}");
            return packages;
        }

        public IPack Get(String name, String semVer) {
            _logger.LogInformation(new EventId(3000), $"Get package: {name} {semVer}");

            var package = Packages.Where(p => p.Id == name).Single();

            if (package == null)
            {
                _logger.LogError(new EventId(3003), "Package not found");
                return null;
            }

            return Get(package, semVer);
        }

        public IPack Get(Package package, String semVer)
        {
            try
            {
                var versions = package.Versions.Select(v => new Version(v.Version));
                var version = new Range(semVer).MaxSatisfying(versions);
                var packageVersion = package.Versions.Where(v => new Version(v.Version) == version).Single();
                return Pack.Get(packageVersion.Url, _packageCacheDir, _logger);
            } catch(Exception ex)
            {
                _logger.LogError(new EventId(3004), ex, "Package version not found");
                return null;
            }

        }

        private PackageFeed GetFeed(Uri url)
        {
            using (var wc = new WebClient())
            {
                try
                {
                    using (var file = wc.OpenRead(url).GetJsonReader())
                    {
                        _logger.LogInformation(new EventId(3000), $"Found package feed: {url}");
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<PackageFeed>(file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(new EventId(3001), ex, $"Could load package feed: {url}");
                    return null;
                }
            }

        }
    }
}
