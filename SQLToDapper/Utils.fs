namespace SQLToDapper.Utils

module String =

    let capitalize (s : string) =
        let first = s.[0].ToString().ToUpper()
        first + s.[1..]

module T =

    let getTypeName a =
        let name = sprintf "%O" a
        match name.IndexOf(' ') with
        | x when x > -1 -> name.Substring(0, x)
        | _ -> name
