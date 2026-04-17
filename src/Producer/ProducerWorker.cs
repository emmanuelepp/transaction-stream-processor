using Microsoft.Extensions.Hosting;

namespace Producer;

public class ProducerWorker : BackgroundService
{
    private readonly TransactionGenerator _transactionGenerator;
    private readonly KafkaProducer _kafkaProducer;

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
        }
    }
}