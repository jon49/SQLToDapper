namespace SQLToDapper.Utils

module String =

    let capitalize (s : string) =
        let first = s.[0].ToString().ToUpper()
        first + s.[1..]
