﻿namespace SQLToDapper

module SQL =

    open System
    open SQLToDapper.Utils

    type Length =
        | Max
        | Int of int16
        | NoLengthValue

        member this.GetInt () =
            match this with
            | Max -> -1s
            | Int x -> x
            | NoLengthValue -> 0s

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
        | NoPrecisionValue

        member this.GetInt () =
            match this with
            | Zero -> 0
            | One -> 1
            | Two -> 2
            | Three -> 3
            | Four -> 4
            | Five -> 5
            | Six -> 6
            | Seven -> 7
            | NoPrecisionValue -> -1

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

        member this.ToCLRType (isNullable : bool) =
            match this, isNullable with
            | BigInt, true -> "Int64?"
            | BigInt, false -> "Int64"
            | Bit, true -> "Boolean?"
            | Bit, false -> "Boolean"
            | Char _, _ | NChar _, _ -> "Char[]"
            | Date, true | DateTime, true | DateTime2 _, true | SmallDateTime, true -> "DateTime?"
            | Date, false | DateTime, false | DateTime2 _, false | SmallDateTime, false -> "DateTime"
            | DateTimeOffset _, true -> "DateTimeOffset?"
            | DateTimeOffset _, false -> "DateTimeOffset"
            | Decimal, true | Money, true | SmallMoney, true -> "Decimal?"
            | Decimal, false | Money, false | SmallMoney, false -> "Decimal"
            | Float _, true -> "Double?"
            | Float _, false -> "Double"
            | Image, _ | Timestamp, _ | Binary _, _ | VarBinary _, _ -> "Byte[]"
            | Int, true -> "Int32?"
            | Int, false -> "Int32"
            | NText, _ | NVarChar _, _ | Text, _ | VarChar _, _ -> "String"
            | Real, true -> "Single?"
            | Real, false -> "Single"
            | SmallInt, true -> "Int16?"
            | SmallInt, false -> "Int16"
            | Time _, true -> "TimeSpan?"
            | Time _, false -> "TimeSpan"
            | TinyInt, true -> "Byte?"
            | TinyInt, false -> "Byte"
            | UniqueIdentifier, true -> "Guid?"
            | UniqueIdentifier, false -> "Guid"
            | Variant, _ -> "Object"
            | Xml, true -> "Xml?"
            | Xml, false -> "Xml"

        member this.GetPrecision () =
            match this with
            | DateTime2 x -> x
            | DateTimeOffset x -> x
            | Time x -> x
            | _ -> NoPrecisionValue

        member this.GetLength () =
            match this with
            | Binary x | Char x | Float x -> x | NChar x -> x | NVarChar x | VarBinary x | VarChar x -> x
            | _ -> NoLengthValue

    let getUdtClrType schemaName objectName =
        sprintf "%s_%s[]" (String.capitalize schemaName) (String.capitalize objectName)

//    type Column =
//        {
//            IsNullable : bool
//            Name : string
//            Order : int
//            Type : SQLDbType
//        }

////    type UDTColumn =
////        {
////            IsNullable : bool
////            Name : string
////            Order : int
////            Type : SQLDbType
////        }

//    type SimpleParameter =
//        {
//            Name : string
//            Order : int
//            Type : SQLDbType
//        }

//    type SchemaName = SchemaName of string
//    type ObjectName = ObjectName of string
//    type FullObjectName = FullObjectName of SchemaName * ObjectName

//    type UDT =
//        {
//            UDTColumns : Column seq
//            Name : FullObjectName
//        }

//    type Parameter =
//        | UDT of ParameterName : string * FullObjectName
//        | SimpleParameter of SimpleParameter


//    type ReturnType =
//        | Columns of Column[]
//        | Int

//    type RoutineName = RoutineName of id : string * fullObjectName : FullObjectName

//    type Routine =
//        {
//            Name : RoutineName
//            Parameters : Parameter[]
//            ReturnType : ReturnType
//        }
