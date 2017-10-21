
DECLARE @Schemas TABLE ([Name] varchar(256));
INSERT INTO @Schemas ([Name])
VALUES
      ('client')
    , ('doh');

SELECT
      t.[schema_id] SchemaID
    , SCHEMA_NAME(t.[schema_id]) SchemaName
    , t.[object_id] RoutineID
    , OBJECT_NAME(t.[object_id]) RoutineName
    --, ROW_NUMBER() OVER (ORDER BY t.[object_id]) RowNumber
FROM sys.procedures t
WHERE t.schema_id IN (
        SELECT SCHEMA_ID(t.[Name]) FROM @Schemas t );

DECLARE @FirstResult TABLE (
      is_hidden bit NOT NULL
    , column_ordinal int NOT NULL
    , name sysname NULL
    , is_nullable bit NOT NULL
    , system_type_id int NOT NULL
    , system_type_name nvarchar(256) NULL
    , max_length smallint NOT NULL
    , precision tinyint NOT NULL
    , scale tinyint NOT NULL
    , collation_name sysname NULL
    , user_type_id int NULL
    , user_type_database sysname NULL
    , user_type_schema sysname NULL
    , user_type_name sysname NULL
    , assembly_qualified_type_name nvarchar(4000)
    , xml_collection_id int NULL
    , xml_collection_database sysname NULL
    , xml_collection_schema sysname NULL
    , xml_collection_name sysname NULL
    , is_xml_document bit NOT NULL
    , is_case_sensitive bit NOT NULL
    , is_fixed_length_clr_type bit NOT NULL
    , source_server sysname NULL
    , source_database sysname NULL
    , source_schema sysname NULL
    , source_table sysname NULL
    , source_column sysname NULL
    , is_identity_column bit NULL
    , is_part_of_unique_key bit NULL
    , is_updateable bit NULL
    , is_computed_column bit NULL
    , is_sparse_column_set bit NULL
    , ordinal_in_order_by_list smallint NULL
    , order_by_list_length smallint NULL
    , order_by_is_descending smallint NULL
    , tds_type_id int NOT NULL
    , tds_length int NOT NULL
    , tds_collation_id int NULL
    , tds_collation_sort_id tinyint NULL
    );

DECLARE @ObjectID int =
--1483152329;
1499152386;
--1515152443
--1531152500
--1547152557
--1563152614
DECLARE @FullName nvarchar(max) =
    CONCAT(N'[',OBJECT_SCHEMA_NAME(@ObjectID),N'].[', OBJECT_NAME(@ObjectID),N']');

INSERT INTO @FirstResult
EXEC sp_describe_first_result_set @tsql = @FullName;

SELECT
      t.column_ordinal ColumnOrder
    , t.[name] ColumnName
    , t.is_nullable IsNullable
    , st.[name] ColumnType
    , t.max_length [MaxLength]
FROM @FirstResult t
JOIN sys.systypes st ON st.xusertype = t.system_type_id;

select DISTINCT t.[name]
from sys.systypes t
where t.xtype <> 243


SELECT
      t.[name] ParameterName
    , t.parameter_id ParameterOrder
    , st.[name] SysColumnType
    , t.max_length [MaxLength]
    , t.[precision] [Precision]
    , t.default_value [DefaultValue]
    , SCHEMA_NAME(tt.[schema_id]) SchemaName
    , tt.[name] UDTName
FROM sys.parameters t
LEFT JOIN sys.systypes st ON st.xusertype = t.system_type_id
LEFT JOIN sys.table_types tt ON tt.user_type_id = t.user_type_id
WHERE t.object_id = 1483152329;

DECLARE @IDs TABLE (ID int NOT NULL PRIMARY KEY);

INSERT INTO @IDs (ID)
VALUES
    (1483152329),
    (1499152386),
    (1515152443),
    (1531152500),
    (1547152557),
    (1563152614);

WITH AUDT_IDs AS (
    SELECT DISTINCT
          tt.type_table_object_id ID
        , tt.[name]
    FROM (SELECT t.ID FROM @IDs t) t
    JOIN sys.parameters p
        ON p.object_id = t.ID
        AND p.system_type_id = 243
    JOIN sys.table_types tt
        ON tt.user_type_id = p.user_type_id
)
SELECT
      t.type_table_object_id ID
    , t.[name] UDTName
    , c.[name] ColumnName
    , st.[name] ColumnType
    , c.user_type_id ColumnTypeID
    , c.is_nullable IsNullable
    , c.max_length [MaxLength]
    , c.column_id ColumnOrder
FROM sys.table_types t
JOIN sys.columns c ON c.[object_id] = t.type_table_object_id
JOIN sys.systypes st ON st.xusertype = c.user_type_id
WHERE t.type_table_object_id IN (SELECT udt.ID FROM AUDT_IDs udt)

