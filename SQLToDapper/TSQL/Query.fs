namespace SQLToDapper.SQLServer

module Query =

    open FSharp.Data
    open FSharpx.Control
    open System.Data.SqlClient
    open SQLToDapper.SQL
    open SQLToDapper.Utils

    let getSQLDbType (x : string option) length precision =
        let x = Option.orElse "UnknownColumnType" x
        match x.ToLower (), length, precision with
        | "bigint", _, precision -> BigInt
        | "binary", len, precision -> Binary len
        | "bit", _, precision -> Bit
        | "char", len, precision -> Char len
        | "date", _, precision -> Date
        | "datetime", _, precision -> DateTime
        | "datetime2", _, precision -> DateTime2 precision
        | "datetimeoffset", _, precision -> DateTimeOffset precision
        | "decimal", _, _ | "numeric", _, _ -> Decimal
        | "float", length, _ ->  Float length
        | "image", _, _ -> Image
        | "int", _, _ -> SQLDbType.Int
        | "money", _, _ -> Money
        | "nchar", length, _ -> NChar length
        | "ntext", _, _ -> NText
        | "nvarchar", length, _ -> NVarChar length
        | "real", _, _ -> Real
        | "smalldatetime", _, _ -> SmallDateTime
        | "smallint", _, _ -> SmallInt
        | "smallmoney", _, _ -> SmallMoney
        | "text", _, _ -> Text
        | "time", _, precision -> Time precision
        | "timestamp", _, _ -> Timestamp
        | "tinyint", _, _ -> TinyInt
        | "uniqueidentifier", _, _ -> UniqueIdentifier
        | "varbinary", length, _ -> VarBinary length
        | "varchar", length, _ -> VarChar length
        | "sql_variant", _, _ -> Variant
        | "xml", _, _ -> Xml
//        | "geography", _, _ | "geometry", _, _ | "hierarchyid", _, _ | "sysname", _, _ -> failwith "geography, geometry, hierarchyid, and sysname are unsupported data types by SqlDbType - see https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings for supported parameters."
        | x, _, _ -> failwithf "'%s' is an unsupported type by SqlDbType see https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings for supported types" x

    [<Literal>]
    let ConnectionString =
        "Data Source=.;Initial Catalog=SQLToDapper;Integrated Security=True;MultipleActiveResultSets=true;"
    
    type DB = SqlProgrammabilityProvider<ConnectionString>

    type RoutineParameters = SqlFile<"RoutineParameters.sql", "TSQL">
    type RoutinesBySchema = SqlFile<"RoutinesBySchema.sql", "TSQL">
    type RoutineReturnTypes = SqlFile<"RoutineReturns.sql", "TSQL">
    type UDTTableColumns = SqlFile<"UDTTableColumns.sql", "TSQL">

    let private getRoutineParameters (conn : SqlConnection) routineID =
        use db = new SqlCommandProvider<RoutineParameters.Text, ConnectionString>(conn)
        db.AsyncExecute(RoutineID = routineID)

    let private getUDTTableColumns (conn : SqlConnection) udtID =
        use db = new SqlCommandProvider<UDTTableColumns.Text, ConnectionString>(conn)
        db.AsyncExecute(UDTID = udtID)

    let private getRoutineReturnType (conn : SqlConnection) routineID =
        use db = new SqlCommandProvider<RoutineReturnTypes.Text, ConnectionString>(conn)
        db.AsyncExecute(RoutineID = routineID)

    let private getRoutines (conn : SqlConnection) schemaName =
        use db = new SqlCommandProvider<RoutinesBySchema.Text, ConnectionString>(conn)
        db.AsyncExecute(SchemaName = schemaName)

    let generateDapperBySchemas (schemas : string[]) (conn : SqlConnection) =
        async {
            let! routines' =
                schemas
                |> Array.map (getRoutines conn)
                |> Async.Parallel

            let routines =
                routines'
                |> (Seq.concat >> Seq.toArray)

            let routineIDs =
                routines
                |> Array.map (fun x -> x.RoutineID)

            let! (routineReturns', parameters') =
                let parameters = routineIDs |> Seq.map (getRoutineParameters conn) |> Async.Parallel
                let returnsAsync  = routineIDs |> Seq.map (getRoutineReturnType conn) |> Async.Parallel
                Async.Parallel (returnsAsync, parameters)
            let routineReturns = routineReturns' |> Seq.map Seq.toArray |> Seq.toArray
            let routineParameters = parameters' |> Seq.map Seq.toArray |> Seq.toArray

            let! udts' =
                routineParameters
                |> Seq.concat
                |> Seq.choose (fun x -> x.UDTID)
                |> set
                |> Seq.map (getUDTTableColumns conn)
                |> Async.Parallel
            let udts = udts' |> Seq.toArray

            let udtTypes =
                udts
                |> Array.map (fun xs ->
                    let xs = xs |> Seq.toArray
                    let head = xs.[0]
                    let fullName = FullObjectName (SchemaName (Option.orElse "Unknown" head.UDTSchemaName), ObjectName head.UDTName)
                    let columns =
                        xs
                        |> Seq.map (fun x ->
                            { IsNullable = Option.orElse true x.IsNullable
                              Name = Option.orElse "UnknownColumnName" x.ColumnName
                              Order = x.ColumnOrder
                              Type = getSQLDbType (Some x.ColumnType) (asLength x.MaxLength) (asPrecision x.Precision) } )
                    { UDTColumns = columns
                      Name = fullName } )

            let routineList =
                routines
                |> Array.mapi (fun idx x -> 
                    let name =
                        FullObjectName (
                            SchemaName <| Option.orElse "" x.SchemaName,
                            ObjectName <| Option.orElse "" x.RoutineName )

                    let parameters =
                        routineParameters.[idx]
                        |> Seq.map (fun x ->
                            let paramName = Option.orElse "UnknowParameterName" x.ParameterName
                            match x.UDTName with
                            | Some udt -> UDT (paramName, FullObjectName (SchemaName <| Option.orElse "SchemaWithNoName?" x.UDTSchemaName, ObjectName udt ))
                            | None ->
                                SimpleParameter
                                    { Name = paramName
                                      Order = x.ParameterOrder
                                      Type = getSQLDbType x.SysColumnType (asLength x.MaxLength) (asPrecision x.Precision) } )
                        |> Seq.toArray

                    let returnTypes =
                        let table = routineReturns.[idx]
                        match (Seq.isEmpty table) with
                        | true -> ReturnType.Int
                        | false ->
                            table
                            |> Seq.map (fun x ->
                                    { IsNullable = x.IsNullable
                                      Name = Option.orElse "UnknownColumnName" x.ColumnName
                                      Order = x.ColumnOrder
                                      Type = getSQLDbType (Some x.ColumnType) (asLength x.MaxLength) (asPrecision x.Precision) } )
                            |> ReturnType.Columns
                        
                    { Name = RoutineName (string x.RoutineID, name)
                      Parameters = parameters
                      ReturnType = returnTypes } )

            return routineList, udtTypes
        }
