
-- required synonym:

create or replace synonym {SchemaName}.cpfelles for {CommonSchemaName}.cpfelles;


-- Title Create sequence {SchemaName}.{SchemaPrefix}CHANGElogTABLE_SEQ 
-- ErrorFilter = 955
create sequence {SchemaName}.{SchemaPrefix}CHANGElogTABLE_SEQ 
minvalue 1 
  maxvalue 999999999999999999999999999 
  start with 1000000 
  increment by 1 
  cache 20;


-- Title Create table {SchemaName}.{SchemaPrefix}CHANGElogTABLE
-- ErrorFilter = 955
create table {SchemaName}.{SchemaPrefix}CHANGELOGTABLE
(
  transactionid            NUMBER(10) not null,
  globaltableid            CHAR(3) not null,
  dmlperformed             NUMBER(3) not null,
  storagetype              NUMBER(3) default 2,
  changedrowversion        NUMBER(10) not null,
  changedrowdipsid         NUMBER(10) not null,
  loggedby                 VARCHAR2(20),
  loggtime                 DATE,
  previousversioncreatedby VARCHAR2(20),
  previousversiontime      DATE
)
tablespace DIPS_LOGG
 ;

-- Add comments to the table 
comment on table {SchemaName}.{SchemaPrefix}CHANGELOGTABLE
  is 'Changelog for changes to data within the DIPS Database.';
-- Add comments to the columns 
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.transactionid
  is 'Primary key for table {SchemaName}.{SchemaPrefix}CHANGElogTable';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.globaltableid
  is 'Identifies which table was changed during this transaction';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.dmlperformed
  is '1: new row, 2: modified row, 3: deleted row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.storagetype
  is '1= temporary, deletable log, 2= logg which is not to be deleted';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.changedrowversion
  is 'Identifies the version of the row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.changedrowdipsid
  is 'Identifies uniquely which row was changed.  Links to the field DIPSID.';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.loggedby
  is 'Who/what script this transaction was performed and logged by';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.loggtime
  is 'When did the change occur';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.previousversioncreatedby
  is 'Who/what script created the previous version of this row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGTABLE.previousversiontime
  is 'When was the previous version stored';

-- Create/Recreate indexes 
-- ErrorFilter = 955, 1408 
create index {SchemaName}.DWCHLOGTAB_IDX on {SchemaName}.{SchemaPrefix}CHANGELOGTABLE (CHANGEDROWDIPSID, GLOBALTABLEID, CHANGEDROWVERSION, DMLPERFORMED)
  tablespace DIPS_INDX
 ;
-- Create/Recreate primary, unique and foreign key constraints 
-- ErrorFilter = 2360, 2260
alter table {SchemaName}.{SchemaPrefix}CHANGELOGTABLE
  add constraint {SchemaPrefix}CHANGElogTable_PK primary key (TRANSACTIONID)
  using index 
  tablespace DIPS_INDX
 ;

-- Title Create table {SchemaName}.{SchemaPrefix}CHANGElogXML
-- ErrorFilter = 955

create table {SchemaName}.{SchemaPrefix}CHANGElogXML
(
  globalcolumnid           CHAR(7) not null,
  changedrowversion        NUMBER(10) not null,
  changedrowdipsid         NUMBER(10) not null,
  dmlperformed             NUMBER(3) not null,
  storagetype              NUMBER(3) default 2 not null,
  oldvalue                 SYS.XMLTYPE,
  loggedby                 VARCHAR2(20),
  loggtime                 DATE,
  previousversioncreatedby VARCHAR2(20),
  previousversiontime      DATE
)
tablespace DIPS_LOGG
;
-- Add comments to the table 
comment on table {SchemaName}.{SchemaPrefix}CHANGElogXML
  is 'Changelog for changes to data stored in XMLTYPE fields, within the DIPS Database.';
-- Add comments to the columns 
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.globalcolumnid
  is 'Identifies which column this row is related to';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.changedrowversion
  is 'Identifies the version of the row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.changedrowdipsid
  is 'Identifies uniquely which row was changed.  Links to the field DIPSID.';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.dmlperformed
  is '1: new row, 2: modified row, 3: deleted row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.storagetype
  is '1= temporary, deletable log, 2= logg which is not to be deleted';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.oldvalue
  is 'The previous value of the column identified by GlobalColumnId.';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.loggedby
  is 'Who/what script this transaction was performed and logged by';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.loggtime
  is 'When did the change occur';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.previousversioncreatedby
  is 'Who/what script created the previous version of this row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogXML.previousversiontime
  is 'When was the previous version stored';

-- Create/Recreate indexes 
-- ErrorFilter = 955, 1408 
create index {SchemaName}.{SchemaPrefix}CHANGElogXML_IDX on {SchemaName}.{SchemaPrefix}CHANGElogXML (CHANGEDROWDIPSID, GLOBALCOLUMNID, CHANGEDROWVERSION)
  tablespace DIPS_INDX
;
-- ErrorFilter = 955, 1408 
create index {SchemaName}.COCHLOGXML_COLID_I on {SchemaName}.{SchemaPrefix}CHANGElogXML (GLOBALCOLUMNID)
  tablespace DIPS_INDX
;

-- Title Create table {SchemaName}.{SchemaPrefix}CHANGElogBLOB
-- ErrorFilter = 955

create table {SchemaName}.{SchemaPrefix}CHANGElogBLOB
(
  globalcolumnid           CHAR(7) not null,
  changedrowversion        NUMBER(10) not null,
  changedrowdipsid         NUMBER(10) not null,
  dmlperformed             NUMBER(3) not null,
  storagetype              NUMBER(3) default 2 not null,
  oldvalue                 BLOB,
  loggedby                 VARCHAR2(20),
  loggtime                 DATE,
  previousversioncreatedby VARCHAR2(20),
  previousversiontime      DATE
)
tablespace DIPS_LOGG
;
-- Add comments to the table 
comment on table {SchemaName}.{SchemaPrefix}CHANGElogBLOB
  is 'Changelog for changes to data stored in BLOB fields, within the DIPS Database.';
-- Add comments to the columns 
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.globalcolumnid
  is 'Identifies which column this row is related to';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.changedrowversion
  is 'Identifies the version of the row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.changedrowdipsid
  is 'Identifies uniquely which row was changed.  Links to the field DIPSID.';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.dmlperformed
  is '1: new row, 2: modified row, 3: deleted row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.storagetype
  is '1= temporary, deletable log, 2= logg which is not to be deleted';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.oldvalue
  is 'The previous value of the column identified by GlobalColumnId.';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.loggedby
  is 'Who/what script this transaction was performed and logged by';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.loggtime
  is 'When did the change occur';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.previousversioncreatedby
  is 'Who/what script created the previous version of this row';
comment on column {SchemaName}.{SchemaPrefix}CHANGElogBLOB.previousversiontime
  is 'When was the previous version stored';


-- Create/Recreate indexes 
-- ErrorFilter = 955, 1408 
create index {SchemaName}.{SchemaPrefix}CHANGElogBLOB_IDX on {SchemaName}.{SchemaPrefix}CHANGElogBLOB (CHANGEDROWDIPSID, GLOBALCOLUMNID, CHANGEDROWVERSION)
  tablespace DIPS_INDX
;
-- ErrorFilter = 955, 1408 
create index {SchemaName}.COCHLOGBLO_COLID_I on {SchemaName}.{SchemaPrefix}CHANGElogBLOB (GLOBALCOLUMNID)
  tablespace DIPS_INDX
;

-- Title Create table {SchemaName}.{SchemaPrefix}CHANGElogCLOB
-- ErrorFilter = 955

create table {SchemaName}.{SchemaPrefix}CHANGELOGCLOB
(
  globalcolumnid           CHAR(7) not null,
  changedrowversion        NUMBER(10) not null,
  changedrowdipsid         NUMBER(10) not null,
  dmlperformed             NUMBER(3) not null,
  storagetype              NUMBER(3) default 2 not null,
  oldvalue                 CLOB,
  loggedby                 VARCHAR2(20),
  loggtime                 DATE,
  previousversioncreatedby VARCHAR2(20),
  previousversiontime      DATE
)
tablespace DIPS_LOGG
;
-- Add comments to the table 
comment on table {SchemaName}.{SchemaPrefix}CHANGELOGCLOB
  is 'Changelog for changes to data stored in CLOB fields, within the DIPS Database.';
-- Add comments to the columns 
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.globalcolumnid
  is 'Identifies which column this row is related to';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.changedrowversion
  is 'Identifies the version of the row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.changedrowdipsid
  is 'Identifies uniquely which row was changed.  Links to the field DIPSID.';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.dmlperformed
  is '1: new row, 2: modified row, 3: deleted row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.storagetype
  is '1= temporary, deletable log, 2= logg which is not to be deleted';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.oldvalue
  is 'The previous value of the column identified by GlobalColumnId.';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.loggedby
  is 'Who/what script this transaction was performed and logged by';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.loggtime
  is 'When did the change occur';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.previousversioncreatedby
  is 'Who/what script created the previous version of this row';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCLOB.previousversiontime
  is 'When was the previous version stored';
-- Create/Recreate indexes 
-- ErrorFilter = 955, 1408 
create index {SchemaName}.COCHLOGCLO_COLID_I on {SchemaName}.{SchemaPrefix}CHANGELOGCLOB (GLOBALCOLUMNID)
  tablespace DIPS_INDX
;
-- ErrorFilter = 955, 1408 
create index {SchemaName}.{SchemaPrefix}CHANGELOGCLOB_IDX on {SchemaName}.{SchemaPrefix}CHANGELOGCLOB (CHANGEDROWDIPSID, GLOBALCOLUMNID, CHANGEDROWVERSION)
  tablespace DIPS_INDX
;



-- Title Create table {SchemaName}.{SchemaPrefix}CHANGElogCOLUMN
-- ErrorFilter = 955
create table {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN
(
  transactionid  NUMBER(10) not null,
  globalcolumnid CHAR(7) not null,
  storagetype    NUMBER(3) default 2 not null,
  oldvalue       VARCHAR2(4000)
)
tablespace DIPS_LOGG
 ;
-- Add comments to the table 
comment on table {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN
  is 'Changelog for changes to data within the DIPS database.  The previous value for one particular field is stored in each row of this table.';
-- Add comments to the columns 
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN.transactionid
  is 'TransactionId links to the corresponding row in the dwChangelogTable table.';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN.globalcolumnid
  is 'Identifies which column this row is related to';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN.storagetype
  is '1= temporary, deletable log, 2= logg which is not to be deleted';
comment on column {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN.oldvalue
  is 'The previous value of the column identified by GlobalColumnId.';
-- Create/Recreate indexes 
-- ErrorFilter = 955, 1408 
create index {SchemaName}.COCHLOGCOL_COCHLOGTAB_FK_I on {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN (TRANSACTIONID, GLOBALCOLUMNID)
  tablespace DIPS_INDX
 ;
-- ErrorFilter = 955, 1408 
create index {SchemaName}.COCHLOGCOL_DWTABELLFE_FK_I on {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN (GLOBALCOLUMNID)
  tablespace DIPS_INDX
 ;
-- Create/Recreate primary, unique and foreign key constraints 
-- ErrorFilter = 2275    
alter table {SchemaName}.{SchemaPrefix}CHANGELOGCOLUMN
  add constraint COCHLOGCOL_COCHLOGTAB_FK foreign key (TRANSACTIONID)
  references {SchemaName}.{SchemaPrefix}CHANGELOGTABLE (TRANSACTIONID);

CREATE OR REPLACE Package {SchemaName}.mcChangeLog Is

  -- $Header: /dw2-Kos1/project/DB/Pkg/ModuleCommon/mcChangeLog.pkg 7     30.01.06 11:39 Hbh $
  -- *************************************************************************************
  -- ************ PACKAGE mcChangeLog - deklarasjoner: *************************************
  -- {CommonSchemaName} skal ha tilgang til denne pakken i de ulike modulskjemaene.
  -- Pakken må være implementert i ethvert modulskjema, inklusive CORE (den skal også være i CORE
  -- for å slippe å spesialbehandle dette skjemaet i kode som kaller denne pakken).
  -- Før utsending må prefix på tabellnavn endres ihht skjemaets prefix, og pakkenavnet prefixes
  -- med skjemanavn.
  -- *************************************************************************************

  -- *************************************************************************************
  --@@package_Version
  --<GROUP $mcChangeLog>
  -- Returnerer pakke-spesifikasjonens versjonsnummer (innenfor en hovedversjon av DIPS).
  -- Author: HBH
  --Syntax:
  package_Version Constant Varchar2(5) := rtrim(substr('$Revision: 7 $', 12),
                                                '$');
  -- $Nokeywords $

  -- ******************************************************************************** --
  -- prosedyrer og funksjoner:
  -- ******************************************************************************** --

  -- *************************************************************************************
  --@@Dwf_Version
  --<GROUP $mcChangeLog>
  -- Returnerer pakke-kroppens versjonsnummer (innenfor en hovedversjon av DIPS).
  -- Author: HBH
  --Syntax:
  Function Dwf_Version(forBody In Char Default 'B') Return Varchar2;
  Pragma Restrict_References(Dwf_Version, Wnds, Wnps);

  -- ******************************************************************************** --
  --@@InitLogChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>
  -- For bruk i: Triggere
  -- Author: HBH
  -- Syntax:
  Function InitLogChange(p_globalTableId            In Varchar2,
                         p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                         p_ChangedRowVersion        In Integer,
                         p_ChangedRowDipsId         In Integer,
                         p_PreviousVersionCreatedBy In Varchar2,
                         p_PreviousVersionTime      In Date,
                         p_LogLevel                 Out Integer)
    Return Integer;

  -- ******************************************************************************** --
  --@@doLogChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>
  -- Lagrer data i DIPS endringslogg.
  -- For bruk i: Triggere.  Logger til endringsloggen i forbindelse med update og delete.
  -- Author: HBH
  -- See also: InitLogChange, EndLogChange

  -- Syntax:
  Procedure doLogChange(p_GlobalColumnId In Varchar2,
                        p_transactionId  In Number,
                        p_DMLPerformed   In Number, -- delete: 3, update: 2 (Insert: 1).
                        p_OldValue       In Varchar2,
                        p_NewValue       In Varchar2,
                        p_LogLevel       In Number,
                        p_RowWasLogged   Out Boolean);

  -- ******************************************************************************** --
  --@@LogBlobChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>
  -- For bruk i: Triggere
  -- Author: HBH
  -- Syntax:
  procedure LogBlobChange(p_GlobalColumnId           In Varchar2,
                          p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                          p_ChangedRowVersion        In Integer,
                          p_ChangedRowDipsId         In Integer,
                          p_PreviousVersionCreatedBy In Varchar2,
                          p_PreviousVersionTime      In Date,
                          p_OldValue                 In BLOB,
                          p_NewValue                 In BLOB);

  -- ******************************************************************************** --
  --@@LogClobChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>
  -- For bruk i: Triggere
  -- Author: HBH
  -- Syntax:
  procedure LogClobChange(p_GlobalColumnId           In Varchar2,
                          p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                          p_ChangedRowVersion        In Integer,
                          p_ChangedRowDipsId         In Integer,
                          p_PreviousVersionCreatedBy In Varchar2,
                          p_PreviousVersionTime      In Date,
                          p_OldValue                 In CLOB,
                          p_NewValue                 In CLOB);

  -- ******************************************************************************** --
  --@@LogXMLChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>
  -- For bruk i: Triggere
  -- Author: HBH
  -- Syntax:
  procedure LogXMLChange(p_GlobalColumnId           In Varchar2,
                         p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                         p_ChangedRowVersion        In Integer,
                         p_ChangedRowDipsId         In Integer,
                         p_PreviousVersionCreatedBy In Varchar2,
                         p_PreviousVersionTime      In Date,
                         p_OldValue                 In XMLType,
                         p_NewValue                 In XMLType);

  -- ******************************************************************************** --
  --@@EndLogChange
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>

  -- For bruk i: Triggere
  -- Author: HBH

  -- Syntax:

  Procedure EndLogChange(p_ActuallyLogged In Boolean,
                         p_transactionid  In Integer);

  -- *************************************************************************************
  --@@ClearTemporaryLog
  --<GROUP $mcChangeLog>
  -- <FLAG For intern bruk>

  -- For bruk i: kalles fra ciJobb
  -- Author: HBH

  -- Syntax:

  Procedure ClearTemporaryLog(antall_timer Binary_Integer,
                              aTest        Boolean Default False);

-- ******************************************************************************** --

End mcChangeLog;


CREATE OR REPLACE Package Body {SchemaName}.mcChangeLog Is
  -- Function and procedure implementations
  -- $Header: /dw2-hv07/project/DB/Pkg/ModuleCommon/mcChangeLogBody.pkg 25    14.03.08 9:50 Mos $Version $
  -- NB : For å få byttet ut &&<xx>&& med riktige verdier, kjør dwDupgen (min. 3.5.0.0) for å lage dupfil.

  c_StorageType_Keep Constant Number(1) := 2;
  c_StorageType_Tmp  Constant Number(1) := 1;

  c_DeletableLog        Constant Number(1) := 0;
  c_NoChangeLogging     Constant Number(1) := 1;
  c_FastLogg            Constant Number(1) := 2;
  c_SysFastLogg         Constant Number(1) := 3;
  c_AlwaysLogFieldValue Constant Number(1) := 4;

  c_Delete Constant Number(1) := 3;
  -- c_Modify Constant Number(1) := 2;
  -- c_Insert Constant Number(1) := 1;

  -- *************************************************************************************
  Function Dwf_Version(forBody In Char) Return Varchar2 Is
  Begin
    If forBody = 'B' Then
      Return rtrim(substr('$Revision: 25 $', 12), '$');
    Else
      Return Package_version;
    End If;
  End;
  -- $Nokeywords$

  -- ******************************************************************************** --
  -- intern hjelpefunksjon i pakken:
  -- ******************************************************************************** --
  Function if_LogLevel(p_globalTableId  In Varchar2,
                       p_DMLPerformed   In Integer,
                       p_GlobalColumnId In Varchar2) Return Integer Is
    v_LogLevelUpd Number(3);
    v_LogLevelDel Number(3);

    -- 18.05.02/HBH: vi må slå opp loggnivået under utføring av triggeren, fordi
    -- vi skal bruke DUP til å etablere triggere, istedenfor å generere disse lokalt på sykehuset.

    Cursor c_FinnLogLevel Is
      Select t.LoggNivaaUpd, t.LoggNivaaDel
        From {CommonSchemaName}.dwTabeller t
       Where t.GlobalTabellId = p_globalTableId;

    Cursor c_FinnLogLevelFelt Is
      Select f.LoggNivaaupd
        From {CommonSchemaName}.dwTabellfelt f
       Where f.GlobalColumnId = p_GlobalColumnId;
  Begin

	-- Sjekker om stort script kjøres, i så fall skal det ikke lagres til endringslogg
    If {CommonSchemaName}.cpSesjon.dwf_ErIStortSkript Then
      Return 1;
    End If;

    -- Slå opp loggnivå:
    If p_GlobalTableId Is Not Null Then
      Open c_FinnLogLevel;
      Fetch c_FinnLogLevel
        Into v_LogLevelUpd, v_LogLevelDel;
      Close c_FinnLogLevel;

      If p_DMLPerformed = 2 Then
        -- update
        Return v_LogLevelUpd;
      Else
        Return v_LogLevelDel;
      End If;
    Elsif p_GlobalColumnId Is Not Null Then
      Open c_FinnLogLevelFelt;
      Fetch c_FinnLogLevelFelt
        Into v_LogLevelUpd;
      Close c_FinnLogLevelFelt;

      Return v_LogLevelUpd;
    End If;
  End;

  -- *************************************************************************************
  Function InitLogChange(p_globalTableId            In Varchar2,
                         p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                         p_ChangedRowVersion        In Integer,
                         p_ChangedRowDipsId         In Integer,
                         p_PreviousVersionCreatedBy In Varchar2,
                         p_PreviousVersionTime      In Date,
                         p_LogLevel                 Out Integer)
    Return Integer Is
    v_transid     Integer;
    v_StorageType Integer;
  Begin
    p_LogLevel := if_LogLevel(p_globalTableId, p_DMLPerformed, Null);

    If p_LogLevel = c_NoChangeLogging Then
      Return - 1;
    Elsif p_LogLevel = c_DeletableLog Then
      v_StorageType := c_StorageType_Tmp;
    Elsif p_LogLevel = c_FastLogg Then
      v_StorageType := c_StorageType_Keep;
    Elsif p_LogLevel = c_SysFastLogg Then
      v_StorageType := c_StorageType_Keep;
    End If;

    If {CommonSchemaName}.cpSesjon.dwf_ErIStortSkript And p_LogLevel = c_DeletableLog Then
      Return - 1;
    End If;

    Select {SchemaName}.{SchemaPrefix}CHANGElogTABLE_SEQ.Nextval
      Into v_TransId
      From DUAL;

    -- lagre i ChangelogTable:
    Insert Into {SchemaPrefix}CHANGElogTable
      (TransactionId,
       globalTableId,
       DMLPerformed,
       ChangedRowVersion,
       ChangedRowDipsId,
       PreviousVersionCreatedBy,
       PreviousVersionTime,
       StorageType)
    Values
      (v_transid,
       p_globalTableId,
       p_DMLPerformed,
       p_ChangedRowVersion,
       p_ChangedRowDipsId,
       p_PreviousVersionCreatedBy,
       p_PreviousVersionTime,
       v_StorageType);

    Return v_transid;
  End;

  -- ******************************************************************************** --
  Procedure doLogChange(p_GlobalColumnId In Varchar2,
                        p_TransactionId  In Number,
                        p_DMLPerformed   In Number, -- delete: 3, update: 2 (Insert: 1).
                        p_OldValue       In Varchar2,
                        p_NewValue       In Varchar2,
                        p_LogLevel       In Number,
                        p_RowWasLogged   Out Boolean) Is
    v_DoLogColumnValue Boolean;
    v_StorageType      Integer;
    v_LogLevel         binary_Integer;
  Begin
    If p_TransactionId = -1 Then
      Return;
    End If;

    v_LogLevel := if_LogLevel(Null, p_DMLPerformed, p_GlobalColumnId);
    /*
    if p_LogLevel = c_SysFastLogg and v_LogLevel != c_AlwaysLogFieldValue then
       v_LogLevel := p_LogLevel;
    end if;
    */
    if v_LogLevel = c_NoChangeLogging then
      return;
    end if;

    -- Er verdien endret ?
    If p_DMLPerformed = C_DELETE -- data slettet, alle felt logges.
       Or v_LogLevel = c_AlwaysLogFieldValue Then
      -- dette feltet skal logges selv om det ikke er endret siden sist.
      v_DoLogColumnValue := True;
    Else
      v_DoLogColumnValue := {CommonSchemaName}.cpFelles.dwf_endretvarchar(p_OldValue,
                                                       p_NewValue); -- logges hvis endret.

      If p_OldValue Is Null And p_NewValue Is Null Then
        v_DoLogColumnValue := False;
      End If;
    End If;

    If Not v_DoLogColumnValue Then
      --1=ingen logging.
      p_RowWasLogged := False;
    Else
      If (v_LogLevel = c_DeletableLog) Then
        V_StorageType := c_StorageType_Tmp;
      Else
        V_StorageType := c_StorageType_Keep;
      End If;

      Insert Into {SchemaPrefix}CHANGElogColumn
        (TransactionId, GlobalColumnId, OldValue, StorageType)
      Values
        (p_TransactionId, p_GlobalColumnId, p_OldValue, V_StorageType);

      p_RowWasLogged := True;
    End If;
  End;

  -- ******************************************************************************** --
  procedure LogBlobChange(p_GlobalColumnId           In Varchar2,
                          p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                          p_ChangedRowVersion        In Integer,
                          p_ChangedRowDipsId         In Integer,
                          p_PreviousVersionCreatedBy In Varchar2,
                          p_PreviousVersionTime      In Date,
                          p_OldValue                 In BLOB,
                          p_NewValue                 In BLOB) IS
    v_LogLevel         binary_Integer;
    V_StorageType      binary_Integer;
    v_DoLogColumnValue Boolean;
  begin
    v_LogLevel := if_LogLevel(NULL, p_DMLPerformed, p_GlobalColumnId);
    if v_LogLevel = c_NoChangeLogging then
      return;
    end if;

    If (v_LogLevel = c_DeletableLog) Then
      V_StorageType := c_StorageType_Tmp;
    Else
      V_StorageType := c_StorageType_Keep;
    End If;

    If p_DMLPerformed = C_DELETE -- data slettet, alle felt logges.
       Or v_LogLevel = c_AlwaysLogFieldValue Then
      -- dette feltet skal logges selv om det ikke er endret siden sist.
      v_DoLogColumnValue := True;
    Elsif (p_OldValue IS NULL and p_NewValue IS NOT NULL) OR
          (p_NewValue IS NULL and p_OldValue IS NOT NULL) OR
          (dbms_lob.compare(p_OldValue, p_NewValue) <> 0) then
      v_DoLogColumnValue := True;
    else
      v_DoLogColumnValue := false;
    end if;

    if v_DoLogColumnValue then
      Insert into {SchemaPrefix}CHANGElogBlob
        (globalcolumnid,
         changedrowversion,
         changedrowdipsid,
         dmlperformed,
         storagetype,
         oldvalue,
         loggedby,
         loggtime,
         previousversioncreatedby,
         previousversiontime)
      Values
        (p_globalcolumnid,
         p_changedrowversion,
         p_changedrowdipsid,
         p_dmlperformed,
         v_storagetype,
         p_oldvalue,
         {CommonSchemaName}.cpSesjon.dwf_Brukerid,
         sysdate,
         p_previousversioncreatedby,
         p_previousversiontime);
    end if;
  end;

  -- ******************************************************************************** --
  procedure LogClobChange(p_GlobalColumnId           In Varchar2,
                          p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                          p_ChangedRowVersion        In Integer,
                          p_ChangedRowDipsId         In Integer,
                          p_PreviousVersionCreatedBy In Varchar2,
                          p_PreviousVersionTime      In Date,
                          p_OldValue                 In CLOB,
                          p_NewValue                 In CLOB) IS
    v_LogLevel         binary_Integer;
    V_StorageType      binary_Integer;
    v_DoLogColumnValue Boolean;
  begin
    v_LogLevel := if_LogLevel(NULL, p_DMLPerformed, p_GlobalColumnId);
    if v_LogLevel = c_NoChangeLogging then
      return;
    end if;

    If (v_LogLevel = c_DeletableLog) Then
      V_StorageType := c_StorageType_Tmp;
    Else
      V_StorageType := c_StorageType_Keep;
    End If;

    If p_DMLPerformed = C_DELETE -- data slettet, alle felt logges.
       Or v_LogLevel = c_AlwaysLogFieldValue Then
      -- dette feltet skal logges selv om det ikke er endret siden sist.
      v_DoLogColumnValue := True;
    Elsif (p_OldValue IS NULL and p_NewValue IS NOT NULL) OR
          (p_NewValue IS NULL and p_OldValue IS NOT NULL) OR
          (dbms_lob.compare(p_OldValue, p_NewValue) <> 0) then
      v_DoLogColumnValue := True;
    else
      v_DoLogColumnValue := false;
    end if;

    if v_DoLogColumnValue then
      Insert into {SchemaPrefix}CHANGElogClob
        (globalcolumnid,
         changedrowversion,
         changedrowdipsid,
         dmlperformed,
         storagetype,
         oldvalue,
         loggedby,
         loggtime,
         previousversioncreatedby,
         previousversiontime)
      Values
        (p_globalcolumnid,
         p_changedrowversion,
         p_changedrowdipsid,
         p_dmlperformed,
         v_storagetype,
         p_oldvalue,
         {CommonSchemaName}.cpSesjon.dwf_Brukerid,
         sysdate,
         p_previousversioncreatedby,
         p_previousversiontime);
    end if;
  end;

  -- ******************************************************************************** --
  procedure LogXMLChange(p_GlobalColumnId           In Varchar2,
                         p_DMLPerformed             In Integer, -- delete: 3, update: 2 (Insert: 1).
                         p_ChangedRowVersion        In Integer,
                         p_ChangedRowDipsId         In Integer,
                         p_PreviousVersionCreatedBy In Varchar2,
                         p_PreviousVersionTime      In Date,
                         p_OldValue                 In XMLType,
                         p_NewValue                 In XMLType) IS
    v_LogLevel         binary_Integer;
    V_StorageType      binary_Integer;
    v_DoLogColumnValue Boolean;
  begin
    v_LogLevel := if_LogLevel(NULL, p_DMLPerformed, p_GlobalColumnId);
    if v_LogLevel = c_NoChangeLogging then
      return;
    end if;

    If (v_LogLevel = c_DeletableLog) Then
      V_StorageType := c_StorageType_Tmp;
    Else
      V_StorageType := c_StorageType_Keep;
    End If;

    If p_DMLPerformed = C_DELETE -- data slettet, alle felt logges.
       Or v_LogLevel = c_AlwaysLogFieldValue Then
      -- dette feltet skal logges selv om det ikke er endret siden sist.
      v_DoLogColumnValue := True;
      /*
          Elsif (p_OldValue IS NULL and p_NewValue IS NOT NULL) OR
                (p_NewValue IS NULL and p_OldValue IS NOT NULL) OR
                (dbms_lob.compare(p_OldValue, p_NewValue) <> 0) then
            v_DoLogColumnValue := True;

          else
            v_DoLogColumnValue := false;
          end if;
      */
    else
      v_DoLogColumnValue := True;
    end if;

    if v_DoLogColumnValue then
      Insert into {SchemaPrefix}CHANGElogXML
        (globalcolumnid,
         changedrowversion,
         changedrowdipsid,
         dmlperformed,
         storagetype,
         oldvalue,
         loggedby,
         loggtime,
         previousversioncreatedby,
         previousversiontime)
      Values
        (p_globalcolumnid,
         p_changedrowversion,
         p_changedrowdipsid,
         p_dmlperformed,
         v_storagetype,
         p_oldvalue,
         {CommonSchemaName}.cpSesjon.dwf_Brukerid,
         sysdate,
         p_previousversioncreatedby,
         p_previousversiontime);
    end if;

  end;

  -- ******************************************************************************** --
  Procedure EndLogChange(p_ActuallyLogged In Boolean,
                         p_transactionid  In Integer) Is
  Begin
    -- er tatt ut av funksjon.  Vi mister med dette faktisk informasjon om hvem som konkret har gjort
    -- en endring!  21.02.01 HBH (JEB)

    -- if not p_ActuallyLogged then
    --    delete {SchemaPrefix}CHANGElogTable where TransactionId = p_transactionid ;
    -- end if;

    Null;
  End;

  -- *************************************************************************************
  Procedure ClearTemporaryLog(antall_timer Binary_Integer,
                              aTest        Boolean Default False) is

    rows_before_commit Binary_Integer := 1000;
    parameterid        Binary_Integer := 1851;
    v                  Binary_Integer;

    funnet        Boolean;
    v_Antall      Binary_Integer;
    v_FeilId      Integer;
    V_expMess     {CommonSchemaName}.dwFeillogg.exceptionMessage%Type;
    v_SqlCode     Number;
    v_dummy       Boolean;
    v_PrevTransId {SchemaPrefix}CHANGElogTable.TransactionId%Type;

    Cursor c(nbr_months Binary_Integer) Is
      Select t.transactionid
        From {SchemaPrefix}CHANGElogTable t,
             {SchemaPrefix}CHANGElogColumn c
       Where c.StorageType = 1 -- Temporary log
         and t.storagetype = 1 -- Temporary log
         and c.transactionId = t.transactionId
         And t.loggtime <= ADD_MONTHS(Sysdate, -nbr_months)
         And ROWNUM <= rows_before_commit;

    c_rec             c%Rowtype;
    latest_running_ts Date;
  Begin

    {CommonSchemaName}.cpSesjon.dwp_logginn('tom_tempendringslog');
    {CommonSchemaName}.cpSesjon.dwp_stortskript(True, 'Tøm temporær endringslogg');

    -- beregn siste kjøretidspunkt før den må avbryte seg selv
    If aTest And antall_timer Is Not Null Then
      -- hvis test oppgir vi antall minutter ist.for antall_timer.
      latest_running_ts := Sysdate + (antall_timer / 24) / 60;
    Elsif antall_timer Is Not Null Then
      latest_running_ts := Sysdate + antall_timer / 24;
    Else
      latest_running_ts := Null;
    End If;

    -- oppsettparameter for antall måneder lagring før sletting av endringslogger
    v := {CommonSchemaName}.cpSesjon.dwf_oppsettverdi(p_parameter       => parameterid,
                                   p_brukerrolleid   => -1,
                                   p_sykehusid       => -1,
                                   p_avdid           => -1,
                                   p_seksjonsid      => -1,
                                   p_postid          => -1,
                                   p_lokid           => -1,
                                   p_brukerspesifikk => v_dummy);
    If v Is Null Then
      raise_application_error(-20503,
                              'Parametervalue for parameterid=1851 in dwOppsettverdier was NULL!');
    End If;

    Loop
      funnet := false;
      v_PrevTransId := -99; -- Bruker denne istedenfor distinct i spørringen, fordi distinct gir dårlig ytelse.
      FOR c_rec in c(v) Loop
        funnet := True;
        if v_PrevTransId != c_rec.transactionid then
          Delete From {SchemaPrefix}CHANGElogColumn
           Where transactionid = c_rec.transactionid
             And Storagetype = 1; -- Temporær logg ;

          Select Count(transactionid)
            Into v_Antall
            From {SchemaPrefix}CHANGElogColumn
           Where transactionid = c_rec.transactionid;

          If v_Antall = 0 Then
            Delete From {SchemaPrefix}CHANGElogTable
             Where transactionid = c_rec.transactionid;
          End If;
        end if;
        v_PrevTransId := c_Rec.TransactionId;

      End Loop;

      Commit;

      if not funnet then
        exit;
      end if;

      If latest_running_ts Is Not Null Then
        If Sysdate >= latest_running_ts Then
          Exit; -- avbryt jobb
        End If;
      End If;
    End Loop;

    -- Viktig: vi skal IKKE slette fra dwBlobEndringsLogg!  Dette er tatt ut 24.5.04/ HBH.
    -- (Helt essensielt for AHUS).

    {CommonSchemaName}.cpSesjon.dwp_stortskript(False, 'Tøm temporær endringslogg');
    {CommonSchemaName}.cpSesjon.dwp_logginn('');
    Commit;

  Exception
    When Others Then
      Rollback;

      V_expMess := substr(Sqlerrm, 1, 4000);
      v_SqlCode := Sqlcode;
      v_FeilId  := {CommonSchemaName}.cpSesjon.dwf_SekvensVerdi('dwFeillogg', 'NEXT');

      Insert Into {CommonSchemaName}.dwFeilLogg
        (FeilId,
         Tid,
         FeilNr,
         FeilType,
         FeilTekst,
         exceptionMessage,
         dipsid)
      Values
        (v_FeilId,
         Sysdate,
         v_SQLCODE,
         'mcChangelog',
         'Exception i ClearTemporaryLog ',
         V_expMess,
         v_FeilId);

      If {CommonSchemaName}.cpSesjon.dwf_ErIStortSkript Then
        {CommonSchemaName}.cpSesjon.dwp_stortskript(False,
                                 'Tøm temporær endringslogg - feilet.');

      End If;
      Commit;

      Raise;
  end;

-- *************************************************************************************
End mcChangeLog;
