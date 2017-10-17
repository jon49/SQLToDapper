// https://gist.github.com/vbfox/1e9f42f6dcdd9efd6660
namespace SQLToDapper

module DapperFs =
    open System.Data.SqlClient
    open System.Dynamic
    open System.Collections.Generic
    open Dapper

//    let dapperQuery<'A> (query:string) (connection:SqlConnection) =
//        connection.Query<'A>(query)
    
    let private queryObj<'A> (query:string) (param:obj) (connection:SqlConnection) =
        connection.Query<'A>(query, param)

    let query<'A> (query:string) (param : Map<string,_>) (connection:SqlConnection) =
        let expando = ExpandoObject()
        let expandoDictionary = expando :> IDictionary<string,obj>
        for paramValue in param do
            expandoDictionary.Add(paramValue.Key, paramValue.Value :> obj)
        connection |> queryObj query expando

    let private queryAsyncObj<'A> (query : string) (param : obj) (conn : SqlConnection) =
        conn.QueryAsync<'A> (query, param)
        |> Async.AwaitTask

    let queryAsync<'A> (query : string) (connection : SqlConnection) =
        connection.QueryAsync<'A> (query)
        |> Async.AwaitTask

    let queryAsyncWithArgs<'A> (query : string) (param : Map<string,obj>) (connection : SqlConnection) =
        let expando = ExpandoObject()
        let expandoDictionary = expando :> IDictionary<string,obj>
        for paramValue in param do
            expandoDictionary.Add (paramValue.Key, paramValue.Value)
        queryAsyncObj query expando connection
