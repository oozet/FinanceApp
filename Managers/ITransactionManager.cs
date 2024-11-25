public interface ITransactionManager
{
    void OnProgramLoad();
    void AddEntry(float amount, TransactionType type);
    void AddTransaction(TransactionEntry transaction);
    void RemoveEntry(int Uid);
    void RemoveTransaction(int uid);
    void Populate(int count);
    float GetTotal();
    public (float, float) GetTotalPerType(List<TransactionEntry> filteredTransactions);

    List<TransactionEntry> FilterTransactions(Predicate<TransactionEntry> predicate);

    TransactionEntry? GetTransaction(int Uid);

    List<TransactionEntry> GetAllTransactions();
}
