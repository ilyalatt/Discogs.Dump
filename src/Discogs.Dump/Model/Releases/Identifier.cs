namespace Discogs.Dump.Model.Releases
{
    public sealed class Identifier
    {
        public string? Description { get; }
        public string Type { get; }
        public string Value { get; }

        public Identifier(string? description, string type, string value)
        {
            Description = description;
            Type = type;
            Value = value;
        }
    }
}
