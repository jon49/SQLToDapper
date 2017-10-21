namespace SQLToDapper

module CLR =

    open TSQL

    type T =
        // Numeric
        | Boolean
        | Decimal
        | Double
        | Int16
        | Int32
        | Int64
        | Single
        // Text/XML
        | String of SQL.Length
        | XElement
        // Date/Time
        | DateTime
        | DateTimeOffset
        | TimeSpan
        // Binary
        | Binary
        // Miscellaneous
        | Guid
        | Object

    // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/linq/sql-clr-type-mapping
    let ofTSQL = function
        | BIT -> Boolean
        | TINYINT -> Int16
        | INT -> Int32
        | BIGINT -> Int64
        | SMALLMONEY | MONEY | DECIMAL | NUMERIC -> Decimal
        | REAL | FLOAT (SQL.Int 24s) -> Single
        | FLOAT _ (*FLOAT or FLOAT 53*) -> Double
        | CHAR x | NCHAR x | VARCHAR x | NVARCHAR x  -> String x
        | TEXT | NTEXT -> String SQL.Max
        | XML -> XElement
        | SMALLDATETIME | DATETIME | DATE -> DateTime
        | DATETIME2 _ -> DateTime
        | DATETIMEOFFSET _ -> DateTimeOffset
        | TIME _ -> TimeSpan
        | BINARY _ | VARBINARY _ -> Binary
        | IMAGE | TIMESTAMP -> Binary
        | UNIQUEIDENTIFIER -> Guid
        | SQLVARIANT -> Object

module SQLDbType =

    open System.Data
    open TSQL

    let ofTSQL = function
        | BIT -> SqlDbType.Bit
        | TINYINT -> SqlDbType.TinyInt
        | INT -> SqlDbType.Int
        | BIGINT -> SqlDbType.BigInt
        | SMALLMONEY -> SqlDbType.SmallMoney
        | MONEY -> SqlDbType.Money
        | DECIMAL -> SqlDbType.Decimal
        | NUMERIC -> SqlDbType.Decimal //????
        | REAL -> SqlDbType.Real
        | FLOAT _ -> SqlDbType.Float
        | CHAR _ -> SqlDbType.Char
        | NCHAR _ -> SqlDbType.NChar
        | VARCHAR _ -> SqlDbType.VarChar
        | NVARCHAR _ -> SqlDbType.NVarChar
        | TEXT -> SqlDbType.Text
        | NTEXT -> SqlDbType.NText
        | XML -> SqlDbType.Xml
        | SMALLDATETIME -> SqlDbType.SmallDateTime
        | DATETIME -> SqlDbType.DateTime
        | DATETIME2 _ -> SqlDbType.DateTime2
        | DATETIMEOFFSET _ -> SqlDbType.DateTimeOffset
        | DATE -> SqlDbType.Date
        | TIME _ -> SqlDbType.Time
        | BINARY _ -> SqlDbType.Binary
        | VARBINARY _ -> SqlDbType.VarBinary
        | IMAGE -> SqlDbType.Image
        | TIMESTAMP -> SqlDbType.Timestamp
        | UNIQUEIDENTIFIER -> SqlDbType.UniqueIdentifier
        | SQLVARIANT -> failwith "SQL_INVARIANT is not supported for routine parameters."

//    let toTSQL = function
//        | Boolean -> BIT
////        | Byte -> TINYINT
//        | Int16 -> SMALLINT
//        | Int32 -> INT
//        | Int64 -> BIGINT
////        | SByte -> SMALLINT
////        | UInt16 -> INT
////        | UInt32 -> BIGINT
////        | UInt64 -> DECIMAL(20)
//        | Decimal -> DECIMAL(29,4)
//        | Single -> REAL
//        | Double -> FLOAT


