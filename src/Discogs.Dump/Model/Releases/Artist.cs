namespace Discogs.Dump.Model.Releases
{
    public sealed class Artist
    {
        public int Id { get; }
        public string Name { get; }
        public string Anv { get; }
        public string Join { get; }
        public string Role { get; }
        public string Tracks { get; }

        public Artist(int id, string name, string anv, string @join, string role, string tracks)
        {
            Id = id;
            Name = name;
            Anv = anv;
            Join = @join;
            Role = role;
            Tracks = tracks;
        }
    }
}
