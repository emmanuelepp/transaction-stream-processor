using System.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;

namespace Producer;

public class ProducerWorker : BackgroundService
{
    private readonly TransactionGenerator _transactionGenerator;
    private readonly KafkaProducer _kafkaProducer;
    private static readonly Meter _meter = new("Producer");
    private static readonly Counter<long> _messagesPublished = _meter.CreateCounter<long>("messages.published");

    public ProducerWorker(TransactionGenerator transactionGenerator, KafkaProducer kafkaProducer)
    {
        _transactionGenerator = transactionGenerator;
        _kafkaProducer = kafkaProducer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var transactionEvent = _transactionGenerator.Generate();
            await _kafkaProducer.PublishAsync(transactionEvent, stoppingToken);
            _messagesPublished.Add(1);
        }
    }
}