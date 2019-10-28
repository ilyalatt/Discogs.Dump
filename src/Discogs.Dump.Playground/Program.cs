using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Discogs.Dump.Playground
{
    static class Program
    {
        static async Task BenchmarkDumpFetching()
        {
            using var dumpFetcher = new DumpFetcher();
            var stream = dumpFetcher.GetLatestReleasesDumpStream();
            var sw = Stopwatch.StartNew();
            var cnt = 0;
            await foreach (var _ in stream)
            {
                cnt++;
                if (cnt % 10000 != 0 || sw.ElapsedMilliseconds < 1000) continue;
                var speed = (int) (cnt / sw.Elapsed.TotalSeconds);
                Console.WriteLine($"{cnt} records fetched, {speed} per second in average");
            }

            Console.WriteLine($"{(int) sw.Elapsed.TotalSeconds}s elapsed");
        }

        static async Task Main()
        {
            await BenchmarkDumpFetching();
        }
    }
}
