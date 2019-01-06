using System;
using System.Collections.Generic;

namespace Bit0.Registry.Core
{
    public interface IPackageManager
    {
        void AddFeed(KeyValuePair<String, String> feeds);
        void AddFeed(IDictionary<String, String> feeds);
        IEnumerable<Package> Find(String name);
        IPack Get(String name, String semVer);
        IPack Get(Package package, String semVer);
        IEnumerable<Package> Packages { get; }
        IDictionary<String, PackageFeed> Feeds { get; }
    }
}
