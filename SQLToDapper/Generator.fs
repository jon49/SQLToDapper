namespace SQLToDapper

module Generator =

    open System.Text
    open SQL
    open SQLToDapper.Utils

    let private imports = sprintf "
using Dapper;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static %s.UserDefinedTable;
using Microsoft.Extensions.Options; %s
"

    type Setting =
        { UsingStatements : string option
          Namespace : string option }

    let tab number =
        "".PadLeft(number, ' ')

    let private createText (columns : Map<Column list, string>) (routine : Routine) (interfaces : Map<string, StringBuilder>, returnClasses : StringBuilder, classes : Map<string, StringBuilder>) =
        let (RoutineName (id, FullObjectName (SchemaName schemaName, ObjectName objectName))) = routine.Name
        let (interfaceB, interfaces) =
            match interfaces.TryFind(schemaName) with
            | Some x -> x, interfaces
            | None ->
                let sb = new StringBuilder ()
                let newInterfaces = interfaces.Add(schemaName, sb)
                sb, newInterfaces

        let returnType =
            match routine.ReturnType with
            | Int -> "int"
            | Columns columnList ->
                let ienumerableOf = sprintf "IEnumerable<%s>"
                match columns.TryFind(columnList) with
                | Some x -> ienumerableOf x
                | None -> ienumerableOf <| sprintf "Return_%s" id

        interfaceB
            .Append(tab 4).Append("public interface I").Append(schemaName).Append("\n")
            .Append(tab 4).Append("{\n")
            .Append(tab 8).Append("Task<").Append(returnType).Append(">") ////// Start here!!!!!
            |> ignore

        (interfaces, returnClasses, classes)

    let generateBySchema (routines : Routine[]) (udt : UDT) (settings : Setting) =
        let namespaceString = Option.orElse "DapperRepositories" settings.Namespace
        let ``namespace`` = new StringBuilder(imports namespaceString (Option.orElse "" settings.UsingStatements))
        let interfaces = Map.empty<string, StringBuilder>
        let returnClasses = new StringBuilder()
        let classes = Map.empty<string, StringBuilder>
        let udts = new StringBuilder()
        ``namespace``

