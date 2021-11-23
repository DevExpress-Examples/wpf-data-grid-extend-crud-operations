using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace UndoOperation {
    public class UndoCRUDOperationsBehavior : Behavior<TableView> {
        public static readonly DependencyProperty CopyOperationsSupporterProperty;

        static UndoCRUDOperationsBehavior() {
            CopyOperationsSupporterProperty = DependencyProperty.Register(nameof(CopyOperationsSupporter), typeof(ICopyOperationSupporter), typeof(UndoCRUDOperationsBehavior), new PropertyMetadata(null));
        }

        IList Source { get { return (IList)AssociatedObject.DataControl.ItemsSource; } }

        Action undoAction;
        bool isNewItemRowEditing;
        object editingCache;
        IMessageBoxService messageBoxService;

        public ICopyOperationSupporter CopyOperationsSupporter {
            get { return (ICopyOperationSupporter)GetValue(CopyOperationsSupporterProperty); }
            set { SetValue(CopyOperationsSupporterProperty, value); }
        }
        public ICommand UndoCommand { get; private set; }

        public UndoCRUDOperationsBehavior() {
            UndoCommand = new DelegateCommand(Undo, CanUndo);
            messageBoxService = new DXMessageBoxService();
        }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.ValidateRow += OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion += OnRowDeleted;
            AssociatedObject.RowEditStarted += OnEditingStarted;
            AssociatedObject.DataSourceRefresh += OnRefresh;
            AssociatedObject.InitNewRow += OnNewRowStarted;
        }

        protected override void OnDetaching() {
            AssociatedObject.ValidateRow -= OnRowAddedOrEdited;
            AssociatedObject.ValidateRowDeletion -= OnRowDeleted;
            AssociatedObject.RowEditStarted -= OnEditingStarted;
            AssociatedObject.DataSourceRefresh -= OnRefresh;
            AssociatedObject.InitNewRow -= OnNewRowStarted;
            base.OnDetaching();
        }

        void OnRefresh(object sender, DataSourceRefreshEventArgs e) {
            Clear();
        }

        void Clear() {
            undoAction = null;
            editingCache = null;
        }

        void OnNewRowStarted(object sender, InitNewRowEventArgs e) {
            isNewItemRowEditing = true;
        }

        void Undo() {
            undoAction?.Invoke();
            undoAction = null;
        }
        bool CanUndo() {
            return undoAction != null && !AssociatedObject.IsEditing && !isNewItemRowEditing && !AssociatedObject.IsDataSourceRefreshing;
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

        void OnEditingStarted(object sender, RowEditStartedEventArgs e) {
            if(e.RowHandle != DataControlBase.NewItemRowHandle) {
                editingCache = CopyOperationsSupporter.Clone(e.Row);
            }
        }

        void OnRowDeleted(object sender, GridValidateRowDeletionEventArgs e) {
            undoAction = new Action(() => UndoDeleteAction(e.RowHandles.Single(), CopyOperationsSupporter.Clone(e.Rows.Single())));
        }

        void OnRowAddedOrEdited(object sender, GridRowValidationEventArgs e) {
            var item = e.Row;
            var isNewItem = e.IsNewItem;
            undoAction = e.IsNewItem ? new Action(() => UndoAddAction(item)) : new Action(() => UndoEditAction(item));
            isNewItemRowEditing = false;
        }

        void ApplyEditingCache(object item) {
            try {
                var args = new RowValidationArgs(editingCache, Source.IndexOf(item), false, new CancellationToken(), false);
                AssociatedObject.ValidateRowCommand?.Execute(args);
                if(!Validate(args.Result)) {
                    return;
                }
                CopyOperationsSupporter.CopyTo(editingCache, item);
                editingCache = null;
                AssociatedObject.DataControl.RefreshRow(AssociatedObject.DataControl.FindRow(item));
            } catch(Exception ex) {
                ShowError(ex.Message);
            }
        }

        void RemoveItem(object item) {
            try {
                var args = new ValidateRowDeletionArgs(new object[] { item }, new int[] { Source.IndexOf(item) });
                AssociatedObject.ValidateRowDeletionCommand?.Execute(args);
                if(!Validate(args.Result)) {
                    return;
                }
                Source.Remove(item);
            } catch(Exception ex) {
                ShowError(ex.Message);
            }
        }

        void InsertItem(int position, object item) {
            try {
                var args = new RowValidationArgs(item, Source.IndexOf(item), true, new CancellationToken(), false);
                AssociatedObject.ValidateRowCommand?.Execute(args);
                if(!Validate(args.Result)) {
                    return;
                }
                Source.Insert(position, item);
            } catch(Exception ex) {
                ShowError(ex.Message);
            }
        }

        bool Validate(ValidationErrorInfo validationInfo) {
            if(validationInfo?.ErrorContent != null) {
                ShowError(validationInfo.ErrorContent);
                return false;
            }
            return true;
        }

        void ShowError(string message) {
            messageBoxService.ShowMessage(message, "Unable to undo the last operation", MessageButton.OK, MessageIcon.Error);
        }
    }
}
