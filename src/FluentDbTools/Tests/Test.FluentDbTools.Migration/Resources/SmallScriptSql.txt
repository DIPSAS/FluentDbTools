create user {SchemaName} identified by "{SchemaName}"
enable editions account lock;

declare
  ROLE_NAME varchar2(50) := '{SchemaPrefix}_TABLE_ROLE';
begin

  execute immediate 'create role ' || ROLE_NAME;
exception
  when others then
    --"ORA-01921: role name 'x' conflicts with another user or role name"
    if sqlcode = -01921 then 
      null;
    else
      raise;
    end if;
end;


-- hei hei
-- aho
/* 
create role {SchemaPrefix}_TABLE_ROLE;
*/
-- CREATE ROLE '{SchemaPrefix}_TABLE_ROLE';
-- CALL {SchemaName}.create_role('{SchemaPrefix}_TABLE_ROLE');

grant connect, resource to {SchemaName};
grant unlimited tablespace to {SchemaName};
