namespace DIPS.FluentDbTools.SqlBuilder.Tests.TestEntities
{
    public class ChildChildEntity
    {
        public int Id { get; set; }

        public int ChildEntityId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Relation { get; set; }
    }
}