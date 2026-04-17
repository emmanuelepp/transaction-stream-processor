using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;

namespace Consumer;

public class ConsumerWorker : BackgroundService
{
    private readonly KafkaConsumer _consumer;
    private static readonly Meter _meter = new("ConsumerWorker");
    private static readonly Counter<long> _processedEventsCounter = _meter.CreateCounter<long>("messages.consumed");

    public ConsumerWorker(KafkaConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync(stoppingToken);
    }
}