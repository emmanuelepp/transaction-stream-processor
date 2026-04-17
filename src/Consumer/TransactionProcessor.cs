using Shared.Models;
namespace Consumer;

public class TransactionProcessor
{
    private readonly EventRepository _repository;

    public TransactionProcessor(EventRepository repository)
    {
        _repository = repository;
    }

    public async Task ProcessAsync(TransactionEvent transaction, CancellationToken cancellationToken)
    {
        if (transaction.Amount <= 0) return;
        await _repository.SaveAsync(transaction, cancellationToken);
    }
}