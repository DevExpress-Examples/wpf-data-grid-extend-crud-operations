Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports EntityFrameworkIssues.Issues
Imports System.Collections.Generic
Imports System.Linq

Namespace DetailCollectionEditing

    Public Class EditUserViewModel
        Inherits EditItemViewModel

        Public Sub New(ByVal item As User, ByVal editOperationContext As IssuesContext, ByVal Optional title As String = Nothing)
            MyBase.New(item, editOperationContext, title:=title)
            Issues = If(item.Issues?.ToList(), New List(Of Issue)())
        End Sub

        Private ReadOnly Property Context As IssuesContext
            Get
                Return CType(EditOperationContext, IssuesContext)
            End Get
        End Property

        Private ReadOnly Property User As User
            Get
                Return CType(Item, User)
            End Get
        End Property

        Public Property Issues As List(Of Issue)
            Get
                Return GetValue(Of List(Of Issue))()
            End Get

            Private Set(ByVal value As List(Of Issue))
                SetValue(value)
            End Set
        End Property

        <Command>
        Public Sub ValidateRow(ByVal args As RowValidationArgs)
            Dim item = CType(args.Item, Issue)
            If args.IsNewItem Then
                item.UserId = User.Id
                Context.Issues.Add(item)
            End If
        End Sub

        <Command>
        Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
            Dim item = CType(args.Items.[Single](), Issue)
            Context.Issues.Remove(item)
        End Sub
    End Class
End Namespace
