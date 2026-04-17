using Dapper;
using Npgsql;
using Shared.Models;
using System.Diagnostics;
namespace Consumer;

public class EventRepository
{
    private readonly string _connectionString;
    private static readonly ActivitySource _activitySource = new("Consumer");

    public EventRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SaveAsync(TransactionEvent transaction, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("db.insert");
        activity?.SetTag("transaction.id", transaction.EventId.ToString());

        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
        INSERT INTO transaction_events (event_id, event_type, amount, account_from, account_to, kafka_partition, kafka_offset)
        VALUES (@EventId, @EventType, @Amount, @AccountFrom, @AccountTo, 0, 0)
        ON CONFLICT (event_id) DO NOTHING
        """;

        await connection.ExecuteAsync(sql, transaction);
    }
}