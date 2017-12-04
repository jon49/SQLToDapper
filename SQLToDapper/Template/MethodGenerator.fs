namespace SQLToDapper.Template

module MethodCode =

    open SQLToDapper
    open SQLToDapper.IntermediateDescriptor
    open System

    type private MethodSig =
        {
            returnType : string
            methodSig : string
        }

    let private parameters (p : Parameter) =
        sprintf "%s %s" p.clrType p.name

    let private signature (returnTypes : ReturnTypeByIDs) (methodSignature : MethodSignature) =
        let (returnName, methodReturn) = returnTypes.Item methodSignature.id
        let (returnType, signature) =
            match methodReturn with
            | MethodReturnSimple x ->
                let r = "DapperEexecute"
                let s =
                    sprintf "
        %s %s
            (%s)"    r methodSignature.name (methodSignature.parameters |> List.map parameters |> String.concat ", ")
                r, s
            | MethodReturnTable xs ->
                let r = sprintf "DapperQuery<IEnumerable<%s>>" returnName
                let s =
                    sprintf "
        %s %s
            (%s)"    r methodSignature.name (methodSignature.parameters |> List.map parameters |> String.concat ", ")
                r, s
        { returnType = returnType; methodSig = signature }
    
    let private createInterface schemaName (signatures : string list) =
        sprintf "
    public interface I%s
    {
%s
    }
"        schemaName (signatures |> String.concat ";")

    let createMethod (returnTypes : ReturnTypeByIDs) (m : Map<int, string>) (methodSignature : MethodSignature) =
        

    let generate
        (returnTypes: ReturnTypeByIDs)
        (schemaObjects: ObjectToSchema)
        (methodSignatures: MethodSignature list) =

        let methodSignatures =
            methodSignatures
            |> List.fold (signature returnTypes) (Map.empty<int, string>)

        //let methodSigs =
        //    schemaObjects
        //    |> Map.fold (fun (acc: Map<string, string list>) objectID (schemaName, _) ->
        //        let sign = methodSignatures.Item objectID
        //        match acc.TryFind schemaName with
        //        | Some signatures -> acc.Add (schemaName, sign :: signatures )
        //        | None -> acc.Add (schemaName, [ sign ])
        //        ) Map.empty<string, string list>

        //let methods =


        ""
