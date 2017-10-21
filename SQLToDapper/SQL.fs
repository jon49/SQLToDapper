namespace SQLToDapper

module SQL =

    open System

    type Length =
        | Max
        | Int of int16

    let asLength =
        function
        | -1s -> Max
        | x -> Int x

    type Precision =
        | Zero
        | One
        | Two
        | Three
        | Four
        | Five
        | Six
        | Seven

    let asPrecision x =
        match x with
        | 0uy -> Zero
        | 1uy -> One
        | 2uy -> Two
        | 3uy -> Three
        | 4uy -> Four
        | 5uy -> Five
        | 6uy -> Six
        | _ -> Seven

    type SQLDbType =
        | BigInt
        | Binary of Length
        | Bit
        | Char of Length
        | Date
        | DateTime
        | DateTime2 of Precision
        | DateTimeOffset of Precision
        | Decimal
        | Float of Length
        | Image
        | Int
        | Money
        | NChar of Length
        | NText
        | NVarChar of Length
        | Real
        | SmallDateTime
        | SmallInt
        | SmallMoney
        | Text
        | Time of Precision
        | Timestamp
        | TinyInt
//        | Udt of SchemaName : string * Name : string
        | UniqueIdentifier
        | VarBinary of Length
        | VarChar of Length
        | Variant
        | Xml

    type Column =
        {
            IsNullable : bool
            Name : string
            Order : int
            Type : SQLDbType
        }

//    type UDTColumn =
//        {
//            IsNullable : bool
//            Name : string
//            Order : int
//            Type : SQLDbType
//        }

    type SimpleParameter =
        {
            Name : string
            Order : int
            Type : SQLDbType
        }

    type SchemaName = SchemaName of string
    type ObjectName = ObjectName of string
    type FullObjectName = FullObjectName of SchemaName * ObjectName

    type UDT =
        {
            UDTColumns : Column seq
            Name : FullObjectName
        }

    type Parameter =
        | UDT of ParameterName : string * FullObjectName
        | SimpleParameter of SimpleParameter


    type ReturnType =
        | Columns of Column seq
        | Int

    type Routine =
        {
            Name : FullObjectName
            Parameters : Parameter[]
            ReturnType : ReturnType
        }
