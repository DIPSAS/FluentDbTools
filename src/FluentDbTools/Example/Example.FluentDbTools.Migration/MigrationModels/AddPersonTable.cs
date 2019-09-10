using System;
using Example.FluentDbTools.Database;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentDbTools.Migration.Contracts;
using FluentMigrator;

namespace Example.FluentDbTools.Migration.MigrationModels
{
    /// <inheritdoc />
    [Migration(1, "Migration Example")]
    public class AddPersonTable : MigrationModel
    {
        private ChangeLogContext PersonLogContextField;
        private ChangeLogContext ParentLogContextField;
        private string PersonTableName => Table.Person.GetPrefixedName(SchemaPrefixId);

        private ChangeLogContext PersonLogContext => PersonLogContextField ??
                                                           (PersonLogContextField = new ChangeLogContext
                                                           {
                                                               GlobalId = "per",
                                                               ShortName = "PAR".GetPrefixedName(SchemaPrefixId),
                                                               SchemaPrefix = SchemaPrefixId
                                                           });

        private ChangeLogContext ParentLogContext => ParentLogContextField ??
                                                           (ParentLogContextField = new ChangeLogContext
                                                           {
                                                               GlobalId = "par",
                                                               ShortName = "PER".GetPrefixedName(SchemaPrefixId),
                                                               SchemaPrefix = SchemaPrefixId
                                                           });

        /// <inheritdoc />
        public override void Up()
        {

            var testChangeLogContext = new ChangeLogContext
            {
                GlobalId = "tes",
                ShortName = "TES".GetPrefixedName(SchemaPrefixId)
            };

            CreateParent();

            var syntax = Create
                .Table(Table.Person, this).InSchema(SchemaName)
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
                .WithColumn(Column.SequenceNumber).AsInt32().NotNullable().WithColumnDescription("sequence number.")
                .WithColumn(Column.Alive).AsBoolean().NotNullable().WithColumnDescription("Alive flag.")
                .WithColumn(Column.Username).AsString().WithColumnDescription("username.")
                .WithColumn("TestCol").AsGuid().WithColumnDescription("TestCol Guid")
                .WithColumn(Column.Password).AsString().WithColumnDescription("password.")
                .WithDefaultColumns()
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
                .ToTable("Testing", this)
                .WithChangeLog(testChangeLogContext, this);

            Rename
                .Column("TestDescription").OnTable("Testing", this)
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
            var testChangeLogContext = new ChangeLogContext
            {
                GlobalId = "tes",
                ShortName = "TES".GetPrefixedName(SchemaPrefixId)
            };
            Delete.Table(Table.Person,this)
                .WithChangeLog(PersonLogContext,this);

            Delete.Table(Table.Parent, this)
                .WithChangeLog(ParentLogContext, this);

            Delete.Table("Testing", this)
                .WithChangeLog(testChangeLogContext, this);
        }
    }
}