using System.Text.Json;
using Shared.Models;
using Confluent.Kafka;
using System.Diagnostics;

namespace Consumer;

public sealed class KafkaConsumer : IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;
    private readonly TransactionProcessor _processor;
    private static readonly ActivitySource _activitySource = new("Consumer");

    public KafkaConsumer(IConsumer<string, string> consumer, string topic, TransactionProcessor processor)
    {
        _consumer = consumer;
        _topic = topic;
        _processor = processor;
    }

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(cancellationToken);
            var transaction = JsonSerializer.Deserialize<TransactionEvent>(result.Message.Value);

            if (transaction is null) continue;

            using var activity = _activitySource.StartActivity("kafka.consume");
            activity?.SetTag("messaging.topic", _topic);
            activity?.SetTag("transaction.id", transaction.EventId.ToString());

            try
            {
                await _processor.ProcessAsync(transaction, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }
        }
    }

    public void Dispose()
    {
        _consumer.Unsubscribe();
        _consumer.Close();
        _consumer.Dispose();
    }
}