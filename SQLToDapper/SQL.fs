namespace SQLToDapper

module SQL =

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
        | Binary
        | Bit
        | Char
        | Date
        | DateTime
        | DateTime2
        | DateTimeOffset
        | Decimal
        | Float
        | Image
        | Int
        | Money
        | NChar
        | NText
        | NVarChar
        | Real
        | SmallDateTime
        | SmallInt
        | SmallMoney
        | Structured
        | Text
        | Time
        | Timestamp
        | TinyInt
        | Udt
        | UniqueIdentifier
        | VarBinary
        | VarChar
        | Variant
        | Xml
