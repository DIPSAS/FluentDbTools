# FluentDbTools

[![Build Status](https://travis-ci.com/DIPSAS/FluentDbTools.svg?branch=master)](https://travis-ci.com/DIPSAS/FluentDbTools)
[![MIT license](http://img.shields.io/badge/license-MIT-brightgreen.svg)](http://opensource.org/licenses/MIT)
[![API documentation](https://img.shields.io/badge/DocFX-API%20documentation-informational)](https://dipsas.github.io/FluentDbTools/docs/doc/)

FluentDbTools provides a fluent SQL abstraction layer for creating database connections, building sql queries and migrating your database.

Following databases are currently supported:
- Oracle
- Postgres

## Nugets
- FluentDbTools.Extensions.DbProvider: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.DbProvider.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.DbProvider)
- FluentDbTools.Extensions.SqlBuilder: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.SqlBuilder.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.SqlBuilder)
- FluentDbTools.Extensions.Migration: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.Migration.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.Migration)
- FluentDbTools.Extension.MSDependencyInjection: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection)
- FluentDbTools.Extension.MSDependencyInjection.Oracle: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection.Oracle.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection.Oracle)
- FluentDbTools.Extension.MSDependencyInjection.Postgres: [![NuGet version](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection.Postgres.svg)](https://badge.fury.io/nu/FluentDbTools.Extensions.MSDependencyInjection.Postgres)

## Example
The `IDbConfig` interface is the only member you need to instantiate.
It provides a simple interface for providing which database type to use, and other necessary database settings.

### Create a Database Connection
```csharp
IDbConfig dbConfig = new DbConfig(..) // Implementantion of IDbConfig requested
OracleClientFactory.Instance.Register(SupportedDatabaseTypes.Oracle); // Register the database factories you see fit
NpgsqlFactory.Instance.Register(SupportedDatabaseTypes.Postgres);
var dbConnection = dbConfig.CreateDbConnection();
```

### Register Database Providers with MSDependencyInjection
```csharp     
var serviceCollection = new ServiceCollection();       
var serviceProvider = serviceCollection
    .AddSingleton<IConfiguration>(serviceProvider => new ConfigurationBuilder()
        .AddJsonFile("config.json"))
    .AddOracleDbProvider()
    .AddPostgresDbProvider()
    .BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetService<DbProviderFactory>();
    var dbConnection = scope.ServiceProvider.GetService<IDbConnection>();
}
```

### Typical Json Configuration with MSDependencyInjection
```json
"database": {
    "type": "postgres",
    "user": "dbuser",
    "password": "dbpassword",
    "adminUser": "admindbuser",
    "adminPassword": "admindbuser",
    "schema": "dbuser", 
    "databaseName": "dbuser", 
    "hostname": "localhost",
    "port": 5432,
    "pooling": true,
	// If you want all tables should be prefixed with EX 
	// i.e. Person will give tablename EXPerson in the database
	"schemaPrefix": {
        "Id": "EX", 
		"UniqueId": "excv", 
		"tables": {
			// ChangeLogContext for table "Person"
			"person": {
				"GlobalId": "abcd",
				"ShortName": "EXTST"
			}
		}
    }
    "migration": {
        "schemaPassword": "dbpassword" 
    }
}

// If database:schema not set, then it equals to 'database:user'
// If database:databaseName not set, then it equals to 'database:schema'
// If database:schemaPassword not set, then it equals to 'database:password'
```

### Build SQL Query Fluently
```csharp
IDbConfig dbConfig = DbConfigDatabaseTargets.Create(databaseTypes, schema);
var sql = dbConfig.CreateSqlBuilder().Select()
            .OnSchema()
            .Fields<Person>(x => x.F(item => item.Id))
            .Fields<Person>(x => x.F(item => item.SequenceNumber))
            .Fields<Person>(x => x.F(item => item.Username))
            .Fields<Person>(x => x.F(item => item.Password))
            .Where<Person>(x => x.WP(item => item.Id))
            .Build();
```

### Register Migration With MSDependencyInjection & Migrate the Database With Extended FluentMigrator
```csharp
IEnumerable<Assembly> migrationAssemblies => new[] { typeof(AddPersonTable).Assembly };
var serviceProvider = new ServiceCollection()
    .AddSingleton<IConfiguration>(serviceProvider => new ConfigurationBuilder()
        .AddJsonFile("config.json"))
    .ConfigureWithMigrationAndScanForVersionTable(migrationAssemblies)
    .BuildServiceProvider();

using (var scope = provider.CreateScope())
{
    var migrationRunner = scope.ServiceProvider.GetService<IMigrationRunner>();

    migrationRunner.MigrateUp();
}
```

```csharp
[Migration(1, "Migration Example With SchemaPrefix")]
public class AddPersonTable : MigrationModel
{
    public override void Up()
    {
        // Create the ChangeLogContext with values from this.GetMigrationConfig()
        // -
        // SchemaPrefixId is fetched from database:migration:schemaPrefix:id or database:schemaPrefix:id<br/>
        // SchemaPrefixUniqueId is fetched from database:migration:schemaPrefix:UniqueId or database:schemaPrefix:UniqueId<br/>
        // -
        // GlobalId is fetched from database:migration:schemaPrefix:tables:person:GlobalId or database:schemaPrefix:tables:person:GlobalId<br/>
        // ShortName is fetched from database:migration:schemaPrefix:tables:person:ShortName or database:schemaPrefix:tables:person:ShortName<br/>
        var personChangeLogContext = new ChangeLogContext(this, Table.Person);

        // When "database:schemaPrefix:Id" or "database:migration:schemaPrefix:Id" have a value,
        // the tableName Person will be created as {SchemaPrefixId}Person.
		//
        // i.e: "database:schemaPrefix:Id" = "EX" => a table with name EXPerson will be created.
        // If both "database:schemaPrefix:Id" and "database:migration:schemaPrefix:Id" is missing,
        // a table with name Person will be created.
        Create.Table(Table.Person.GetPrefixedName(SchemaPrefixId)).InSchema(personChangeLogContext)
            .WithChangeLog(PersonLogContext) // ChangeLog activation for Create.Table(..)
            .WithColumn(Column.Id).AsGuid().PrimaryKey()
            .WithColumn(Column.SequenceNumber).AsInt32().NotNullable()
            .WithColumn(Column.Username).AsString()
            .WithColumn(Column.Password).AsString()
            .WithDefaultColumns() // Enable DefaultColumns functionality
            .WithTableSequence(this);
    }

    public override void Down()
    {
        var personChangeLogContext = new ChangeLogContext(this, Table.Person);
        Delete.Table(GetPrefixedName(Table.Person))
            .WithChangeLog(personChangeLogContext,this)
            .InSchema(SchemaName);

    }
}
```

### More Examples
Please have a look in the example folder: 
- [src/FluentDbTools/Example](src/FluentDbTools/Example)

## Get Started
1. Install [Docker](https://www.docker.com/)
2. Install [Python](https://www.python.org/) and [pip](https://pypi.org/project/pip/)
    - Windows:  https://matthewhorne.me/how-to-install-python-and-pip-on-windows-10/
    - Ubuntu: Python is installed by default
        - Install pip: sudo apt-get install python-pip
3. Install python dependencies:
    - pip install DockerBuildManagement
4. See available commands:
    - `dbm -help`

## Build & Run
1. Start domain development by deploying service dependencies:
    - `dbm -swarm -start`
2. Test solution in containers:
    - `dbm -test`
3. Open solution and continue development:
    - [FluentDbTools](src/FluentDbTools)
4. Publish new nuget version:
    - Bump version in [CHANGELOG.md](CHANGELOG.md)
    - Build and publish nugets: `dbm -build -run -publish`
7. Stop development when you feel like it:
    - `dbm -swarm -stop`

### Buildsystem
- [DockerBuildManagement](https://github.com/DIPSAS/DockerBuildManagement)
- [DockerBuildSystem](https://github.com/DIPSAS/DockerBuildSystem)
- [SwarmManagement](https://github.com/DIPSAS/SwarmManagement)
