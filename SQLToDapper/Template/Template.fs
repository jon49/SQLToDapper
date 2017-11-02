namespace SQLToDapper

module Template =

    let usingStatements ``namespace`` = sprintf "
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DapperWrapper;
using static DapperWrapper.%s.UserDefinedTable;" ``namespace``

    type DapperReturn =
        | DapperQuery of MethodReturnType : string
        | DapperExecute
        override this.ToString () =
            match this with
            | DapperQuery methodReturnType -> sprintf "DapperQuery<%s>" methodReturnType
            | DapperExecute -> sprintf "DapperExecute"

    type ParamName = ParamName of string

    type MethodParameters =
        MethodParameters of string[]

    let interfaceContent (dapperReturn : DapperReturn) (SQL.ObjectName methodName) (MethodParameters parameters) =
        sprintf "
        %s %s
            %s;" (dapperReturn.ToString ())
                 methodName
                 (String.concat ", "
                    (parameters
                     |> Array.map (fun (ParamName name, sqlDbType) ->
                        sprintf "%s : %s" name (sqlDbType.ToCLRType ()) )))

    let ``interface`` name interfaceContent =
        sprintf "
    public interface %s
    {
%s
    }
"        name (String.concat "" interfaceContent)
