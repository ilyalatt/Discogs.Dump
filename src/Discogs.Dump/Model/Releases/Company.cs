namespace Discogs.Dump.Model.Releases
{
    public sealed class Company
    {
        public int Id { get; }
        public string Name { get; }
        public string CatNo { get; }
        // TODO: store enum EntityType only
        public int EntityType { get; }
        public string EntityTypeName { get; }

        public Company(int id, string name, string catNo, int entityType, string entityTypeName)
        {
            Id = id;
            Name = name;
            CatNo = catNo;
            EntityType = entityType;
            EntityTypeName = entityTypeName;
        }
    }
}
