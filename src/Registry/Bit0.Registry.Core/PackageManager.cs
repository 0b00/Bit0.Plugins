using Bit0.Registry.Core.Exceptions;
using Bit0.Registry.Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
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

        public Package GetPackage(String name, String semVer)
        {
            _logger.LogInformation(new EventId(3000), $"Get package: {name} {semVer}");

            var package = Packages.Where(p => p.Id == name).SingleOrDefault();

            if (package == null)
            {
                var exp = new PackageNotFoundException("Package not found");
                _logger.LogError(exp.EventId, exp, "Package not found");
                throw exp;
            }

            return package;
        }

        public PackageVersion GetPackageVersion(String name, String semVer)
        {
            _logger.LogInformation(new EventId(3000), $"Get packageVersion: {name} {semVer}");

            var package = GetPackage(name, semVer);
            return GetPackageVersion(package, semVer);
        }

        public PackageVersion GetPackageVersion(Package package, String semVer)
        {
            try
            {
                var versions = package.Versions.Select(v => new Version(v.Version));
                var version = new Range(semVer).MaxSatisfying(versions);
                var packageVersion = package.Versions.Where(v => new Version(v.Version) == version).Single();
                return packageVersion;
            }
            catch (Exception ex)
            {
                var exp = new PackageVersionNotFoundException(package, ex);
                _logger.LogError(exp.EventId, exp, "Package version not found");
                throw exp;
            }
        }

        public IPack Get(String name, String semVer)
        {
            return Get(GetPackage(name, semVer), semVer);
        }

        public IPack Get(Package package, String semVer)
        {
            return Get(GetPackageVersion(package, semVer));
        }
        
        public IPack Get(PackageVersion version)
        {
            return Get(version.Url);
        }

        public IPack Get(String url)
        {
#if TEST
            url = new FileInfo(url.Replace("http://feed1.test/", @"TestData\registry1\")).FullName;
#endif
            try
            {
                ZipArchive zip;
                using (var wc = new WebClient())
                {
                    var file = new FileInfo($"pack{DateTime.Now.ToBinary().ToString()}.zip");
                    wc.DownloadFile(url, file.FullName);

                    zip = ZipFile.Open(file.FullName, ZipArchiveMode.Read, Encoding.UTF8);
                    _logger.LogInformation(new EventId(3000), $"Downloaded Pack archive: {url}");
                }

                IPack pack;
                var packEntry = zip.GetEntry("pack.json");
                using (var jr = packEntry.OpenJsonReader())
                {
                    var serializer = new JsonSerializer();
                    pack = serializer.Deserialize<Pack>(jr);
                    _logger.LogInformation(new EventId(3000), $"Read Pack file: {packEntry.FullName}");
                }

                // TODO: Use CombinePath Extensions
                var packDir = new DirectoryInfo($"{_packageCacheDir.FullName}/{pack.Id}/{pack.Version}");

                if (!packDir.Exists)
                {
                    zip.ExtractToDirectory(packDir.FullName);
                    _logger.LogInformation(new EventId(3000), $"Extracted Pack archive to: {packDir.FullName}");
                }

                pack.PackFile = new List<FileInfo>(packDir.GetFiles("pack.json", SearchOption.TopDirectoryOnly)).Single();

                return pack;
            }
            catch (Exception ex)
            {
                var exp = new InvalidPackFileException(url, ex);
                _logger.LogError(exp.EventId, exp, "Invalid Pack file");
                throw exp;
            }
        }


        public IEnumerable<IPack> GetWithDependancies(Package package)
        {
            throw new NotImplementedException();
        }
        public IDictionary<String, String> GetDependancies(Package package)
        {
            var deps = package.Dependencies.SelectMany(d =>
            {
                var p = GetPackage(d.Key, d.Value);

                return GetDependancies(p);
            });

            return deps.ToDictionary(k => k.Key, v=> v.Value);
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
                    var exp = new InvalidFeedException(url, ex);
                    _logger.LogInformation(exp.EventId, exp, $"Could load package feed: {url}");
                    throw exp;
                }
            }

        }
        
    }
}
