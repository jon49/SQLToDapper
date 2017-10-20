--DECLARE @SchemaName varchar(500) = 'client';

SELECT
      t.[schema_id] SchemaID
    , SCHEMA_NAME(t.[schema_id]) SchemaName
    , t.[object_id] RoutineID
    , OBJECT_NAME(t.[object_id]) RoutineName
    --, ROW_NUMBER() OVER (ORDER BY t.[object_id]) RowNumber
FROM sys.procedures t
WHERE t.schema_id = SCHEMA_ID(@SchemaName);
