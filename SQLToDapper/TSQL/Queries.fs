namespace SQLToDapper

module Queries =

    open System.Data.SqlClient

    [<Literal>]
    let ConnectionString =
        @"Data Source=.;Initial Catalog=sakila;Integrated Security=True"

    type Routine =
        {
            SchemaID : int
            SchemaName : string
            RoutineID : int
            RoutineName : string
        }

    let getRoutines (schemas : string[]) =
        let schemas = sprintf "('%s')" <| System.String.Join ("'),('", schemas)
        let query = sprintf "
             DECLARE @Schemas TABLE ([Name] varchar(256));
             INSERT INTO @Schemas ([Name])
             VALUES %s;

             SELECT
                   t.[schema_id] SchemaID
                 , SCHEMA_NAME(t.[schema_id]) SchemaName
                 , t.[object_id] RoutineID
                 , OBJECT_NAME(t.[object_id]) RoutineName
                 --, ROW_NUMBER() OVER (ORDER BY t.[object_id]) RowNumber
             FROM sys.procedures t
             WHERE t.schema_id IN (
                     SELECT SCHEMA_ID(t.[Name]) FROM @Schemas t ); "
        DapperFs.queryAsync<Routine> <| query schemas

    let generateDapperBySchemas (conn : SqlConnection) (schemas : string[]) =
        async {
            let! routines = getRoutines schemas conn
            return routines
        }
