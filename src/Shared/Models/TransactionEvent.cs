namespace Shared.Models;

public record TransactionEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public string EventType { get; init; } = "TransactionCreated";
    public decimal Amount { get; init; }
    public string AccountFrom { get; init; } = string.Empty;
    public string AccountTo { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
