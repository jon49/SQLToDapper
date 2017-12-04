namespace SQLToDapper.Template

module ReturnCode =

    open SQLToDapper.IntermediateDescriptor

    let private property (p : Property) =
        sprintf "
            public %s %s { get; set; }
"        p.clrType p.name

    let generate (returnType : ReturnType) =
        returnType
        |> Map.toList
        |> List.map (fun (methodReturn, (name, _)) ->
            match methodReturn with
            | MethodReturnSimple _ -> ""
            | MethodReturnTable xs ->
                xs
                |> List.map property
                |> String.concat ""
                |> sprintf "
         public class %s
         {
%s
         }
"                   name
        )
        |> String.concat ""

