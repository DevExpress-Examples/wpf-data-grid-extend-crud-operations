using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using EntityFrameworkIssues.Issues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;

namespace DetailCollectionEditing {
    public class EditUserViewModel : EditItemViewModel {
        public EditUserViewModel(User item, IssuesContext editOperationContext, Action dispose = null, string title = null)
            : base(item, editOperationContext, dispose, title) {
            Issues = new ObservableCollectionEx<Issue>(item.Issues ?? new List<Issue>());
        }
        IssuesContext Context => (IssuesContext)EditOperationContext;
        User User => (User)Item;
        public ObservableCollectionEx<Issue> Issues {
            get { return GetValue<ObservableCollectionEx<Issue>>(); }
            private set { SetValue(value); }
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (Issue)args.Items.Single();
            Context.Issues.Remove(item);
            Issues.Remove(item);
        }
        [Command]
        public void CreateEditEntityViewModel(CreateEditItemViewModelArgs args) {
            Issue item;
            if(args.Key != null) {
                var existingItem = Context.Issues.Find(args.Key);
                item = new Issue();
                CopyIssueProperties(existingItem, item);
            } else {
                item = new Issue();
            }
            args.ViewModel = new EditItemViewModel(item, null, title: (args.IsNewItem ? "New " : "Edit ") + nameof(Issue));
        }
        [Command]
        public void ValidateRow(EditFormRowValidationArgs args) {
            var item = (Issue)args.Item;
            if(args.IsNewItem) {
                var newItem = Context.Issues.Create();
                Context.Issues.Add(newItem);
                CopyIssueProperties(item, newItem);
                newItem.UserId = User.Id;
                Issues.Add(newItem);
            } else {
                var existingItem = Context.Issues.Find(item.Id);
                CopyIssueProperties(item, existingItem);
                Issues.ReloadItemAt(Issues.IndexOf(existingItem));
            }
        }
        static void CopyIssueProperties(Issue from, Issue to) {
            to.Id = from.Id;
            to.Created = from.Created;
            to.Priority = from.Priority;
            to.Subject = from.Subject;
            to.Votes = from.Votes;
        }
    }
    public class ObservableCollectionEx<T> : ObservableCollection<T> {
        public ObservableCollectionEx(IEnumerable<T> collection) : base(collection) {
        }

        public void ReloadItemAt(int index) {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], this[index], index));
        }
    }
}
