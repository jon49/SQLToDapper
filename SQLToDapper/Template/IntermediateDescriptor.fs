namespace SQLToDapper

module IntermediateDescriptor =

    type Property =
        {
            name: string
            ``type``: string
            order: int
            isNullable: bool
        }

    type MethodReturnSimple = string

    type MethodReturnTable =
        {
            objectName: string
            properties: Property list
        }

    type MethodReturn =
        | MethodReturnSimple of MethodReturnSimple
        | MethodReturnTable of MethodReturnTable

    type ReturnType =
        {
            methodIDs: int list
            returnTypes: MethodReturn
        }

    type UserDefinedType =
        {
            id: int
            schema: string
            name: string
            properties: Property list
        }

    type Parameter =
        {
            /// Only used for UDT types
            id: int option
            name: string
            ``type``: string
            maxLength: int
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
            ``include``: string list option
            /// Stored procedures to to exclude. If include has values this will be ingored.
            exclude: string list option
        }

    type Intermediate =
        {
            ``namespace``: string
            className: string
            schemas: Schema list
            classDescription: ClassDescription
        }

