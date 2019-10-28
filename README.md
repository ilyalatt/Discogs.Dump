# Discogs.Dump

[![NuGet version](https://badge.fury.io/nu/Discogs.Dump.svg)](https://www.nuget.org/packages/Discogs.Dump)

## Why

Discogs API is limited to 60 requests per minute.
You can not search a lot of tracks fast.
Discogs search behaves strange sometimes.
Building your own search index is a good alternative.
Dicsogs provides [monthly backups](https://data.discogs.com/).
Releases info takes about 6.4 GB only.

## What can I do with this library

You can use `DumpFetcher` class to retrieve the latest release and artist backups.
You can parse releases online from network file stream without downloading.
If you use the same backup more then once I recommend to download it and parse the data from a local file.

## Quick start

```C#
using var dumpFetcher = new DumpFetcher();
var stream = dumpFetcher.GetLatestReleasesDumpStream();
await foreach (var release in stream)
{
    Console.WriteLine(release.Title);
}
```

For complete example check out [Playground](https://github.com/ilyalatt/Discogs.Dump/blob/master/src/Discogs.Dump.Playground/Program.cs).
