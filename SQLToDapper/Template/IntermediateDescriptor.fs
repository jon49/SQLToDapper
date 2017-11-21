namespace SQLToDapper

module IntermediateDescriptor =

    type SqlProperty =
        {
            name: string
            clrType: string
            order: int
            isNullable: bool
            sqlDbType: string
            precision: int
            length: int16
        }

    type Property =
        {
            name: string
            clrType: string
            order: int
            isNullable: bool
        }

    type MethodReturnSimple = string

    type MethodReturnTable = Property list

    type MethodReturn =
        | MethodReturnSimple of MethodReturnSimple
        | MethodReturnTable of MethodReturnTable

    type ReturnType =
        {
            methodIDs: int list
            returnType: MethodReturn
        }

    type UserDefinedType =
        {
            id: int
            schema: string
            name: string
            properties: SqlProperty list
        }

    type Parameter =
        {
            /// Only used for UDT types
            id: int option
            name: string
            clrType: string
            order: int
            maxLength: int16
        }

    type MethodSignature =
        {
            id: int
            name: string
            parameters: Parameter list
        }

    type ClassDescription =
        {
            methodSignatures: MethodSignature list
            userDefinedTypes: UserDefinedType list
            returnTypes: ReturnType list
        }

    type Schema =
        {
            /// Schema name
            name: string
            /// Stored procedures to explicitly include. Will not include other stored procedures
            ``include``: string list
            /// Stored procedures to to exclude. If include has values this will be ingored.
            exclude: string list
        }

    type Intermediate =
        {
            ``namespace``: string
            className: string
            schemas: Schema list
            classDescription: ClassDescription
        }

