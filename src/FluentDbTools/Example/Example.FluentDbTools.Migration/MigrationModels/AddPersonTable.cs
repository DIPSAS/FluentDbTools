using Example.FluentDbTools.Database;
using FluentDbTools.Migration.Contracts;
using FluentMigrator;

namespace Example.FluentDbTools.Migration.MigrationModels
{
    [Migration(1, "Migration Example")]
    public class AddPersonTable : MigrationModel
    {
        public override void Up()
        {
            Create.Table(Table.Person).InSchema(SchemaName)
                .WithColumn(Column.Id).AsGuid().PrimaryKey().WithColumnDescription("Unique id.")
                .WithColumn(Column.SequenceNumber).AsInt32().NotNullable().WithColumnDescription("sequence number.")
                .WithColumn(Column.Alive).AsBoolean().NotNullable().WithColumnDescription("Alive flag.")
                .WithColumn(Column.Username).AsString().WithColumnDescription("username.")
                .WithColumn(Column.Password).AsString().WithColumnDescription("password.")
                .WithTableSequence(this);
        }

        public override void Down()
        {
            Delete.Table(Table.Person).InSchema(SchemaName);
        }
    }
}