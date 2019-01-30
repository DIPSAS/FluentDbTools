namespace FTest.FluentDbTools.SqlBuilderTestEntities
{
    public class Entity
    {
        public int Id { get; set; }
        public int ChildEntityId { get; set; }
        public int ChildChildEntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}