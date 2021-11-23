Imports DevExpress.Mvvm
Imports EntityFrameworkIssues.Issues
Imports System.Collections.ObjectModel
Imports System.Linq

Namespace UndoOperation

    Public Class MainViewModel
        Inherits ViewModelBase

        Private _CopyOperationsSupporter As UserCopyOperationSupporter

        Private _Context As IssuesContext

        Private _ItemsSource As ObservableCollection(Of User)

        Public ReadOnly Property ItemsSource As ObservableCollection(Of User)
            Get
                If _ItemsSource Is Nothing AndAlso Not IsInDesignMode Then
                    _Context = New IssuesContext()
                    _ItemsSource = New ObservableCollection(Of User)(_Context.Users)
                End If

                Return _ItemsSource
            End Get
        End Property

        Public Property CopyOperationsSupporter As UserCopyOperationSupporter
            Get
                Return _CopyOperationsSupporter
            End Get

            Private Set(ByVal value As UserCopyOperationSupporter)
                _CopyOperationsSupporter = value
            End Set
        End Property

        <DataAnnotations.Command>
        Public Sub ValidateRow(ByVal args As Xpf.RowValidationArgs)
            Dim item = CType(args.Item, User)
            If args.IsNewItem Then _Context.Users.Add(item)
            _Context.SaveChanges()
        End Sub

        <DataAnnotations.Command>
        Public Sub ValidateRowDeletion(ByVal args As Xpf.ValidateRowDeletionArgs)
            Dim item = CType(args.Items.[Single](), User)
            _Context.Users.Remove(item)
            _Context.SaveChanges()
        End Sub

        <DataAnnotations.Command>
        Public Sub DataSourceRefresh(ByVal args As Xpf.DataSourceRefreshArgs)
            _ItemsSource = Nothing
            _Context = Nothing
            RaisePropertyChanged(NameOf(MainViewModel.ItemsSource))
        End Sub

        Public Sub New()
            CopyOperationsSupporter = New UserCopyOperationSupporter()
        End Sub
    End Class
End Namespace
