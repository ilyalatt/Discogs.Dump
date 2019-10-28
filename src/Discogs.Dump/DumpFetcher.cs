using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Discogs.Dump.Model.Releases;
using Artist = Discogs.Dump.Model.Artists.Artist;

namespace Discogs.Dump
{
    public sealed class DumpFetcher : IDisposable
    {
        readonly HttpClient _httpClient;

        static HttpClient GetHttpClient() =>
            new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

        public DumpFetcher() => _httpClient = GetHttpClient();
        public void Dispose() => _httpClient?.Dispose();


        const string BaseUrl = "https://discogs-data.s3-us-west-2.amazonaws.com";

        async Task<XElement> ListRaw(string prefix)
        {
            var url = $"{BaseUrl}/?delimiter=/&prefix={prefix}";
            var xml = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
            var xDoc = XDocument.Parse(xml);
            var root = xDoc.Root;
            if (root == null) throw new Exception("XML does not have root.");
            return root;
        }

        async Task<List<string>> ListDirs(string prefix)
        {
            var root = await ListRaw(prefix).ConfigureAwait(false);
            return root
                .Elements()
                .Where(x => x.Name.LocalName == "CommonPrefixes")
                .Select(x => x.Elements().Single())
                .Select(x => x.Value)
                .ToList();
        }

        async Task<List<string>> ListFiles(string prefix)
        {
            var root = await ListRaw(prefix).ConfigureAwait(false);
            return root
                .Elements()
                .Where(x => x.Name.LocalName == "Contents")
                .Select(x => x.FirstNode as XElement)
                .Where(x => x != null)
                .Select(x => x!.Value)
                .ToList();
        }

        async Task<string> GetLatestDumpUrl(string suffix)
        {
            var yearDirs = await ListDirs("data/").ConfigureAwait(false);
            if (yearDirs.Count == 0) throw new Exception("Can not fetch years.");
            var yearDir = yearDirs.Max();
            var dumps = await ListFiles(yearDir).ConfigureAwait(false);
            if (dumps.Count == 0) throw new Exception($"Can not fetch '{yearDir}' dumps.");
            var latestReleaseDump = dumps.Where(x => x.EndsWith(suffix)).Max();
            return $"{BaseUrl}/{latestReleaseDump}";
        }

        public async Task<string> GetLatestReleasesDumpUrl() =>
            await GetLatestDumpUrl("releases.xml.gz").ConfigureAwait(false);

        public async Task<string> GetLatestArtistsDumpUrl() =>
            await GetLatestDumpUrl("artists.xml.gz").ConfigureAwait(false);

        async Task<Stream> OpenDumpStream(string url)
        {
            var stream = await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
            stream = new BufferedStream(stream, 10 * 1024 * 1024);
            stream = new GZipStream(stream, CompressionMode.Decompress);
            return stream;
        }

        public async IAsyncEnumerable<Release> GetLatestReleasesDumpStream([EnumeratorCancellation] CancellationToken ct = default)
        {
            var url = await GetLatestReleasesDumpUrl().ConfigureAwait(false);
            await using var stream = await OpenDumpStream(url).ConfigureAwait(false);
            await foreach (var x in DumpParser.ParseReleasesDumpStream(stream, ct)) yield return x;
        }

        public async IAsyncEnumerable<Artist> GetLatestArtistsDumpStream([EnumeratorCancellation] CancellationToken ct = default)
        {
            var url = await GetLatestArtistsDumpUrl().ConfigureAwait(false);
            await using var stream = await OpenDumpStream(url).ConfigureAwait(false);
            await foreach (var x in DumpParser.ParseArtistsDumpStream(stream, ct)) yield return x;
        }
    }
}
