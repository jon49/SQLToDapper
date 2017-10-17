namespace SQLToDapper

module TSQL =

    open SQL

    type Column =
        // Numeric
        | BIT
        | TINYINT
        | INT
        | BIGINT
        | SMALLMONEY
        | MONEY
        | DECIMAL
        | NUMERIC
        | REAL
        | FLOAT of Length
        // Text/XML
        | CHAR of Length
        | NCHAR of Length
        | VARCHAR of Length
        | NVARCHAR of Length
        | TEXT
        | NTEXT
        | XML
        // Date/Time
        | SMALLDATETIME
        | DATETIME
        | DATETIME2 of Precision
        | DATETIMEOFFSET of Precision
        | DATE
        | TIME of Precision
        // Binary
        | BINARY of Length
        | VARBINARY of Length
        | IMAGE
        | TIMESTAMP
        // Miscellaneous
        | UNIQUEIDENTIFIER
        | SQLVARIANT

        


