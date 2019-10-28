namespace Discogs.Dump.Model.Releases
{
    public sealed class Label
    {
        public string CatNo { get; }
        public int Id { get; }
        public string Name { get; }

        public Label(string catNo, int id, string name)
        {
            CatNo = catNo;
            Id = id;
            Name = name;
        }
    }
}
