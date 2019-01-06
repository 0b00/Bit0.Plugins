using Bit0.Registry.Core.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Bit0.Registry.Core
{
    public class Pack : IPack
    {
        [JsonIgnore]
        public FileInfo PackFile { get; set; }

        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("version")]
        public String Version { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("homepage")]
        public String Homepage { get; set; }

        [JsonProperty("tags")]
        public IEnumerable<String> Tags { get; set; }

        [JsonProperty("features")]
        public IEnumerable<String> Features { get; set; }

        [JsonProperty("license")]
        public License License { get; set; }

        public static IPack Get(String url, DirectoryInfo packageCacheDir, ILogger<IPackageManager> logger)
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
                    logger.LogInformation(new EventId(3000), $"Downloaded Pack archive: {url}");
                }

                IPack pack;
                var packEntry = zip.GetEntry("pack.json");
                using (var jr = packEntry.OpenJsonReader())
                {
                    var serializer = new JsonSerializer();
                    pack = serializer.Deserialize<Pack>(jr);
                    logger.LogInformation(new EventId(3000), $"Read Pack file: {packEntry.FullName}");
                }

                // TODO: Use CombinePath Extensions
                var packDir = new DirectoryInfo($"{packageCacheDir.FullName}/{pack.Id}/{pack.Version}");

                if (!packDir.Exists)
                {
                    zip.ExtractToDirectory(packDir.FullName);
                    logger.LogInformation(new EventId(3000), $"Extracted Pack archive to: {packDir.FullName}");
                }

                pack.PackFile = new List<FileInfo>(packDir.GetFiles("pack.json", SearchOption.TopDirectoryOnly)).Single();

                return pack;
            } catch (Exception ex)
            {
                logger.LogError(new EventId(3002), ex, "Invalid Pack file");
            }

            return null;
        }
    }
}
