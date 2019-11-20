using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Migration.Abstractions.ExtendedExpressions;

namespace FluentDbTools.Migration.Abstractions
{
    /// <summary>
    /// Specify Change Log information for ChangeLog operation <see cref="ICustomMigrationProcessor.Process(IChangeLogTabledExpression)"/>
    /// </summary>
    public class ChangeLogContext : MigrationMetadata
    {

        /// <summary>
        /// Can be used to specify generic generation of triggers and views<br/>
        /// Default is Null, witch indicated that property is not configured
        /// </summary>
        public TriggersAndViewsGeneration? EnabledTriggersAndViewsGeneration { get; set; }

        /// <summary>
        /// Can be used to specify the Global Id of the changed resource (mostly a table)<br/>
        /// i.e: "abc", "lmn", "xyz" 
        /// </summary>
        public string GlobalId { get; set; }

        /// <summary>
        /// Can be used to specify what kind og key the changed resource have<br/>
        /// i.e "Guid", "Gid" or "Id"
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// Can be used to specify the short name of the changed resource<br/>
        /// i.e "DWDELINSTI" for table "DWDELINSTITUSJON", or "DWDSVERDI" for the "DWDATASETTVERDIER" table
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Can be used to specify witch Anonymous operation the changed resource should have<br/>
        /// i.e "NOOP" (Default), "ALTER", "EXTERNAL" or "TRUNCATE"
        /// </summary>
        public string AnonymousOperation { get; set; }

        /// <summary>
        /// Can be used to specifying a short-name (prefix) for the Schema. <br/>
        /// i.e: Then Schema "Example" can use "EX" as <see cref="SchemaPrefixId"/><br/>
        ///      EX => Tables will be prefixed with EX. <br/>
        ///      Entity Person will result in the EXPerson table in the database.
        /// </summary>
        public string SchemaPrefixId { get; set; }

        /// <summary>
        /// Can be used to specifying a unique-name for the Schema. <br/>
        /// </summary>
        public string SchemaPrefixUniqueId { get; set; }


        /// <summary>
        /// Can be used for customized properties on this <see cref="ChangeLogContext"/> class
        /// </summary>
        public Dictionary<string,object> AdditionalContext { get; }

        /// <summary>
        /// Default constructor<br/>
        /// AnonymousOperation is assigned with "NOOP"
        /// </summary>
        public ChangeLogContext() : base()
        {
            AnonymousOperation = "NOOP";
            AdditionalContext = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constructor<br/>
        /// -------------<br/>
        /// AnonymousOperation is assigned with "NOOP"<br/>
        /// SchemaPrefixId is assigned with value from parameter <paramref name="migrationConfig"/> - property <see cref="IMigrationModel.SchemaPrefixId"/><br/>
        /// SchemaPrefixUniqueId is assigned with value from parameter <paramref name="migrationConfig"/> - property <see cref="IMigrationModel.SchemaPrefixUniqueId"/><br/>
        /// <br/>
        /// i.e:<br/>
        /// When configuration have database:migration:schemaPrefix:Id = "PR" and database:migration:schemaPrefix:UniqueId = "abode" <br/>
        /// SchemaPrefixId will be resolved to "PR"<br/>
        /// SchemaPrefixUniqueId will be resolved to "abode"<br/>
        /// </summary>
        /// <param name="migrationConfig"></param>
        public ChangeLogContext(IDbMigrationConfig migrationConfig)
            :this()
        {
            SchemaPrefixId = migrationConfig?.GetSchemaPrefixId();
            SchemaPrefixUniqueId = migrationConfig?.GetSchemaPrefixUniqueId();
            InitMetadata(migrationConfig);
        }


        /// <summary>
        /// Constructor<br/>
        /// -------------<br/>
        /// SchemaPrefixId is assigned with value from parameter <paramref name="migrationConfig"/> - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixId()"/><br/> 
        /// SchemaPrefixUniqueId is assigned with value from parameter <paramref name="migrationConfig"/> - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixUniqueId()"/><br/> 
        /// ShortName is assigned value from configuration database:migration:schemaPrefix:tables:{tableName}:ShortName or database:schemaPrefix:tables:{tableName}:ShortName<br/>
        /// GlobalId is assigned value from configuration database:migration:schemaPrefix:tables:{tableName}:GlobalId or database:schemaPrefix:tables:{tableName}:GlobalId<br/>
        /// KeyType is assigned value from configuration database:migration:schemaPrefix:tables:{tableName}:KeyType or database:schemaPrefix:tables:{tableName}:KeyType<br/>
        /// AnonymousOperation is assigned to "NOOP" OR value from configuration database:migration:schemaPrefix:tables:{tableName}:AnonymousOperation or database:schemaPrefix:tables:{tableName}:AnonymousOperation<br/>
        /// <br/>
        /// i.e:<br/>
        /// When configuration have database:migration:schemaPrefix:Id = "PR" and database:migration:schemaPrefix:UniqueId = "abode" <br/>
        /// SchemaPrefixId will be resolved to "PR"<br/>
        /// SchemaPrefixUniqueId will be resolved to "abode"<br/>
        /// <br/>
        /// When <paramref name="tableName"/> is "testing", <br/>
        /// ShortName is fetched from database:migration:schemaPrefix:tables:testing:ShortName or database:schemaPrefix:tables:testing:ShortName<br/>
        /// GlobalId is fetched from database:migration:schemaPrefix:tables:testing:GlobalId or database:schemaPrefix:tables:testing:GlobalId<br/>
        /// </summary>
        /// <param name="migrationConfig"></param>
        /// <param name="tableName"></param>
        public ChangeLogContext(IDbMigrationConfig migrationConfig, string tableName)
            :this(migrationConfig)
        {
            ShortName = migrationConfig.GetTableConfigValue("schemaPrefix:tables:{tableName}:shortName", tableName) ?? ShortName;
            GlobalId = migrationConfig.GetTableConfigValue("schemaPrefix:tables:{tableName}:globalId", tableName) ?? GlobalId;
            KeyType = migrationConfig.GetTableConfigValue("schemaPrefix:tables:{tableName}:keyType", tableName) ?? KeyType;
            AnonymousOperation = migrationConfig.GetTableConfigValue("schemaPrefix:tables:{tableName}:anonymousOperation", tableName) ?? AnonymousOperation;

            if (ShortName.IsNotEmpty() && migrationConfig != null)
            {
                ShortName = ShortName.GetPrefixedName(SchemaPrefixId);
            }

            var skipTriggersAndViews = migrationConfig.GetTableConfigValue( "schemaPrefix:triggersAndViewsGeneration:tables:{tableName}", tableName);
            if (skipTriggersAndViews.IsEmpty())
            {
                return;
            }

            if (Enum.TryParse(skipTriggersAndViews,true, out TriggersAndViewsGeneration enumValue))
            {
                EnabledTriggersAndViewsGeneration = enumValue;
            }
        }



        /// <summary>
        /// Constructor<br/>
        /// -------------<br/>
        /// Inherited from constructor <see cref="ChangeLogContext(IDbMigrationConfig)"/><br/>
        /// AnonymousOperation is assigned with "NOOP"<br/>
        /// SchemaPrefixId is assigned with value from <paramref name="model"/>(<see cref="IMigrationModel.GetMigrationConfig()"/>)  - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixId()"/><br/> 
        /// SchemaPrefixUniqueId is assigned with value from <paramref name="model"/>(<see cref="IMigrationModel.GetMigrationConfig()"/>)  - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixUniqueId()"/><br/> 
        /// <br/>
        /// i.e:<br/>
        /// When configuration have database:migration:schemaPrefix:Id = "PR" and database:migration:schemaPrefix:UniqueId = "abode" <br/>
        /// SchemaPrefixId will be resolved to "PR"<br/>
        /// SchemaPrefixUniqueId will be resolved to "abode"<br/>
        /// </summary>
        /// <param name="model"></param>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public ChangeLogContext(IMigrationModel model)
            :this(model.GetMigrationConfig())
        {
            InitMetadata(model);
        }

        /// <summary>
        /// Constructor<br/>
        /// -------------<br/>
        /// Inherited from constructor <see cref="ChangeLogContext(IDbMigrationConfig,string)"/><br/>
        /// SchemaPrefixId is assigned with value from <paramref name="model"/>(<see cref="IMigrationModel.GetMigrationConfig()"/>)  - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixId()"/><br/> 
        /// SchemaPrefixUniqueId is assigned with value from <paramref name="model"/>(<see cref="IMigrationModel.GetMigrationConfig()"/>)  - method <see cref="IDbConfigDatabaseTargets.GetSchemaPrefixUniqueId()"/><br/> 
        /// ShortName is assigned value from configuration database:migration:schemaPrefix:tables:{tableName}:ShortName or database:schemaPrefix:tables:{tableName}:ShortName<br/>
        /// GlobalId is assigned value from configuration database:migration:schemaPrefix:tables:{tableName}:GlobalId or database:schemaPrefix:tables:{tableName}:GlobalId<br/>
        /// <br/>
        /// i.e:<br/>
        /// When configuration have database:migration:schemaPrefix:Id = "PR" and database:migration:schemaPrefix:UniqueId = "abode" <br/>
        /// SchemaPrefixId will be resolved to "PR"<br/>
        /// SchemaPrefixUniqueId will be resolved to "abode"<br/>
        /// <br/>
        /// When <paramref name="tableName"/> is "testing", <br/>
        /// ShortName is fetched from database:migration:schemaPrefix:tables:testing:ShortName or database:schemaPrefix:tables:testing:ShortName<br/>
        /// GlobalId is fetched from database:migration:schemaPrefix:tables:testing:GlobalId or database:schemaPrefix:tables:testing:GlobalId<br/>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        public ChangeLogContext(IMigrationModel model, string tableName)
            :this(model?.GetMigrationConfig(), tableName)
        {
        }
    }
}