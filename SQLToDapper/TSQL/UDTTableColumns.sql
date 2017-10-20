WITH AUDT_IDs AS (
    SELECT DISTINCT
          tt.type_table_object_id ID
        , tt.[name]
    FROM sys.parameters t
    JOIN sys.table_types tt
        ON tt.user_type_id = t.user_type_id
    WHERE t.object_id = @RoutineID
      AND t.system_type_id = 243
)
SELECT
      t.type_table_object_id ID
    , t.[name] UDTName
    , c.[name] ColumnName
    , st.[name] ColumnType
    --, c.user_type_id ColumnTypeID
    , c.is_nullable IsNullable
    , c.max_length [MaxLength]
    , c.column_id ColumnOrder
FROM sys.table_types t
JOIN sys.columns c ON c.[object_id] = t.type_table_object_id
JOIN sys.systypes st ON st.xusertype = c.user_type_id
WHERE t.type_table_object_id IN (SELECT udt.ID FROM AUDT_IDs udt)
