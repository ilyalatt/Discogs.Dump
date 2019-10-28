namespace Discogs.Dump.Model.Artists
{
    public sealed class Ref
    {
        public int Id { get; }
        public string Name { get; }

        public Ref(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
