namespace UndoOperation {
    public interface ICopyOperationSupporter {
        object Clone(object item);
        void CopyTo(object source, object target);
    }
}
