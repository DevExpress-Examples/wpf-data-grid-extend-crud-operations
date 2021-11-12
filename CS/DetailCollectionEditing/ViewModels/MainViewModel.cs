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
        EntityFrameworkIssues.Issues.IssuesContext _Context;
        System.Collections.Generic.IList<EntityFrameworkIssues.Issues.User> _ItemsSource;

        public System.Collections.Generic.IList<EntityFrameworkIssues.Issues.User> ItemsSource {
            get {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new IssuesContext();
                    _ItemsSource = _Context.Users.ToList();
                }
                return _ItemsSource;
            }
        }
        //[Command]
        //public void ValidateRow(RowValidationArgs args) {
        //    var item = (User)args.Item;
        //    if(args.IsNewItem)
        //        _Context.Users.Add(item);
        //    _Context.SaveChanges();
        //}
        [Command]
        public void CreateEditEntityViewModel(DevExpress.Mvvm.Xpf.CreateEditItemViewModelArgs args) {
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
        public void ValidateRow(DevExpress.Mvvm.Xpf.EditFormRowValidationArgs args) {
            var context = (IssuesContext)args.EditOperationContext;
            context.SaveChanges();
        }
        [Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs args) {
            var item = (User)args.Items.Single();
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}
