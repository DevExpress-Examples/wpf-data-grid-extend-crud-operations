using EntityFrameworkIssues.Issues;
using System;

namespace UndoOperation {
    public class UserCopyOperationSupporter : ICopyOperationSupporter {
        public object Clone(object item) {
            var userItem = GetUser(item);
            return new User() { FirstName = userItem.FirstName, Id = userItem.Id, LastName = userItem.LastName };
        }

        public void CopyTo(object source, object target) {
            var userSource = GetUser(source);
            var userTarget = GetUser(target);
            userTarget.FirstName = userSource.FirstName;
            userTarget.Id = userSource.Id;
            userTarget.LastName = userSource.LastName;
        }

        User GetUser(object item) {
            var result = item as User;
            if(result == null) {
                throw new InvalidOperationException();
            }
            return result;
        }
    }
}
