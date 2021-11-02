using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace UndoOperation {
    public class UndoBehavior : Behavior<TableView> {
        public static readonly DependencyProperty CopyOperationsSupporterProperty =
           DependencyProperty.Register(nameof(CopyOperationsSupporter), typeof(IDataItemCopyOperationsSupporter), typeof(UndoBehavior), new PropertyMetadata(null));

        public IDataItemCopyOperationsSupporter CopyOperationsSupporter {
            get { return (IDataItemCopyOperationsSupporter)GetValue(CopyOperationsSupporterProperty); }
            set { SetValue(CopyOperationsSupporterProperty, value); }
        }

        public static readonly DependencyProperty ValidateRowCommandProperty =
          DependencyProperty.Register(nameof(ValidateRowCommand), typeof(ICommand<RowValidationArgs>), typeof(UndoBehavior), new PropertyMetadata(null));

        public ICommand<RowValidationArgs> ValidateRowCommand {
            get { return (ICommand<RowValidationArgs>)GetValue(ValidateRowCommandProperty); }
            set { SetValue(ValidateRowCommandProperty, value); }
        }

        public static readonly DependencyProperty ValidateRowDeletionCommandProperty =
        DependencyProperty.Register(nameof(ValidateRowDeletionCommand), typeof(ICommand<ValidateRowDeletionArgs>), typeof(UndoBehavior), new PropertyMetadata(null));

        public ICommand<ValidateRowDeletionArgs> ValidateRowDeletionCommand {
            get { return (ICommand<ValidateRowDeletionArgs>)GetValue(ValidateRowDeletionCommandProperty); }
            set { SetValue(ValidateRowDeletionCommandProperty, value); }
        }

        public ICommand UndoCommand { get; private set; }

        public UndoBehavior() {
            UndoCommand = new DelegateCommand(Undo, CanUndo);
        }
        
        IList source;

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.ValidateRow += OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion += OnRowDeleted;
            AssociatedObject.RowEditStarted += OnEditingStarted;
            AssociatedObject.DataSourceRefresh += OnRefresh;
            AssociatedObject.InitNewRow += OnNewRowStarted;
            source = (IList)AssociatedObject.DataControl.ItemsSource;
            AssociatedObject.DataControl.ItemsSourceChanged += ItemsSourceChanged;
        }

        void ItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
            source = (IList)AssociatedObject.DataControl.ItemsSource;
        }

        void OnRefresh(object sender, DataSourceRefreshEventArgs e) {
            undoAction = null;
            editingCache = null;
        }

        protected override void OnDetaching() {
            AssociatedObject.ValidateRow -= OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion -= OnRowDeleted;
            AssociatedObject.RowEditStarted -= OnEditingStarted;
            AssociatedObject.DataSourceRefresh -= OnRefresh;
            AssociatedObject.InitNewRow -= OnNewRowStarted;
            source = null;
            AssociatedObject.DataControl.ItemsSourceChanged -= ItemsSourceChanged;
            base.OnDetaching();
        }

        void OnNewRowStarted(object sender, InitNewRowEventArgs e) {
            isNewItemRowEditing = true;
        }

        bool isNewItemRowEditing;

        object editingCache;

        void OnEditingStarted(object sender, RowEditStartedEventArgs e) {
            if(e.RowHandle != DataControlBase.NewItemRowHandle) {
                editingCache = CopyOperationsSupporter.Clone(e.Row);
            }
        }

        void OnRowDeleted(object sender, GridValidateRowDeletionEventArgs e) {
            undoAction = new Action(() => UndoDeleteAction(e.RowHandles.Single(), CopyOperationsSupporter.Clone(e.Rows.Single())));
        }

        void UndoEditAction(object item) {
            ApplyEditingCache(item);
            AssociatedObject.DataControl.CurrentItem = item;
        }

        void UndoAddAction(object item) {
            AssociatedObject.DataControl.CurrentItem = item;
            RemoveItem(item);
        }

        void UndoDeleteAction(int position, object item) {
            InsertItem(position, item);
            AssociatedObject.DataControl.CurrentItem = item;
        }

        void OnRowAddedOrEdited(object sender, GridRowValidationEventArgs e) {
            var item = e.Row;
            var isNewItem = e.IsNewItem;
            undoAction = e.IsNewItem ? new Action(() => UndoAddAction(item)) : new Action(() => UndoEditAction(item));
            isNewItemRowEditing = false;
        }

        void ApplyEditingCache(object item) {
            CopyOperationsSupporter.CopyTo(editingCache, item);
            AssociatedObject.DataControl.RefreshRow(AssociatedObject.DataControl.FindRow(item));
            ValidateRowCommand.Execute(new RowValidationArgs(editingCache, source.IndexOf(item), false, new CancellationToken(), false));
        }

        Action undoAction;

        void Undo() {
            undoAction?.Invoke();
            undoAction = null;
        }

        bool CanUndo() {
            return undoAction != null && !AssociatedObject.IsEditing && !isNewItemRowEditing && !AssociatedObject.IsDataSourceRefreshing;
        }

        void RemoveItem(object item) {
            source.Remove(item);
            ValidateRowDeletionCommand.Execute(new ValidateRowDeletionArgs(new object[] { item }, new int[] { source.IndexOf(item) }));
        }

        void InsertItem(int position, object item) {
            source.Insert(position, item);
            ValidateRowCommand.Execute(new RowValidationArgs(item, source.IndexOf(item), true, new CancellationToken(), false));
        }

    }
}
