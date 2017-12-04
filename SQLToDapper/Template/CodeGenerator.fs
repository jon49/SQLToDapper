namespace SQLToDapper

module Code =

    open SQLToDapper.IntermediateDescriptor
    open SQLToDapper.Template

    let generate (classDescription : ClassDescription) =
        (+) (UdtCode.generate classDescription.userDefinedTypes) (ReturnCode.generate (fst classDescription.returnTypes))

