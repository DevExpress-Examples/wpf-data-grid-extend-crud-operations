using DevExpress.Mvvm;
using EntityFrameworkIssues.Issues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoOperation {
    public class MainViewModel : ViewModelBase {
        EntityFrameworkIssues.Issues.IssuesContext _Context;
        ObservableCollection<User> _ItemsSource;

        public ObservableCollection<EntityFrameworkIssues.Issues.User> ItemsSource {
            get {
                if(_ItemsSource == null && !IsInDesignMode) {
                    _Context = new EntityFrameworkIssues.Issues.IssuesContext();
                    _ItemsSource = new ObservableCollection<User>(_Context.Users);
                }
                return _ItemsSource;
            }
        }

        UserCopyOperationsSupporter _CopyOperationsSupporter;
        public UserCopyOperationsSupporter CopyOperationsSupporter {
            get {
                if(_CopyOperationsSupporter == null) {
                    _CopyOperationsSupporter = new UserCopyOperationsSupporter();
                }
                return _CopyOperationsSupporter;
            }
        }

        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRow(DevExpress.Mvvm.Xpf.RowValidationArgs args) {
            var item = (EntityFrameworkIssues.Issues.User)args.Item;
            if(args.IsNewItem)
                _Context.Users.Add(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void ValidateRowDeletion(DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs args) {
            var item = (EntityFrameworkIssues.Issues.User)args.Items.Single();
            _Context.Users.Remove(item);
            _Context.SaveChanges();
        }
        [DevExpress.Mvvm.DataAnnotations.Command]
        public void DataSourceRefresh(DevExpress.Mvvm.Xpf.DataSourceRefreshArgs args) {
            _ItemsSource = null;
            _Context = null;
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}
