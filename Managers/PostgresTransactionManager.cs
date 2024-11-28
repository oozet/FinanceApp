public class DatabaseTransactionManager : ITransactionManager
{
    private AppDbContext _appDbContext;

    public DatabaseTransactionManager(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public TransactionEntry CreateTransaction(float amount, TransactionType type)
    {
        TransactionEntry entry = new TransactionEntry
        {
            Uid = Guid.NewGuid(),
            Date = DateTime.Now,
            Type = type,
            AmountInMinorUnit = (long)(amount * 100),
        };

        string sqlQuery = "INSERT INTO transactions ()";
        // _appDbContext.ExecuteCommandAsync();

        return null;
    }

    public void AddTransaction(TransactionEntry transaction)
    {
        throw new NotImplementedException();
    }

    public List<TransactionEntry> GetAllTransactions()
    {
        throw new NotImplementedException();
    }

    public float GetTotal()
    {
        throw new NotImplementedException();
    }

    public TransactionEntry? GetTransaction(Guid Uid)
    {
        throw new NotImplementedException();
    }

    public List<TransactionEntry> Populate(int count)
    {
        throw new NotImplementedException();
    }

    public void RemoveEntry(Guid uid)
    {
        throw new NotImplementedException();
    }

    public void RemoveTransaction(Guid uid)
    {
        throw new NotImplementedException();
    }
}
