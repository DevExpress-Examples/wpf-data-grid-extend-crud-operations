Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.Mvvm.Xpf
Imports EntityFrameworkIssues.Issues
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace AsyncCRUDOperations

    Public Class MainViewModel
        Inherits ViewModelBase

        Private _Context As IssuesContext

        Private _ItemsSource As IList(Of User)

        Public ReadOnly Property ItemsSource As IList(Of User)
            Get
                Return _ItemsSource
            End Get
        End Property

        <Command>
        Public Sub ValidateRow(ByVal args As RowValidationArgs)
            Dim item = CType(args.Item, User)
            args.ResultAsync = Task.Run(Async Function()
                Await Task.Delay(3000)
                If args.IsNewItem Then
                    _Context.Users.Add(item)
                Else
                    _Context.SaveChanges()
                End If

                Return New ValidationErrorInfo(Nothing)
            End Function)
        End Sub

        <Command>
        Public Sub ValidateRowDeletion(ByVal args As ValidateRowDeletionArgs)
            Call Task.Delay(3000).Wait()
            Dim item = CType(args.Items.[Single](), User)
            _Context.Users.Remove(item)
            _Context.SaveChanges()
        End Sub

        <Command>
        Public Sub DataSourceRefresh(ByVal args As DataSourceRefreshArgs)
            args.RefreshAsync = RefreshDataSoruceAsync()
        End Sub

        Private Async Function RefreshDataSoruceAsync() As Task
            Await Task.Delay(3000)
            _Context = New IssuesContext()
            _ItemsSource = _Context.Users.ToList()
            RaisePropertyChanged(NameOf(MainViewModel.ItemsSource))
        End Function
    End Class
End Namespace
