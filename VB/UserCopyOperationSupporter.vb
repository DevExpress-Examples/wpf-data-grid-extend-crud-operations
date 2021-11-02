Imports EntityFrameworkIssues.Issues
Imports System

Namespace UndoOperation

    Public Class UserCopyOperationSupporter
        Implements UndoOperation.ICopyOperationSupporter

        Public Function Clone(ByVal item As Object) As Object Implements Global.UndoOperation.ICopyOperationSupporter.Clone
            Dim userItem = Me.GetUser(item)
            Return New EntityFrameworkIssues.Issues.User() With {.FirstName = userItem.FirstName, .Id = userItem.Id, .LastName = userItem.LastName}
        End Function

        Public Sub CopyTo(ByVal source As Object, ByVal target As Object) Implements Global.UndoOperation.ICopyOperationSupporter.CopyTo
            Dim userSource = Me.GetUser(source)
            Dim userTarget = Me.GetUser(target)
            userTarget.FirstName = userSource.FirstName
            userTarget.Id = userSource.Id
            userTarget.LastName = userSource.LastName
        End Sub

        Private Function GetUser(ByVal item As Object) As User
            Dim result = TryCast(item, EntityFrameworkIssues.Issues.User)
            If result Is Nothing Then
                Throw New System.InvalidOperationException()
            End If

            Return result
        End Function
    End Class
End Namespace
