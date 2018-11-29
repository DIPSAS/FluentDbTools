using System;
using System.IO;
using System.Linq;
using DIPS.FluentDbTools.Common.Abstractions;
using DIPS.FluentDbTools.Migration.Abstractions;
using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Oracle;

namespace DIPS.FluentDbTools.Migration.Oracle
{
    internal class ExtendedOracleMigrationGenerator : IExtendedMigrationGenerator<ExtendedOracleMigrationGenerator>
    {
        private const string CreateIndexSqlTemplate = "CREATE {0}INDEX {1} ON {2} ({3})";
        private const string CreateConstraintSqlTemplate = "ALTER TABLE {0} ADD CONSTRAINT {1} {2} ({3})";

        private const string GrantSessionAccessToUserSqlTemplate = "GRANT CREATE SESSION TO {0}";

        private const string CreateDefaultTableSpaceSqlTemplate = "CREATE BIGFILE TABLESPACE {0} DATAFILE '{0}.dat' SIZE 20M AUTOEXTEND ON";
        private const string GrantTablespaceAccessToUserSqlTemplate = "GRANT UNLIMITED TABLESPACE TO {0}";
        private const string CreateTempTableSpaceSqlTemplate = "CREATE TEMPORARY TABLESPACE {0} TEMPFILE '{0}.dbf' SIZE 50m autoextend on next 2m maxsize unlimited";

        private const string SchemaExistsSqlTemplate = "SELECT 1 FROM ALL_USERS WHERE USERNAME = '{0}'";
        private const string CreateSchemaSqlTemplate = "CREATE USER {0} IDENTIFIED BY {1} DEFAULT TABLESPACE {2} TEMPORARY TABLESPACE {3}";
        private const string DropSchemaSqlTemplate = 
@"DECLARE
    lc_username   VARCHAR2 (32) := '{0}';
BEGIN
    FOR ln_cur IN (SELECT sid, serial# FROM v$session WHERE username = lc_username)
    LOOP
        EXECUTE IMMEDIATE ('ALTER SYSTEM KILL SESSION ''' || ln_cur.sid || ',' || ln_cur.serial# || ''' IMMEDIATE');
    END LOOP;
    EXECUTE IMMEDIATE ('DROP USER {0} CASCADE');
END;";

        private const string TableSpaceExistsSqlTemplate = "SELECT 1 FROM DBA_TABLESPACES WHERE TABLESPACE_NAME = '{0}'";
        
        private readonly IQuoter Quoter;
        private readonly IDbConfig DbConfig;

        public ExtendedOracleMigrationGenerator(
            OracleQuoterBase quoter,
            IDbConfig dbConfig)
        {
            Quoter = quoter;
            DbConfig = dbConfig;
        }

        public virtual string GetUniqueString(CreateIndexExpression column)
        {
            return column.Index.IsUnique ? "UNIQUE " : string.Empty;
        }


        public string Generate(CreateIndexExpression expression)
        {
            var indexColumns = new string[expression.Index.Columns.Count];

            for (var i = 0; i < expression.Index.Columns.Count; i++)
            {
                var columnDef = expression.Index.Columns.ElementAt(i);
                if (columnDef.Direction == Direction.Ascending)
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " ASC";
                }
                else
                {
                    indexColumns[i] = Quoter.QuoteColumnName(columnDef.Name) + " DESC";
                }
            }

            return string.Format(CreateIndexSqlTemplate
                , GetUniqueString(expression)
                , QuoteSchemaAndIdentifier(expression.Index.Name, expression.Index.SchemaName)
                , Quoter.QuoteTableName(expression.Index.TableName, expression.Index.SchemaName)
                , string.Join(", ", indexColumns));
        }

        public string Generate(CreateConstraintExpression expression)
        {
            var constraintType = (expression.Constraint.IsPrimaryKeyConstraint) ? "PRIMARY KEY" : "UNIQUE";

            var columns = new string[expression.Constraint.Columns.Count];

            for (var i = 0; i < expression.Constraint.Columns.Count; i++)
            {
                columns[i] = Quoter.QuoteColumnName(expression.Constraint.Columns.ElementAt(i));
            }

            return string.Format(CreateConstraintSqlTemplate, Quoter.QuoteTableName(expression.Constraint.TableName, expression.Constraint.SchemaName),
                Quoter.QuoteConstraintName(expression.Constraint.ConstraintName, expression.Constraint.SchemaName),
                constraintType, 
                string.Join(", ", columns));
        }

        public string GenerateDefaultPrivilegesSql(string schemaName)
        {
            throw new NotImplementedException();
        }

        public string GenerateSequenceExistsSql(string schemaName, string sequenceName)
        {
            if (sequenceName == null)
            {
                throw new ArgumentNullException(nameof(sequenceName));
            }

            if (sequenceName.Length == 0)
            {
                sequenceName = "-empty-";
            }

            return string.IsNullOrEmpty(schemaName) 
                ? $"SELECT 1 FROM ALL_SEQUENCES WHERE SEQUENCE_NAME = '{sequenceName.ToUpper()}'" 
                : $"SELECT 1 FROM ALL_SEQUENCES WHERE SEQUENCE_NAME = '{sequenceName.ToUpper()}' and SEQUENCE_OWNER = '{schemaName.ToUpper()}'";
        }


        public string Generate(DeleteSchemaExpression expression)
        {
            return string.Format(DropSchemaSqlTemplate, expression.SchemaName.ToUpper());
        }

        public string Generate(DeleteUserExpression expression)
        {
            throw new NotImplementedException();
        }
        
        public string Generate(CreateUserExpression expression)
        {
            throw new NotImplementedException();
        }

        public string Generate(CreateSchemaExpression expression)
        {
            var schemaPassword = expression.SchemaName;
            if (expression.SchemaName.Equals(DbConfig.Schema,
                StringComparison.OrdinalIgnoreCase))
            {
                schemaPassword = DbConfig.Password;
            }

            using (var writer = new StringWriter())
            {
                writer.WriteLine(CreateSchemaSqlTemplate,
                    Quoter.QuoteSchemaName(expression.SchemaName),
                    Quoter.QuoteSchemaName(schemaPassword),
                    Quoter.QuoteSchemaName(DbConfig.DefaultTablespace),
                    Quoter.QuoteSchemaName(DbConfig.TempTablespace));
                writer.WriteLine(";");
                writer.WriteLine(GrantTableSpaceAccess(expression.SchemaName));
                writer.WriteLine(";");
                writer.WriteLine(GrantSessionAccess(expression.SchemaName));

                return writer.ToString();

            };
        }

        public string GenerateUserExistsSql(string user)
        {
            throw new NotImplementedException();
        }

        public string GenerateSchemaExistsSql(string schemaName)
        {
            return string.Format(SchemaExistsSqlTemplate, Quoter.QuoteSchemaName(schemaName.ToUpper()));
        }

        public string GenerateTableSpaceExistsSql(TableSpaceType tableSpaceType)
        {
            return string.Format(TableSpaceExistsSqlTemplate, GetTableSpaceName(tableSpaceType));
        }

        public string GenerateCreateTableSpaceSql(TableSpaceType tableSpaceType)
        {
            return string.Format(GetCreateTableSpaceSqlTemplate(tableSpaceType), GetTableSpaceName(tableSpaceType));
        }

        public string GetTableSpaceName(TableSpaceType tableSpaceType)
        {
            var tableSpace = tableSpaceType == TableSpaceType.Temp
                ? DbConfig.TempTablespace
                : DbConfig.DefaultTablespace;
            return tableSpace?.ToUpper();
        }

        private string GrantSessionAccess(string schemaName)
        {
            return string.Format(GrantSessionAccessToUserSqlTemplate, Quoter.QuoteSchemaName(schemaName));
        }

        private string GrantTableSpaceAccess(string schemaName)
        {
            return string.Format(GrantTablespaceAccessToUserSqlTemplate, Quoter.QuoteSchemaName(schemaName));
        }


        private static string GetCreateTableSpaceSqlTemplate(TableSpaceType tableSpaceType)
        {
            var tableSpace = tableSpaceType == TableSpaceType.Temp
                ? CreateTempTableSpaceSqlTemplate
                : CreateDefaultTableSpaceSqlTemplate;
            return tableSpace;
        }

        private string QuoteSchemaAndIdentifier(string quotedSchemaName, string quotedIdentifier)
        {
            return Quoter.QuoteTableName(quotedSchemaName, quotedIdentifier);
        }
    }
}