--DECLARE @UDTID int = 261;

SELECT
      t.user_type_id ID
    , t.[name] UDTName
    , SCHEMA_NAME(t.[schema_id]) UDTSchemaName
    , c.[name] ColumnName
    , st.[name] ColumnType
    --, c.user_type_id ColumnTypeID
    , c.is_nullable IsNullable
    , c.max_length [MaxLength]
    , c.column_id ColumnOrder
    , t.[precision] [Precision]
FROM sys.table_types t
JOIN sys.columns c ON c.[object_id] = t.type_table_object_id
JOIN sys.systypes st ON st.xusertype = c.user_type_id
WHERE t.user_type_id = @UDTID
