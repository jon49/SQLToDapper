## About

FSharp.Data.SqlClient is a pretty awesome ORM. It's not your grandfather's ORM.
You can write SQL directly in your code and it will determine what parameters
are needed and what the return type is. It can tell you what stored procedures
are available and also generate all the code you need with TypeProviders on the
fly. It would be nice to have something like that for C#. We have
EntityFramework but you have to learn the C# way of writing queries and it is
more difficult to do optimizations.

`SQLToDapper` hopes to bridge the gap by bringing a similar capability to C#.
`SQLToDapper` has the following goals:

1. Be able to generate code by schema.
2. Be able to generate code with SQL files.
3. Be able to refresh files by clicking on a folder or file and clicking
   `Refresh`.

## Implementation Goals

1. Have a `MyNamespace/MyClassName.DapperWrapper.json` file that points to specified database schema.
2. Have separate `MyNamespace/MyClassName/MyMethodName.DapperWrapper.sql` files
   for each SQL statement called. These would exist in a directory that would be
   the name of the `class`, e.g., `MyClassName` in the example path. Likewise
   for the `namespace`. The names of the methods would be the name of the file,
   e.g., method would be named `MyMethodName`.
3. Have a intermediate `*.json` file which would contain the types as a cache. Can regenerate the class.
    a. Possibly just update parts of the class - future work.
4. Reuse return types for each class.
5. Update when file is saved.

## Intermediate JSON Type Descriptor

Note:  
    The property `schemaName` is optional. It is used for generating types
    from schema. Could possibly add `include` and/or `exclude` for stored procs
    that the user wants in the class.

```json
{
    "namespace": "MyNamespace",
    "className": "MyClassName",
    "schemas": [
        {
            "name": "MySchemaName",
            "include": [ "MyStoredProc" ],
            "exclude": [ "MyStoredProc1" ]
        }
    ],
    "methodSignatures": [
        {
            "id": 11234,
            "name": "GetPerson",
            "parameters": [
                {
                    "name": "id",
                    "type": "int",
                    "maxLength": -1
                }, {
                    "id": 23456,
                    "name": "associations",
                    "type": "udt",
                    "maxLength": 499
                }
            ]
        }
    ],
    "userDefinedTypes": [
        {
            "id": 23456,
            "schema": "utils",
            "name": "Name",
            "properties": [
                {
                    "name": "FirstName",
                    "type": "string",
                    "order": 1,
                    "isNullable": false
                }
            ]
        }
    ],
    "returnTypes": [
        {
            "methodIDs": [11234, 112345],
            "returnType": {
                "objectName": "Return_11234",
                "properties": [
                    {
                        "name": "FirstName",
                        "type": "string",
                        "order": 1
                        "isNullable": true
                    }, {
                        "name": "LastName",
                        "type": "string",
                        "order": 2,
                        "isNullable": false
                    }
                ]
            }
        }, {
            "methodIDs": [1556, 178],
            "returnType": "int"
        }
    ]
}
```


