using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using EntityFrameworkIssues.Issues;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DetailCollectionEditing {
    public class MainViewModel : ViewModelBase {
        IssuesContext _Context;
        IList<User> _ItemsSource;

        public IList<User> ItemsSource {
            get {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new IssuesContext();
                    _ItemsSource = _Context.Users.ToList();
                }
                return _ItemsSource;
            }
        }
        [Command]
        public void CreateEditEntityViewModel(CreateEditItemViewModelArgs args) {
            var context = new IssuesContext();
            User item;
            if(args.Key != null)
                item = context.Users.Find(args.Key);
            else {
                item = new User();
                context.Entry(item).State = EntityState.Added;
            }
            args.ViewModel = new EditUserViewModel(item, context, title: (args.IsNewItem ? "New " : "Edit ") + nameof(User));
        }
        [Command]
        public void ValidateRow(EditFormRowValidationArgs args) {
            var context = (IssuesContext)args.EditOperationContext;
            context.SaveChanges();
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (User)args.Items.Single();
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}
