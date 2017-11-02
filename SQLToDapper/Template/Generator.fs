namespace SQLToDapper

module Generator =

    open System.Text
    open SQL
    open SQLToDapper.Utils

    type Setting =
        { UsingStatements : string option
          Namespace : string option }

    let getParameters (parameters : Parameter[]) =
        let sb = new StringBuilder()
        parameters
        |> Array.map (fun x ->
            match x with
            | UDT (name, FullObjectName (SchemaName schemaName, ObjectName objectName)) ->
                sprintf "%s_%s[] %s" (String.capitalize schemaName) (String.capitalize objectName) name
            | SimpleParameter { Name = name; Order = _; Type = sqlType } ->
                ""
        )
        |> String.concat ", "
        |> sprintf "(%s)"

    type MethodSignatureParts =
        {
            DapperReturn : Template.DapperReturn
            MethodParameters : Template.MethodParameters
        }

    type MethodSignature =
        | MethodSignature of Map<FullObjectName, MethodSignatureParts>
            static member Init () =
                MethodSignature Map.empty

    let private buildMethodSignature (columns : Map<Column[], Template.DapperReturn>, MethodSignature methodsBySchema) (routine : Routine) =
        let (RoutineName (id, fullObjectName)) = routine.Name
        let (columns, returnType) =
            match routine.ReturnType with
            | Int -> columns, Template.DapperExecute
            | Columns columnList ->
                let ienumerableOf = sprintf "IEnumerable<%s>"
                match columns.TryFind(columnList) with
                | Some x -> columns, x
                | None ->
                    let returnType = Template.DapperQuery (ienumerableOf <| sprintf "Return_%s" id)
                    columns.Add(columnList, returnType), returnType

        let parameters =
            routine.Parameters
            |> Array.map (fun x ->
                match x with
                | UDT (paramName, FullObjectName (SchemaName schemaName, ObjectName objectName)) ->
                    sprintf "%s_%s[] %s" (String.capitalize schemaName) (String.capitalize objectName) paramName
                | SimpleParameter { Name = name; Order = _; Type = sqlDbType } ->
                    sprintf "%s %s" (sqlDbType.ToCLRType(true)) name
            )

        columns, MethodSignature <| methodsBySchema.Add(fullObjectName, {DapperReturn = returnType; MethodParameters = Template.MethodParameters parameters})

    let private createText (columns : Map<Column list, string>) (routine : Routine) (interfaces : Map<string, StringBuilder>, returnClasses : StringBuilder, classes : Map<string, StringBuilder>) =
        (interfaces, returnClasses, classes)

    let generateBySchema (routines : Routine[]) (udt : UDT) (settings : Setting) =
        let (returnClasses, methodSignatures) =
            routines
            |> Array.fold buildMethodSignature (Map.empty, MethodSignature.Init())
        ()
        
//        let namespaceString = Option.orElse "DapperRepositories" settings.Namespace
//        let ``namespace`` = new StringBuilder(imports namespaceString (Option.orElse "" settings.UsingStatements))
//        let interfaces = Map.empty<string, StringBuilder>
//        let returnClasses = new StringBuilder()
//        let classes = Map.empty<string, StringBuilder>
//        let udts = new StringBuilder()
//        ``namespace``

