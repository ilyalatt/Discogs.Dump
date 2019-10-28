using System.Collections.Generic;

namespace Discogs.Dump.Model.Releases
{
    public sealed class Release
    {
        public int Id { get; }
        public Status Status { get; }
        public IReadOnlyList<Image> Images { get; }
        public IReadOnlyList<Artist> Artists { get; }
        public IReadOnlyList<Artist> ExtraArtists { get; }
        public string Title { get; }
        public IReadOnlyList<Label> Labels { get; }
        public IReadOnlyList<Format> Formats { get; }
        public IReadOnlyList<string> Genres { get; }
        public IReadOnlyList<string> Styles { get; }
        public string? Country { get; }
        public string? Released { get; }
        public string? Notes { get; }
        public string DataQuality { get; } // TODO: enum
        public int? MasterId { get; }
        public IReadOnlyList<Track> TrackList { get; }
        public IReadOnlyList<Identifier> Identifiers { get; }
        public IReadOnlyList<Video> Videos { get; }
        public IReadOnlyList<Company> Companies { get; }

        public Release(int id, Status status, IReadOnlyList<Image> images, IReadOnlyList<Artist> artists, IReadOnlyList<Artist> extraArtists, string title, IReadOnlyList<Label> labels, IReadOnlyList<Format> formats, IReadOnlyList<string> genres, IReadOnlyList<string> styles, string? country, string? released, string? notes, string dataQuality, int? masterId, IReadOnlyList<Track> trackList, IReadOnlyList<Identifier> identifiers, IReadOnlyList<Video> videos, IReadOnlyList<Company> companies)
        {
            Id = id;
            Status = status;
            Images = images;
            Artists = artists;
            ExtraArtists = extraArtists;
            Title = title;
            Labels = labels;
            Formats = formats;
            Genres = genres;
            Styles = styles;
            Country = country;
            Released = released;
            Notes = notes;
            DataQuality = dataQuality;
            MasterId = masterId;
            TrackList = trackList;
            Identifiers = identifiers;
            Videos = videos;
            Companies = companies;
        }
    }
}
