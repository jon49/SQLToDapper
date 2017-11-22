namespace SQLToDapper.Template

module UdtGenerator =

    open SQLToDapper.IntermediateDescriptor
    open SQLToDapper.Utils

    let getUDTName schemaName objectName =
        sprintf "%s_%s[]" (String.capitalize schemaName) (String.capitalize objectName)

    let private generateStringProperty (prop: SqlProperty) =
        sprintf "
            private %s _%s;
            public %s %s
            {
                get { return _%s }
                set
                {
                    var s = value.Trim();
                    _%s = %i == -1 || s.Length < %i ? s : s.Substring(0, %i);
                }
            }
"           prop.clrType prop.name prop.clrType prop.name prop.name prop.name prop.length prop.length (prop.length - 1s)

    let private generateProperty (prop: SqlProperty) =
        match prop.clrType with
        | "string" -> generateStringProperty prop
        | _ ->
            sprintf "
            public %s %s { get; set; }
"               prop.clrType prop.name


    let private generateSqlDbType (prop: SqlProperty) =
        let generateSqlDbType2 (prop: SqlProperty) =
            sprintf """
            new SqlMetaData("%s", SqlDbType.%s)
"""          prop.name prop.sqlDbType

        let generateSqlDbType3 (prop: SqlProperty) =
            sprintf """
            new SqlMetaData("%s", SqlDbType.%s, %i)
"""          prop.name prop.sqlDbType prop.length

        match prop.sqlDbType with
        | "Bit"| "BigInt"| "DateTime"| "Decimal"| "Float"| "Int"| "Money"| "Numeric"| "SmallDateTime"| "SmallInt"| "SmallMoney"| "TimeStamp"| "TinyInt"| "UniqueIdentifier"| "Xml"->
            generateSqlDbType2 prop
        | "Binary"| "Char"| "Image"| "NChar"| "Ntext"| "NVarChar"| "Text"| "VarBinary"| "VarChar" ->
            generateSqlDbType3 prop
        | _ -> failwith <| sprintf "The type %s was not accounted for." prop.sqlDbType

    let private setSql (prop: SqlProperty) =
        sprintf "
            record.Set%s(%i, x.%s);
"        (String.capitalize prop.clrType) prop.order prop.name

    let generateUDT (udts: UserDefinedType list) =
        udts
        |> List.map (fun x ->
        let udtName = getUDTName x.schema x.name
        let pocoClass =
            sprintf "
        public class %s
        {
%s
        }
"               udtName (x.properties |> List.map generateProperty |> String.concat "")

        let udtClass =
            sprintf """
        public class %s_UDT : IUDT
        {
            private readonly %s[] %s;
            public %s_UDT(%s[] %s)
            {
                this.%s = %s;
            }

            public IEnumerable<SqlDataRecord> SqlMeta()
            {
                var TVP = new SqlMetaData[]
                {
%s
                };

                return this.%s.Select(
                    x =>
                    {
                        var record = new SqlDataRecord(TVP);
                        %s

                        return record;
                    });
            }

            public string TypeName { get { return "[%s].[%s]"; } }

        }
"""          udtName udtName udtName udtName udtName udtName udtName udtName
             (x.properties |> List.map generateSqlDbType |> String.concat ",")
             udtName (x.properties |> List.map setSql |> String.concat "") x.schema x.name
        pocoClass
        )

