// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open System.Data.SqlClient
open SQLToDapper
open SQLToDapper.SQLServer

[<EntryPoint>]
let main argv = 
//    printfn "%A" argv
    use conn = new SqlConnection("Data Source=.;Initial Catalog=SymptomChecker;Integrated Security=True;MultipleActiveResultSets=true;")
    conn.Open ()
    Query.generateDapperBySchemas [| "doh"; "client" |] conn
    |> Async.RunSynchronously
    |> printfn "%A"
    Console.ReadLine () |> ignore
    0 // return an integer exit code
