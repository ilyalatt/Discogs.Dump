using System.Collections.Generic;

namespace Discogs.Dump.Model.Artists
{
    public sealed class Artist
    {
        public IReadOnlyList<Image> Images { get; }
        public int Id { get; }
        public string Name { get; }
        public string? RealName { get; }
        public string Profile { get; }
        public string DataQuality { get; }
        public IReadOnlyList<string> Urls { get; }
        public IReadOnlyList<string> NameVariations { get; }
        public IReadOnlyList<Ref> Aliases { get; }
        public IReadOnlyList<Ref> Members { get; }
        public IReadOnlyList<Ref> Groups { get; }

        public Artist(IReadOnlyList<Image> images, int id, string name, string? realName, string profile, string dataQuality, IReadOnlyList<string> urls, IReadOnlyList<string> nameVariations, IReadOnlyList<Ref> aliases, IReadOnlyList<Ref> members, IReadOnlyList<Ref> groups)
        {
            Images = images;
            Id = id;
            Name = name;
            RealName = realName;
            Profile = profile;
            DataQuality = dataQuality;
            Urls = urls;
            NameVariations = nameVariations;
            Aliases = aliases;
            Members = members;
            Groups = groups;
        }
    }
}
