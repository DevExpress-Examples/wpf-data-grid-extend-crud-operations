Namespace UndoOperation

    Public Interface ICopyOperationSupporter

        Function Clone(ByVal item As Object) As Object

        Sub CopyTo(ByVal source As Object, ByVal target As Object)

    End Interface
End Namespace
