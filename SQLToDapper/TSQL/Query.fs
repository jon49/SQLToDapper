namespace SQLToDapper.SQLServer

module Query =

    open FSharp.Data
    open FSharpx.Control
    open System.Data.SqlClient
    open SQLToDapper.SQL
    open SQLToDapper.IntermediateDescriptor
    open SQLToDapper.Utils

    let getSQLDbType (x : string option) length precision =
        let x = Option.defaultValue "UnknownColumnType" x
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

    let unknown = Option.defaultValue "unknown"

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

    let generateDapperBySchemas schemas (conn : SqlConnection) =
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
                    let columns =
                        xs
                        |> Seq.map (fun x ->
                            let sqlDbType = getSQLDbType (Some x.ColumnType) (asLength x.MaxLength) (asPrecision x.Precision)
                            let isNullable = Option.defaultValue true x.IsNullable
                            { name = unknown x.ColumnName
                              clrType = sqlDbType.ToCLRType(isNullable)
                              order = x.ColumnOrder
                              isNullable = isNullable
                              sqlDbType = T.getTypeName sqlDbType
                              precision = sqlDbType.GetPrecision().GetInt()
                              length = sqlDbType.GetLength().GetInt() } )
                        |> Seq.toList
                    { id = head.ID
                      schema = unknown head.UDTSchemaName
                      name = head.UDTName
                      properties = columns } )
                |> Array.toList

            let routinesSignatures =
                routines
                |> Array.mapi (fun idx x -> 
                    let objectName = unknown x.RoutineName
                    let objectID = x.RoutineID

                    let param =
                        routineParameters.[idx]
                        |> Seq.map (fun x ->
                            let paramName = unknown x.ParameterName
                            match x.UDTName with
                            | Some udt ->
                                let schemaName = unknown x.UDTSchemaName
                                let udtName = unknown x.UDTName
                                { id = x.UDTID
                                  name = paramName
                                  clrType = getUdtClrType schemaName udtName
                                  order = x.ParameterOrder
                                  maxLength = x.MaxLength }
                            | None ->
                                let sqlDbType = getSQLDbType (x.SysColumnType) (asLength x.MaxLength) (asPrecision x.Precision)
                                { id = None
                                  name = paramName
                                  clrType =  sqlDbType.ToCLRType(true)
                                  order = x.ParameterOrder
                                  maxLength = x.MaxLength } )
                        |> Seq.toList

                    { id = objectID
                      name = objectName
                      parameters = param } )
                |> Array.toList

            let routinesReturnTypes =
                routines
                |> Array.fold (fun (idx, acc: Map<MethodReturn, int list>) x ->
                    let objectID = x.RoutineID
                    let table = routineReturns.[idx]
                    let r : MethodReturn =
                        match (Seq.isEmpty table) with
                        | true -> MethodReturnSimple "int"
                        | false ->
                            MethodReturnTable (
                                  table
                                  |> Seq.map (fun x ->
                                      let sqlDbType = getSQLDbType (Some x.ColumnType) (asLength x.MaxLength) (asPrecision x.Precision)
                                      { name = unknown x.ColumnName
                                        clrType = sqlDbType.ToCLRType(x.IsNullable)
                                        order = x.ColumnOrder
                                        isNullable = x.IsNullable } )
                                  |> Seq.toList )
                    let acc' =
                        match acc.TryFind r with
                        | Some x ->
                            let ids = acc.Item r
                            acc.Add (r, objectID :: ids)
                        | None -> acc.Add (r, [ objectID ])
                    idx + 1, acc'
                    ) (0, Map.empty<MethodReturn, int list>)
                |> snd
                |> Map.toList
                |> List.map (fun (methodReturn, ids) ->
                    { methodIDs = ids
                      returnType = methodReturn } )

            return { methodSignatures = routinesSignatures
                     userDefinedTypes = udtTypes
                     returnTypes = routinesReturnTypes }
        }
