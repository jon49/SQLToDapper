namespace SQLToDapper

module TSQL =

    type Column =
        | BIT
        | TINYINT
        | INT
        | BIGINT
        | SMALLMONEY
        | MONEY
        | DECIMAL
        | NUMERIC
        | REAL of int option
        | FLOAT of int option

