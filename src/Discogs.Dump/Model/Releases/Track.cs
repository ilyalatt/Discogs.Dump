using System.Collections.Generic;

namespace Discogs.Dump.Model.Releases
{
    public sealed class Track
    {
        public string Position { get; }
        public string Title { get; }
        public string Duration { get; }
        public IReadOnlyList<Artist> Artists { get; }

        public Track(string position, string title, string duration, IReadOnlyList<Artist> artists)
        {
            Position = position;
            Title = title;
            Duration = duration;
            Artists = artists;
        }
    }
}
