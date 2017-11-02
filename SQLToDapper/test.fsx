
open System.Text

let m = Map.empty<string, string list>
let sb = ["Hello, "]
let sb' =
    "world" :: sb

let m' =
    m.Add("1", sb)
    |> fun x -> x.Add ("1", sb')

m'.Item "1"
|> List.rev
|> String.concat ""

