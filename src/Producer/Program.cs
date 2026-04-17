using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Producer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var bootstrapServers = context.Configuration["KAFKA__BOOTSTRAPSERVERS"]
        ?? "localhost:9092";
        var topic = context.Configuration["KAFKA__TOPIC"]
            ?? "transactions";
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        var kafkaProducer = new ProducerBuilder<string, string>(producerConfig).Build();
        services.AddSingleton<TransactionGenerator>();
        services.AddHostedService<ProducerWorker>();
        services.AddSingleton<KafkaProducer>(sp => new KafkaProducer(kafkaProducer, topic));
    })
    .Build();

await host.RunAsync();