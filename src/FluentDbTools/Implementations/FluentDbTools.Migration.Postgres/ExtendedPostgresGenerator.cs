using System;
using System.IO;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Postgres;

namespace FluentDbTools.Migration.Postgres
{
    internal class ExtendedPostgresGenerator : IExtendedMigrationGenerator<ExtendedPostgresGenerator>
    {
        private const string GrantSchemaDefaultPrivilegesTemplate = "ALTER DEFAULT PRIVILEGES IN SCHEMA {0} GRANT ALL ON TABLES TO PUBLIC;" +
                                                                   "ALTER DEFAULT PRIVILEGES IN SCHEMA {0} GRANT SELECT, USAGE ON SEQUENCES TO PUBLIC;" +
                                                                   "ALTER DEFAULT PRIVILEGES IN SCHEMA {0} GRANT EXECUTE ON FUNCTIONS TO PUBLIC;" +
                                                                   "ALTER DEFAULT PRIVILEGES IN SCHEMA {0} GRANT USAGE ON TYPES TO PUBLIC;";

        private const string GrantSchemaAccessToUserSqlTemplate = "GRANT ALL ON SCHEMA {0} TO PUBLIC;" + GrantSchemaDefaultPrivilegesTemplate;
        private const string AlterUserSqlTemplate = "ALTER USER {0} IN DATABASE {1} SET search_path={2};";
        private const string UserExistsSqlTemplate = "SELECT 1 FROM pg_roles WHERE rolname= '{0}';";

        private const string SchemaExistsSqlTemplate = "SELECT 1 FROM pg_namespace WHERE nspname = '{0}';";

        private const string CreateUserSqlTemplate = "CREATE ROLE {0} WITH LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION CONNECTION LIMIT -1 PASSWORD '{1}';";
        private const string CreateSchemaSqlTemplate = "CREATE SCHEMA {0} AUTHORIZATION {1};" +
                                                       "GRANT ALL ON SCHEMA {0} TO PUBLIC;";

        private const string DropSchemaSqlTemplate = "DROP SCHEMA IF EXISTS {0} CASCADE;";
        private const string DropUserSqlTemplate = "DROP ROLE IF EXISTS {0};";

        private readonly IQuoter Quoter;
        private readonly IDbConfig DbConfig;

        public ExtendedPostgresGenerator(
            PostgresQuoter quoter,
            IDbConfig dbConfig)
        {
            Quoter = quoter;
            DbConfig = dbConfig;
        }


        public string Generate(DeleteSchemaExpression expression)
        {
            return string.Format(DropSchemaSqlTemplate, Quoter.QuoteSchemaName(expression.SchemaName));
        }

        public string Generate(DeleteUserExpression expression)
        {
            return string.Format(DropUserSqlTemplate, Quoter.QuoteSchemaName(expression.SchemaName));
        }

        public string Generate(CreateSchemaExpression expression)
        {
            using (var writer = new StringWriter())
            {
                writer.WriteLine(CreateSchemaSqlTemplate, Quoter.QuoteSchemaName(expression.SchemaName), Quoter.QuoteSchemaName(expression.SchemaName));
                writer.WriteLine(GrantSchemaAccessToUser(expression.SchemaName, expression.SchemaName));
                //writer.WriteLine(AlterUser(expression.SchemaName, expression.SchemaName));


                return writer.ToString();
            };
        }

        public string Generate(CreateUserExpression expression)
        {
            var schemaPassword = expression.SchemaName;
            if (expression.SchemaName.Equals(DbConfig.Schema,
                StringComparison.OrdinalIgnoreCase))
            {
                schemaPassword = DbConfig.Password;
            }

            var sql = string.Format(CreateUserSqlTemplate, Quoter.QuoteSchemaName(expression.SchemaName), schemaPassword);
            return sql;
        }

        public string GenerateUserExistsSql(string user)
        {
            return string.Format(UserExistsSqlTemplate, user);
        }

        public string GenerateSchemaExistsSql(string schemaName)
        {
            return string.Format(SchemaExistsSqlTemplate, schemaName);
        }

        public string GenerateTableSpaceExistsSql(TableSpaceType tableSpaceType)
        {
            throw new NotImplementedException();
        }

        public string GenerateCreateTableSpaceSql(TableSpaceType tableSpaceType)
        {
            throw new NotImplementedException();
        }

        public string GetTableSpaceName(TableSpaceType tableSpaceType)
        {
            throw new NotImplementedException();
        }

        public string Generate(CreateIndexExpression expression)
        {
            throw new NotImplementedException();
        }

        public string Generate(CreateConstraintExpression expression)
        {
            throw new NotImplementedException();
        }

        public string GenerateDefaultPrivilegesSql(string schemaName)
        {
            return string.Format(GrantSchemaDefaultPrivilegesTemplate, Quoter.QuoteSchemaName(schemaName));
        }
    
        public string GenerateSequenceExistsSql(string schemaName, string sequenceName)
        {
            throw new NotImplementedException();
        }
        
        private string GrantSchemaAccessToUser(string user, string schemaName)
        {
            return string.Format(GrantSchemaAccessToUserSqlTemplate, Quoter.QuoteSchemaName(schemaName) , Quoter.QuoteSchemaName(user));
        }

        private string AlterUser(string user, string schemaName)
        {
            
            return string.Format(AlterUserSqlTemplate, Quoter.QuoteSchemaName(user), Quoter.QuoteSchemaName(DbConfig.DatabaseConnectionName), Quoter.QuoteSchemaName(schemaName));
        }

    }
}