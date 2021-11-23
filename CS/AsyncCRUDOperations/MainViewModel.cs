using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using EntityFrameworkIssues.Issues;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncCRUDOperations {
    public class MainViewModel : ViewModelBase {
        IssuesContext _Context;
        IList<User> _ItemsSource;

        public IList<User> ItemsSource { get { return _ItemsSource; } }

        [Command]
        public void ValidateRow(RowValidationArgs args) {
            var item = (User)args.Item;
            args.ResultAsync = Task.Run(async () => {
                await Task.Delay(3000);
                if(args.IsNewItem) {
                    _Context.Users.Add(item);
                } else {
                    _Context.SaveChanges();
                }
                return new ValidationErrorInfo(null);
            });
        }
        [Command]
        public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
            var item = (User)args.Items.Single();
            args.ResultAsync = Task.Run(async () => {
                await Task.Delay(3000);
                _Context.Users.Remove(item);
                _Context.SaveChanges();
                return new ValidationErrorInfo(null);
            });
        }
        [Command]
        public void DataSourceRefresh(DataSourceRefreshArgs args) {
            args.RefreshAsync = RefreshDataSoruceAsync();
        }

        async Task RefreshDataSoruceAsync() {
            await Task.Delay(3000).ConfigureAwait(false);
            _Context = new IssuesContext();
            _ItemsSource = _Context.Users.ToList();
            RaisePropertyChanged(nameof(ItemsSource));
        }
    }
}
