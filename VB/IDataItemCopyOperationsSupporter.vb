Namespace UndoOperation

    Public Interface IDataItemCopyOperationsSupporter

        Function Clone(ByVal item As Object) As Object

        Sub CopyTo(ByVal source As Object, ByVal target As Object)

    End Interface
End Namespace
