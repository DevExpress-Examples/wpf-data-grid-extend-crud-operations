using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using EntityFrameworkIssues.Issues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DetailCollectionEditing {
    public class EditUserViewModel : EditItemViewModel {
        public EditUserViewModel(User item, IssuesContext editOperationContext, string title = null)
            : base(item, editOperationContext, title : title) {
            Issues = item.Issues?.ToList() ?? new List<Issue>();
        }
        IssuesContext Context => (IssuesContext)EditOperationContext;
        User User => (User)Item;
        public List<Issue> Issues {
            get { return GetValue<List<Issue>>(); }
            private set { SetValue(value); }
        }
        [Command]
        public void ValidateRow(RowValidationArgs args) {
            var item = (Issue)args.Item;
            if(args.IsNewItem) {
                item.UserId = User.Id;
                Context.Issues.Add(item);
            }
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (Issue)args.Items.Single();
            Context.Issues.Remove(item);
        }
    }
}
