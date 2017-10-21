--DECLARE @RoutineID int = 1515152443;

SELECT
      t.[name] ParameterName
    , t.parameter_id ParameterOrder
    , st.[name] SysColumnType
    , t.max_length [MaxLength]
    , t.[precision] [Precision]
    , t.default_value [DefaultValue]
    , SCHEMA_NAME(tt.[schema_id]) UDTSchemaName
    , tt.[name] UDTName
    , tt.user_type_id UDTID
FROM sys.parameters t
LEFT JOIN sys.systypes st ON st.xusertype = t.system_type_id
LEFT JOIN sys.table_types tt ON tt.user_type_id = t.user_type_id
WHERE t.object_id = @RoutineID;
