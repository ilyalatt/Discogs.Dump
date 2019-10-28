namespace Discogs.Dump.Model
{
    public sealed class Image
    {
        public int Height { get; }
        public string Type { get; }
        public string Uri { get; }
        public string Uri150 { get; }
        public int Width { get; }

        public Image(int height, string type, string uri, string uri150, int width)
        {
            Height = height;
            Type = type;
            Uri = uri;
            Uri150 = uri150;
            Width = width;
        }
    }
}
