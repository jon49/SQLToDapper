namespace SQLToDapper.Utils

module Option =
    
    let orElse ``default`` = function
        | Some x -> x
        | None -> ``default``
