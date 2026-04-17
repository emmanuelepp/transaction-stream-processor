using Bogus;
using Shared.Models;
namespace Producer;

public class TransactionGenerator
{
    private readonly Faker<TransactionEvent> _faker;
    
 public TransactionGenerator()
{
    _faker = new Faker<TransactionEvent>()
        .RuleFor(te => te.EventId, f => Guid.NewGuid())
        .RuleFor(te => te.EventType, f => "TransactionCreated")
        .RuleFor(te => te.Amount, f => f.Finance.Amount(1, 10000))
        .RuleFor(te => te.AccountFrom, f => f.Finance.Account())
        .RuleFor(te => te.AccountTo, f => f.Finance.Account())
        .RuleFor(te => te.CreatedAt, f => DateTime.UtcNow);
}

    public TransactionEvent Generate()
    {
        return _faker.Generate();
    }
}