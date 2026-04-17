using Confluent.Kafka;
using Shared.Models;
using System.Text.Json;

namespace Producer;

public sealed class KafkaProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(IProducer<string, string> producer, string topic)
    {
        _producer = producer;
        _topic = topic;
    }

    public async Task PublishAsync(TransactionEvent transactionEvent, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(transactionEvent);

        var message = new Message<string, string>
        {
            Key = transactionEvent.EventId.ToString(),
            Value = json
        };

        await _producer.ProduceAsync(_topic, message, cancellationToken);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}