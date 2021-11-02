namespace UndoOperation {
    public interface IDataItemCopyOperationsSupporter {
        object Clone(object item);
        void CopyTo(object source, object target);
    }
}
