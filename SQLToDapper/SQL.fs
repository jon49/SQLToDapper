namespace SQLToDapper

module SQL =

    open System

    type Length =
        | Max
        | Int of int

    type Precision =
        | Zero
        | One
        | Two
        | Three
        | Four
        | Five
        | Six
        | Seven

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
        | Structured
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

    type UDTColumn =
        {
            IsNullable : bool
            MaxLength : int
            Name : string
            Order : int
            Type : SQLDbType
        }

    type SimpleParameter =
        {
            MaxLength : int
            Name : string
            Order : int
            Type : SQLDbType
        }

    type SchemaName = SchemaName of string
    type ObjectName = ObjectName of string
    type FullObjectName = SchemaName * ObjectName

    type UDT =
        {
            UDTColumns : UDTColumn[]
            Name : FullObjectName
        }

    type Parameter =
        | UDT of FullObjectName
        | SimpleParameter of SimpleParameter

    type Column =
        {
            IsNullable : bool
            Name : string
            Order : int
            Type : SQLDbType
        }

    type Routine =
        {
            Name : FullObjectName
            Parameters : Parameter[]
            Return : Column[]
        }
