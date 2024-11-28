public class TransactionService
{
    private readonly IRepository<TransactionEntry> _repository;

    public TransactionService(IRepository<TransactionEntry> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TransactionEntry>> GetAllEntitiesAsync()
    {
        return await _repository.GetAllAsync();
    }

    // Additional methods for business logic
}
