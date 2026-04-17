using Confluent.Kafka;
using Shared.Models;
using System.Text.Json;
using System.Diagnostics;

namespace Producer;

public sealed class KafkaProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private static readonly ActivitySource _activitySource = new("Producer");

    public KafkaProducer(IProducer<string, string> producer, string topic)
    {
        _producer = producer;
        _topic = topic;
    }

    public async Task PublishAsync(TransactionEvent transactionEvent, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("kafka.publish");
        activity?.SetTag("messaging.topic", _topic);
        activity?.SetTag("transaction.id", transactionEvent.EventId.ToString());
        activity?.SetTag("transaction.amount", transactionEvent.Amount.ToString());

        var json = JsonSerializer.Serialize(transactionEvent);

        var message = new Message<string, string>
        {
            Key = transactionEvent.EventId.ToString(),
            Value = json
        };

        try
        {
            await _producer.ProduceAsync(_topic, message, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}