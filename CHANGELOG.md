# Changelog FluentDbTools
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

<!-- the topmost header version must be set manually in the VERSION file -->
### Version 1.2.40 2022-02-28
- Improved ExtractSqlStatements. Problem with nested begin / end

### Version 1.2.39 2022-02-24
- Added UserGrant configs

### Version 1.2.38 2022-02-21
- Even more migration logging improvements

### Version 1.2.37 2022-02-14
- Even more migration logging improvements

### Version 1.2.36 2022-02-14
- Migration logging improvements

### Version 1.2.35 2022-02-10
- Added Errorfilter to Create and Alter syntax
- Improved resolving migration log-file
- Ignored Disposed exception for Migration RollbackTransaction 

### Version 1.2.34 2022-01-21
- Improved IsInvalidDatabaseAdminException extension method.
- Added tests for the IsInvalidDatabaseAdminException-method

### Version 1.2.33 2022-01-20
- Added DatabaseAdmin validation feature to IDbConfig.AdminUser and IDbConfig.AdminPassword

### Version 1.2.32 2022-01-19
- Fixed bug introduced with empty Default admin username and password

### Version 1.2.31 2022-01-19
- Made Default admin username and password configurable

### Version 1.2.30 2022-01-14
- More DefaultDbConfigValues.DefaultDbType stuff
- Added tests

### Version 1.2.29 2022-01-14
- Made DefaultDbConfigValues.DefaultDbType changable

### Version 1.2.28 2022-01-12
- Extended PrepareSql with additional parameters

### Version 1.2.27 2021-11-08
- Improved LogFileAppendFluentMigratorLoggerProvider. 
  If directories or directory is missing, the directory will be created before the logfile is created

### Version 1.2.26 2021-11-05
- Improved LogFileAppendFluentMigratorLoggerProvider. 
  If logfile is already open, change logfile to a filename with timestamp - ex. "abcd.log" => "abcd-20211105.log"

### Version 1.2.25 2021-06-17
- Added openConnectionWhenRequest parameter to ServiceCollectionDbProviderExtensions:AddDbProvider method

### Version 1.2.24 2021-06-14
- Added database:poolingKeyValues to tune connecting-pooling 

### Version 1.2.23 2021-04-08
- Overrided OracleProcessorBase:DataSet Read(string template, params object[] args) in ExtendedOracleProcessorBase to log exception if it happens
- Overrided OracleProcessorBase:DataSet ReadTableData(string schemaName, string tableName) in ExtendedOracleProcessorBase to handle exception better

### Version 1.2.20 2021-03-25
- Expaned DataDictionaryRequiredLevel with new enum-values

### Version 1.2.19 2021-02-15
- Improved ExtractSqlStatements extension method. 
- Implemented ErrorFilter detection

### Version 1.2.18 2020-08-28
- Improved Connection.Open. 

### Version 1.2.17 2020-04-03
- Added IDataDictionarySchemaPrefixAppender to FluentDbTools.Migration.Abstractions

### Version 1.2.16 2020-04-02
- Made configuration optional for MsConfigurationChangedHandler

### Version 1.2.15 2020-03-05
- Improved PrioritizedConfigValues Password resolving

### Version 1.2.14 2020-03-05
- Extended IPrioritizedConfigKeys with Order property
- Added more API documentations

### Version 1.2.13 2020-03-05
- Even more PrioritizedConfig stuff. Improved MsConfig and MsMigrationConfig

### Version 1.2.12 2020-03-05
- Added PrioritizedConfig to MsMigrationConfig

### Version 1.2.11 2020-03-05
- Fixed build/merge fail

### Version 1.2.10 2020-03-05
- More PrioritizedConfig stuff (IConfigurationDelimiter)

### Version 1.2.9 2020-03-02
- More PrioritizedConfig stuff

### Version 1.2.8 2020-03-02
- More PrioritizedConfig stuff

### Version 1.2.7 2020-02-28
- Made it possible to define how some configValues is fetched (IPrioritizedConfigValues <-> PrioritizedConfigValues)

### Version 1.2.6 2020-02-18
- Fixed Scope registrations of DbConfig and DbProvider
- Fixed Scope registrations of Migration LoggerProvider.

### Version 1.2.5 2020-02-18
- Fixed Scope registrations of DbConfig and DbProvider
- Fixed Scope registrations of Migration LoggerProvider.

### Version 1.2.4 2020-02-02
- Resolved issue on SqlBuilder where a invalid alias (as) was genereted.
- Introduced SqlAliasHelper with Blacklisted Aliases functionallity.
- The Alias-Blacklist is pre-filled with "as", "on" etc...

### Version 1.2.3 2020-01-20
- Added SolutionInfo.proj to FluentDbTools.Abstractions and FluentDbTools.SqlBuilder.Dapper project

### Version 1.2.2 2020-01-20
- Changed extension method names in FluentDbTools.Extensions.SqlBuilder

### Version 1.2.1 2020-01-20
- Removed Dapper dependecies fra FluentDbTools.SqlBuilder
- Introduced FluentDbTools.Abstraction project
- Changed FluentDbTools.SqlBuilder.Abstraction to depend on FluentDbTools.Abstraction

### Version 1.1.35 2020-01-13
- Improved ConfigMatching (CaseInsesitive)
- Improved NoConnection an Preview Migration handling

### Version 1.1.33 2019-11-22
- Added Password secret functionallity

### Version 1.1.32 2019-11-20
- Improved Sql-logging

### Version 1.1.31 2019-10-30
- Added Disabled as Default Enum value in TriggersAndViewsGeneration enum

### Version 1.1.30 2019-10-30
- Implemented a mechanism for handling configuration-reload of changed config-files
- Added more Code documentation 

### Version 1.1.29 2019-10-24
- Improved LogFileAppendFluentMigratorLoggerProvider
  Will try 10 times with wait of 250ms to create StreamWriter if IOException is thrown 

### Version 1.1.28 2019-09-27
- Fixed  FluentDbTools.Common.Abstractions.Extensions.ExtractSqlStatements
- Added test to verify that ExtractSqlStatements do the work
- Improved Embedded resource handling

### Version 1.1.27 2019-09-23
- Fixed SqlResource for netstandard2.0

### Version 1.1.26 2019-09-23
- Improved implementation of DefaultOracleCustomMigrationProcessor

### Version 1.1.25 2019-09-22
- Implemented DefaultOracleCustomMigrationProcessor and improved sql-statement extracts


### Version 1.1.24 2019-09-22
- Implemented MigrationSchema and added trigging of ICustomMigrationProcessor.MigrationMetadataChanged()

### Version 1.1.23 2019-09-16
- Improved Configuration-dictionary lookup to be case-independent
- VersionTable will now consider whether SchemPrefixId is defined in the configuration 
-  i.e: SchemPrefixId = EX => VersionTable will be EXVersion
-  i.e: SchemPrefixId is unspicified => VersionTable will be Version
- SchemaPrefixId is fetched from database:migration:schemaPrefix:id OR database:schemaPrefix:id
- SchemaPrefixUniqueId is fetched from database:migration:schemaPrefix:UniqueId OR database:schemaPrefix:UniqueId
- Improved how data in ChangeLogContext initialized.
- Added LogFileAppendFluentMigratorLoggerProvider. Sql log can be added to the end of a existing file

### Version 1.1.22 2019-09-12
- Improved how external oracle sql statements is executed

### Version 1.1.21 2019-09-10
- Implemented ICustomMigrationProcessor for ChangeLog, Adding DefaultColumns, and hooks method running after schema is created or after schema is removed

### Version 1.1.20 2019-09-04
- database:migration:defaultTablespace and database:migration:tempTablespace must be specified
  to create oracle user with specific tablespaces

### Version 1.1.19 2019-08-26
- Renamed AdditionalConfigValues to GetAllMigrationConfigValues() function in IDbMigrationConfig
- Removed TableRoleName and CodeRoleName from IDbMigrationConfig
- Added GetAllDatabaseConfigValues() function to IDbConfig

### Version 1.1.18 2019-08-23
- Added AdditionalConfigValues, TableRoleName and CodeRoleName to IDbMigrationConfig
- Added UnitTest for new IDbMigrationConfig properties

### Version 1.1.17 2019-07-10
- Improved Migration-logging configuration
- Removed logging of "SELECT 1" statement for Oracle
- Upgraded FluentMigrator 
- Added some documentation

### Version 1.1.16 2019-07-10
- Improved Migration-logging configuration

### Version 1.1.15 2019-07-10
- Improved Migration-logging configuration

### Version 1.1.14 2019-07-10
- Rebuilded version 1.1.3

### Version 1.1.13 2019-07-10
 - Improved tnsname.ora detection
 - Added migration-logging configuration

### Version 1.1.12 2019-07-08
 - Improved tnsname.ora detection

### Version 1.1.11 2019-05-21
 - Bug fix: Switched scope of DbProviderFactory from singleton to scoped.

### Version 1.1.10 2019-04-12
 - Removed more dependency to Microsoft Extensions.

### Version 1.1.9 2019-04-12
 - Removed MSDepencenyInjection unecessary dependencies.

### Version 1.1.8 2019-04-12
 - Removed MSDepencenyInjection unecessary extensions.

### Version 1.1.7 2019-04-09
 - Added support for scanning for VersionTableMetaData
 - Added extension method ConfigureOracleTnsAdminPath to configure TNS_ADMIN path

### Version 1.1.6 2019-03-25
 - Problem with publish to nuget
 
### Version 1.1.5 2019-03-23
 - Added support of simplified Oracle EzConnect
   
        e.g: database:dataSource = "host/service"
 - Added support of Oracle TnsAliasName
      
        e.g: database:dataSource = "tnsAliasName"
            => "tnsAliasName" must be available in tnsnames.ora 
            => tnsnames.ora must be localized in running folder or specified with 
               Enviroment-variable TNS_ADMIN 
               ie: TNS_ADMIN=C:\oracle\product\11.2.0.3.0\network\admin
 - Added UnitTests for ConnectionStringBuilders
 - Refacted IDbConfig:
   - IDbConfig is now mainly used for reading configuration and data holder for ConnectionString parameters
   - IDbConfig is devided into:
     - IDbMigrationConfig used by DbMigration
     - IDbConnectionStringBuilder used by ConnectionString builders
     - IDbConfigDatabaseTargets used by SqlBuilder
- Reduced use of IDbConfig in UnitTests and Example codes.      

### Version 1.1.4 2019-03-23
 - Fixed Enum bug with sql builder.

### Version 1.1.3 2019-03-19
 - Servicename in config, to support Oracle naming conventions.

### Version 1.1.2 2019-03-15
 - Added skip flag to db provider registration.

### Version 1.1.1 2019-03-14
 - Added service collection extension feature.

### Version 1.1.0 2019-03-13
 - Removed direct db driver dependencies.
 - Included extension projects for oracle and postgres.

### Version 1.0.12 2019-03-12
 - Fixed bug in postgres description generator.

### Version 1.0.11 2019-03-06
 - Added features to specify table name.

### Version 1.0.10 2019-02-28
 - Added DbConfig to dbProvider solution.
 - Renamed DefaultDbConfig to MSDbConfig.
 - Added a default DbConfig to DbProvider (non MS specific).

### Version 1.0.9 2019-02-27
 - Updated default values for DbConfig.

### Version 1.0.8 2019-02-27
 - Default schema/schema password should be equal to user/user password.

### Version 1.0.7 2019-01-18
 - Added support for overriding functions in DefaultDbConfig.

### Version 1.0.6 2019-01-18
 - Added support for setting the connection string template.

### Version 1.0.5 2019-01-02
 - Added support for select with count.

### Version 1.0.4 2019-01-01
 - Added FluentDbProviderFactory extension.

### Version 1.0.3 2019-01-01
 - added SchemaPassword property to IDbConfig
 - Deleted old stuff from FluentDbTools.Migration.Abstractions 
 - Removed FluentDbTools.Database.Abstractions assembly

### Version 1.0.2 2018-11-31
 - Updated nuget metadata.

### Version 1.0.1 2018-11-31
 - Minor fix

### Version 1.0.0 2018-11-28
 - Initial release
