using System;
using System.Globalization;
using FluentAssertions;
using Xunit;
using FluentDbTools.Common.Abstractions;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace Test.FluentDbTools
{
    public class ExtensionsTests
    {
        [Fact]
        public void SubstringFrom_StringExtensions_ShouldBeOk()
        {
            var phrase =
                "S:5FCC88CA75053EDA113E7105CE536B17872C9F8EEAF794682435A1534680;T:A3276EB989542D18A5E7B4B7F57479B6248CFD2F7797A649F98A0E8718390D4A401581CBA7965ACB775503A4E0D5114239B893B12B12F110D7230AE40891144647B30B974F96A77A016A6A4B9FEF8225;74DDAF058DD979E8";
            $"CREATE USER SCHEMANAME IDENTIFIED by values '{phrase}' ACCOUNT LOCK "
                .SubstringFrom("S:", "'").Should().Be(phrase);
        }

        [Theory]
        [InlineData("CREATE USER UserName IDENTIFIED by values 'XXXXXXXXX' ACCOUNT LOCK ", "Create User [UserName]")]
        [InlineData("CREATE OR REPLACE FUNCTION Schema.Function(p IN VARCHAR2) \n RETURN \n PRAGMA AUTONOMOUS_TRANSACTION", "Create Function [Schema.Function]")]
        [InlineData("CREATE OR REPLACE FUNCTION Schema.Function \n RETURN \n PRAGMA AUTONOMOUS_TRANSACTION", "Create Function [Schema.Function]")]
        [InlineData("CREATE OR REPLACE PROCEDURE Schema.Procedure(p IN VARCHAR2) IS\n PRAGMA AUTONOMOUS_TRANSACTION", "Create Procedure [Schema.Procedure]")]
        [InlineData("CREATE OR REPLACE PROCEDURE Schema.Procedure IS\n PRAGMA AUTONOMOUS_TRANSACTION", "Create Procedure [Schema.Procedure]")]
        [InlineData("CREATE OR REPLACE Package Body Schema.Package Is", "Create Package Body [Schema.Package]")]
        [InlineData("CREATE OR REPLACE Package Schema.Package Is", "Create Package [Schema.Package]")]
        [InlineData("create sequence Schema.TABLE_SEQ \nminvalue 1", "Create Sequence [Schema.TABLE_SEQ]")]
        [InlineData("create or replace synonym Schema.dest for OtherSchema.source", "Create Synonym [Schema.dest for OtherSchema.source]")]
        [InlineData("/* My comment */", "-- My comment")]
        [InlineData("/* My comment1 */\n/* My comment2 */", "-- My comment1\n-- My comment2")]
        [InlineData("create index Schema.IndexName_idx on Schema.Table (COL1,COL2)", "Create Index [Schema.IndexName_idx => Schema.Table(COL1,COL2)]")]
        [InlineData("alter table Schema.Table\nadd constraint ..", "Alter Table [Schema.Table]")]
        [InlineData("create table Schema.Table", "Create Table [Schema.Table]")]
        [InlineData("create table Schema.Table\n(column1,column2)", "Create Table [Schema.Table (column1, column2)]")]
        [InlineData("create table Schema.Table\n(column....", "Create Table [Schema.Table (column....)]")]
        [InlineData("comment on column Schema.Table.Column\n is 'my comment'", "Add Column Comment [Schema.Table.Column => 'my comment']")]
        [InlineData("comment on column Schema.Table.Column is 'my comment'", "Add Column Comment [Schema.Table.Column => 'my comment']")]
        [InlineData("-- Create/Recreate indexes", "-- Create/Recreate indexes")]
        [InlineData("-- Title = Create table Schema.Table", "-- Title Create table Schema.Table")]
        [InlineData("-- Title= Create table Schema.Table", "-- Title Create table Schema.Table")]
        [InlineData("-- Title =Create table Schema.Table", "-- Title Create table Schema.Table")]
        [InlineData("-- Title Create table Schema.Table\n-- Title = Title2\n-- Title= Title3\n-- Title=Title4\nBEGIN", "-- Title Create table Schema.Table\nTitle2\nTitle3\nTitle4")]
        [InlineData("-- Title Create table Schema.Table\n-- Title = Title2\n-- Title= Title3\n-- Title=Title4\n-- EndTitle", "-- Title Create table Schema.Table\nTitle2\nTitle3\nTitle4")]
        [InlineData(@"
-- Title = Testing ErrorFilter parsing
-- ErrorFilter = 6512, 955 	
BEGIN 
  EXECUTE IMMEDIATE 'CREATE TABLE {SchemaName}.{SchemaPrefixId}Person (PersonId RAW(16) DEFAULT sys_guid() NOT NULL, SequenceNumber NUMBER(10,0) NOT NULL, Alive NUMBER(1,0) NOT NULL, Username NVARCHAR2(255) NOT NULL, ExtraInformation BLOB, TestCol RAW(16) NOT NULL, Password NVARCHAR2(255) NOT NULL, ParentId_FK RAW(16))';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.PersonId IS ''Unique id.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.SequenceNumber IS ''sequence number.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.Alive IS ''Alive flag.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.Username IS ''username.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.ExtraInformation IS ''Extra Information as blob.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.TestCol IS ''TestCol Guid''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN {SchemaName}.{SchemaPrefixId}Person.Password IS ''password.'''; 
END;", 
            "-- Title Testing ErrorFilter parsing")]

        [InlineData(@"
-- ErrorFilter = 6512, 1918
BEGIN 
  EXECUTE IMMEDIATE 'CREATE TABLE InvalidSchema.Person (PersonId RAW(16) DEFAULT sys_guid() NOT NULL, SequenceNumber NUMBER(10,0) NOT NULL, Alive NUMBER(1,0) NOT NULL, Username NVARCHAR2(255) NOT NULL, ExtraInformation BLOB, TestCol RAW(16) NOT NULL, Password NVARCHAR2(255) NOT NULL, ParentId_FK RAW(16))';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.PersonId IS ''Unique id.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.SequenceNumber IS ''sequence number.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Alive IS ''Alive flag.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Username IS ''username.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.ExtraInformation IS ''Extra Information as blob.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.TestCol IS ''TestCol Guid''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Password IS ''password.'''; 
END;", 
@"Create Table [InvalidSchema.Person (PersonId, SequenceNumber, Alive, Username, ExtraInformation, TestCol, Password, ParentId_FK)]
With Column Comment [PersonId => 'Unique id.']
With Column Comment [SequenceNumber => 'sequence number.']
With Column Comment [Alive => 'Alive flag.']
With Column Comment [Username => 'username.']
With Column Comment [ExtraInformation => 'Extra Information as blob.']
With Column Comment [TestCol => 'TestCol Guid']
With Column Comment [Password => 'password.']"
)]
        [InlineData(@"
-- ErrorFilter = 6512, 1918
BEGIN 
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.PersonId IS ''Unique id.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.SequenceNumber IS ''sequence number.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Alive IS ''Alive flag.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Username IS ''username.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.ExtraInformation IS ''Extra Information as blob.''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.TestCol IS ''TestCol Guid''';
  EXECUTE IMMEDIATE 'COMMENT ON COLUMN InvalidSchema.Person.Password IS ''password.'''; 
END;", 
@"Add Column Comment [InvalidSchema.Person.PersonId => 'Unique id.']
Add Column Comment [InvalidSchema.Person.SequenceNumber => 'sequence number.']
Add Column Comment [InvalidSchema.Person.Alive => 'Alive flag.']
Add Column Comment [InvalidSchema.Person.Username => 'username.']
Add Column Comment [InvalidSchema.Person.ExtraInformation => 'Extra Information as blob.']
Add Column Comment [InvalidSchema.Person.TestCol => 'TestCol Guid']
Add Column Comment [InvalidSchema.Person.Password => 'password.']"
)]

        public void ConvertToSqlTitle_StringExtensions_ShouldBeOk(string sql, string expected)
        {
            sql.ConvertToSqlTitle().Should().Be(expected.Replace("\r",""));
        }

        [Theory]
        [InlineData(true, "test")]
        [InlineData(true, "test", "TEST")]
        [InlineData(true, "test", null, StringComparison.CurrentCulture)]
        [InlineData(false, "test", "TEST", StringComparison.CurrentCulture)]
        [InlineData(false, "ss", "ß", StringComparison.OrdinalIgnoreCase)]
        [InlineData(null, "ss", "ß", StringComparison.InvariantCulture)]
        [InlineData(null, "ss", "ß", StringComparison.InvariantCultureIgnoreCase)]
        [InlineData(false, "i", "I", StringComparison.CurrentCulture, "en-GB")]
        [InlineData(true, "i", "I", StringComparison.CurrentCultureIgnoreCase, "en-GB")]
        [InlineData(false, "i", "İ", StringComparison.CurrentCultureIgnoreCase, "en-GB")]
        [InlineData(false, "i", "I", StringComparison.CurrentCultureIgnoreCase, "tr-TR")]
        [InlineData(true, "i", "İ", StringComparison.CurrentCultureIgnoreCase, "tr-TR")]
        public void EqualsIgnoreCase_StringExtensions_ShouldBeOk(bool? isEqual, string s1, string s2 = null, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase, string cultureName = "nb-NO")
        {
            var current = CultureInfo.CurrentCulture;
            var currentIgnoreCaseStringComparison = StringExtensions.CurrentIgnoreCaseStringComparison;

            isEqual ??= false;

            s2 ??= s1;

            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName ?? "nb-NO");
                StringExtensions.CurrentIgnoreCaseStringComparison = stringComparison;
                s1.EqualsIgnoreCase(s2).Should().Be(isEqual == true);
            }
            finally
            {
                CultureInfo.CurrentCulture = current;
                StringExtensions.CurrentIgnoreCaseStringComparison = currentIgnoreCaseStringComparison;
            }
        }
    }
}