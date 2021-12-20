Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports EntityFrameworkIssues.Issues
Imports System.Collections.Generic
Imports System.Data.Entity
Imports System.Linq

Namespace DetailCollectionEditing

    Public Class MainViewModel
        Inherits ViewModelBase

        Private _Context As IssuesContext

        Private _ItemsSource As IList(Of User)

        Public ReadOnly Property ItemsSource As IList(Of User)
            Get
                If _ItemsSource Is Nothing AndAlso Not IsInDesignMode Then
                    _Context = New IssuesContext()
                    _ItemsSource = _Context.Users.ToList()
                End If

                Return _ItemsSource
            End Get
        End Property

        <Command>
        Public Sub CreateEditEntityViewModel(ByVal args As CreateEditItemViewModelArgs)
            Dim context = New IssuesContext()
            Dim item As User
            If args.Key IsNot Nothing Then
                item = context.Users.Find(args.Key)
            Else
                item = New User()
                context.Entry(item).State = EntityState.Added
            End If

            args.ViewModel = New EditUserViewModel(item, context, title:=If(args.IsNewItem, "New ", "Edit ") & NameOf(User))
        End Sub

        <Command>
        Public Sub ValidateRow(ByVal args As EditFormRowValidationArgs)
            Dim context = CType(args.EditOperationContext, IssuesContext)
            context.SaveChanges()
        End Sub

        <Command>
        Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
            Dim item = CType(args.Items.[Single](), User)
            _Context.Users.Remove(item)
            _Context.SaveChanges()
        End Sub

        <Command>
        Public Sub DataSourceRefresh(ByVal args As DataSourceRefreshArgs)
            _ItemsSource = Nothing
            _Context = Nothing
            RaisePropertyChanged(NameOf(MainViewModel.ItemsSource))
        End Sub
    End Class
End Namespace
