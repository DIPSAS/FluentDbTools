-- Title = Create schemaprefix {SchemaName}.{SchemaPrefixId}
-- SCHEMAPREFIXGID '{SchemaPrefixUniqueId}' is extracted from UTV18.WORLD
begin
  update {CommonSchemaName}.coSchemaPrefix
  set    schema      = '{SchemaName}',
         VALIDPREFIX = '{SchemaPrefixId}',
         OBJECT_TYPE = 'TABLE',
         PREFIX_TYPE = 'O',
         REMARKS     = null
  where  SCHEMAPREFIXGID = '{SchemaPrefixUniqueId}';

  if sql%rowcount=0 then 
            insert into {CommonSchemaName}.coSchemaPrefix
            (
                SCHEMAPREFIXGID,
                schema,
                VALIDPREFIX,
                OBJECT_TYPE,
                PREFIX_TYPE,
                REMARKS
            )
            values
            (   
                '{SchemaPrefixUniqueId}',
                '{SchemaName}',
                '{SchemaPrefixId}',
                'TABLE',
                'O',
                null
            );

  end if;
end;
/
