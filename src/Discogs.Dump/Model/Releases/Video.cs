namespace Discogs.Dump.Model.Releases
{
    public sealed class Video
    {
        public int Duration { get; }
        public bool Embed { get; }
        public string Src { get; }
        public string Title { get; }
        public string Description { get; }

        public Video(int duration, bool embed, string src, string title, string description)
        {
            Duration = duration;
            Embed = embed;
            Src = src;
            Title = title;
            Description = description;
        }
    }
}
