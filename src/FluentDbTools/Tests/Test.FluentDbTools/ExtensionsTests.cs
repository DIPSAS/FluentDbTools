using FluentAssertions;
using Xunit;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;

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
        [InlineData("CREATE USER UserName IDENTIFIED by values 'XXXXXXXXX' ACCOUNT LOCK ","Create User [UserName]")]
        [InlineData("CREATE OR REPLACE FUNCTION Schema.Function(p IN VARCHAR2) \n RETURN \n PRAGMA AUTONOMOUS_TRANSACTION","Create Function [Schema.Function]")]
        [InlineData("CREATE OR REPLACE FUNCTION Schema.Function \n RETURN \n PRAGMA AUTONOMOUS_TRANSACTION","Create Function [Schema.Function]")]
        [InlineData("CREATE OR REPLACE PROCEDURE Schema.Procedure(p IN VARCHAR2) IS\n PRAGMA AUTONOMOUS_TRANSACTION","Create Procedure [Schema.Procedure]")]
        [InlineData("CREATE OR REPLACE PROCEDURE Schema.Procedure IS\n PRAGMA AUTONOMOUS_TRANSACTION","Create Procedure [Schema.Procedure]")]
        [InlineData("CREATE OR REPLACE Package Body Schema.Package Is","Create Package Body [Schema.Package]")]
        [InlineData("CREATE OR REPLACE Package Schema.Package Is","Create Package [Schema.Package]")]
        [InlineData("create sequence Schema.TABLE_SEQ \nminvalue 1","Create Sequence [Schema.TABLE_SEQ]")]
        [InlineData("create or replace synonym Schema.dest for OtherSchema.source","Create Synonym [Schema.dest for OtherSchema.source]")]
        [InlineData("/* My comment */","-- My comment")]
        [InlineData("/* My comment1 */\n/* My comment2 */","-- My comment1\n-- My comment2")]
        [InlineData("create index Schema.IndexName_idx on Schema.Table (COL1,COL2)","Create Index [Schema.IndexName_idx on Schema.Table]")]
        [InlineData("alter table Schema.Table\nadd constraint ..","Alter Table [Schema.Table]")]
        [InlineData("create table Schema.Table\n(column....","Create Table [Schema.Table]")]
        [InlineData("comment on column Schema.Table.Column\n is 'my comment'","Add Column Comment [Schema.Table.Column => 'my comment']")]
        public void ConvertToSqlTitle_StringExtensions_ShouldBeOk(string sql, string expected)
        {
            sql.ConvertToSqlTitle().Should().Be(expected);
        }

    }
}