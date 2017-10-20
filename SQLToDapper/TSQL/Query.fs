namespace SQLToDapper.SQLServer

module Query =

    open FSharp.Data
    open FSharpx.Control
    open System.Data.SqlClient
    open SQLToDapper

    [<Literal>]
    let ConnectionString =
        "Data Source=.;Initial Catalog=SQLToDapper;Integrated Security=True;MultipleActiveResultSets=true;"
    
    type DB = SqlProgrammabilityProvider<ConnectionString>

    type RoutinesBySchema = SqlFile<"RoutinesBySchema.sql", "TSQL">
    type RoutineReturnTypes = SqlFile<"RoutineReturns.sql", "TSQL">
    type UDTTableColumns = SqlFile<"UDTTableColumns.sql", "TSQL">

    let private getUDTTableColumns (conn : SqlConnection) routineID =
        use db = new SqlCommandProvider<UDTTableColumns.Text, ConnectionString>(conn)
        db.AsyncExecute(RoutineID = routineID)

    let private getRoutineReturnType (conn : SqlConnection) routineID =
        use db = new SqlCommandProvider<RoutineReturnTypes.Text, ConnectionString>(conn)
        db.AsyncExecute(RoutineID = routineID)

    let private getRoutines (conn : SqlConnection) (schemaName : string) =
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

            let! (routineReturns, udts) =
                let returnsAsync = routineIDs |> Seq.map (getRoutineReturnType conn) |> Async.Parallel
                let udtAsync     = routineIDs |> Seq.map (getUDTTableColumns conn)   |> Async.Parallel
                Async.Parallel (returnsAsync, udtAsync)

            return udts
        }
