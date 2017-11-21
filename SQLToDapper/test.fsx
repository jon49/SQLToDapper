
type T =
    | Int of int
    | SomeSay of string
    | None

let arr = [ Int 1; SomeSay "lala"; None]

let getTypeName a =
    let name = sprintf "%O" a
    match name.IndexOf(' ') with
    | x when x > -1 -> name.Substring(0, x)
    | _ -> name

do
    arr
    |> List.map getTypeName
    |> List.iter (printfn "`%O`")

