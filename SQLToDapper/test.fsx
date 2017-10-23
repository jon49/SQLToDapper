
open System.Text

let m = Map.empty<string, StringBuilder>
let sb = new StringBuilder()

let m' = m.Add("1", sb)

sb.Append("Hello, \n").Append("world")

(m'.Item "1").ToString()
