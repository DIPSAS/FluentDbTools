using System;
using Example.FluentDbTools.Database;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Contracts;
using FluentMigrator;

namespace Example.FluentDbTools.Migration.MigrationModels
{
    /// <inheritdoc />
    [Migration(1, "Migration Example with SchemaPrefix")]
    public class AddPersonTable : MigrationModel
    {
        private ChangeLogContext PersonLogContextField;
        private ChangeLogContext ParentLogContextField;
        private string PersonTableName => Table.Person.GetPrefixedName(SchemaPrefixId);

        private ChangeLogContext PersonLogContext => PersonLogContextField ??
                                                           (PersonLogContextField = new ChangeLogContext(this, Table.Person));

        private ChangeLogContext ParentLogContext => ParentLogContextField ??
                                                           (ParentLogContextField = new ChangeLogContext(this, Table.Parent));
        /// <inheritdoc />
        public override void Up()
        {
            // Create the ChangeLogContext with values from this.GetMigrationConfig()
            // -
            // SchemaPrefixId is fetched from database:migration:schemaPrefix:id or database:schemaPrefix:id<br/>
            // SchemaPrefixUniqueId is fetched from database:migration:schemaPrefix:UniqueId or database:schemaPrefix:UniqueId<br/>
            // -
            // GlobalId is fetched from database:migration:schemaPrefix:tables:testing:GlobalId or database:schemaPrefix:tables:testing:GlobalId<br/>
            // ShortName is fetched from database:migration:schemaPrefix:tables:testing:ShortName or database:schemaPrefix:tables:testing:ShortName<br/>
            var testChangeLogContext = new ChangeLogContext(this, Table.Testing);

            CreateParent();
            
            // If "database:schemaPrefix:Id" or "database:migration:schemaPrefix:Id" have a value,
            // the tableName Person will be created as {SchemaPrefixId}Person.
            //
            // i.e: "database:schemaPrefix:Id" = "EX" => will create table EXPerson
            // If both "database:schemaPrefix:Id" and "database:migration:schemaPrefix:Id" is missing,
            // the tableName Person will be created.
            var syntax = Create
                .Table(Table.Person.GetPrefixedName(SchemaPrefixId)).InSchema(SchemaName)
                .WithChangeLog(PersonLogContext)
                .WithColumn(Table.Person + "Id").AsGuid().PrimaryKey().WithColumnDescription("Unique id.");

            if (IsOracle())
            {
                syntax.WithDefault(SystemMethods.NewGuid);
            }
            else
            {
                syntax.WithDefaultValue(Guid.NewGuid());
            }

            syntax
                .WithChangeLog(PersonLogContext) // ChangeLog activation for Create.Table(..)
                .WithColumn(Column.SequenceNumber).AsInt32().NotNullable().WithColumnDescription("sequence number.")
                .WithColumn(Column.Alive).AsBoolean().NotNullable().WithColumnDescription("Alive flag.")
                .WithColumn(Column.Username).AsString().WithColumnDescription("username.")
                .WithColumn("TestCol").AsGuid().WithColumnDescription("TestCol Guid")
                .WithColumn(Column.Password).AsString().WithChangeLog(PersonLogContext).WithColumnDescription("password.")
                .WithDefaultColumns() // Enable DefaultColumns functionality
                .WithChangeLog(PersonLogContext)
                .WithForeignKeyColumn("Parent".GetPrefixedName(SchemaPrefixId), this, "ParentId_FK", "ParentId").AsGuid().Nullable()
                .WithDefaultColumns()
                .WithChangeLog(PersonLogContext)
                .WithChangeLog(PersonLogContext)
                .WithTableSequence(this);

            Alter.Table(Table.Person, this).WithDescription("description of " + PersonTableName).InSchema(SchemaName)
                .WithChangeLog(PersonLogContext)
                .AddColumn("NewCol").AsInt64().Nullable().WithColumnDescription("NewCol description")
                .AlterColumn(Column.Password).AsFixedLengthString(255).Nullable()
                .AlterColumn("TestCol").AsString(255).Nullable().WithColumnDescription("TestCol String(255)")
                .WithChangeLog(PersonLogContext);

            Create.Table("Test", this)
                .WithDescription("Hei hei")
                .WithColumn("TestId").AsInt32().PrimaryKey().WithColumnDescription("Unique id.")
                .WithColumn("TestDescription").AsAnsiString().WithColumnDescription("Description")
                .WithDefaultColumns()
                .WithChangeLog(testChangeLogContext);

            Rename.Table("Test", this)
                .WithChangeLog(testChangeLogContext, this)
                .ToTable(Table.Testing, this)
                .WithChangeLog(testChangeLogContext, this);

            Rename
                .Column("TestDescription").OnTable(Table.Testing, this)
                .To("TestIngDescription", this)
                .WithChangeLog(testChangeLogContext, this);

            //CreateOrReplace.View("").WithColumn("ss").WithColumns("sds", "sss").WithSqlScript("");

            Create.Column("CreateColumn")
                .OnTable(Table.Person, this).AsInt16().Nullable()
                .WithChangeLog(PersonLogContext);

        }

        private void CreateParent()
        {
            var syntax = Create
                .Table(Table.Parent, this).InSchema(SchemaName)
                .WithColumn(Table.Parent + "Id").AsGuid().PrimaryKey().WithDefaultValue(Guid.NewGuid())
                .WithColumnDescription("Unique id.");

            if (IsOracle())
            {
                syntax.WithDefault(SystemMethods.NewGuid);
            }
            else
            {
                syntax.WithDefaultValue(Guid.NewGuid());
            }

            syntax
                .WithDefaultColumns()
                .WithChangeLog(ParentLogContext)
                .WithDefaultColumns();
        }

        /// <inheritdoc />
        public override void Down()
        {
            var testChangeLogContext = new ChangeLogContext(this, Table.Testing);
            Delete.Table(GetPrefixedName(Table.Person))
                .WithChangeLog(PersonLogContext,this)
                .InSchema(SchemaName);

            Delete.Table(Table.Parent, this)
                .WithChangeLog(ParentLogContext, this);

            Delete.Table(Table.Testing, this)
                .WithChangeLog(testChangeLogContext, this);
        }
    }
}